document.addEventListener("DOMContentLoaded", () => {
    setTimeout(() => {
        window.open(verifyUrl, "_blank", "noopener");
        connect();
    }, 1000);
});

function connect() {
    const evt = new EventSource(`/poll/sse?txn=${txnId}`, { withCredentials: false });
    
    evt.addEventListener("waiting", e => {
        console.log("waiting", e.data);
    });

    evt.addEventListener("complete", e => {
        try {
            const payload = JSON.parse(e.data);
            const to = new URL(redirectUri);
            to.searchParams.set("code", payload.code);
            to.searchParams.set("state", state);
            evt.close();
            window.location = to.toString();
        } catch { }
    });

    evt.onerror = () => {
        console.warn("SSE connection error");
    };
}