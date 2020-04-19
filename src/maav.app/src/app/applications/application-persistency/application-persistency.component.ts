import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Application } from '../../shared/models/application/application.model';
import { BranchVersion } from '../../shared/models/application/branch-version.model';
import { NgForm, FormsModule, FormControl } from '@angular/forms';
import { BranchMap } from '../../shared/models/application/branch-map.model';
import { IncrementMode } from '../../shared/models/application/increment-mode.model';

@Component({
  selector: 'app-application-persistency',
  templateUrl: './application-persistency.component.html',
  styleUrls: ['./application-persistency.component.scss']
})
export class ApplicationPersistencyComponent implements OnInit {
  @Input() public persistencyMode: string = 'Add';
  @Output() public closed: EventEmitter<boolean> = new EventEmitter(true);
  public title: string = 'Add Application'
  public app: Application = new Application();
  public branchVersion: BranchVersion = new BranchVersion();
  public branchMap: BranchMap = new BranchMap();
  private internalShowDialog: boolean = true;


  constructor() { }

  public ngOnInit(): void {
  }

  public onSubmit(form: any) {
    this.showDialog = false;
  }

  public set showDialog(showing: boolean) {
    if (!showing) {
      this.close()
    }
  }
  
  public get showDialog() {
    return this.internalShowDialog;
  }

  private close() {
    this.reset();
    this.closed.emit(true);
  }

  private reset() {
    this.app = new Application();
    this.branchVersion = new BranchVersion();
    this.branchMap = new BranchMap();
  }

  public addBranchVersion(branch: BranchVersion){
    this.app.branchVersions.push({...branch});
    this.branchVersion = new BranchVersion();
  }  

  public deleteBranchVersion(branch: BranchVersion) {
    this.app.branchVersions = this.app.branchVersions.filter(b => b.name !== branch.name);
  }

  public addBranchMap(mapping: BranchMap){
    this.app.map.push({...mapping});
    this.branchMap = new BranchMap();
  }

  deleteBranchMap(mapping: BranchMap) {
    this.app.branchVersions = this.app.branchVersions.filter(b => b.name !== mapping.name);
  }
}
