/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import {PhotoSourceFactory} from './photo-source-factory.model';

describe('PhotoSourceFactory', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PhotoSourceFactory]
    });
  });

  it('should ...', inject([PhotoSourceFactory], (service: PhotoSourceFactory) => {
    expect(service).toBeTruthy();
  }));
});
