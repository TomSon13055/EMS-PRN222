document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-action-dropdown]").forEach((dropdown) => {
        const trigger = dropdown.querySelector("[data-action-dropdown-trigger]");
        if (!trigger) return;
        trigger.addEventListener("click", (event) => {
            event.stopPropagation();
            const expanded = dropdown.getAttribute("aria-expanded") === "true";
            document.querySelectorAll("[data-action-dropdown][aria-expanded='true']").forEach((open) => {
                if (open !== dropdown) open.setAttribute("aria-expanded", "false");
            });
            dropdown.setAttribute("aria-expanded", expanded ? "false" : "true");
        });
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest("[data-action-dropdown]")) {
            document.querySelectorAll("[data-action-dropdown][aria-expanded='true']").forEach((dropdown) => {
                dropdown.setAttribute("aria-expanded", "false");
            });
        }
    });
});