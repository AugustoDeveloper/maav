import { Injectable } from '@angular/core';
import { CommonRestService } from '../services/common-rest.service';
import { environment } from 'src/environments/environment';
import { Registration } from '../models/registration.model';
import { SessionService } from '../services/session.service';
import { Organisation } from '../models/organisation/organisation.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrganisationService {
  private resource = environment.baseUri;
  constructor(private rest: CommonRestService,
              private session:SessionService) { }
  
  public register(registration: Registration) {
    let url = this.resource.concat(`${registration.id}`);
    return this.rest.post<Organisation>(url, registration);
  }

  public getOrganisationById(orgId:string): Observable<Organisation> {
    let url = this.resource.concat(`${orgId}`);
    return this.rest.get<Organisation>(url);
  }
}
