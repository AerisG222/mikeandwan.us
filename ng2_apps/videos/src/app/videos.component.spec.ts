import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { VideosAppComponent } from '../app/videos.component';

beforeEachProviders(() => [VideosAppComponent]);

describe('App: Videos', () => {
  it('should create the app',
      inject([VideosAppComponent], (app: VideosAppComponent) => {
    expect(app).toBeTruthy();
  }));

  /*
  it('should have as title \'videos works!\'',
      inject([VideosAppComponent], (app: VideosAppComponent) => {
    expect(app.title).toEqual('videos works!');
  }));
  */
});
