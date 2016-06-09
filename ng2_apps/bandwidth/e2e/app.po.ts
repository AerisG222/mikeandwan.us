export class BandwidthPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('bandwidth-app h1')).getText();
    }
}
