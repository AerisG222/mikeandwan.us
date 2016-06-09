export class FilesizePage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('filesize-app h1')).getText();
    }
}
