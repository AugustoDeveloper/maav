import { Component, OnInit } from '@angular/core';
import { Application } from 'src/app/shared/models/application/application.model';
import { Team } from 'src/app/shared/models/team/team.model';
import { BranchMap } from 'src/app/shared/models/application/branch-map.model';
import { IncrementMode } from 'src/app/shared/models/application/increment-mode.model';
import { BranchVersion } from 'src/app/shared/models/application/branch-version.model';

@Component({
  selector: 'app-application-overview',
  templateUrl: './application-overview.component.html',
  styleUrls: ['./application-overview.component.scss']
})
export class ApplicationOverviewComponent implements OnInit {
  public currentApp: Application = new Application();
  public selectedBranchVersion: BranchVersion;
  
  constructor() { }

  ngOnInit(): void {
    this.currentApp.name = "TMS PoiProxy";
    this.currentApp.initialVersion = "1.0.0";
    this.currentApp.formatVersion = "{major}.{minor}.{patch}";
    this.currentApp.team = new Team();
    this.currentApp.team.name = "TMS";
    for (let index = 0; index < 4; index++) {
      var map = new BranchMap();
      let inc = (index+1) % 4;

      this.currentApp.map.push(map);
      map.name = `Map - ${index}`;
      map.allowBumpMajorVersion = index % 2 == 0;
      map.incrementMode =  inc == 0 ? 1 : inc;
      map.inheritedFrom = 'develop';
      map.isKeyBranch = true;
      map.pattern = "dev[elop]"
      map.suffixFormat = ".{build}";
    }

    for (let index = 0; index < 3; index++) {
      var version = new BranchVersion();
      this.currentApp.branchVersions.push(version);
      version.name = "develop";
      version.enabled = index % 2 === 0;
      version.version = `1.${index}${index+1}`      
    }
  }

  deleteApplication(currentApp: Application) {
    /*public name: string = '';
    public initialVersion: string = '';
    public formatVersion: string = '';
    public branchVersions: Array<BranchVersion> = []
    public map: Array<BranchMap> = []
    public team: Team = new Team();*/

  }
}
