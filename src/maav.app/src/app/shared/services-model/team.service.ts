import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CommonRestService } from '../services/common-rest.service';
import { SessionService } from '../services/session.service';
import { Team } from '../models/team/team.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  private resource = environment.baseUri;
  
  constructor(private rest: CommonRestService,
              private session:SessionService) { 
    this.resource = this.resource.concat(this.session.organisationId).concat('/').concat('teams').concat('/');
  }
  
  public getById(id: string): Observable<Team> {
    let url = this.resource.concat(`${id}`);
    return this.rest.get<Team>(url);
  }

  public create(team: Team): Observable<Team> {
    let url = this.resource;
    return this.rest.post<Team>(url, team);
  }
  public delete(teamId: string) {
    let url = this.resource.concat(teamId);
    return this.rest.delete(url);
  }
}
