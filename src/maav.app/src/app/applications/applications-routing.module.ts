import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApplicationOverviewComponent } from './application-overview/application-overview.component';
import { ApplicationsListComponent } from './applications-list/applications-list.component';

const routes: Routes = [
  { path: 'list', component: ApplicationsListComponent },
  { path: ':id', component: ApplicationOverviewComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApplicationsRoutingModule { }
