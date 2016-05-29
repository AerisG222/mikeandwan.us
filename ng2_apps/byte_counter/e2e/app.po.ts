export class ByteCounterPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('byte-counter-app h1')).getText();
  }
}
