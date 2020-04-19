import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CommonRestService } from '../services/common-rest.service';
import { SessionService } from '../services/session.service';
import { Authorization } from '../models/authorization.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private resource = environment.baseUri;
  
  constructor(private rest: CommonRestService,
              private session: SessionService) { }
  
  public authenticate(orgId: string, username: string, password: string) {
    let url = this.resource.concat(`${orgId}/authenticate`);
    return this.rest.post<Authorization>(url, {username, password}, true);
  }
}
