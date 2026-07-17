document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-image-preview-input]").forEach((input) => {
        const targetSelector = input.getAttribute("data-image-preview-input");
        if (!targetSelector) return;
        const preview = document.querySelector(targetSelector);
        if (!preview) return;

        const render = (url) => {
            preview.innerHTML = "";
            if (!url) {
                const cap = document.createElement("span");
                cap.className = "image-preview__caption";
                cap.textContent = "Paste an image URL above to preview it here.";
                preview.appendChild(cap);
                return;
            }
            const wrap = document.createElement("div");
            wrap.className = "image-preview__thumb";
            const img = document.createElement("img");
            img.src = url;
            img.alt = "Cover preview";
            img.addEventListener("error", () => {
                wrap.remove();
                const cap = document.createElement("span");
                cap.className = "image-preview__caption";
                cap.textContent = "Couldn't load that image. Check the URL and try again.";
                preview.appendChild(cap);
            });
            wrap.appendChild(img);
            preview.appendChild(wrap);
            const cap = document.createElement("span");
            cap.className = "image-preview__caption";
            cap.textContent = "Preview · saves when you save the form";
            preview.appendChild(cap);
        };

        input.addEventListener("input", () => render(input.value.trim()));
    });
});