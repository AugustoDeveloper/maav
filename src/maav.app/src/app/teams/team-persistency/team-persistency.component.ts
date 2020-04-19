import { Component, OnInit, Input, Output, EventEmitter, ViewChild, OnDestroy } from '@angular/core';
import { Team } from 'src/app/shared/models/team/team.model';
import { CommonPatterns } from '../../shared/utils/common-patterns.utils'
import { AlertManagementService } from 'src/app/shared/services/alert-management.service';
import { TeamService } from 'src/app/shared/services-model/team.service';
import { SessionService } from 'src/app/shared/services/session.service';
import { TeamPermission } from 'src/app/shared/models/team/team-permission.model';

@Component({
  selector: 'app-team-persistency',
  templateUrl: './team-persistency.component.html',
  styleUrls: ['./team-persistency.component.scss']
})
export class TeamPersistencyComponent implements OnInit {
  @Input() public persistencyMode: string = 'Add';
  @Output() public closed: EventEmitter<any> = new EventEmitter(null);
  public title: string = 'Add Team'
  public team: Team = new Team();
  public validationPatterns = CommonPatterns;
  private internalShowDialog: boolean = true;
  public showProgress: boolean = false;

  constructor(public alertService: AlertManagementService,
              private teamService: TeamService,
              private session: SessionService) { 
  }

  ngOnInit(): void {
  }

  onSubmit(form: any) {
    if (!form.value.teamName || !form.value.teamId) {
      this.alertService.warn('Fill the required fields');
      return;
    }

    this.showProgress = true;
    this.teamService.create(this.team).subscribe(response => {
      this.team = response;
      let permission = new TeamPermission();
      permission.isOwner = true;
      permission.teamId = this.team.id;
      this.session.currentUser.teamsPermissions.push(permission);
      this.close(true);
    }, error => {
      if (error.status == 400) {
        this.alertService.warn(error.error.reason);
      }

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

  close(finished: boolean) {
    this.showProgress = false;
    this.closed.emit({ finished: finished, newTeam: this.team});
  }

  reset() {
    this.team = new Team();
  }
}