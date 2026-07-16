import { getAntiForgeryTokenFromDocument, readJson } from "./form-utils.js";

export async function postJson(url, payload, options = {}) {
    const headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
        ...(options.headers || {})
    };

    const token = options.token || getAntiForgeryTokenFromDocument();
    if (token) {
        headers["RequestVerificationToken"] = token;
    }

    const response = await fetch(url, {
        method: "POST",
        credentials: "same-origin",
        headers,
        body: JSON.stringify(payload ?? {})
    });

    const data = await readJson(response);

    if (!response.ok) {
        const error = new Error((data && data.message) || `Request failed with status ${response.status}`);
        error.status = response.status;
        error.data = data;
        throw error;
    }

    return data;
}

export async function postForm(url, form, options = {}) {
    const headers = {
        "Accept": "application/json",
        ...(options.headers || {})
    };

    const token = options.token || getAntiForgeryTokenFromDocument();
    if (token) {
        headers["RequestVerificationToken"] = token;
    }

    const body = form instanceof FormData ? form : new FormData(form);

    const response = await fetch(url, {
        method: "POST",
        credentials: "same-origin",
        headers,
        body
    });

    const data = await readJson(response);

    if (!response.ok) {
        const error = new Error((data && data.message) || `Request failed with status ${response.status}`);
        error.status = response.status;
        error.data = data;
        throw error;
    }

    return data;
}

export function buildQuery(params) {
    const usp = new URLSearchParams();
    Object.entries(params || {}).forEach(([key, value]) => {
        if (value === null || value === undefined || value === "") return;
        usp.append(key, String(value));
    });
    return usp.toString();
}