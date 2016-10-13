/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ResponsiveService } from './responsive.service';

describe('Responsive Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ResponsiveService]
    });
  });

  it('should ...', inject([ResponsiveService], (service: ResponsiveService) => {
    expect(service).toBeTruthy();
  }));
});
