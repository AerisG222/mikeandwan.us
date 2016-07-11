import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { StateService } from './state.service';

describe('State Service', () => {
    beforeEachProviders(() => [StateService]);

    it('should ...',
        inject([StateService], (service: StateService) => {
            expect(service).toBeTruthy();
        }));
});
