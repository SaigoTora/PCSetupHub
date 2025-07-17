document.addEventListener('DOMContentLoaded', () => {
    const avatarPreview = document.getElementById('avatar-preview');
    const avatarInput = document.getElementById('avatar-input');
    const cropperImage = document.getElementById('cropper-image');
    const cropperSection = document.getElementById('avatar-cropper-section');
    const saveButton = document.getElementById('save-avatar-btn');
    const userLogin = avatarPreview.dataset.login;
    let cropper;

    avatarPreview.addEventListener('click', () => {
        avatarInput.click();
    });

    avatarInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = () => {
            cropperImage.src = reader.result;
            cropperSection.style.display = 'block';

            if (cropper) cropper.destroy();
            cropper = new Cropper(cropperImage, {
                aspectRatio: 1
            });
        };
        reader.readAsDataURL(file);
    });

    saveButton.addEventListener('click', () => {
        if (!cropper) return;

        const canvas = cropper.getCroppedCanvas({
            width: 256,
            height: 256
        });

        canvas.toBlob((blob) => {
            const formData = new FormData();
            formData.append("file", blob, `${userLogin}.webp`);

            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const uploadUrl = `/Profile/UploadAvatar/${encodeURIComponent(userLogin)}`;

            fetch(uploadUrl, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                body: formData
            })
                .then(() => location.reload())
                .catch(err => alert("An error occurred while loading the avatar."));
        }, 'image/webp', 0.85);
    });
});