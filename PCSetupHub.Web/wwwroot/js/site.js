document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll("[id^='openModal-']").forEach(function (trigger) {
        trigger.addEventListener("click", function () {
            const idSuffix = trigger.id.replace("openModal-", "");
            const modalElement = document.getElementById("modal-" + idSuffix);

            if (modalElement) {
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
            }
        });
    });
});

(function () {
    if (document.cookie.includes("TokenRefreshed=true")) {
        // This is necessary to avoid a reload loop
        document.cookie = "TokenRefreshed=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        location.reload();
    }
})();