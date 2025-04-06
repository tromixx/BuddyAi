For a **simple MVP** of your Documentation AI Buddy, you can drastically simplify the tech stack. Here's the **minimal viable setup** to get a working prototype quickly:

---

### **Ultra-Simple MVP Tech Stack**
| Component | Technology | Why? |
|-----------|------------|------|
| **Frontend** | Plain Blazor Server (no WASM) | No separate API needed |
| **UI Library** | MudBlazor | Quick professional UI |
| **LLM** | Groq's free Llama 3 70B API | No setup, no GPU needed |
| **Document Storage** | SQLite (in-memory) | No database setup |
| **Embeddings** | None (for MVP) | Skip RAG complexity initially |
| **Hosting** | Local machine (show via LiveShare/NGROK) | Avoid cloud setup |

---

### **4-Step MVP Implementation**

#### **1. One-File Blazor Server App (30 mins)**
```bash
dotnet new blazorserver -n AIDocBuddy --no-https
dotnet add package MudBlazor
```

**`Pages/Index.razor`** (Complete chat UI):
```html
@page "/"
@inject HttpClient Http

<MudContainer>
    <MudChat Messages="_messages" 
             OnSend="SendMessage" 
             UserName="You" />
</MudContainer>

@code {
    private List<ChatMessage> _messages = new();
    
    private async Task SendMessage(string message)
    {
        _messages.Add(new ChatMessage("You", message));
        
        // Directly call Groq API from frontend (for MVP only!)
        var response = await Http.PostAsJsonAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            new {
                model = "llama3-70b-8192",
                messages = new[] { new { 
                    role = "user", 
                    content = $"Answer as a documentation assistant: {message}" 
                }}
            });
        
        var content = await response.Content.ReadFromJsonAsync<GroqResponse>();
        _messages.Add(new ChatMessage("AI Buddy", content.choices[0].message.content));
    }
    
    public record ChatMessage(string Sender, string Text);
    public record GroqResponse(List<Choice> choices);
    public record Choice(Message message);
    public record Message(string content);
}
```

#### **2. Add Jira Simulator (1 hour)**
```csharp
// Fake "Jira docs" for MVP
public static class JiraSimulator
{
    public static Dictionary<string, string> Docs = new() 
    {
        ["PROJ-123"] = "How to reset passwords: Go to Settings > Security",
        ["PROJ-456"] = "API rate limits: 100 requests/minute"
    };
    
    public static string Search(string query)
    {
        return Docs.FirstOrDefault(d => d.Value.Contains(query)).Value 
               ?? "No matching docs found";
    }
}
```

#### **3. Modify LLM Prompt (15 mins)**
```csharp
// Updated prompt in SendMessage()
content = $"Answer based on these Jira docs: {JiraSimulator.Search(message)}\n\nQuestion: {message}";
```

#### **4. Demo Preparation**
1. **Get Groq API Key** (free): [groq.com](https://console.groq.com)
2. **Run locally**:
```bash
dotnet run
```
3. **Expose temporarily**:
```bash
ngrok http 5000  # Share the ngrok URL
```

---

### **What We Removed for MVP**
1. **No backend API** - Direct frontend-to-LLM calls
2. **No database** - Hardcoded "Jira docs"
3. **No embeddings/RAG** - Simple string matching
4. **No auth** - Local demo doesn't need it
5. **No deployment** - Show via ngrok/LiveShare

---

### **MVP Demo Flow**
1. User asks: *"How do I reset passwords?"*
2. App finds hardcoded Jira doc `PROJ-123`
3. Sends to Llama 3 with prompt:  
   *"Answer based on: 'Go to Settings > Security'\n\nQuestion: How do I reset passwords?"*
4. Displays response:  
   *"According to our docs, you can reset passwords in Settings > Security."*

---

### **Next Steps After MVP**
1. Add real Jira API connection
2. Implement proper RAG with ChromaDB
3. Move to Blazor WASM + proper backend
4. Add authentication

---




*UI*

Here's a **step-by-step guide** to create your Blazor UI with a talking 3D robot companion:

---

### **Step 1: Create Blazor Server App**
1. **Open Visual Studio 2022**
   - Click "Create a new project"
   - Search for "Blazor Server App" (not WebAssembly)
   - Name it `AIBuddyMVP` and click "Next"
   - Framework: `.NET 8.0` → Create

2. **Add MudBlazor** (Package Manager Console):
```bash
Install-Package MudBlazor
```

3. **Configure MudBlazor**:
   - In `Program.cs`, add before `var app = builder.Build();`:
   ```csharp
   builder.Services.AddMudServices();
   ```

   - In `_Imports.razor`, add:
   ```razor
   @using MudBlazor
   @using MudBlazor.Components
   ```

---

### **Step 2: Add 3D Robot (Using Three.js)**
1. **Install JavaScript interop**:
```bash
Install-Package Microsoft.JSInterop
```

2. **Add these files**:

`wwwroot/js/robot.js`:
```javascript
// 3D Robot with mouse-following eyes
export function initRobot() {
    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({ alpha: true });
    renderer.setSize(200, 200);
    document.getElementById('robot-container').appendChild(renderer.domElement);

    // Robot head
    const head = new THREE.Mesh(
        new THREE.SphereGeometry(1, 32, 32),
        new THREE.MeshBasicMaterial({ color: 0x00ff00 })
    );
    scene.add(head);

    // Eyes
    const leftEye = createEye(-0.3, 0.2);
    const rightEye = createEye(0.3, 0.2);
    scene.add(leftEye); scene.add(rightEye);

    camera.position.z = 3;

    function createEye(x, y) {
        const eye = new THREE.Mesh(
            new THREE.SphereGeometry(0.15),
            new THREE.MeshBasicMaterial({ color: 0x000000 })
        );
        eye.position.set(x, y, 0.8);
        return eye;
    }

    // Mouse follow
    document.addEventListener('mousemove', (e) => {
        const x = (e.clientX / window.innerWidth) * 2 - 1;
        const y = -(e.clientY / window.innerHeight) * 2 + 1;
        leftEye.position.x = -0.3 + x * 0.1;
        rightEye.position.x = 0.3 + x * 0.1;
        leftEye.position.y = 0.2 + y * 0.1;
        rightEye.position.y = 0.2 + y * 0.1;
    });

    function animate() {
        requestAnimationFrame(animate);
        renderer.render(scene, camera);
    }
    animate();
}
```

`Pages/_Host.cshtml` (add in `<head>`):
```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js"></script>
<script src="~/js/robot.js" type="module"></script>
```

---

### **Step 3: Create Chat UI with Robot**
Replace `Pages/Index.razor` with:
```razor
@page "/"
@inject IJSRuntime JsRuntime
@inject ISnackbar Snackbar

<div class="app-container">
    <div id="robot-container" @ref="robotRef" class="robot"></div>
    
    <MudPaper Class="chat-paper" Elevation="5">
        <MudChat Messages="_messages" 
                 OnSend="SendMessage" 
                 UserName="You"
                 Avatar="avatars/robot.png">
            <TypingIndicator>
                <MudAvatar Class="robot-talk" Image="avatars/robot_talking.gif"/>
            </TypingIndicator>
        </MudChat>
    </MudPaper>
</div>

@code {
    private ElementReference robotRef;
    private List<ChatMessage> _messages = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            await JsRuntime.InvokeVoidAsync("initRobot");
        }
    }
    
    private async Task SendMessage(string message) {
        _messages.Add(new ChatMessage("You", message, "avatars/user.png"));
        
        // Robot talking animation
        await JsRuntime.InvokeVoidAsync("document.querySelector('.robot-talk').style.display", "block");
        
        // Simulate AI response
        await Task.Delay(1000); // Fake processing time
        var response = GetAIResponse(message);
        _messages.Add(new ChatMessage("Doc Buddy", response, "avatars/robot.png"));
        
        // Stop talking animation
        await JsRuntime.InvokeVoidAsync("document.querySelector('.robot-talk').style.display", "none");
    }
    
    private string GetAIResponse(string query) {
        // MVP: Simple doc matching
        var docs = new Dictionary<string, string> {
            ["login"] = "Use SSO with your company email",
            ["error"] = "Check logs at /var/log/app.log"
        };
        
        var match = docs.FirstOrDefault(d => query.ToLower().Contains(d.Key));
        return match.Value ?? "I'll check the docs and get back to you!";
    }
    
    public record ChatMessage(string Sender, string Text, string Avatar);
}
```

---

### **Step 4: Add Styling**
`wwwroot/css/site.css`:
```css
.app-container {
    display: flex;
    height: 100vh;
}

.robot {
    width: 200px;
    height: 200px;
    margin-right: 20px;
}

.chat-paper {
    flex-grow: 1;
    padding: 20px;
    height: 70vh;
    margin-top: 20px;
}

.robot-talk {
    animation: pulse 0.5s infinite;
}

@keyframes pulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.1); }
    100% { transform: scale(1); }
}
```

---

### **Step 5: Add Robot Assets**
1. Create `wwwroot/avatars/` folder
2. Add these files (or find free ones):
   - `robot.png` (neutral robot)
   - `robot_talking.gif` (animated talking)
   - `user.png` (user avatar)

---

### **Final Steps**
1. **Run the app**:
```bash
dotnet run
```
2. **You'll see**:
   - 3D robot head on the left (eyes follow mouse)
   - Chat interface on the right
   - Robot avatar pulses when "thinking"
   - Simple doc-based responses

---

### **Key Interactions**
1. User asks: *"How to fix login?"*
2. Robot eyes wiggle + talking animation plays
3. Response appears: *"Use SSO with your company email"*
4. Robot returns to idle state

---

### **Next Enhancements**
1. Add real Jira API docs
2. Implement proper RAG
3. Add voice synthesis (robot voice!)
4. More complex robot animations

Want me to provide **pre-made asset files** (robot images/animations) or a **complete project ZIP**?
