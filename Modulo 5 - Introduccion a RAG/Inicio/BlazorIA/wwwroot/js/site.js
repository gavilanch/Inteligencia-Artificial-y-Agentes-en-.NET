window.chatScroll = {
    scrollToBottom: function (element) {
        if (!element) return;
        element.scrollTop = element.scrollHeight;
    }
};
