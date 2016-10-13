/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { PhotoDataService } from './photo-data.service';

describe('PhotoData Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PhotoDataService]
    });
  });

  it('should ...', inject([PhotoDataService], (service: PhotoDataService) => {
    expect(service).toBeTruthy();
  }));
});
