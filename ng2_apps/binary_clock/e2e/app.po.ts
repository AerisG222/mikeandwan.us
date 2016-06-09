export class BinaryClockPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('binary-clock-app h1')).getText();
    }
}
