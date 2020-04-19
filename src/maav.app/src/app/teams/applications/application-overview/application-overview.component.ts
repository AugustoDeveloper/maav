import { Component, OnInit } from '@angular/core';
import { Application } from 'src/app/shared/models/application/application.model';
import { Team } from 'src/app/shared/models/team/team.model';
import { BranchMap } from 'src/app/shared/models/application/branch-map.model';
import { KeyBranch } from 'src/app/shared/models/application/key-branch.model';
import { SemanticVersion } from 'src/app/shared/models/application/semantic-version.model';
import { ApplicationService } from 'src/app/shared/services-model/application.service';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamService } from 'src/app/shared/services-model/team.service';
import { VersionService } from 'src/app/shared/services-model/version.service';
import { VersionHistory } from 'src/app/shared/models/versions/version-history.model';
import { KeyBranchVersion } from 'src/app/shared/models/versions/keybranch-version.model';
import { environment } from 'src/environments/environment';
import { SessionService } from 'src/app/shared/services/session.service';
import { interval, Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-application-overview',
  templateUrl: './application-overview.component.html',
  styleUrls: ['./application-overview.component.scss']
})
export class ApplicationOverviewComponent implements OnInit {
  public currentApp: Application = new Application();
  public currentTeam: Team = new Team();
  public selectedKeyBranch: KeyBranch;
  public teamId: string;
  public appId: string;
  public initialVersionFormatted: string;
  public showProgress: boolean = true;
  public showDeleteModal: boolean = false;
  public showApplicationPersistencyModal: boolean = false;
  public currentHistory: VersionHistory;
  public sortedHistory: Array<KeyBranchVersion>;
  public webhookEndpoint: string;
  public updateHistorySub: Subscription;

  constructor(private applicationService: ApplicationService,
              private router: Router,
              private route: ActivatedRoute,
              private versionService: VersionService,
              private teamService: TeamService,
              private session: SessionService) {
   }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.showProgress = true;
      this.teamId = params['id'];
      this.appId = params['appId'];
      this.getApplication(this.teamId, this.appId);
      this.webhookEndpoint = `${environment.baseUri}${this.session.organisationId}/teams/${this.teamId}/apps/${this.appId}/webhook`;
      this.updateHistorySub = interval(15000).subscribe(val => {
        if (this.sortedHistory != null) {
          this.updateHistory();
        }
    });
    });
  }

  getApplication(teamId: string, applicationId: string) {
    this.applicationService.get(teamId, applicationId).subscribe(responseApp => {
        this.currentApp = responseApp;
        this.initialVersionFormatted = SemanticVersion.format(responseApp.initialVersion);
        this.teamService.getById(teamId).subscribe(responseTeam => {
          this.currentTeam = responseTeam;
          this.selectedKeyBranch = responseApp.keyBranches[0];
        }, errorTeam => {
          console.log(errorTeam);
          this.showProgress = false;
        });
    }, errorApp => {
      if (errorApp.status == 404) {
        this.router.navigate(['../../team/'+this.teamId]);
      }
      console.log(errorApp);
      this.showProgress = false;
    });
  }

  isLastVersion(version: KeyBranchVersion): boolean {
    let result = version.id == this.currentHistory.lastHistoryId;

    return result;
  }

  onSelectedKeyBranch(event: any) {
    this.showProgress = true;
    this.updateHistory()
  }

  updateHistory() {
    this.versionService.getByKeyBranchName(this.teamId, this.appId, this.selectedKeyBranch.name).subscribe(response => {
      this.currentHistory = response;
      this.sortedHistory = response.versionHistory.sort((a, b) =>  { return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime() });
      this.showProgress = false;
    }, error => {
      this.showProgress = false;
    })
  }

  getFormattedVersion(version: KeyBranchVersion): string {
    let result = SemanticVersion.formatFrom(version.version, version.formatVersion)
    return result;
  }
  
  editApp(app: Application) {
    this.showApplicationPersistencyModal = true;
  }

  getInheritedFromName(branch: BranchMap) {
    return branch.inheritedFrom?.name ?? '';
  }

  deleteApplication(currentApp: Application) {
    this.showDeleteModal = true;

  }
  confirmDelete() {
    this.showProgress = true;
    this.applicationService.delete(this.currentTeam.id, this.currentApp.id).subscribe(response => {
      this.showProgress = false;
      this.showDeleteModal = false;
      this.router.navigate(['../../team/'+this.teamId]);
    }, error => {
      if (error.status == 404) {
        this.router.navigate(['../../team/'+this.teamId]);
      }
      console.log(error);
      this.showProgress = false;
    });
  }

  cancelDelete() {
    this.showDeleteModal = false;
  }

  onClosedApplicationPersistencyModal(event: any) {
    if (event) {
      this.getApplication(this.teamId, this.appId);
    }
    this.showApplicationPersistencyModal = false;
  }
}
