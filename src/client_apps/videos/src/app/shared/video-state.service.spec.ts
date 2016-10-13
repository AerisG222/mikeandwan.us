/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { VideoStateService } from './video-state.service';

describe('VideoState Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [VideoStateService]
    });
  });

  it('should ...', inject([VideoStateService], (service: VideoStateService) => {
    expect(service).toBeTruthy();
  }));
});
