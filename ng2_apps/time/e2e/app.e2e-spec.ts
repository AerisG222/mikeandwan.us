import { TimePage } from './app.po';

describe('time App', function() {
  let page: TimePage;

  beforeEach(() => {
    page = new TimePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
