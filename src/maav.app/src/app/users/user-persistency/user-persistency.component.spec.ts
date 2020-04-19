import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { UserPersistencyComponent } from './user-persistency.component';

describe('UserPersistencyComponent', () => {
  let component: UserPersistencyComponent;
  let fixture: ComponentFixture<UserPersistencyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserPersistencyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPersistencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
