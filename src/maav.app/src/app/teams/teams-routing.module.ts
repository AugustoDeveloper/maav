import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeamListComponent } from './team-list/team-list.component';
import { TeamOverviewComponent } from './team-overview/team-overview.component';
import { ApplicationPersistencyComponent } from './applications/application-persistency/application-persistency.component';
import { ApplicationOverviewComponent } from './applications/application-overview/application-overview.component';
import { AuthGuardService } from '../shared/services/auth-guard.service';


const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'list', component: TeamListComponent , canActivate: [AuthGuardService] },
  { path: ':id', component: TeamOverviewComponent , canActivate: [AuthGuardService] },
  { path: ':id/app/:appId', component: ApplicationOverviewComponent, canActivate: [AuthGuardService] },
  { path: ':id/new-app', component: ApplicationPersistencyComponent, canActivate: [AuthGuardService] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeamsRoutingModule { }
