/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { VideoDataService } from './video-data.service';

describe('VideoData Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [VideoDataService]
    });
  });

  it('should ...', inject([VideoDataService], (service: VideoDataService) => {
    expect(service).toBeTruthy();
  }));
});
