Here's a **clean, step-by-step implementation plan** for your **Librarian AI Buddy** using a specific tech stack:

---

### **Tech Stack Decision**
| Component | Technology Choice |
|-----------|-------------------|
| **Frontend** | Blazor WebAssembly (MudBlazor UI components) |
| **Backend** | ASP.NET Core Web API |
| **Database** | PostgreSQL (for chat history) + Redis (caching) |
| **AI/LLM** | OpenAI GPT-4 (or GPT-3.5-turbo for cost efficiency) |
| **Embeddings** | OpenAI `text-embedding-3-small` |
| **Vector DB** | Chroma (local) or Pinecone (cloud) |
| **Jira Integration** | Atlassian REST API (Jira Cloud) |
| **Auth** | ASP.NET Core Identity (with JWT) |
| **Hosting** | Azure App Service (or Docker + Azure Container Apps) |
| **CI/CD** | GitHub Actions |

---

### **Step-by-Step Implementation Plan**

#### **Step 1: Set Up Blazor Frontend (UI)**
1. **Create Blazor WebAssembly App**  
   ```bash
   dotnet new blazorwasm -n AIBuddy --hosted
   ```
2. **Add MudBlazor for UI Components**  
   ```bash
   dotnet add package MudBlazor
   ```
3. **Design Chat Interface**  
   - Use `MudChat` component for messaging  
   - Add a "Buddy Selector" (dropdown for Librarian/Sports/Code buddies)  
   - Implement streaming response UI (using `SignalR` later)  

#### **Step 2: Build Backend API (ASP.NET Core)**
1. **Scaffold API Endpoints**  
   - `/api/chat` (POST) – Handle chat requests  
   - `/api/jira-sync` (POST) – Sync Jira docs (cron job)  
2. **Add Jira API Client**  
   ```csharp
   // Example: Fetch Jira issues with docs
   var issues = await _jiraClient.Issues.GetIssuesAsync("project = DOCS");
   ```
3. **Set Up Auth**  
   - Use `Microsoft.Identity.Web` for JWT validation  
   - Secure `/api/jira-sync` with API key  

#### **Step 3: Jira Data Pipeline**
1. **Sync Jira Docs** (Run daily)  
   - Extract text from Jira issues/comments  
   - Store raw docs in `Azure Blob Storage` (backup)  
2. **Chunk & Embed Docs**  
   - Use `LangChain` (or custom C# lib) to split text into chunks  
   - Generate embeddings via OpenAI:  
     ```python
     # Python script (run as Azure Function)
     embeddings = openai.Embedding.create(input=text_chunk, model="text-embedding-3-small")
     ```
   - Store embeddings in **ChromaDB** (local) or **Pinecone** (cloud)  

#### **Step 4: RAG Implementation (Retrieval-Augmented Generation)**
1. **On User Question**  
   - Embed the question using same model  
   - Query vector DB for top 3 relevant doc chunks  
2. **Feed to LLM**  
   ```csharp
   var prompt = $"""
     Use ONLY the following Jira docs to answer:
     {retrieved_chunks}

     Question: {user_question}
     Answer:
   """;
   var response = await openai.ChatCompletion.Create(
       model: "gpt-4",
       messages: new[] { new { role = "user", content = prompt } }
   );
   ```

#### **Step 5: Real-Time Chat with SignalR**
1. **Add SignalR to Blazor**  
   ```csharp
   // Server
   builder.Services.AddSignalR();
   // Client
   hubConnection = new HubConnectionBuilder()
       .WithUrl("/chatHub")
       .Build();
   ```
2. **Stream Responses**  
   - Backend sends partial responses via `hubContext.Clients.User(userId).SendAsync("ReceiveToken", chunk)`  

#### **Step 6: Deploy to Azure**
1. **Infrastructure**  
   - Azure App Service (Blazor + API)  
   - Azure PostgreSQL (chat history)  
   - Azure Functions (Jira sync cron job)  
2. **CI/CD**  
   - GitHub Actions to auto-deploy on `main` push  

---

### **Key Code Snippets**
#### **Jira Doc Sync (C#)**
```csharp
// Sync all Jira docs to DB
var issues = await jira.Issues.GetIssuesAsync("project = LIBRARY");
foreach (var issue in issues)
{
    var doc = new JiraDoc {
        Id = issue.Key, 
        Text = $"{issue.Summary}\n{issue.Description}",
        LastUpdated = issue.Updated
    };
    await _dbContext.Docs.AddAsync(doc);
}
```

#### **RAG Query (Python)**
```python
# Run in Azure Function
query_embedding = openai.Embedding.create(input=question, model="text-embedding-3-small")
results = chroma_db.query(query_embeddings=[query_embedding], n_results=3)
context = "\n".join(results["documents"][0])
```

---

### **Next Steps**
1. Build the Blazor UI (1-2 days)  
2. Implement Jira sync (1 day)  
3. Set up RAG pipeline (2 days)  
4. Add auth + deploy (1 day)  

Want me to dive deeper into any step (e.g., exact MudBlazor layout or SignalR streaming)?
