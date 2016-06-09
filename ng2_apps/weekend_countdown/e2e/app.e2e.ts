import { WeekendCountdownPage } from './app.po';

describe('weekend-countdown App', function () {
    let page: WeekendCountdownPage;

    beforeEach(() => {
        page = new WeekendCountdownPage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('weekend-countdown works!');
    });
});
