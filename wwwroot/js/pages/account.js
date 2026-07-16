document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-password-toggle]").forEach((toggle) => {
        const targetSelector = toggle.getAttribute("data-password-toggle");
        const input = targetSelector ? document.querySelector(targetSelector) : null;
        if (!(input instanceof HTMLInputElement)) return;

        toggle.setAttribute("aria-pressed", "false");
        toggle.setAttribute("type", "button");

        toggle.addEventListener("click", () => {
            const isPassword = input.type === "password";
            input.type = isPassword ? "text" : "password";
            toggle.setAttribute("aria-pressed", String(isPassword));

            const icon = toggle.querySelector("i");
            if (icon) {
                icon.classList.toggle("bi-eye");
                icon.classList.toggle("bi-eye-slash");
            }

            const label = toggle.querySelector("[data-password-toggle-label]");
            if (label) {
                label.textContent = isPassword ? "Hide password" : "Show password";
            }
        });
    });
});