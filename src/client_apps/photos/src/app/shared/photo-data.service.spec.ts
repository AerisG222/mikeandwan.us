import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { PhotoDataService } from './photo-data.service';

describe('PhotoData Service', () => {
    beforeEachProviders(() => [PhotoDataService]);

    it('should ...',
        inject([PhotoDataService], (service: PhotoDataService) => {
            expect(service).toBeTruthy();
        }));
});
