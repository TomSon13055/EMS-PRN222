document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-wishlist-remove]").forEach((form) => {
        form.addEventListener("submit", (event) => {
            const button = form.querySelector("[data-wishlist-remove-button]");
            if (button instanceof HTMLButtonElement && !button.disabled) {
                button.setAttribute("aria-busy", "true");
                button.disabled = true;
            }
        });
    });
});