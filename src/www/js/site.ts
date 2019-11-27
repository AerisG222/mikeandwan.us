$(function() {
    $('.remove-focus-on-click').on('click', (evt: Event) => (evt.target as HTMLElement).blur());
});
