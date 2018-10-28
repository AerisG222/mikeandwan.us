/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { PhotoNavigationService } from './photo-navigation.service';

describe('PhotoNavigation Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PhotoNavigationService]
    });
  });

  it('should ...', inject([PhotoNavigationService], (service: PhotoNavigationService) => {
    expect(service).toBeTruthy();
  }));
});
