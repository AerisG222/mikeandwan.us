import { BandwidthPage } from './app.po';

describe('bandwidth App', function() {
  let page: BandwidthPage;

  beforeEach(() => {
    page = new BandwidthPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('bandwidth works!');
  });
});
