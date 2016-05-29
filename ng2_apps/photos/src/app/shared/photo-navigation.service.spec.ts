import {
  beforeEachProviders,
  it,
  describe,
  expect,
  inject
} from '@angular/core/testing';
import { PhotoNavigationService } from './photo-navigation.service';

describe('PhotoNavigation Service', () => {
  beforeEachProviders(() => [PhotoNavigationService]);

  it('should ...',
      inject([PhotoNavigationService], (service: PhotoNavigationService) => {
    expect(service).toBeTruthy();
  }));
});
