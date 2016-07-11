import { FilesizePage } from './app.po';

describe('filesize App', function() {
  let page: FilesizePage;

  beforeEach(() => {
    page = new FilesizePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
