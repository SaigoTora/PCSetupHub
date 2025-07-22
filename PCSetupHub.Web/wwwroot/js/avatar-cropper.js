document.addEventListener('DOMContentLoaded', () => {
    const avatarPreview = document.getElementById('avatar-preview');
    const buttonChangeAvatar = document.getElementById('button-change-avatar');

    if (!avatarPreview.dataset.canEdit) return;

    const avatarInput = document.getElementById('avatar-input');
    const cropperImage = document.getElementById('cropper-image');
    const cropperSection = document.getElementById('avatar-cropper-section');
    const saveButton = document.getElementById('save-avatar-btn');
    const cancelButton = document.getElementById('cancel-avatar-btn');
    const userLogin = avatarPreview.dataset.login;
    let cropper;

    avatarPreview.addEventListener('click', () => {
        avatarInput.click();
    });

    if (buttonChangeAvatar) {
        buttonChangeAvatar.addEventListener('click', () => {
            avatarInput.click();
        });
    }

    avatarInput.addEventListener('change', (e) => {
        const file = e.target.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = () => {
            cropperImage.src = reader.result;
            cropperSection.style.display = 'block';

            if (cropper) cropper.destroy();
            let initialRatio = null;
            cropper = new Cropper(cropperImage, {
                aspectRatio: 1,
                viewMode: 2,
                minCropBoxWidth: 128,
                minCropBoxHeight: 128,

                ready() {
                    // Save the original image scale
                    const imageData = cropper.getImageData();
                    initialRatio = imageData.width / imageData.naturalWidth;
                },

                zoom(event) {
                    if (!initialRatio) return;

                    const newRatio = event.detail.ratio;
                    const maxRatio = initialRatio * 3;

                    if (newRatio > maxRatio) {
                        event.preventDefault();
                        cropper.zoomTo(maxRatio);
                    }
                }
            });
        };
        reader.readAsDataURL(file);
    });

    saveButton.addEventListener('click', async () => {
        if (!cropper) return;

        const canvas = cropper.getCroppedCanvas({
            width: 400,
            height: 400
        });

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const uploadUrl = `/Profile/UploadAvatar/${encodeURIComponent(userLogin)}`;

        const blob = await new Promise(resolve => {
            canvas.toBlob(blob => resolve(blob), 'image/webp', 0.85);
        });

        const formData = new FormData();
        formData.append("file", blob, `${userLogin}.webp`);

        saveButton.disabled = true;
        try {
            const response = await fetch(uploadUrl, {
                method: 'POST',
                headers: { 'RequestVerificationToken': token },
                body: formData
            });

            if (!response.ok) {
                throw new Error(`Upload failed with status ${response.status}`);
            }

            location.reload();
        }
        catch (err) {
            alert("An error occurred while uploading the avatar.");
            console.error(err);
        }
        finally {
            saveButton.disabled = false;
        }
    });

    cancelButton.addEventListener('click', () => {
        if (cropper) {
            cropper.destroy();
            cropper = null;
        }

        cropperImage.src = '#';
        cropperSection.style.display = 'none';
        avatarInput.value = ''; // Reset the file input
    });

});