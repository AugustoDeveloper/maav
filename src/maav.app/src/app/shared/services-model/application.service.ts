import { Injectable } from '@angular/core';
import { Application } from '../models/application/application.model';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SessionService } from '../services/session.service';
import { CommonRestService } from '../services/common-rest.service';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private get resource() : string {
    return environment.baseUri.concat(this.session.organisationId).concat('/');
   }
  
  constructor(private session: SessionService,
              private rest: CommonRestService) { 
  }

  public add(teamId: string, application: Application): Observable<Application> {

    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps');
    console.log(url);
    return this.rest.post(url, application);
  }

  public update(teamId: string, application: Application): Observable<Application> {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps').concat('/').concat(application.id);
    return this.rest.put(url, application);
  }

  public get(teamId: string, applicationId: string): Observable<Application> {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps').concat('/').concat(applicationId);
    return this.rest.get(url);
  }

  public loadByTeam(teamId: string): Observable<Array<Application>> {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps');
    return this.rest.get(url);
  }

  public delete(teamId: string, appId: string): Observable<any> {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('apps').concat('/').concat(appId);
    return this.rest.delete(url);
  }
}
