const checkbox = document.getElementById('remember');
const warning = document.getElementById('cookieWarning');
const errorContainer = document.getElementById('error-container');

if (checkbox)
    checkbox.addEventListener('change', function () {
        if (warning)
            warning.style.display = this.checked ? 'none' : 'block';

        if (errorContainer) {
            errorContainer.innerHTML = '';
        }
    });

document.getElementById('toggle-password').addEventListener('click', function () {
    const passwordField = document.getElementById('Password');
    const passwordIcon = this.querySelector('i');

    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        passwordIcon.classList.remove('fa-eye');
        passwordIcon.classList.add('fa-eye-slash');
        this.title = 'Hide password';
    } else {
        passwordField.type = 'password';
        passwordIcon.classList.remove('fa-eye-slash');
        passwordIcon.classList.add('fa-eye');
        this.title = 'Show password';
    }
});


warning.style.display = checkbox.checked ? 'none' : 'block';