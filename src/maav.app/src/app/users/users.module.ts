import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersListComponent } from './users-list/users-list.component';
import { ClarityModule } from '@clr/angular';
import { FormsModule } from '@angular/forms';
import { UsersRoutingModule } from './users-routing.module';
import { UserPersistencyComponent } from './user-persistency/user-persistency.component';

@NgModule({
  declarations: [
    UsersListComponent,
    UserPersistencyComponent
  ],
  imports: [
    CommonModule,
    ClarityModule,
    FormsModule,
    UsersRoutingModule
  ]
})
export class UsersModule { }
