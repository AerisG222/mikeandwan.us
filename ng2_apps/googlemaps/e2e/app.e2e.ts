import { GooglemapsPage } from './app.po';

describe('googlemaps App', function() {
  let page: GooglemapsPage;

  beforeEach(() => {
    page = new GooglemapsPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('googlemaps works!');
  });
});
