// Neumorphism Login Form JavaScript (refactored)
// قرار دهی این فایل در مسیر ~/Student/LoginStudent/script.js

class NeumorphismLoginForm {
    constructor() {
        // DOM references (به هنگام ساخته شدن نمونه خوانده می‌شوند)
        this.form = document.querySelector(".login-form");
        this.card = document.querySelector(".login-card");
        this.successMessage = document.getElementById("successMessage");
        this.socialButtons = document.querySelectorAll(".neu-social");

        // bound handlers ذخیره می‌شوند تا قابل حذف باشند
        this._onMouseMove = null;
        this._onSubmit = null;
        this._onEmailBlur = null;
        this._onPasswordBlur = null;
        this._onEmailInput = null;
        this._onPasswordInput = null;

        // اگر فرم وجود نداشت، سازنده را خاتمه بده
        if (!this.form) return;

        this.emailInput = this.form.querySelector("#email");
        this.passwordInput = this.form.querySelector("#password");
        this.passwordToggle = this.form.querySelector("#passwordToggle");
        this.submitButton = this.form.querySelector(".login-btn");

        this.init();
    }

    init() {
        // bind ایمن فراخوانی‌ها
        this._onMouseMove = (e) => this.updateAmbientLight(e);
        this._onSubmit = (e) => this.handleSubmit(e);
        this._onEmailBlur = () => this.validateEmail();
        this._onPasswordBlur = () => this.validatePassword();
        this._onEmailInput = () => this.clearError("email");
        this._onPasswordInput = () => this.clearError("password");

        document.addEventListener("mousemove", this._onMouseMove);

        this.form.addEventListener("submit", this._onSubmit);

        if (this.emailInput) {
            this.emailInput.addEventListener("blur", this._onEmailBlur);
            this.emailInput.addEventListener("input", this._onEmailInput);
            this.emailInput.addEventListener("focus", (e) => this.addSoftPress(e));
            this.emailInput.addEventListener("blur", (e) => this.removeSoftPress(e));
        }

        if (this.passwordInput) {
            this.passwordInput.addEventListener("blur", this._onPasswordBlur);
            this.passwordInput.addEventListener("input", this._onPasswordInput);
            this.passwordInput.addEventListener("focus", (e) => this.addSoftPress(e));
            this.passwordInput.addEventListener("blur", (e) => this.removeSoftPress(e));
        }

        this.setupPasswordToggle();
        this.setupSocialButtons();
        this.setupNeumorphicEffects();
    }

    setupPasswordToggle() {
        if (!this.passwordToggle || !this.passwordInput) return;

        // از یک listener مشترک استفاده کن تا حذف شود
        this._passwordToggleHandler = () => {
            const type = this.passwordInput.type === "password" ? "text" : "password";
            this.passwordInput.type = type;
            this.passwordToggle.classList.toggle("show-password", type === "text");
            this.animateSoftPress(this.passwordToggle);
        };

        this.passwordToggle.addEventListener("click", this._passwordToggleHandler);
    }

    setupSocialButtons() {
        if (!this.socialButtons) return;

        // حذف listener قبلی اگر بوده با دانستن اینکه ممکن است چند نمونه ساخته شود
        this._socialHandlers = [];

        this.socialButtons.forEach((button) => {
            const handler = (e) => {
                this.animateSoftPress(button);
                // تطبیق provider با روش امن تر: data-provider attribute اگر داشت، استفاده کن
                let provider = button.dataset.provider || "Social";
                // fallback: تلاش برای تشخیص از محتویات svg
                try {
                    const path = button.querySelector("svg path");
                    if (path) {
                        const d = path.getAttribute("d") || "";
                        if (d.includes("22.56")) provider = "Google";
                        else if (d.toLowerCase().includes("github")) provider = "GitHub";
                        else if (d.includes("23.953")) provider = "Twitter";
                    }
                } catch (err) { /* ignore */ }

                this.handleSocialLogin(provider, button);
            };

            this._socialHandlers.push({ button, handler });
            button.addEventListener("click", handler);
        });
    }

    setupNeumorphicEffects() {
        // hover scale effects
        const neuElements = document.querySelectorAll(".neu-icon, .neu-checkbox, .neu-social");
        this._hoverHandlers = [];

        neuElements.forEach((element) => {
            const enter = () => element.style.transform = "scale(1.05)";
            const leave = () => element.style.transform = "scale(1)";

            element.addEventListener("mouseenter", enter);
            element.addEventListener("mouseleave", leave);

            this._hoverHandlers.push({ element, enter, leave });
        });
    }

    updateAmbientLight(e) {
        if (!this.card) return;
        const rect = this.card.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;
        const centerX = rect.width / 2;
        const centerY = rect.height / 2;
        const angleX = (x - centerX) / centerX;
        const angleY = (y - centerY) / centerY;
        const shadowX = angleX * 30;
        const shadowY = angleY * 30;

        this.card.style.boxShadow = `
            ${shadowX}px ${shadowY}px 60px #a9b7f5ff,
            ${-shadowX}px ${-shadowY}px 60px #ddf0ffff
        `;
    }

    addSoftPress(e) {
        const inputGroup = e.target.closest(".neu-input");
        if (inputGroup) inputGroup.style.transform = "scale(0.98)";
    }

    removeSoftPress(e) {
        const inputGroup = e.target.closest(".neu-input");
        if (inputGroup) inputGroup.style.transform = "scale(1)";
    }

    animateSoftPress(element) {
        if (!element) return;
        element.classList.add("press");
        setTimeout(() => {
            element.classList.remove("press");
        }, 150);
    }

    validateEmail() {
        if (!this.emailInput) return true;
        const email = this.emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email) {
            this.showErrorByElement(this.emailInput, "Email is required");
            return false;
        }
        if (!emailRegex.test(email)) {
            this.showErrorByElement(this.emailInput, "Please enter a valid email");
            return false;
        }
        this.clearErrorByElement(this.emailInput);
        return true;
    }

    validatePassword() {
        if (!this.passwordInput) return true;
        const password = this.passwordInput.value;
        if (!password) {
            this.showErrorByElement(this.passwordInput, "Password is required");
            return false;
        }
        if (password.length < 6) {
            this.showErrorByElement(this.passwordInput, "Password must be at least 6 characters");
            return false;
        }
        this.clearErrorByElement(this.passwordInput);
        return true;
    }

    showError(field, message) {
        const el = document.getElementById(field);
        if (el) this.showErrorByElement(el, message);
    }

    showErrorByElement(inputElement, message) {
        if (!inputElement) return;
        const fieldId = inputElement.id;
        const formGroup = inputElement.closest(".form-group");
        const errorElement = formGroup ? formGroup.querySelector(".error-message") : null;

        if (formGroup) formGroup.classList.add("error");
        if (errorElement) {
            errorElement.textContent = message;
            errorElement.classList.add("show");
        }

        inputElement.style.animation = "gentleShake 0.5s ease-in-out";
        setTimeout(() => {
            inputElement.style.animation = "";
        }, 500);
    }

    clearError(field) {
        const el = document.getElementById(field);
        if (el) this.clearErrorByElement(el);
    }

    clearErrorByElement(inputElement) {
        if (!inputElement) return;
        const formGroup = inputElement.closest(".form-group");
        const errorElement = formGroup ? formGroup.querySelector(".error-message") : null;

        if (formGroup) formGroup.classList.remove("error");
        if (errorElement) {
            errorElement.classList.remove("show");
            setTimeout(() => {
                errorElement.textContent = "";
            }, 300);
        }
    }

    async handleSubmit(e) {
        e.preventDefault();

        const isEmailValid = this.validateEmail();
        const isPasswordValid = this.validatePassword();

        if (!isEmailValid || !isPasswordValid) {
            this.animateSoftPress(this.submitButton);
            return;
        }

        this.setLoading(true);

        try {
            await new Promise((resolve) => setTimeout(resolve, 1200));
            this.showNeumorphicSuccess();
        } catch (error) {
            this.showErrorByElement(this.passwordInput, "Login failed. Please try again.");
        } finally {
            this.setLoading(false);
        }
    }

    async handleSocialLogin(provider, button) {
        console.log(`Initiating ${provider} login...`);
        if (!button) return;
        button.style.pointerEvents = "none";
        button.style.opacity = "0.7";

        try {
            await new Promise((resolve) => setTimeout(resolve, 900));
            console.log(`Redirecting to ${provider} authentication...`);
        } catch (error) {
            console.error(`${provider} authentication failed: ${error && error.message}`);
        } finally {
            button.style.pointerEvents = "auto";
            button.style.opacity = "1";
        }
    }

    setLoading(loading) {
        if (this.submitButton) {
            this.submitButton.classList.toggle("loading", loading);
            this.submitButton.disabled = loading;
        }
        if (this.socialButtons && this.socialButtons.length) {
            this.socialButtons.forEach((button) => {
                button.style.pointerEvents = loading ? "none" : "auto";
                button.style.opacity = loading ? "0.6" : "1";
            });
        }
    }

    showNeumorphicSuccess() {
        if (this.form) {
            this.form.style.transform = "scale(0.95)";
            this.form.style.opacity = "0";
        }

        setTimeout(() => {
            if (this.form) this.form.style.display = "none";
            const social = document.querySelector(".social-login");
            const signup = document.querySelector(".signup-link");
            if (social) social.style.display = "none";
            if (signup) signup.style.display = "none";
            if (this.successMessage) this.successMessage.classList.add("show");
            const successIcon = this.successMessage ? this.successMessage.querySelector(".neu-icon") : null;
            if (successIcon) successIcon.style.animation = "successPulse 0.6s ease-out";
        }, 300);

        setTimeout(() => {
            console.log("Redirecting to dashboard...");
        }, 2000);
    }

    // remove all event listeners and revert inline styling
    destroy() {
        try {
            if (this._onMouseMove) document.removeEventListener("mousemove", this._onMouseMove);

            if (this.form && this._onSubmit) this.form.removeEventListener("submit", this._onSubmit);

            if (this.emailInput) {
                this.emailInput.removeEventListener("blur", this._onEmailBlur);
                this.emailInput.removeEventListener("input", this._onEmailInput);
            }

            if (this.passwordInput) {
                this.passwordInput.removeEventListener("blur", this._onPasswordBlur);
                this.passwordInput.removeEventListener("input", this._onPasswordInput);
            }

            if (this.passwordToggle && this._passwordToggleHandler) {
                this.passwordToggle.removeEventListener("click", this._passwordToggleHandler);
            }

            if (this._socialHandlers && this._socialHandlers.length) {
                this._socialHandlers.forEach(({ button, handler }) => {
                    if (button && handler) button.removeEventListener("click", handler);
                });
            }

            if (this._hoverHandlers && this._hoverHandlers.length) {
                this._hoverHandlers.forEach(({ element, enter, leave }) => {
                    if (element) {
                        element.removeEventListener("mouseenter", enter);
                        element.removeEventListener("mouseleave", leave);
                        element.style.transform = "";
                    }
                });
            }

            if (this.card) this.card.style.boxShadow = "";
            if (this.form) {
                this.form.style.transform = "";
                this.form.style.opacity = "";
                this.form.style.display = "";
            }

            // clear refs
            this.form = null;
            this.card = null;
            this.emailInput = null;
            this.passwordInput = null;
            this.passwordToggle = null;
            this.submitButton = null;
            this.successMessage = null;
            this.socialButtons = null;
        } catch (err) {
            console.error('Error during NeumorphismLoginForm.destroy:', err);
        }
    }
}

// keyframes injection (only once)
if (!document.querySelector("#neu-keyframes")) {
    const style = document.createElement("style");
    style.id = "neu-keyframes";
    style.textContent = `
        @keyframes gentleShake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-3px); }
            75% { transform: translateX(3px); }
        }
        @keyframes successPulse {
            0% { transform: scale(0.8); opacity: 0; }
            50% { transform: scale(1.1); }
            100% { transform: scale(1); opacity: 1; }
        }
    `;
    document.head.appendChild(style);
}

// auto-initialize if a static form exists on DOMContentLoaded
document.addEventListener("DOMContentLoaded", function () {
    if (!window._neumorphismInstance) {
        window._neumorphismInstance = new NeumorphismLoginForm();
    }
});
