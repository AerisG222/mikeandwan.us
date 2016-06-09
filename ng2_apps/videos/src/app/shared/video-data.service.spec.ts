import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { VideoDataService } from './video-data.service';

describe('VideoData Service', () => {
    beforeEachProviders(() => [VideoDataService]);

    it('should ...',
        inject([VideoDataService], (service: VideoDataService) => {
            expect(service).toBeTruthy();
        }));
});
