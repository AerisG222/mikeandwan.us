export class VideosPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('videos-app h1')).getText();
  }
}
