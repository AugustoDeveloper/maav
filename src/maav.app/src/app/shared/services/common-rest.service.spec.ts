import { TestBed } from '@angular/core/testing';

import { CommonRestService } from './common-rest.service';

describe('CommonRestService', () => {
  let service: CommonRestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonRestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
