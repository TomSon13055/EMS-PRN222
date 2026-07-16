export function formatVnd(value) {
    const number = Number(value) || 0;
    return new Intl.NumberFormat("vi-VN").format(number) + " \u20ab";
}

export function formatNumber(value) {
    const number = Number(value) || 0;
    return new Intl.NumberFormat("vi-VN").format(number);
}

export function formatDate(value, options) {
    if (!value) return "";
    const date = value instanceof Date ? value : new Date(value);
    if (Number.isNaN(date.getTime())) return "";
    return new Intl.DateTimeFormat("vi-VN", options || {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit"
    }).format(date);
}

export function getAntiForgeryToken(form) {
    if (!form) return "";
    const input = form.querySelector('input[name="__RequestVerificationToken"]');
    return input instanceof HTMLInputElement ? input.value : "";
}

export function getAntiForgeryTokenFromDocument() {
    const input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input instanceof HTMLInputElement ? input.value : "";
}

export function setBusy(button, busy) {
    if (!(button instanceof HTMLElement)) return;
    if (busy) {
        button.setAttribute("aria-busy", "true");
        button.setAttribute("disabled", "true");
    } else {
        button.removeAttribute("aria-busy");
        button.removeAttribute("disabled");
    }
}

export function safeText(value, fallback = "") {
    if (value === null || value === undefined) return fallback;
    return String(value);
}

export function readJson(response) {
    const contentType = response.headers.get("content-type") || "";
    if (!contentType.includes("application/json")) {
        return Promise.resolve(null);
    }
    return response.json();
}

export function debounce(fn, delay) {
    let timer = null;
    return function debounced(...args) {
        if (timer) clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), delay);
    };
}

export function getInitials(name) {
    if (!name) return "?";
    const parts = String(name).trim().split(/\s+/).filter(Boolean);
    if (parts.length === 0) return "?";
    if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
}

export function escapeHtml(value) {
    return String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
}