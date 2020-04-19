import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-applications-list',
  templateUrl: './applications-list.component.html',
  styleUrls: ['./applications-list.component.scss']
})
export class ApplicationsListComponent implements OnInit {

  applications = [{
      name: 'poireport',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'TMS' }
    },{
      name: 'src',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'Taskne' }
    },{
      name: 'poiactivation',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'TMS' }
    },{
      name: 'SIG',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'BD' }
    },{
      name: 'WebDesk',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'Taskne' }
    },{
      name: 'Automator',
      initialVersion: 'v0.0.1',
      currentVersion: 'v3.2.1',
      lastDeploy: '2020-12-12',
      branches: 'develop, master',
      team: { name: 'Taskne' }
    },
  ];

  public showPersistencyApplicationModal: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  performShowApplicationPersistency() {
    this.showPersistencyApplicationModal = true;
  }

  onClosePersistencyModal(finished: boolean) {
    this.showPersistencyApplicationModal = false;
  }

}
