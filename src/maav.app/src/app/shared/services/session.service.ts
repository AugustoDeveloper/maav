import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { StorageService } from './storage.service';
import { User } from '../models/user/user.model';
import { Team } from '../models/team/team.model';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  private readonly authTokenKey = 'authToken';
  private readonly authTokenExpiresKey = 'authTokenExpires';
  private readonly currentUserKey = 'currentUser';
  private readonly organisationIdKey = 'organisationId';
  private readonly teamsKey = 'teams';
  private internalTeams: Array<Team> = [];

  constructor(private storageService: StorageService) { }

  get currentUser(): User {
    return this.storageService.getItem<User>(this.currentUserKey);
  }
  set currentUser(value: User) {
    this.storageService.setItem(this.currentUserKey, value);
  }

  get teams(): Array<Team> {
    return this.internalTeams;
  }

  addTeam(value: Team) {
    this.internalTeams = this.teams.length == 0 ? this.storageService.getItem(this.teamsKey) ?? [] : this.internalTeams;
    if (!this.internalTeams.some(t => value.id === t.id)) {
      this.internalTeams.push(value);
      this.storageService.setItem(this.teamsKey, this.internalTeams);
    }
  }

  removeTeam(id: string) {
    this.internalTeams = this.teams.length == 0 ? this.storageService.getItem(this.teamsKey) ?? [] : this.internalTeams;
    this.internalTeams = this.internalTeams.filter(t => t.id !== id);
    this.storageService.setItem(this.teamsKey, this.internalTeams);
  }

  get organisationId(): string {
    return this.storageService.getItem<string>(this.organisationIdKey);
  }
  set organisationId(value: string) {
    this.storageService.setItem(this.organisationIdKey, value);
  }

  get authToken(): string {
    return this.storageService.getItem<string>(this.authTokenKey);
  }
  set authToken(value: string) {
    this.storageService.setItem(this.authTokenKey, value);
  }

  get authTokenExpires(): string {
    return this.storageService.getItemWithDefaultValue<string>(this.authTokenExpiresKey, '0');
  }
  set authTokenExpires(value: string) {
    this.storageService.setItem(this.authTokenExpiresKey, value);
  }

  refresh(authToken: string, expirationInMinutes: number): any {
    this.authToken = authToken;
    this.setAuthTokenExpires(expirationInMinutes);
  }

  destroy(): void {
    this.storageService.removeItem(this.currentUserKey);
    this.storageService.removeItem(this.authTokenKey);
    this.storageService.removeItem(this.authTokenExpiresKey);
    this.storageService.removeItem(this.teamsKey);
    this.internalTeams = [];
  }

  isLoggedIn(): boolean {
    const hasAuthToken = this.storageService.getItemWithDefaultValue<string>(this.authTokenKey, '');
    return hasAuthToken !== '' && !this.authTokenExpired();
  }

  shouldRefreshToken(): boolean {
    return this.getExpirationInMinutes() < 5;
  }

  tokenShortlyExpires(): boolean {
    return this.getExpirationInMinutes() < 2;
  }

  getExpirationInMinutes(): number {
    return moment(this.authTokenExpires).diff(new Date, 'minutes');
  }

  getExpirationInSeconds(): number {
    return moment(this.authTokenExpires).diff(new Date, 'seconds');
  }

  private setAuthTokenExpires(expirationInMinutes: number) {
    this.authTokenExpires = moment().add(expirationInMinutes, 'minutes').toJSON();
  }

  private authTokenExpired(): boolean {
    return !this.authTokenExpires || this.getExpirationInSeconds() < 1;
  }
}