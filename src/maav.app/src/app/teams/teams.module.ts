import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamListComponent } from './team-list/team-list.component';
import { TeamOverviewComponent } from './team-overview/team-overview.component';
import { TeamsRoutingModule } from './teams-routing.module';
import { ClarityModule } from '@clr/angular';
import { TeamPersistencyComponent } from './team-persistency/team-persistency.component';
import { FormsModule } from '@angular/forms';
import { TeamUserPersistencyComponent } from './team-user-persistency/team-user-persistency.component';
import { ApplicationPersistencyComponent } from './applications/application-persistency/application-persistency.component';
import { ApplicationOverviewComponent } from './applications/application-overview/application-overview.component';

@NgModule({
  declarations: [
    TeamListComponent, 
    TeamOverviewComponent, 
    TeamPersistencyComponent, 
    TeamUserPersistencyComponent,
    ApplicationPersistencyComponent,
    ApplicationOverviewComponent
  ],
  imports: [
    CommonModule,
    ClarityModule,
    FormsModule,
    TeamsRoutingModule
  ]
})
export class TeamsModule { }
