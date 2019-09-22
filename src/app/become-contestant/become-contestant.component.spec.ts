import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BecomeContestantComponent } from './become-contestant.component';

describe('BecomeContestantComponent', () => {
  let component: BecomeContestantComponent;
  let fixture: ComponentFixture<BecomeContestantComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BecomeContestantComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BecomeContestantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
