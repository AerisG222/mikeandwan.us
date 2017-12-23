import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryCardGridComponent } from './category-card-grid.component';

describe('CategoryCardGridComponent', () => {
  let component: CategoryCardGridComponent;
  let fixture: ComponentFixture<CategoryCardGridComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CategoryCardGridComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CategoryCardGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
