import { postJson } from "./api.js";
import { toast } from "./toast.js";
import { getAntiForgeryTokenFromDocument } from "./form-utils.js";

let modalEl = null;
let pendingAction = null;

function ensureModal() {
    if (modalEl && document.body.contains(modalEl)) return modalEl;

    const wrapper = document.createElement("div");
    wrapper.className = "app-modal";
    wrapper.id = "global-confirm-modal";
    wrapper.setAttribute("role", "dialog");
    wrapper.setAttribute("aria-modal", "true");
    wrapper.setAttribute("aria-hidden", "true");
    wrapper.dataset.confirmModal = "true";
    wrapper.innerHTML = `
        <div class="app-modal__dialog" role="document">
            <div class="row" style="gap: var(--spacing-3);">
                <div class="app-modal__icon" aria-hidden="true">
                    <i class="bi bi-exclamation-lg"></i>
                </div>
                <div class="stack" style="gap: var(--spacing-2); flex: 1; min-width: 0;">
                    <h3 class="app-modal__title" data-confirm-title>Are you sure?</h3>
                    <p class="app-modal__body" data-confirm-message>This action cannot be undone.</p>
                </div>
            </div>
            <div class="app-modal__actions">
                <button type="button" class="btn btn--outline-secondary" data-confirm-cancel>Cancel</button>
                <button type="button" class="btn btn--danger" data-confirm-confirm>Confirm</button>
            </div>
        </div>
    `;

    document.body.appendChild(wrapper);
    modalEl = wrapper;
    return wrapper;
}

function close() {
    if (!modalEl) return;
    modalEl.classList.remove("app-modal--open");
    modalEl.setAttribute("aria-hidden", "true");
    pendingAction = null;
}

async function executePending() {
    const action = pendingAction;
    if (!action) {
        close();
        return;
    }

    const confirmButton = modalEl?.querySelector("[data-confirm-confirm]");
    confirmButton?.setAttribute("aria-busy", "true");
    confirmButton?.setAttribute("disabled", "true");

    try {
        if (action.kind === "ajax") {
            const token = action.token || getAntiForgeryTokenFromDocument();
            await postJson(action.url, action.payload || {}, { token });
            if (typeof action.onSuccess === "function") {
                await action.onSuccess();
            }
            if (action.successMessage) {
                toast.success(action.successMessage);
            }
        } else if (action.kind === "function") {
            await action.fn();
            if (action.successMessage) {
                toast.success(action.successMessage);
            }
        } else if (action.kind === "redirect") {
            window.location.href = action.url;
        }
    } catch (error) {
        console.error("Confirm action failed.", error);
        toast.error(error?.message || "Action failed");
        if (typeof action.onError === "function") {
            action.onError(error);
        }
    } finally {
        confirmButton?.removeAttribute("aria-busy");
        confirmButton?.removeAttribute("disabled");
        close();
    }
}

export function openConfirm({ title, message, confirmText, cancelText, action }) {
    const modal = ensureModal();
    const titleEl = modal.querySelector("[data-confirm-title]");
    const messageEl = modal.querySelector("[data-confirm-message]");
    const confirmButton = modal.querySelector("[data-confirm-confirm]");
    const cancelButton = modal.querySelector("[data-confirm-cancel]");

    if (titleEl) titleEl.textContent = title || "Are you sure?";
    if (messageEl) messageEl.textContent = message || "This action cannot be undone.";
    if (confirmButton) confirmButton.textContent = confirmText || "Confirm";
    if (cancelButton) cancelButton.textContent = cancelText || "Cancel";

    pendingAction = action || null;

    modal.classList.add("app-modal--open");
    modal.setAttribute("aria-hidden", "false");
    setTimeout(() => confirmButton?.focus(), 50);
}

function bindDelegation() {
    document.addEventListener("click", (event) => {
        const trigger = event.target.closest("[data-confirm-trigger]");
        if (!trigger) return;

        event.preventDefault();
        event.stopPropagation();

        const form = trigger.closest("form");
        const url = trigger.dataset.confirmUrl || (form && form.action) || trigger.getAttribute("href");
        const title = trigger.dataset.confirmTitle || "Are you sure?";
        const message = trigger.dataset.confirmMessage || "This action cannot be undone.";
        const confirmText = trigger.dataset.confirmText || "Confirm";
        const cancelText = trigger.dataset.confirmTextCancel || "Cancel";
        const method = (trigger.dataset.confirmMethod || (form && form.method) || "POST").toUpperCase();
        const successMessage = trigger.dataset.confirmSuccess;

        if (!url) return;

        if (method === "GET") {
            openConfirm({
                title,
                message,
                confirmText,
                cancelText,
                action: { kind: "redirect", url }
            });
        } else if (form) {
            openConfirm({
                title,
                message,
                confirmText,
                cancelText,
                action: {
                    kind: "form",
                    url,
                    form,
                    successMessage,
                    onSuccess: () => {
                        form.submit();
                    }
                }
            });
        } else {
            openConfirm({
                title,
                message,
                confirmText,
                cancelText,
                action: {
                    kind: "ajax",
                    url,
                    payload: trigger.dataset.confirmPayload ? JSON.parse(trigger.dataset.confirmPayload) : {},
                    successMessage
                }
            });
        }
    });
}

export function initConfirmDialogs() {
    const modal = ensureModal();
    modal.querySelector("[data-confirm-cancel]")?.addEventListener("click", close);
    modal.querySelector("[data-confirm-confirm]")?.addEventListener("click", executePending);
    modal.addEventListener("click", (event) => {
        if (event.target === modal) close();
    });
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape" && modal.classList.contains("app-modal--open")) {
            close();
        }
    });
    bindDelegation();
}

export { close as closeConfirmModal };