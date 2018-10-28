/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { PhotoStateService } from './photo-state.service';

describe('PhotoState Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PhotoStateService]
    });
  });

  it('should ...', inject([PhotoStateService], (service: PhotoStateService) => {
    expect(service).toBeTruthy();
  }));
});
