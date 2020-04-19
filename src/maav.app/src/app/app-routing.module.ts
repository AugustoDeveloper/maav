import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { AuthGuardService } from './shared/services/auth-guard.service';
import { RegistrationComponent } from './registration/registration.component';


const routes: Routes = [
  { 
    path: 'dashboard', 
    canActivate: [ AuthGuardService ],
    component: DashboardComponent
  },
  { 
    path: '', 
    redirectTo: 'team',
    pathMatch: 'full', 
  },
  { 
    path: 'login', 
    component: LoginComponent,
    canActivate: [ AuthGuardService ]
  },
  { 
    path: 'register',
    component: RegistrationComponent, 
    canActivate: [ AuthGuardService ]
  },
  {
    path: 'application',
    loadChildren: () => import('./applications/applications.module').then(m => m.ApplicationsModule),
    canActivate: [ AuthGuardService ]
  },
  {
    path: 'team',
    loadChildren: () => import('./teams/teams.module').then(m => m.TeamsModule),
    canActivate: [ AuthGuardService ]
  },
  {
    path: 'user',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule),
    canActivate: [ AuthGuardService ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
