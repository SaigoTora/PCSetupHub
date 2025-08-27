"use strict";

const messageForm = document.getElementById("messageForm");
const chatId = messageForm.getAttribute("data-chat-id");
const userId = parseInt(messageForm.getAttribute("data-user-id"));
const inputMessage = document.getElementById("inputMessage");
const sendButton = document.getElementById("sendButton");
const sendFirstButton = document.getElementById("sendFirstButton");


if (sendFirstButton && inputMessage) {
    inputMessage.addEventListener("keydown", function (e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            if (!inputMessage.value) {
                return;
            }
            sendFirstButton.click();
        }
    });
}

if (sendButton && !sendButton.disabled) {
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

    sendButton.addEventListener("click", (e) => {
        const message = inputMessage.value;

        if (!message) {
            return;
        }
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

    connection.on("ReceiveMessage", (response) => {
        const messageList = document.getElementById("messageList");
        if (!messageList) return;

        const dateSeparator = createDateSeparatorIfNeeded(messageList, new Date(response.createdAt));
        if (dateSeparator) {
            messageList.prepend(dateSeparator);
        }

        const messageEl = createMessageElement(response, userId);
        messageList.prepend(messageEl);
        messageEl.scrollIntoView({ behavior: "smooth", block: "start" });

        if (response.senderId !== userId) {
            markAsRead(chatId, [response.messageId]);
        }
    });

    connection.on("MessageRead", function (messageId) {
        const msgEl = document.querySelector(`.message[data-message-id="${messageId}"]`);
        if (!msgEl) return;

        const statusSpan = msgEl.querySelector(".read-status");
        if (statusSpan) {
            statusSpan.classList.remove("is-sent");
            statusSpan.classList.add("is-read");
            statusSpan.title = "Read";
        }

        const statusIcon = msgEl.querySelector(".read-status i");
        if (statusIcon) {
            statusIcon.classList.remove("fa-check");
            statusIcon.classList.add("fa-check-double");
            statusIcon.setAttribute('aria-label', 'Read');
            msgEl.querySelector(".read-status").classList.add("is-read");
        }
    });

    function markAsRead(chatId, messageIds) {
        connection.invoke("MarkMessagesAsRead", chatId, messageIds)
            .catch(err => console.error(err.toString()));
    }
}

function createMessageElement(response, userId) {
    const el = document.createElement("div");
    el.classList.add("message");

    if (response.senderId === userId) {
        el.classList.add("message-out");
    } else {
        el.classList.add("message-in");
    }

    el.setAttribute("data-message-id", response.messageId);
    const textDiv = document.createElement("div");
    textDiv.classList.add("message-text");
    textDiv.innerHTML = response.text.replace(/\n/g, "<br>");

    const bottomDiv = document.createElement("div");
    bottomDiv.classList.add("message-bottom-container");
    const timeSpan = document.createElement("span");
    timeSpan.classList.add("message-time");
    const messageDate = new Date(response.createdAt);
    timeSpan.textContent = messageDate.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    timeSpan.title = messageDate.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
    bottomDiv.appendChild(timeSpan);

    if (response.senderId === userId) {
        const statusSpan = document.createElement("span");
        statusSpan.classList.add("read-status");
        statusSpan.classList.add("is-sent");
        statusSpan.setAttribute('aria-label', 'Message status');
        statusSpan.title = "Sent";

        const statusIcon = document.createElement("i");
        statusIcon.classList.add("fa-solid");
        statusIcon.classList.add("fa-check");
        statusIcon.setAttribute('aria-label', 'Sent');
        statusSpan.appendChild(statusIcon);

        bottomDiv.appendChild(statusSpan);
    }

    el.appendChild(textDiv);
    el.appendChild(bottomDiv);

    return el;
}
function createDateSeparatorIfNeeded(messageList, newMessageDate) {
    const lastMessageDateStr = messageList.getAttribute("data-last-message-date");
    if (!lastMessageDateStr) return null;

    let dateEl = null;

    const [year, month, day] = lastMessageDateStr.split("-").map(Number);
    const lastMessageDate = { year, month, day };

    if (lastMessageDate.year !== newMessageDate.getFullYear() ||
        lastMessageDate.month !== newMessageDate.getMonth() + 1 ||
        lastMessageDate.day !== newMessageDate.getDate()) {

        dateEl = document.createElement("div");
        dateEl.classList.add("message-date-separator");

        const msgDateObj = new Date(year, month - 1, day);
        const currentDate = new Date();

        let options;
        if (year === new Date().getFullYear()) {
            options = { month: "long", day: "2-digit" };
        } else {
            options = { month: "long", day: "2-digit", year: "numeric" };
        }

        dateEl.textContent = newMessageDate.toLocaleDateString("en-US", options);
    }

    const newDateStr = `${newMessageDate.getFullYear()}-${(newMessageDate.getMonth() + 1).toString().padStart(2, '0')}-${newMessageDate.getDate().toString().padStart(2, '0')}`;
    messageList.setAttribute("data-last-message-date", newDateStr);

    return dateEl;
}