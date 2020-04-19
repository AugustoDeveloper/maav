import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationPersistencyComponent } from './application-persistency.component';

describe('ApplicationPersistencyComponent', () => {
  let component: ApplicationPersistencyComponent;
  let fixture: ComponentFixture<ApplicationPersistencyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationPersistencyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationPersistencyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
