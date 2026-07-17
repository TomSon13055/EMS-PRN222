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
    const radios = document.querySelectorAll("[data-ticket-type-radio]");
    const quantity = document.querySelector("[data-ticket-quantity]");
    const subTotalEl = document.querySelector("[data-sub-total]");
    const finalAmountEl = document.querySelector("[data-final-amount]");
    const unitPriceEl = document.querySelector("[data-unit-price]");
    const summaryQtyEl = document.querySelector("[data-summary-quantity]");
    const discountRow = document.querySelector("[data-discount-row]");
    const discountCell = document.querySelector("[data-discount-amount]");
    const discountPercentAttr = discountCell?.getAttribute("data-discount-percent");

    const format = (value) => new Intl.NumberFormat("vi-VN").format(Math.max(0, Math.round(Number(value) || 0))) + " ₫";

    const getSelectedPrice = () => {
        if (radios.length > 0) {
            const checked = Array.from(radios).find((r) => r.checked);
            if (checked) {
                return Number(checked.getAttribute("data-ticket-price")) || 0;
            }
        }
        if (select instanceof HTMLSelectElement) {
            const selectedOption = select.options[select.selectedIndex];
            return Number(selectedOption?.getAttribute("data-ticket-price")) || 0;
        }
        return 0;
    };

    const recalc = () => {
        if (!quantity || !subTotalEl) return;
        const price = getSelectedPrice();
        const qty = Math.max(1, Number(quantity.value) || 1);
        const subtotal = price * qty;

        if (unitPriceEl) unitPriceEl.textContent = format(price);
        if (summaryQtyEl) summaryQtyEl.textContent = qty.toString();
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

    radios.forEach((radio) => {
        radio.addEventListener("change", () => {
            if (select instanceof HTMLSelectElement) {
                select.value = radio.value;
            }
            recalc();
        });
    });

    // Quantity stepper
    document.querySelectorAll("[data-qty-stepper]").forEach((stepper) => {
        const minus = stepper.querySelector("[data-qty-minus]");
        const plus = stepper.querySelector("[data-qty-plus]");
        const input = stepper.querySelector("[data-qty-input]");
        if (!(input instanceof HTMLInputElement)) return;

        const min = Number(input.min) || 1;
        const max = input.max ? Number(input.max) : Infinity;

        minus?.addEventListener("click", () => {
            const current = Number(input.value) || 1;
            const next = Math.max(min, current - 1);
            input.value = String(next);
            input.dispatchEvent(new Event("input", { bubbles: true }));
        });

        plus?.addEventListener("click", () => {
            const current = Number(input.value) || 1;
            const next = Math.min(max, current + 1);
            input.value = String(next);
            input.dispatchEvent(new Event("input", { bubbles: true }));
        });
    });

    recalc();
});