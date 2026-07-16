const TOAST_ICONS = {
    success: "bi-check-circle-fill",
    error: "bi-x-octagon-fill",
    warning: "bi-exclamation-triangle-fill",
    info: "bi-info-circle-fill"
};

const TOAST_TITLES = {
    success: "Success",
    error: "Error",
    warning: "Warning",
    info: "Information"
};

let container = null;
let counter = 0;

function ensureContainer() {
    if (container && document.body.contains(container)) return container;
    container = document.createElement("div");
    container.className = "toast-container";
    container.setAttribute("aria-live", "polite");
    container.setAttribute("aria-atomic", "true");
    document.body.appendChild(container);
    return container;
}

function dismissToast(toast) {
    if (!toast || toast.classList.contains("toast--leaving")) return;
    toast.classList.add("toast--leaving");
    setTimeout(() => {
        toast.remove();
    }, 220);
}

function buildToast(type, title, message) {
    const root = ensureContainer();
    const id = `toast-${++counter}`;
    const toast = document.createElement("div");
    toast.className = `toast toast--${type}`;
    toast.setAttribute("role", type === "error" ? "alert" : "status");
    toast.dataset.toastId = id;

    const icon = TOAST_ICONS[type] || TOAST_ICONS.info;
    const titleText = title || TOAST_TITLES[type] || TOAST_TITLES.info;

    toast.innerHTML = "";

    const iconWrap = document.createElement("div");
    iconWrap.className = "toast__icon";
    const iconEl = document.createElement("i");
    iconEl.className = `bi ${icon}`;
    iconEl.setAttribute("aria-hidden", "true");
    iconWrap.appendChild(iconEl);

    const body = document.createElement("div");
    body.className = "toast__body";
    const titleEl = document.createElement("p");
    titleEl.className = "toast__title";
    titleEl.textContent = titleText;
    body.appendChild(titleEl);
    if (message) {
        const msgEl = document.createElement("p");
        msgEl.className = "toast__message";
        msgEl.textContent = message;
        body.appendChild(msgEl);
    }

    const closeBtn = document.createElement("button");
    closeBtn.type = "button";
    closeBtn.className = "toast__close";
    closeBtn.setAttribute("aria-label", "Dismiss notification");
    const closeIcon = document.createElement("i");
    closeIcon.className = "bi bi-x-lg";
    closeIcon.setAttribute("aria-hidden", "true");
    closeBtn.appendChild(closeIcon);
    closeBtn.addEventListener("click", () => dismissToast(toast));

    toast.appendChild(iconWrap);
    toast.appendChild(body);
    toast.appendChild(closeBtn);

    root.appendChild(toast);

    const lifetime = type === "error" ? 6000 : 4000;
    setTimeout(() => dismissToast(toast), lifetime);

    return toast;
}

export function initToast() {
    ensureContainer();
}

export function showToast(type, title, message) {
    const allowed = ["success", "error", "warning", "info"];
    const safeType = allowed.includes(type) ? type : "info";
    return buildToast(safeType, title, message);
}

export const toast = {
    success(message, title) { return showToast("success", title, message); },
    error(message, title) { return showToast("error", title, message); },
    warning(message, title) { return showToast("warning", title, message); },
    info(message, title) { return showToast("info", title, message); }
};