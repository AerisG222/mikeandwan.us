export class LearningPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('learning-app h1')).getText();
    }
}
