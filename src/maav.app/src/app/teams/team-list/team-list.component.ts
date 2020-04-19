import { Component, OnInit } from '@angular/core';
import { Team } from 'src/app/shared/models/team/team.model';
import { SessionService } from 'src/app/shared/services/session.service';
import { TeamService } from 'src/app/shared/services-model/team.service';
import { TeamNameFilter } from './shared/team-name.filter';

@Component({
  selector: 'app-team-list',
  templateUrl: './team-list.component.html',
  styleUrls: ['./team-list.component.scss']
})
export class TeamListComponent implements OnInit {
  public showPersistencyTeamModal: boolean = false;
  public teamNameFilter: TeamNameFilter = new TeamNameFilter();

  constructor(private session: SessionService,
              private teamService: TeamService) { }


  public get teams(): Array<Team> {
    return this.session.teams;
  }

  ngOnInit(): void {
  }

  performShowPersistencyModal() {
    this.showPersistencyTeamModal = true;
  }

  onClosePersistencyModal(event: any) {
    this.showPersistencyTeamModal = false;
    if (event.finished) {
      var team = {...event.newTeam as Team};
      this.session.addTeam(team);
    }
  }

  onSelectTeam(event: any) {
  }
}
