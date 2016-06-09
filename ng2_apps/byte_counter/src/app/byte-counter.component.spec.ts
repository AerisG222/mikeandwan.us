import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { ByteCounterAppComponent } from '../app/byte-counter.component';

beforeEachProviders(() => [ByteCounterAppComponent]);

describe('App: ByteCounter', () => {
    it('should create the app',
        inject([ByteCounterAppComponent], (app: ByteCounterAppComponent) => {
            expect(app).toBeTruthy();
        }));

    /*
      it('should have as title \'byte-counter works!\'',
          inject([ByteCounterAppComponent], (app: ByteCounterAppComponent) => {
        expect(app.title).toEqual('byte-counter works!');
      }));
    */
});
