/* eslint-disable @typescript-eslint/no-unused-vars */

import { TestBed, waitForAsync } from '@angular/core/testing';

import { AppComponent } from './app.component';

describe('App: Googlemaps', () => {
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [
        AppComponent
      ],
    }).compileComponents();
  }));

  it('should create the app', waitForAsync(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));
});
