/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { VideoNavigationService } from './video-navigation.service';

describe('VideoNavigation Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [VideoNavigationService]
    });
  });

  it('should ...', inject([VideoNavigationService], (service: VideoNavigationService) => {
    expect(service).toBeTruthy();
  }));
});
