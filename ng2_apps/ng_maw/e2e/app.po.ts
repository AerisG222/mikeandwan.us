export class NgMawPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('ng-maw-app h1')).getText();
    }
}
