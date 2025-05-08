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