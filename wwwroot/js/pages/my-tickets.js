document.addEventListener("DOMContentLoaded", () => {
    const filterForm = document.querySelector("[data-my-tickets-filter]");
    if (filterForm instanceof HTMLFormElement) {
        const status = filterForm.querySelector("[data-my-tickets-status]");
        status?.addEventListener("change", () => {
            if (typeof filterForm.requestSubmit === "function") {
                filterForm.requestSubmit();
            } else {
                filterForm.submit();
            }
        });
    }
});