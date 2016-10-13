/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import {RandomPhotoSource} from './random-photo-source.model';

describe('RandomPhotoSource', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RandomPhotoSource]
    });
  });

  it('should ...', inject([RandomPhotoSource], (service: RandomPhotoSource) => {
    expect(service).toBeTruthy();
  }));
});
