Here's your refined README.md with Jira documentation added and clearer UI improvement goals:

# AI Documentation Buddy (MVP)

## Tech Stack
| Component          | Technology               | Purpose                          |
|--------------------|--------------------------|----------------------------------|
| Frontend           | Blazor Server            | Integrated frontend/backend      |
| UI Framework       | MudBlazor                | Professional component library   |
| AI Service         | Groq's Llama 3 70B API   | Free, high-performance LLM       |
| Documentation      | Jira REST API            | Real-time project documentation  |
| State Management   | In-memory Storage        | Lightweight session storage      |

## Quick Start
```bash
# Clone and run
git clone https://your-repo.git
cd AIDocBuddy
dotnet run
```

Access: `https://localhost:7222`

## Current Implementation
![Current UI](https://via.placeholder.com/800x400?text=Current+UI+Screenshot)

### Features Working
- Basic chat interface shell
- Static 3D robot placeholder
- Mock Jira data responses
- Mobile-responsive layout (needs improvement)

## Priority Upgrades

### UI/UX Overhaul (Phase 1)
1. **Chat Interface**
   - [ ] Implement proper `MudChat` component
   - [ ] Add typing indicators and read receipts
   - [ ] Enable markdown/formatted responses

2. **3D Robot Upgrade**
   - [ ] Replace placeholder with high-quality Three.js model
   - [ ] Add emotional states (thinking, speaking, idle)
   - [ ] Implement gaze tracking

3. **Navigation**
   - [ ] Clean up menu bar with `MudAppBar`
   - [ ] Add responsive drawer component
   - [ ] Implement breadcrumb navigation

```razor
<MudAppBar Elevation="2">
    <MudIconButton Icon="@Icons.Material.Menu" Color="Color.Inherit"/>
    <MudText Typo="Typo.h6">DocBuddy</MudText>
</MudAppBar>
```

### Jira Integration (Phase 2)
1. **API Connection**
   - [ ] Implement OAuth2 authentication
   - [ ] Create Jira service wrapper
   - [ ] Add document caching

2. **UI Components**
   - [ ] Jira issue preview cards
   - [ ] Project selector dropdown
   - [ ] Documentation search panel

```csharp
// Jira service example
public async Task<JiraDoc> GetDocAsync(string issueKey) {
    return await _jiraClient.Issues.GetIssueAsync(issueKey);
}
```

## Roadmap

### Near-Term
- [ ] Password protected access
- [ ] Session persistence
- [ ] Mobile layout fixes

### Future
- [ ] Voice interaction
- [ ] Documentation version diffing
- [ ] Team collaboration features

## How to Contribute
1. UI/Design Help:
   - Improve Three.js robot model
   - Create custom MudBlazor theme

2. Backend Help:
   - Implement Jira webhook integration
   - Build document vectorization

> **Pro Tip**: Use `MudThemeProvider` to customize colors and styling

Key changes made:
1. Added explicit Jira API to tech stack
2. Included placeholder for current UI screenshot
3. Organized upgrades into clear phases
4. Added specific MudBlazor component examples
5. Created dedicated "How to Contribute" section
6. Added code samples for both UI and Jira integration
7. Improved visual hierarchy with more markdown elements

Would you like me to:
1. Add specific implementation tutorials for any components?
2. Include recommended Three.js resources for the robot upgrade?
3. Expand the Jira API integration details?
