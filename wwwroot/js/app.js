import { initToast } from "./core/toast.js";
import { initConfirmDialogs } from "./core/confirm-dialog.js";
import { initAccountMenu, initDropdowns } from "./core/modal.js";

document.addEventListener("DOMContentLoaded", () => {
    initToast();
    initConfirmDialogs();
    initAccountMenu();
    initDropdowns();
});

export function bootstrap() {
    initToast();
    initConfirmDialogs();
    initAccountMenu();
    initDropdowns();
}