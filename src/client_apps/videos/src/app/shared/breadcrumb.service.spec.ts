/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { BreadcrumbService } from './breadcrumb.service';

describe('Breadcrumb Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BreadcrumbService]
    });
  });

  it('should ...', inject([BreadcrumbService], (service: BreadcrumbService) => {
    expect(service).toBeTruthy();
  }));
});
