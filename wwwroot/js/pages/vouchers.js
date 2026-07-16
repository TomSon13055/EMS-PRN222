document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-copy-button]").forEach((button) => {
        button.addEventListener("click", async () => {
            const target = button.getAttribute("data-copy-button");
            const text = target ? document.querySelector(target)?.textContent?.trim() : "";
            if (!text) return;
            try {
                await navigator.clipboard.writeText(text);
                button.setAttribute("data-copied", "true");
                setTimeout(() => button.removeAttribute("data-copied"), 1500);
            } catch (error) {
                console.error("Unable to copy.", error);
            }
        });
    });
});