import {
  beforeEachProviders,
  it,
  describe,
  expect,
  inject
} from '@angular/core/testing';
import { PhotoStateService } from './photo-state.service';

describe('PhotoState Service', () => {
  beforeEachProviders(() => [PhotoStateService]);

  it('should ...',
      inject([PhotoStateService], (service: PhotoStateService) => {
    expect(service).toBeTruthy();
  }));
});
