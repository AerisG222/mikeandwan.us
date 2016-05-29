export class MoneySpinPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('money-spin-app h1')).getText();
  }
}
