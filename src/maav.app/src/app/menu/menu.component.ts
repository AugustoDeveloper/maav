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


@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent implements OnInit {
  public applications: Array<Application> = [];
  public user: User;
  
  constructor(private router: Router,
    private session: SessionService,
    private teamService: TeamService,
    private applicationService: ApplicationService,
    private userService: UserService,
    private activateRoute: ActivatedRoute) {
  }
    
  ngOnInit(): void {
    this.user = this.session.currentUser;
    this.loadUser();
  }

  public get teams(): Array<Team> {
    return this.session.teams;
  }

  private loadUser() {
    this.userService.getByUsername(this.session.currentUser.username).subscribe(response => {
      this.session.currentUser = response;
      this.user = this.session.currentUser;
      this.loadTeams();
    }) 
  }

  private loadTeams() {
    let teamIds = this.session.currentUser.teamsPermissions.map(r => r.teamId);
    forkJoin(teamIds.map(teamId => this.teamService.getById(teamId))).subscribe(teams => {
        teams.forEach(team => this.session.addTeam(team));
    });
  }

  performLogout() {
    this.session.destroy();
    this.router.navigate(['../login']);
  }
}
