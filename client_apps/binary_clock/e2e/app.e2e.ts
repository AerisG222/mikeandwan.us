import { BinaryClockPage } from './app.po';

describe('binary-clock App', function () {
    let page: BinaryClockPage;

    beforeEach(() => {
        page = new BinaryClockPage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('binary-clock works!');
    });
});
