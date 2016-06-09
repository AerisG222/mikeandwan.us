import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { PhotosAppComponent } from '../app/photos.component';

beforeEachProviders(() => [PhotosAppComponent]);

describe('App: Photos', () => {
    it('should create the app',
        inject([PhotosAppComponent], (app: PhotosAppComponent) => {
            expect(app).toBeTruthy();
        }));

    /*
      it('should have as title \'photos works!\'',
          inject([PhotosAppComponent], (app: PhotosAppComponent) => {
        expect(app.title).toEqual('photos works!');
      }));
    */
});
