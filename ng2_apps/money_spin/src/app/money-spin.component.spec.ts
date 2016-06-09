import {
    beforeEachProviders,
    describe,
    expect,
    it,
    inject
} from '@angular/core/testing';
import { MoneySpinAppComponent } from '../app/money-spin.component';

beforeEachProviders(() => [MoneySpinAppComponent]);

describe('App: MoneySpin', () => {
    it('should create the app',
        inject([MoneySpinAppComponent], (app: MoneySpinAppComponent) => {
            expect(app).toBeTruthy();
        }));

    /*
    it('should have as title \'money-spin works!\'',
        inject([MoneySpinAppComponent], (app: MoneySpinAppComponent) => {
      expect(app.title).toEqual('money-spin works!');
    }));
    */
});
