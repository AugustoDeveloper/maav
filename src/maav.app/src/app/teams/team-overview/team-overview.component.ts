import { Component, OnInit } from '@angular/core';
import { Team } from 'src/app/shared/models/team/team.model';
import { Router, ActivatedRoute } from '@angular/router';
import { User } from 'src/app/shared/models/user/user.model';
import { Role } from 'src/app/shared/models/user/role.model';
import { Application } from 'src/app/shared/models/application/application.model';
import { BranchVersion } from 'src/app/shared/models/application/branch-version.model';
import { TeamService } from 'src/app/shared/services-model/team.service';
import { UserService } from 'src/app/shared/services-model/user.service';
import { AlertManagementService } from 'src/app/shared/services/alert-management.service';
import { SessionService } from 'src/app/shared/services/session.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-team-overview',
  templateUrl: './team-overview.component.html',
  styleUrls: ['./team-overview.component.scss']
})
export class TeamOverviewComponent implements OnInit {
  public team: Team = new Team();
  public sortUsers: Array<User> = [];
  public selectedUser: User = new User();
  public showUserPersistencyModal: boolean = false;
  public showProgress: boolean = true;
  public showDeleteModal: boolean = false;
  public deleteType: string = '';
  public deleteTitle: string;
  public showUserTeamPersistencyModal: boolean = false;

  constructor(private router: Router,
              private activatedRoute: ActivatedRoute,
              public alertService: AlertManagementService,
              private session: SessionService,
              private teamService: TeamService,
              private userService: UserService) { 
    this.activatedRoute.params.subscribe(params => {
      this.loadTeam(params['id']);
    });
  }

  private loadTeam(teamId: string) {
    this.showProgress = true;
      this.sortUsers = [];
      this.teamService.getById(teamId).subscribe(responseTeam => {
        this.team = responseTeam;
        let usernames = responseTeam.users.map(u => u.username);
        let obs = usernames.map(usr => this.userService.getByUsername(usr));
        forkJoin(obs).subscribe(responseUser =>{
          this.session.currentUser = responseUser.filter(u => u.username === this.session.currentUser.username)[0];
          this.sortUsers = responseUser;
          this.showProgress = false;
        });
      }, errorTeam => {
        if (errorTeam.status == 404) {
          this.session.removeTeam(this.team.id);
          this.router.navigate(['list']);
        }
        this.showProgress = false;
      });
  }

  public loadRoles(user: User): string {
    return user.roles.join(',');
  }

  public loadPermissions(user: User): string {
    let permissions = user.teamsPermissions.filter(t => t.teamId == this.team.id)[0];
    var permissionText = []
    if (permissions.isOwner) {
      permissionText.push("owner");
    }
    if (permissions.isReader) {
      permissionText.push("reader");
    }
    if (permissions.isWriter) {
      permissionText.push("writer");
    }
    return permissionText.join(',');
  }

  ngOnInit(): void {
  }

  public deleteTeam(team:Team) {
    this.deleteType = 'team';
    this.deleteTitle = `Do you want delete the team ${team.name}?`;
    this.showDeleteModal = true;
  }

  public deleteUser(user: User) {
    this.selectedUser = user;
    this.deleteTitle = `Do you want delete the user ${user.username}?`;
    this.deleteType = 'user';
    this.showDeleteModal = true;
    
  }

  public performAddUser() {
    this.showUserTeamPersistencyModal = true;
  }

  public performAddApp() {

  }

  onUserPersistencyModalClosed(event: any) {
    if (event.finished) {
      this.loadTeam(this.team.id);
    }
    this.showUserPersistencyModal = false;
  }

  private performDeleteUser() {
    this.showProgress = true;
    this.userService.dettachFromTeam(this.team.id, this.selectedUser.username).subscribe(r => {
      this.loadTeam(this.team.id);
      this.showDeleteModal = false;
    });
  }

  private performDeleteTeam() {
    this.showProgress = true;
    this.teamService.delete(this.team.id).subscribe(r => {
      this.session.removeTeam(this.team.id);
      this.userService.getByUsername(this.session.currentUser.username).subscribe(response => {
        this.session.currentUser = response;
        this.showDeleteModal = false;
        this.showProgress = false;
        this.router.navigate(['team/list']);
      });
    });
  }

  confirmDelete() {
    if (this.deleteType === 'team') {
      this.performDeleteTeam();
    }

    if (this.deleteType === 'user') {
      this.performDeleteUser();
    }
  }

  cancelDelete() {
    this.showDeleteModal = false;
  }

  onCloseUserTeamPersistencyModal(event: any) {
    if (event.finished){
      this.loadTeam(this.team.id);
    }
    this.showUserTeamPersistencyModal = false;
  }
}