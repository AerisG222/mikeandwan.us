import { VideosPage } from './app.po';

describe('videos App', function() {
  let page: VideosPage;

  beforeEach(() => {
    page = new VideosPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
