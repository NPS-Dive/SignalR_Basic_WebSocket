var chatterName = "Visitor";
var dialogEl = document.getElementById("chatDialog");

// Initialize SignalR
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", function (name, time, message) {
    console.log(`Received message from ${name} at ${time}: ${message}`);
    renderMessage(name, time, message);
});

connection.onclose(function () {
    onDisconnected();
    console.log("Reconnecting in 5 seconds...");
    setTimeout(startConnection, 5000);
});

document.addEventListener("DOMContentLoaded", ready);

function renderMessage(name, time, message) {
    console.log("Rendering message:", name, time, message);
    var nameSpan = document.createElement("span");
    nameSpan.className = "name";
    nameSpan.textContent = name;

    var timeSpan = document.createElement("span");
    timeSpan.className = "time";
    var timeFriendly = moment(time).format("H:mm:ss");
    timeSpan.textContent = timeFriendly;

    var headerDiv = document.createElement("div");
    headerDiv.appendChild(nameSpan);
    headerDiv.appendChild(timeSpan);

    var messageDiv = document.createElement("div");
    messageDiv.className = "message";
    messageDiv.textContent = message;

    var newItem = document.createElement("li");
    newItem.appendChild(headerDiv);
    newItem.appendChild(messageDiv);

    var chatHistory = document.getElementById("chatHistory");
    if (chatHistory) {
        chatHistory.appendChild(newItem);
        chatHistory.scrollTop = chatHistory.scrollHeight - chatHistory.clientHeight;
        console.log("Message appended to chat history");
    } else {
        console.error("chatHistory element not found");
    }
}

function ready() {
    var chatForm = document.getElementById("chatForm");
    if (!chatForm) {
        console.error("Chat form not found");
        return;
    }

    chatForm.addEventListener("submit", function (e) {
        e.preventDefault();
        console.log("Form submitted");
        var text = e.target[0].value;
        if (text && text.length) {
            e.target[0].value = "";
            sendMessage(text);
        }
    });

    var welcomeForm = document.getElementById("chatConnectForm");
    if (!welcomeForm) {
        console.error("Welcome form not found");
        return;
    }

    welcomeForm.addEventListener("submit", function (e) {
        e.preventDefault();
        var name = e.target[0].value;
        if (name && name.length) {
            document.getElementById("chatWelcomePanel").style.display = "none";
            chatterName = name;
            startConnection();
        }
    });
}

function sendMessage(text) {
    if (connection.state === signalR.HubConnectionState.Connected) {
        console.log("Sending message:", text);
        connection
            .invoke("SendMessage", chatterName, text)
            .then(() => console.log("Message sent successfully"))
            .catch((err) => console.error("SendMessage Error:", err));
    } else {
        console.warn("Cannot send message: SignalR not connected");
    }
}

function startConnection() {
    connection
        .start()
        .then(onConnected)
        .catch(function (err) {
            console.error("Connection Error:", err);
        });
}

function onConnected() {
    dialogEl.classList.remove("disconnected");
    var messageTextBox = document.getElementById("messageTextBox");
    messageTextBox.focus();
    connection.invoke("SetName", chatterName);
}

function onDisconnected() {
    dialogEl.classList.add("disconnected"); 
}