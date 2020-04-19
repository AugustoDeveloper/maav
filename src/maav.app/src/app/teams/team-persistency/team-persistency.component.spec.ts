import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamPersistencyComponent } from './team-persistency.component';

describe('TeamPersistencyComponent', () => {
  let component: TeamPersistencyComponent;
  let fixture: ComponentFixture<TeamPersistencyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamPersistencyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamPersistencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
