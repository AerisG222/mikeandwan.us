/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SizeService } from './size.service';

describe('Size Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SizeService]
    });
  });

  it('should ...', inject([SizeService], (service: SizeService) => {
    expect(service).toBeTruthy();
  }));
});
