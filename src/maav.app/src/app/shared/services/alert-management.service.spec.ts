import { TestBed } from '@angular/core/testing';

import { AlertManagementService } from './alert-management.service';

describe('AlertManagementService', () => {
  let service: AlertManagementService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AlertManagementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
