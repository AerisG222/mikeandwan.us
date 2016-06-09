export class PhotosPage {
    navigateTo() {
        return browser.get('/');
    }

    getParagraphText() {
        return element(by.css('photos-app h1')).getText();
    }
}
