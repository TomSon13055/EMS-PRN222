document.addEventListener("DOMContentLoaded", () => {
    const buyForm = document.querySelector("[data-buy-ticket-form]");
    if (buyForm instanceof HTMLFormElement) {
        buyForm.addEventListener("submit", (event) => {
            const submitter = event.submitter instanceof HTMLButtonElement ? event.submitter : null;
            if (!submitter) return;
            if (submitter.hasAttribute("data-apply-voucher-button")) return;
            if (submitter.getAttribute("name") === "applyVoucher") return;

            const buyButton = buyForm.querySelector("[data-buy-button]");
            if (buyButton instanceof HTMLButtonElement && !buyButton.disabled) {
                buyButton.setAttribute("aria-busy", "true");
                buyButton.disabled = true;
            }
        });
    }

    const select = document.querySelector("[data-ticket-type-select]");
    const quantity = document.querySelector("[data-ticket-quantity]");
    const subTotalEl = document.querySelector("[data-sub-total]");
    const finalAmountEl = document.querySelector("[data-final-amount]");
    const discountRow = document.querySelector("[data-discount-row]");
    const discountCell = document.querySelector("[data-discount-amount]");
    const discountPercentAttr = discountCell?.getAttribute("data-discount-percent");

    const format = (value) => new Intl.NumberFormat("vi-VN").format(Math.max(0, Math.round(Number(value) || 0))) + " \u20ab";

    const recalc = () => {
        if (!select || !quantity || !subTotalEl) return;
        const selectedOption = select.options[select.selectedIndex];
        const price = Number(selectedOption?.getAttribute("data-ticket-price")) || 0;
        const qty = Math.max(1, Number(quantity.value) || 1);
        const subtotal = price * qty;
        subTotalEl.textContent = format(subtotal);

        if (finalAmountEl) {
            const percent = Number(discountPercentAttr) || 0;
            const isVoucherApplied = Boolean(discountRow && !discountRow.hasAttribute("hidden"));

            if (isVoucherApplied && percent > 0) {
                const discount = Math.round(subtotal * percent / 100);
                if (discountCell) discountCell.textContent = "-" + format(discount);
                finalAmountEl.textContent = format(subtotal - discount);
            } else {
                finalAmountEl.textContent = format(subtotal);
            }
        }
    };

    select?.addEventListener("change", recalc);
    quantity?.addEventListener("input", recalc);
    recalc();
});