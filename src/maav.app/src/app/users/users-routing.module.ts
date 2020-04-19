import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserPersistencyComponent } from './user-persistency/user-persistency.component';
import { UsersListComponent } from './users-list/users-list.component';


const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'list', component: UsersListComponent },
  { path: ':id', component: UserPersistencyComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
