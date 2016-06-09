import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { ResponsiveService } from './responsive.service';

describe('Responsive Service', () => {
    beforeEachProviders(() => [ResponsiveService]);

    it('should ...',
        inject([ResponsiveService], (service: ResponsiveService) => {
            expect(service).toBeTruthy();
        }));
});
