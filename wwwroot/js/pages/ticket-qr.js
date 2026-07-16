document.addEventListener("DOMContentLoaded", () => {
    const printButton = document.querySelector("[data-print-button]");
    printButton?.addEventListener("click", () => {
        window.print();
    });
});