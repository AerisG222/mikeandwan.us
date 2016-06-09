export class WeekendCountdownPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('weekend-countdown-app h1')).getText();
    }
}
