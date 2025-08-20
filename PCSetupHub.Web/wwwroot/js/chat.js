"use strict";

const sendButton = document.getElementById("sendButton");
const messages = document.getElementById("messages");
const chatId = messages.getAttribute("data-chat-id");
const inputMessage = document.getElementById("inputMessage");

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
        const userId = parseInt(messages.getAttribute("data-user-id"));
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
}

connection.on("ReceiveMessage", (response) => {
    const el = document.createElement("div");
    document.getElementById("messageList").prepend(el);
    el.textContent = `${response.senderId}: ${response.text} (${response.createdAt})`;
});