import {
  beforeEachProviders,
  describe,
  expect,
  it,
  inject
} from '@angular/core/testing';
import { MemoryAppComponent } from '../app/memory.component';

beforeEachProviders(() => [MemoryAppComponent]);

describe('App: Memory', () => {
  it('should create the app',
      inject([MemoryAppComponent], (app: MemoryAppComponent) => {
    expect(app).toBeTruthy();
  }));

/*
  it('should have as title \'memory works!\'',
      inject([MemoryAppComponent], (app: MemoryAppComponent) => {
    expect(app.title).toEqual('memory works!');
  }));
*/
});
