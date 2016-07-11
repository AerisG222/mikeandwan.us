import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { GooglemapsAppComponent } from '../app/app.component';

beforeEachProviders(() => [GooglemapsAppComponent]);

describe('App: Googlemaps', () => {
    it('should create the app',
        inject([GooglemapsAppComponent], (app: GooglemapsAppComponent) => {
            expect(app).toBeTruthy();
        }));

    /*
      it('should have as title \'googlemaps works!\'',
          inject([GooglemapsAppComponent], (app: GooglemapsAppComponent) => {
        expect(app.title).toEqual('googlemaps works!');
      }));
    */
});
