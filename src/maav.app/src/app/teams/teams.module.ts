import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamListComponent } from './team-list/team-list.component';
import { TeamOverviewComponent } from './team-overview/team-overview.component';
import { TeamsRoutingModule } from './teams-routing.module';
import { ClarityModule } from '@clr/angular';
import { TeamPersistencyComponent } from './team-persistency/team-persistency.component';
import { FormsModule } from '@angular/forms';
import { TeamUserPersistencyComponent } from './team-user-persistency/team-user-persistency.component';

@NgModule({
  declarations: [
    TeamListComponent, 
    TeamOverviewComponent, 
    TeamPersistencyComponent, TeamUserPersistencyComponent],
  imports: [
    CommonModule,
    ClarityModule,
    FormsModule,
    TeamsRoutingModule
  ]
})
export class TeamsModule { }
