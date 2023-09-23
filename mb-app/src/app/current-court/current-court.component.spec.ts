import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentCourtComponent } from './current-court.component';

describe('CurrentCourtComponent', () => {
  let component: CurrentCourtComponent;
  let fixture: ComponentFixture<CurrentCourtComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CurrentCourtComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CurrentCourtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
