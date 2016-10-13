/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import {RandomPhotoListContext} from './random-photo-list-context.model';

describe('RandomPhotoListContext', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RandomPhotoListContext]
    });
  });

  it('should ...', inject([RandomPhotoListContext], (service: RandomPhotoListContext) => {
    expect(service).toBeTruthy();
  }));
});
