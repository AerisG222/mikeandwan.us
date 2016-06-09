import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { FilesizeAppComponent } from '../app/filesize.component';

beforeEachProviders(() => [FilesizeAppComponent]);

describe('App: Filesize', () => {
    it('should create the app',
        inject([FilesizeAppComponent], (app: FilesizeAppComponent) => {
            expect(app).toBeTruthy();
        }));

    /*
      it('should have as title \'filesize works!\'',
          inject([FilesizeAppComponent], (app: FilesizeAppComponent) => {
        expect(app.title).toEqual('filesize works!');
      }));
    */
});
