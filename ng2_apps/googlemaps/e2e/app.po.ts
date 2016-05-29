export class GooglemapsPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('googlemaps-app h1')).getText();
  }
}
