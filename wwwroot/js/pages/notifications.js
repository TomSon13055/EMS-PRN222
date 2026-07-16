document.addEventListener("DOMContentLoaded", () => {
    const filterButtons = document.querySelectorAll("[data-notification-filter]");
    filterButtons.forEach((button) => {
        button.addEventListener("click", (event) => {
            const target = button.getAttribute("data-notification-filter-target");
            if (!target) return;
            const items = document.querySelectorAll(target);
            const filter = button.getAttribute("data-notification-filter");
            items.forEach((item) => {
                const status = item.getAttribute("data-notification-status");
                if (filter === "all") {
                    item.removeAttribute("hidden");
                } else if (filter === status) {
                    item.removeAttribute("hidden");
                } else {
                    item.setAttribute("hidden", "");
                }
            });
            filterButtons.forEach((b) => b.classList.remove("is-active"));
            button.classList.add("is-active");
        });
    });

    document.querySelectorAll("[data-mark-read-button]").forEach((button) => {
        button.addEventListener("click", (event) => {
            event.preventDefault();
            const form = button.closest("form");
            if (form) form.submit();
        });
    });
});