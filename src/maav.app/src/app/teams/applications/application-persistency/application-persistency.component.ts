import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Application } from '../../../shared/models/application/application.model';
import { KeyBranch } from '../../../shared/models/application/key-branch.model';
import { NgForm, FormsModule, FormControl } from '@angular/forms';
import { BranchMap } from '../../../shared/models/application/branch-map.model';
import { IncrementMode } from '../../../shared/models/application/increment-mode.model';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationService } from 'src/app/shared/services-model/application.service';
import { TeamService } from 'src/app/shared/services-model/team.service';
import { AlertManagementService } from 'src/app/shared/services/alert-management.service';
import { CommonPatterns } from 'src/app/shared/utils/common-patterns.utils';
import { IncrementModeExtension } from 'src/app/shared/utils/increment-mode.extension';
import { create } from 'domain';

@Component({
  selector: 'app-application-persistency',
  templateUrl: './application-persistency.component.html',
  styleUrls: ['./application-persistency.component.scss']
})
export class ApplicationPersistencyComponent implements OnInit {
  @Input() public persistencyMode: string = 'add';
  @Input('modelApp') public currentApplication: Application = new Application();
  @Output() public closed: EventEmitter<boolean> = new EventEmitter(true);
  public teamId: string;
  public title: string = 'Add Application'
  public appName: string;
  public keyBranch: KeyBranch = new KeyBranch();
  public branchMap: BranchMap = new BranchMap();
  private internalShowDialog: boolean = true;
  public hasPreReleaseLabel: boolean = false;
  public hasBuildLabel: boolean = false;
  public usePreReleaseLabel: boolean = false;
  public useBuildLabel: boolean = false;
  public showProgress: boolean = false;
  public validationPatterns = CommonPatterns;
  public incrementModeMap: string;
  public buttonTitle: string = 'Create';
  public expandable: any;

  constructor(private router: Router,
              private route: ActivatedRoute,
              public alertService: AlertManagementService,
              private applicationService: ApplicationService,
              private teamService: TeamService) {
  }

  public ngOnInit(): void {
    if (this.persistencyMode === 'add') {
      this.buttonTitle = 'Create';
      this.route.params.subscribe(params => {
        this.teamId = params['id'];
        this.currentApplication = new Application();
      });
    } else {
      this.buttonTitle = 'Apply';
      this.teamId = this.currentApplication.teamId;
      this.hasBuildLabel = this.currentApplication.initialVersion.build && this.currentApplication.initialVersion.build.replace(' ', '').length > 0;
      this.hasPreReleaseLabel = this.currentApplication.initialVersion.preRelease && this.currentApplication.initialVersion.preRelease.replace(' ', '').length > 0;
      console.log(this.currentApplication);
    }
  }

  public onSubmit(form: any) {
    this.alertService.clear();
    this.showProgress = true;
    if (this.persistencyMode == 'add') {
      this.createApp();
    } else {
      this.updateApp();
    }
  }

  private updateApp() {
    this.applicationService.update(this.teamId, this.currentApplication).subscribe(response => {
      console.log(response);
      this.currentApplication = response;
      this.internalShowDialog = false;
      this.showProgress = false;
      this.close(true);
    }, error => {
      if (error.status == 400) {
        console.log(error);
        if (error.error && error.error.reason) {
          this.alertService.warn(error.error.reason);
        } else {
          this.alertService.error('Oops...');
        }
      }
      this.alertService.error('Oops...');
      this.showProgress = false;
    });
  }

  private createApp() {
    this.applicationService.add(this.teamId, this.currentApplication).subscribe(response => {
      this.currentApplication = response;
      this.internalShowDialog = false;
      this.showProgress = false;
      this.close(true);
    }, error => {
      if (error.status == 400) {
        console.log(error);
        if (error.error && error.error.reason) {
          this.alertService.warn(error.error.reason);
        } else {
          this.alertService.error('Oops...');
        }
      }

      this.alertService.error('Oops...');
      this.showProgress = false;
    });
  }

  public set showDialog(showing: boolean) {
    if (!showing) {
      this.close(false)
    }
  }
  
  public get showDialog() {
    return this.internalShowDialog;
  }

  private close(finished: boolean) {
    if (this.persistencyMode == 'add') {
      if (finished) {
        this.router.navigate([`../../team/${this.teamId}/app/${this.currentApplication.id}`]);
      } else {
        this.router.navigate(['../../team/'+this.teamId]);
      }
      return;
    } 
    this.reset();
    this.closed.emit(finished);
  }

  private reset() {
    if (this.persistencyMode === 'add'){
      this.currentApplication = new Application();
      this.keyBranch = new KeyBranch();
      this.branchMap = new BranchMap();
    }
  }

  public addKeyBranch(branch: KeyBranch) {
    console.log(branch);
    this.currentApplication.keyBranches.push(branch);
    this.keyBranch = new KeyBranch();
    this.usePreReleaseLabel = false;
    this.useBuildLabel = false;
  }  

  public deleteBranchVersion(branch: KeyBranch) {
    this.currentApplication.keyBranches = this.currentApplication.keyBranches.filter(b => b.name !== branch.name);
  }

  public addBranchMap(mapping: BranchMap) {
    mapping.increment = IncrementModeExtension.from(this.incrementModeMap);
    console.log(mapping);
    this.currentApplication.branches.push(mapping);
    this.branchMap = new BranchMap();
  }

  deleteBranchMap(mapping: BranchMap) {
    this.currentApplication.branches = this.currentApplication.branches.filter(b => b.name !== mapping.name);    
  }

  onUseLabel(event: any) {
    this.keyBranch.formatVersion = "{major}.{minor}.{patch}"
    if (this.usePreReleaseLabel) {
      this.keyBranch.formatVersion = this.keyBranch.formatVersion.concat('{prerelease}');
    }

    if (this.useBuildLabel){
      this.keyBranch.formatVersion = this.keyBranch.formatVersion.concat('{build}');
    }
  }
}
