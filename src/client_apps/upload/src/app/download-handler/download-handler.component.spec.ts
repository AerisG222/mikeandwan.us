import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DownloadHandlerComponent } from './download-handler.component';

describe('DownloadHandlerComponent', () => {
  let component: DownloadHandlerComponent;
  let fixture: ComponentFixture<DownloadHandlerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DownloadHandlerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadHandlerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
