document.addEventListener("DOMContentLoaded", () => {
    const codeInput = document.querySelector("[data-voucher-code-input]");
    const discountInput = document.querySelector("[data-voucher-discount-input]");
    const limitInput = document.querySelector("[asp-for='UsageLimit'], [name='UsageLimit']");
    const previewCode = document.querySelector("[data-preview-code]");
    const previewDiscount = document.querySelector("[data-preview-discount]");
    const previewLimit = document.querySelector("[data-preview-limit]");

    const updatePreview = () => {
        if (previewCode && codeInput instanceof HTMLInputElement) {
            const v = codeInput.value.trim().toUpperCase();
            previewCode.textContent = v || "EMS-SPRING";
        }
        if (previewDiscount && discountInput instanceof HTMLInputElement) {
            const v = Number(discountInput.value);
            previewDiscount.textContent = v > 0 ? v.toString() : "10";
        }
        if (previewLimit && limitInput instanceof HTMLInputElement) {
            const v = Number(limitInput.value);
            previewLimit.textContent = v > 0 ? v.toString() : "100";
        }
    };

    codeInput?.addEventListener("input", updatePreview);
    discountInput?.addEventListener("input", updatePreview);
    limitInput?.addEventListener("input", updatePreview);

    // Code generator
    const generator = document.querySelector("[data-generate-code-input]");
    generator?.addEventListener("click", () => {
        const targetSelector = generator.getAttribute("data-generate-code-target");
        if (!targetSelector) return;
        const target = document.querySelector(targetSelector);
        if (!(target instanceof HTMLInputElement)) return;

        const adjectives = ["SPRING", "SUMMER", "WINTER", "FALL", "EARLY", "VIP", "MEGA"];
        const suffix = Math.random().toString(36).substring(2, 6).toUpperCase();
        const pick = adjectives[Math.floor(Math.random() * adjectives.length)];
        target.value = `EMS-${pick}${suffix}`;
        updatePreview();
    });

    updatePreview();
});