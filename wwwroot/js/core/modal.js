export function initAccountMenu() {
    const menus = document.querySelectorAll("[data-account-menu]");
    if (!menus.length) return;

    menus.forEach((menu) => {
        const trigger = menu.querySelector("[data-account-trigger]");
        if (!trigger) return;

        trigger.addEventListener("click", (event) => {
            event.stopPropagation();
            const expanded = menu.getAttribute("aria-expanded") === "true";
            closeAll();
            if (!expanded) {
                menu.setAttribute("aria-expanded", "true");
                trigger.setAttribute("aria-expanded", "true");
            }
        });

        menu.addEventListener("keydown", (event) => {
            if (event.key === "Escape") {
                menu.setAttribute("aria-expanded", "false");
                trigger.setAttribute("aria-expanded", "false");
                trigger.focus();
            }
        });
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest("[data-account-menu]")) {
            closeAll();
        }
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") closeAll();
    });
}

function closeAll() {
    document.querySelectorAll("[data-account-menu][aria-expanded='true']").forEach((menu) => {
        menu.setAttribute("aria-expanded", "false");
        const trigger = menu.querySelector("[data-account-trigger]");
        trigger?.setAttribute("aria-expanded", "false");
    });
}

export function initDropdowns() {
    const dropdowns = document.querySelectorAll("[data-dropdown]");
    dropdowns.forEach((dropdown) => {
        const trigger = dropdown.querySelector("[data-dropdown-trigger]");
        if (!trigger) return;
        trigger.addEventListener("click", (event) => {
            event.stopPropagation();
            const expanded = dropdown.getAttribute("aria-expanded") === "true";
            document.querySelectorAll("[data-dropdown][aria-expanded='true']").forEach((open) => {
                if (open !== dropdown) {
                    open.setAttribute("aria-expanded", "false");
                }
            });
            dropdown.setAttribute("aria-expanded", expanded ? "false" : "true");
        });
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest("[data-dropdown]")) {
            document.querySelectorAll("[data-dropdown][aria-expanded='true']").forEach((dropdown) => {
                dropdown.setAttribute("aria-expanded", "false");
            });
        }
    });

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            document.querySelectorAll("[data-dropdown][aria-expanded='true']").forEach((dropdown) => {
                dropdown.setAttribute("aria-expanded", "false");
            });
        }
    });
}