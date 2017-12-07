import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VideoCardGridComponent } from './video-card-grid.component';

describe('VideoCardGridComponent', () => {
  let component: VideoCardGridComponent;
  let fixture: ComponentFixture<VideoCardGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VideoCardGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VideoCardGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
