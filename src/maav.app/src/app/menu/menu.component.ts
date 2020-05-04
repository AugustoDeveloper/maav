import { Component, OnInit } from '@angular/core';
import { Router, Route, ActivatedRoute } from '@angular/router';
import { SessionService } from '../shared/services/session.service';
import { TeamService } from '../shared/services-model/team.service';
import { ApplicationService } from '../shared/services-model/application.service';
import { Application } from '../shared/models/application/application.model';
import { Team } from '../shared/models/team/team.model';
import { UserService } from '../shared/services-model/user.service';
import { Roles } from '../teams/shared/roles.model';
import { User } from '../shared/models/user/user.model';
import { forkJoin } from 'rxjs';
import { OrganisationService } from '../shared/services-model/organisation.service';
import { error } from 'protractor';


@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
  public applications: Array<Application> = [];
  public user: User;
  public orgName: string;
  public showProgress: boolean;

  constructor(private router: Router,
    private session: SessionService,
    private teamService: TeamService,
    private organisatioService: OrganisationService,
    private userService: UserService,
    private activateRoute: ActivatedRoute) {
  }
    
  ngOnInit(): void {
    this.user = this.session.currentUser;
    this.loadOrganisation(this.session.organisationId);
  }
  
  public get teams(): Array<Team> {
    return this.session.teams;
  }

  private loadOrganisation(organisationId: string) {
    this.showProgress = true;
    this.organisatioService.getOrganisationById(organisationId).subscribe(response =>{
      this.orgName = response.name;
      this.loadUser();
    })
  }

  private loadUser() {
    this.userService.getByUsername(this.session.currentUser.username).subscribe(response => {
      this.session.currentUser = response;
      this.user = this.session.currentUser;
      this.loadTeams();
    }, error => {
      this.showProgress = false;
    }) 
  }

  private loadTeams() {
    let teamIds = this.session.currentUser.teamsPermissions.map(r => r.teamId);
    if (teamIds.length > 0 ) {
      forkJoin(teamIds.map(teamId => this.teamService.getById(teamId))).subscribe(teams => {
          teams.forEach(team => this.session.addTeam(team));
          this.showProgress = false;
      }, error => {
        this.showProgress = false;
      });
    } else {
      this.showProgress = false;
    }
  }

  performLogout() {
    this.session.destroy();
    this.router.navigate(['../login']);
  }
}
