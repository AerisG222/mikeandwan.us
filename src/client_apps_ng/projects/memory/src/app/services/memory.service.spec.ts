/* eslint-disable @typescript-eslint/no-unused-vars */

import { TestBed, inject, waitForAsync } from '@angular/core/testing';
import { MemoryService } from './memory.service';

describe('Memory Service', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MemoryService]
    });
  });

  it('should ...', inject([MemoryService], (service: MemoryService) => {
    expect(service).toBeTruthy();
  }));
});
