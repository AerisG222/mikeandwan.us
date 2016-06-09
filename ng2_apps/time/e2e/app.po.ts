export class TimePage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('time-app h1')).getText();
    }
}
