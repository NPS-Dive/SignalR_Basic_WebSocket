# SignalR_Basic_WebSocket

A real-time chat application built with ASP.NET Core and SignalR, demonstrating WebSocket-based communication between visitors and support agents.

## Overview

`SignalR_Basic_WebSocket` is a lightweight chat application that enables unauthenticated visitors to start chat sessions and authenticated support agents to respond in real-time. Powered by SignalR’s WebSocket capabilities, it supports group-based chat rooms, in-memory message storage, and a responsive UI styled with Bootstrap.

### Features
- **Real-Time Messaging**: Instant message exchange between visitors and agents.
- **Chat Rooms**: Unique chat rooms for each visitor, joinable by agents.
- **Chat History**: Messages stored in memory and displayed chronologically.
- **Agent Authentication**: Secure login for agents using cookie authentication.
- **Responsive UI**: Built with Bootstrap 4.6.2 and custom CSS.
- **CORS Support**: Configured for WebSocket connections from specified origins.

## Project Structure

```markdown
SignalR_Basic_WebSocket/
├── wwwroot/
│   ├── css/
│   │   └── chat.css           # Consolidated styles
│   ├── js/
│   │   ├── agent.js          # Agent dashboard logic
│   │   └── chat.js           # Visitor chat logic
├── Hubs/
│   ├── AgentHub.cs           # SignalR hub for agents
│   ├── ChatHub.cs            # SignalR hub for visitors
├── Models/
│   ├── ChatMessage.cs        # Message model
│   ├── ChatRoom.cs           # Room model
├── Pages/
│   ├── Index.cshtml          # Visitor chat page
│   ├── ChatSupportAgent.cshtml  # Agent dashboard
│   ├── Login.cshtml          # Agent login page
├── Services/
│   ├── IChatRoomService.cs   # Chat room service interface
│   ├── InMemoryChatRoomService.cs # In-memory service
├── Program.cs                # Application startup
└── README.md                 # This file

```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or any .NET-compatible IDE (e.g., VS Code)
- A modern web browser (Chrome, Firefox, Edge)

## Setup Instructions

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-username/SignalR_Basic_WebSocket.git
   cd SignalR_Basic_WebSocket

2. **Restore Dependencies**:
   ```bash
   dotnet restore
3. **Update CORS Configuration**:
   Open Program.cs and ensure the CORS policy matches your client URL:
   ```csharp
   builder.Services.AddCors(options =>
   {
    options.AddPolicy("ChatPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:5094") // Update if port differs
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials();
    });
   });

  - Verify the port in your browser’s address bar (e.g., 5094).

 4. **Run the Application**:
      ```bash
      dotnet run

  - Or open the solution in Visual Studio and press F5.
  - The app runs at http://localhost:5094 (or your configured port).


## Usage
**Visitor Chat**
1. Navigate to http://localhost:5094.
2. Enter a name in the welcome form and click Connect.
3. Confirm the welcome message ("Hey there, welcome!") appears in the chat history.
4. Send a message (e.g., "Hello") and verify it displays.

**Agent Dashboard**
1. Open http://localhost:5094/Login in a new tab.
2. Log in with:
 - Username: amir123
 - Password: Amir@123
3. Go to http://localhost:5094/ChatSupportAgent.
4. Select a visitor’s room (e.g., "Chat with [VisitorName] from IPBSES").
5. Verify the chat history loads and send a message, which should appear in both the agent and visitor interfaces.

**Debugging**
- Browser Console: Use developer tools (F12) → Console to check SignalR logs (e.g., "Chat Connection Started").
- Network Tab: Filter by "WS" to confirm /chathub and /agenthub WebSocket connections (status 101).
- Server Logs: View Visual Studio’s Output window or terminal for server-side errors.

**Troubleshooting**
- Chat History Not Updating:
 * Check console for errors (e.g., "SendMessage Error").
 * Ensure WebSocket connections are active in the Network tab.
 * Verify clients are in the same room (group).

- CORS Errors:
 * Ensure Program.cs’s WithOrigins matches your URL (e.g., http://localhost:5094).
 * Check Network tab for CORS issues.

- Message Text Missing:
 * Inspect agent.js console logs for Received messages and Adding message.
 * Confirm ChatMessage.Text is sent from AgentHub.cs.

## Contributing
Contributions are welcome! To contribute:
```
1- Fork the repository.
2- Create a feature branch (git checkout -b feature/your-feature).
3- Commit changes (git commit -m "Add your feature").
4- Push to the branch (git push origin feature/your-feature).
5- Open a pull request.
6- Include tests and update documentation where applicable.
```
## License
This project is licensed under the MIT License. See the  file for details.

## Acknowledgments
- Powered by ASP.NET Core and SignalR.
- Styled with Bootstrap 4.6.2.
- Timestamp formatting via Moment.js.

## Contact
For questions or feedback, open an issue on GitHub or contact the maintainer at **[amirmohammadshi@gmail.com]**.
