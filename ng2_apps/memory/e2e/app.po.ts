export class MemoryPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('memory-app h1')).getText();
    }
}
