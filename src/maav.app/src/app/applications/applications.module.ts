import { NgModule } from '@angular/core';
import { ApplicationsRoutingModule } from './applications-routing.module';
import { CommonModule } from '@angular/common';
import { ApplicationsListComponent } from './applications-list/applications-list.component';
import { ApplicationOverviewComponent } from './application-overview/application-overview.component';
import { ClarityModule } from '@clr/angular';
import { ApplicationPersistencyComponent } from './application-persistency/application-persistency.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    ApplicationsListComponent, 
    ApplicationOverviewComponent, 
    ApplicationPersistencyComponent
  ],
  imports: [
    CommonModule,
    ClarityModule,
    ApplicationsRoutingModule,
    FormsModule
  ]
})
export class ApplicationsModule { }
