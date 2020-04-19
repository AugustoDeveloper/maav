import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { CommonPatterns } from 'src/app/shared/utils/common-patterns.utils';
import { Team } from 'src/app/shared/models/team/team.model';
import { UserService } from 'src/app/shared/services-model/user.service';
import { AlertManagementService } from 'src/app/shared/services/alert-management.service';
import { User } from 'src/app/shared/models/user/user.model';
import { TeamPermission } from 'src/app/shared/models/team/team-permission.model';
import { UsernameFilter } from './shared/username.filter';
import { FirstNameFilter } from './shared/first-name.filter';
import { forkJoin, zip } from 'rxjs';

@Component({
  selector: 'app-team-user-persistency',
  templateUrl: './team-user-persistency.component.html',
  styleUrls: ['./team-user-persistency.component.scss']
})
export class TeamUserPersistencyComponent implements OnInit {
  @Output() public closed: EventEmitter<any> = new EventEmitter(null);
  @Input('modelTeam') public team: Team = new Team();
  public usernameFilter: UsernameFilter = new UsernameFilter();
  public firstNameFilter: FirstNameFilter = new FirstNameFilter();
  public title: string = 'Add Team';
  public validationPatterns = CommonPatterns;
  private internalShowDialog: boolean = true;
  public showProgress: boolean = false;
  public orgUsers: Array<User> = [];
  public teamUsers: Array<User> = [];
  public selectedOrgUser: User;
  public optionPermission: string = '';

  constructor(private userService: UserService,
              public alertService: AlertManagementService) { }

  ngOnInit(): void {
    this.showProgress = true;
    this.userService.loadAllUsers().subscribe(response => {      
      this.orgUsers = response.filter(u => !this.team.users.some(tu => tu.username === u.username));
      this.showProgress = false;
    });
  }

  public set showDialog(showing: boolean) {
    if (!showing) {
      this.close(false);
    }
  }

  public get showDialog() {
    return this.internalShowDialog;
  }

  onSubmit(form: any) {
    this.showProgress = true;
    this.userService.attachUsersToTeam(this.team.id, this.teamUsers).subscribe(response => {
      this.teamUsers = [];
      this.showProgress = false;
      this.close(true);
    });
  }

  close(finished: boolean) {
    this.showProgress = false;
    this.closed.emit({ finished: finished});
  }

  reset() {
    this.team = new Team();
  }

  addOrgUserToTeam(user: User) {
    this.alertService.clear();
    if (this.optionPermission === ''){
      this.alertService.warn('Select some permission for user');
      return;
    }
    let permission = TeamPermission.from(this.optionPermission, this.team.id);
    user.teamsPermissions.push(permission);
    this.teamUsers.push(user);
    this.orgUsers = this.orgUsers.filter(u => u.username !== user.username);
    this.optionPermission = '';
  }

  deleteUser(user: User) {
    this.alertService.clear();
    user.teamsPermissions = user.teamsPermissions.filter(tp => tp.teamId !== tp.teamId);
    this.orgUsers.push(user);
    this.teamUsers = this.teamUsers.filter(u => u.username !== user.username)
  }
  
  public loadPermissions(user: User): string {
    let permissions = user.teamsPermissions.filter(t => t.teamId === this.team.id)[0];
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
}
