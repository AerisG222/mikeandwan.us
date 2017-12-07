import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PhotoCardGridComponent } from './photo-card-grid.component';

describe('PhotoCardGridComponent', () => {
  let component: PhotoCardGridComponent;
  let fixture: ComponentFixture<PhotoCardGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhotoCardGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotoCardGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
