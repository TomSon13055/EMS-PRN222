document.addEventListener("DOMContentLoaded", () => {
    const filterForms = document.querySelectorAll("[data-host-filter]");
    filterForms.forEach((form) => {
        const status = form.querySelector("[data-host-status]");
        status?.addEventListener("change", () => {
            if (typeof form.requestSubmit === "function") form.requestSubmit();
            else form.submit();
        });
    });
});