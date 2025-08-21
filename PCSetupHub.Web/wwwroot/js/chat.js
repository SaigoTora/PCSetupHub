"use strict";

const sendButton = document.getElementById("sendButton");
const messages = document.getElementById("messages");
const chatId = messages.getAttribute("data-chat-id");
const inputMessage = document.getElementById("inputMessage");
const userId = parseInt(messages.getAttribute("data-user-id"));

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

connection.start()
    .then(() => {
        if (chatId) {
            return connection.invoke("JoinToChat", chatId);
        }
    })
    .catch(err => console.error(err.toString()));

if (sendButton) {
    sendButton.addEventListener("click", (e) => {
        const message = inputMessage.value;
        connection.invoke("SendMessage",
            {
                chatPublicId: chatId,
                senderId: userId,
                text: message
            })
            .then(() => { inputMessage.value = ""; })
            .catch(e => console.error(e.toString()));
        e.preventDefault();
    });

    if (inputMessage) {
        inputMessage.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                sendButton.click();
            }
        });
    }
}

connection.on("ReceiveMessage", (response) => {
    const messageList = document.getElementById("messageList");
    if (!messageList) return;

    const el = document.createElement("div");
    el.classList.add("message");

    if (response.senderId === userId) {
        el.classList.add("message-out");
    } else {
        el.classList.add("message-in");
    }

    const textDiv = document.createElement("div");
    textDiv.classList.add("message-text");

    textDiv.innerHTML = response.text.replace(/\n/g, "<br>");

    const timeDiv = document.createElement("div");
    timeDiv.classList.add("message-time");
    const date = new Date(response.createdAt);
    timeDiv.textContent = date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });

    el.appendChild(textDiv);
    el.appendChild(timeDiv);

    messageList.prepend(el);
    el.scrollIntoView({ behavior: "smooth", block: "start" });
});