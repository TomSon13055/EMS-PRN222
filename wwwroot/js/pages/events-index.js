import { debounce } from "../core/form-utils.js";

document.addEventListener("DOMContentLoaded", () => {
    const searchInputs = document.querySelectorAll("[data-search-input]");
    searchInputs.forEach((input) => {
        if (!(input instanceof HTMLInputElement)) return;
        const formId = input.getAttribute("data-search-input");
        const form = formId ? document.getElementById(formId) : input.closest("form");
        if (!form) return;

        const submit = debounce(() => {
            if (typeof form.requestSubmit === "function") {
                form.requestSubmit();
            } else {
                form.submit();
            }
        }, 500);

        input.addEventListener("input", submit);
    });

    const wishlistButtons = document.querySelectorAll("[data-wishlist-toggle]");
    wishlistButtons.forEach((button) => {
        button.addEventListener("click", (event) => {
            event.preventDefault();
            const form = button.closest("form");
            if (form) form.submit();
        });
    });
});