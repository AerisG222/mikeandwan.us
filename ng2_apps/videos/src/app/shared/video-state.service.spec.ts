import {
  beforeEachProviders,
  it,
  describe,
  expect,
  inject
} from '@angular/core/testing';
import { VideoStateService } from './video-state.service';

describe('VideoState Service', () => {
  beforeEachProviders(() => [VideoStateService]);

  it('should ...',
      inject([VideoStateService], (service: VideoStateService) => {
    expect(service).toBeTruthy();
  }));
});
