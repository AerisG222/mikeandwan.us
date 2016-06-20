import { ByteCounterPage } from './app.po';

describe('byte-counter App', function() {
  let page: ByteCounterPage;

  beforeEach(() => {
    page = new ByteCounterPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
