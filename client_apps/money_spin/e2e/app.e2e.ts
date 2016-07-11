import { MoneySpinPage } from './app.po';

describe('money-spin App', function () {
    let page: MoneySpinPage;

    beforeEach(() => {
        page = new MoneySpinPage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('money-spin works!');
    });
});
