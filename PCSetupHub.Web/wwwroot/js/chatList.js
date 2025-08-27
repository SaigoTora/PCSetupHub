"use strict";

const chatList = document.getElementById("chatList");
const userId = parseInt(chatList.getAttribute("data-user-id"));

if (chatList) {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .build();

    connection.start()
        .then(() => {
            if (userId) {
                return connection.invoke("JoinUserGroup", userId);
            }
        })
        .catch(err => console.error(err.toString()));

    connection.on("ReceiveMessage", (response) => {
        const emptyChat = document.getElementById("emptyChat");
        if (emptyChat) {
            emptyChat.remove();
        }

        const chatContainer = document.querySelector(`.message-container[data-chat-public-id="${response.chatPublicId}"]`);
        if (chatContainer) {
            chatContainer.remove();
        }

        const newChatContainer = document.createElement("div");
        newChatContainer.classList.add("message-container");
        newChatContainer.classList.add("mb-1");
        newChatContainer.setAttribute("data-chat-public-id", response.chatPublicId);
        newChatContainer.innerHTML = getChatContainerInnerHTML(response);

        chatList.prepend(newChatContainer);
    });
}

function getChatContainerInnerHTML(response) {
    const createdAt = new Date(response.createdAt);
    const displayCreatedAt = getDisplayDate(createdAt);
    const titleCreatedAt = getTitleDate(createdAt);

    return `
        <a class="message-link" href="/Chat/${response.chatPublicId}">
            <div class="d-flex justify-content-start align-items-start">
                <div class="d-flex align-items-center me-2-5">
                    <img src="${response.senderAvatarUrl}" alt="User avatar ${response.senderLogin}"
                         title="${response.senderLogin}" width="65" height="65"
                         class="user-avatar" />
                </div>
                <div class="d-flex flex-column w-100" style="min-width: 0;">
                    <div class="d-flex justify-content-between mb-1">
                        <div class="d-flex align-items-center">
                            <span class="preview-user-login" title="${response.senderName}">
                                ${response.senderLogin}
                            </span>
                            <i class="fas fa-circle preview-new-message-icon"
                               title="Unread"
                               ${response.senderId !== userId ? '' : 'hidden'}>
                            </i>
                        </div>
                        <div class="preview-message-datetime"
                             title="${titleCreatedAt}">
                            ${displayCreatedAt}
                        </div>
                    </div>
                    <div class="preview-message-text text-truncate ${(response.senderId !== userId) ? 'message-unread' : 'message-read'}"
                         title="${response.text}">
                        ${response.text}
                    </div>
                </div>
            </div>
        </a>
    `;
}
function getDisplayDate(createdAt) {
    const currentDate = new Date();
    let displayCreatedAt = '';

    if (
        createdAt.getUTCFullYear() === currentDate.getUTCFullYear() &&
        createdAt.getUTCMonth() === currentDate.getUTCMonth() &&
        createdAt.getUTCDate() === currentDate.getUTCDate()
    ) {
        // Today
        displayCreatedAt = `Today at ${createdAt.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false })}`;
    }
    else {
        const day = createdAt.getDate();
        const month = createdAt.toLocaleString('en-US', { month: 'long' });

        if (createdAt.getUTCFullYear() === currentDate.getUTCFullYear()) {
            // This year
            const time = createdAt.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
            displayCreatedAt = `${day} ${month} at ${time}`;
        }
        else {
            // Before this year
            const year = createdAt.getFullYear();
            displayCreatedAt = `${day} ${month} ${year}`;
        }
    }

    return displayCreatedAt;
}
function getTitleDate(createdAt) {
    const day = createdAt.getDate().toString().padStart(2, '0');
    const month = createdAt.toLocaleString('en-US', { month: 'long' });
    const year = createdAt.getFullYear();
    const time = createdAt.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false
    });

    return `${day} ${month}, ${year} at ${time}`;
}
