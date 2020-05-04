import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { CommonRestService } from '../services/common-rest.service';
import { SessionService } from '../services/session.service';
import { Observable } from 'rxjs';
import { User } from '../models/user/user.model';
import { TeamPermission } from '../models/team/team-permission.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private get resource() : string {
   return environment.baseUri.concat(this.session.organisationId).concat('/');
  }
  
  constructor(private rest: CommonRestService,
              private session:SessionService) { 
  }

  public getByUsername(username: string): Observable<User> {
    let url = this.resource.concat('users').concat('/').concat(`${username}`);
    return this.rest.get<User>(url);
  }

  public loadAllUsers():Observable<Array<User>> {
    let url = this.resource.concat('users').concat('/');
    return this.rest.get<Array<User>>(url);
  }

  public add(user: User): Observable<User> {
    let url = this.resource.concat('users');
    return this.rest.post(url, user);
  }

  public update(user: User): Observable<User> {
    let url = this.resource.concat('users').concat('/').concat(`${user.username}`);
    return this.rest.put(url, user);
  }

  public resetPassword(user: User): Observable<User> {
    let url = this.resource.concat('users').concat('/').concat(`${user.username}`);
    return this.rest.patch(url, user);
  }

  public dettachFromTeam(teamId: string , username: string) {
    let url = this.resource.concat('teams').concat('/').concat(teamId).concat('/').concat('users').concat('/').concat(`${username}`);
    return this.rest.delete(url);
  }

  public attachToTeam(teamId: string, username: string, permission: TeamPermission) {
    let url = this.resource.concat('team').concat('/').concat(teamId).concat('/').concat('users').concat('/').concat(`${username}`);
    return this.rest.put(url, permission);
  }
  
  public attachUsersToTeam(teamId: string, users: User[]) {
    let url = this.resource.concat('team').concat('/').concat(teamId).concat('/').concat('users');
    return this.rest.put(url, users);
  }

  public delete(username: string) {
    let url = this.resource.concat('users').concat('/').concat(`${username}`);
    return this.rest.delete(url);
  }
}
