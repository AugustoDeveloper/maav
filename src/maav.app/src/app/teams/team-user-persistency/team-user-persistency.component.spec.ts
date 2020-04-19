import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamUserPersistencyComponent } from './team-user-persistency.component';

describe('TeamUserPersistencyComponent', () => {
  let component: TeamUserPersistencyComponent;
  let fixture: ComponentFixture<TeamUserPersistencyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamUserPersistencyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamUserPersistencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
