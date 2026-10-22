const listeners = new WeakMap();

export function initialize(canvas, dotNetReference, initialValue) {
    const context = canvas.getContext("2d");
    const controller = new AbortController();
    const options = { signal: controller.signal };
    let drawing = false;
    let savedImage = initialValue || "";

    const restore = () => {
        if (!savedImage) return;
        const image = new Image();
        image.onload = () => {
            const bounds = canvas.getBoundingClientRect();
            context.drawImage(image, 0, 0, bounds.width, bounds.height);
        };
        image.src = savedImage;
    };

    const resize = () => {
        const bounds = canvas.getBoundingClientRect();
        if (!bounds.width || !bounds.height) return;
        const ratio = window.devicePixelRatio || 1;
        canvas.width = Math.round(bounds.width * ratio);
        canvas.height = Math.round(bounds.height * ratio);
        context.setTransform(ratio, 0, 0, ratio, 0, 0);
        context.strokeStyle = "#23201f";
        context.lineWidth = 2.2;
        context.lineCap = "round";
        context.lineJoin = "round";
        restore();
    };

    const point = event => {
        const bounds = canvas.getBoundingClientRect();
        return { x: event.clientX - bounds.left, y: event.clientY - bounds.top };
    };

    canvas.addEventListener("pointerdown", event => {
        event.preventDefault();
        drawing = true;
        canvas.setPointerCapture(event.pointerId);
        const p = point(event);
        context.beginPath();
        context.moveTo(p.x, p.y);
        context.lineTo(p.x + 0.1, p.y + 0.1);
        context.stroke();
    }, options);

    canvas.addEventListener("pointermove", event => {
        if (!drawing) return;
        event.preventDefault();
        const p = point(event);
        context.lineTo(p.x, p.y);
        context.stroke();
    }, options);

    const finish = async event => {
        if (!drawing) return;
        drawing = false;
        if (event?.pointerId !== undefined && canvas.hasPointerCapture(event.pointerId))
            canvas.releasePointerCapture(event.pointerId);
        savedImage = canvas.toDataURL("image/png");
        await dotNetReference.invokeMethodAsync("SignatureChanged", savedImage);
    };

    canvas.addEventListener("pointerup", finish, options);
    canvas.addEventListener("pointercancel", finish, options);
    window.addEventListener("resize", resize, options);
    listeners.set(canvas, controller);
    resize();
}

export async function clear(canvas, dotNetReference) {
    const context = canvas.getContext("2d");
    context.clearRect(0, 0, canvas.width, canvas.height);
    await dotNetReference.invokeMethodAsync("SignatureChanged", "");
}

export function dispose(canvas) {
    listeners.get(canvas)?.abort();
    listeners.delete(canvas);
}
