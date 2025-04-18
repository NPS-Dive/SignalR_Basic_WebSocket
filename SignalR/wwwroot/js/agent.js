var activeRoomId = "";
var agentConnection = new signalR.HubConnectionBuilder()
    .withUrl("/agenthub")
    .build();

var chatConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

agentConnection.on("ActiveRooms", loadRooms);
agentConnection.on("ReceiveMessages", addMessages);
chatConnection.on("ReceiveMessage", addMessage);

agentConnection.onclose(() => {
    handleDisconnected(startAgentConnection);
});

chatConnection.onclose(() => {
    handleDisconnected(startChatConnection);
});

var roomListElement = document.getElementById("roomList");
var roomHistoryElement = document.getElementById("chatHistory");

roomListElement.addEventListener("click", (e) => {
    if (e.target.tagName === "A") {
        roomHistoryElement.style.display = "block";
        setActiveRoomButton(e.target);
        var roomId = e.target.getAttribute("data-id");
        switchActiveRoomTo(roomId);
    }
});

document.addEventListener("DOMContentLoaded", ready);

function loadRooms(rooms) {
    if (!rooms) return;

    var roomIds = Object.keys(rooms);
    if (!roomIds.length) return;

    switchActiveRoomTo(null);
    removeAllChildren(roomListElement);

    roomIds.forEach((id) => {
        var roomInfo = rooms[id];
        if (!roomInfo.name) return;

        var roomButton = CreateRoomButton(id, roomInfo);
        roomListElement.appendChild(roomButton);
    });
}

function switchActiveRoomTo(roomId) {
    if (roomId === activeRoomId) return;

    if (activeRoomId) {
        chatConnection.invoke("LeaveRoom", activeRoomId)
            .catch(err => console.error("LeaveRoom Error:", err));
    }

    activeRoomId = roomId;
    removeAllChildren(roomHistoryElement);

    if (!roomId) return;

    chatConnection.invoke("JoinRoom", activeRoomId)
        .catch(err => console.error("JoinRoom Error:", err));
    agentConnection.invoke("LoadHistory", activeRoomId)
        .catch(err => console.error("LoadHistory Error:", err));
}

function removeAllChildren(node) {
    if (!node) return;
    while (node.lastChild) {
        node.removeChild(node.lastChild);
    }
}

function handleDisconnected(retryFunc) {
    setTimeout(retryFunc, 5000);
}

function startChatConnection() {
    chatConnection.start()
        .then(() => console.log("Chat Connection Started"))
        .catch(err => console.error("Chat Connection Error:", err));
}

function startAgentConnection() {
    agentConnection.start()
        .then(() => console.log("Agent Connection Started"))
        .catch(err => console.error("Agent Connection Error:", err));
}

function CreateRoomButton(id, roomInfo) {
    var anchorElement = document.createElement("a");
    anchorElement.className = "list-group-item list-group-item-action d-flex justify-content-between align-items-center";
    anchorElement.setAttribute("data-id", id);
    anchorElement.textContent = roomInfo.name;
    anchorElement.href = "#";
    return anchorElement;
}

function sendMessage(text) {
    if (text && text.length && activeRoomId) {
        agentConnection.invoke("SendAgentMessages", activeRoomId, text)
            .catch(err => console.error("SendAgentMessages Error:", err));
    }
}

function ready() {
    startAgentConnection();
    startChatConnection();

    var chatFormElement = document.getElementById("chatForm");
    if (!chatFormElement) {
        console.error("Chat form not found");
        return;
    }

    chatFormElement.addEventListener("submit", (e) => {
        e.preventDefault();
        var text = e.target[0].value;
        if (text && text.length) {
            e.target[0].value = "";
            sendMessage(text);
        }
    });
}

function setActiveRoomButton(element) {
    var allButtons = roomListElement.querySelectorAll("a.list-group-item");
    allButtons.forEach(btn => btn.classList.remove("active"));
    element.classList.add("active");
}

function addMessages(messages) {
    console.log("Received messages:", messages); // Debug log
    if (!messages) return;
    messages.forEach(m => {
        console.log("Processing message:", m); // Debug log
        addMessage(m.SenderName, m.SentAt, m.Text);
    });
}

function addMessage(name, time, message) {
    console.log("Adding message:", { name, time, message }); // Debug log
    var nameSpan = document.createElement("span");
    nameSpan.className = "name";
    nameSpan.textContent = name || "Unknown";

    var timeSpan = document.createElement("span");
    timeSpan.className = "time";
    var friendlyTime = moment(time).format("H:mm:ss");
    timeSpan.textContent = friendlyTime;

    var headerDiv = document.createElement("div");
    headerDiv.appendChild(nameSpan);
    headerDiv.appendChild(timeSpan);

    var messageDiv = document.createElement("div");
    messageDiv.className = "message";
    messageDiv.textContent = message || "[Empty message]"; // Fallback for undefined

    var newItem = document.createElement("li");
    newItem.appendChild(headerDiv);
    newItem.appendChild(messageDiv);

    roomHistoryElement.appendChild(newItem);
    roomHistoryElement.scrollTop = roomHistoryElement.scrollHeight - roomHistoryElement.clientHeight;
}