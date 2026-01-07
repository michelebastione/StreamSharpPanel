function scrollToBottom(id) {
    const el = document.getElementById(id);
    if (el) {
        el.scrollTop = el.scrollHeight;
    }
}

function openInNewTab(url) {
    window.open(url, '_blank');
}
