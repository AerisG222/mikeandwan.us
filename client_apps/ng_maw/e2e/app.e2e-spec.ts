import { NgMawPage } from './app.po';

describe('ng-maw App', function() {
  let page: NgMawPage;

  beforeEach(() => {
    page = new NgMawPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
