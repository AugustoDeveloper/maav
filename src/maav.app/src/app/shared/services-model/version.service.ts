import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SessionService } from '../services/session.service';
import { CommonRestService } from '../services/common-rest.service';
import { VersionHistory } from '../models/versions/version-history.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VersionService {
  private get resource() : string {
    return environment.baseUri.concat(this.session.organisationId).concat('/');
   }
  
  constructor(private session: SessionService,
              private rest: CommonRestService) { 
  }

  
  public getByKeyBranchName(teamId: string, appId: string, keyBranchName: string): Observable<VersionHistory> {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps').concat('/').concat(appId).concat('/').concat('versions').concat('/').concat(keyBranchName);
    return this.rest.get(url);
  }
}
