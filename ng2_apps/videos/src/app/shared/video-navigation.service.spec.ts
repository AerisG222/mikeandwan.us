import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { VideoNavigationService } from './video-navigation.service';

describe('VideoNavigation Service', () => {
    beforeEachProviders(() => [VideoNavigationService]);

    it('should ...',
        inject([VideoNavigationService], (service: VideoNavigationService) => {
            expect(service).toBeTruthy();
        }));
});
