import { Component, OnInit } from '@angular/core';
import { Authentication } from './shared/authentication.model';
import { SessionService } from '../shared/services/session.service';
import { Router, ActivatedRoute } from '@angular/router';
import { transAnimation } from '../animations';
import { animate, transition, style, state, trigger } from '@angular/animations';
import { AlertManagementService } from '../shared/services/alert-management.service';
import { AuthService } from '../shared/services-model/auth.service';
import { User } from '../shared/models/user/user.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  animations: [
    trigger('fadeInOut', [
      state('void', style({
        opacity: 0
      })),
      transition('void <=> *', animate(1000)),
    ]),
  ]
})
export class LoginComponent implements OnInit {
  public auth: Authentication = new Authentication();
  public showRegistration: boolean = false;
  public showProgress: boolean = false;


  constructor(private session: SessionService,
              private router: Router,
              private authService: AuthService,
              public alertService: AlertManagementService,
              private route: ActivatedRoute) {
      if (!this.session.isLoggedIn()){
        this.session.destroy();
      }
    }

  ngAfterContentInit() {
    
  }

  ngOnInit(): void {
    this.showRegistration = false;
    this.auth.organisationId = this.session.organisationId;
  }

  onSubmit(form: any) {
    if (!this.auth.organisationId || this.auth.organisationId.length < 1 || 
        !this.auth.username || this.auth.username?.length < 1 || 
        !this.auth.password ||
        this.auth.password?.length < 1) {
        this.alertService.warn("All fields are required");
      return;
    }

    this.showProgress = true;
    this.alertService.clear();
    this.authService.authenticate(this.auth.organisationId, this.auth.username, this.auth.password)
    .subscribe(response => {
      this.session.destroy();
      this.session.authToken = response.accessToken;
      this.session.authTokenExpires = response.expiration;
      this.session.currentUser = response.user;
      this.session.organisationId = response.organisationId;
      
      this.router.navigate(['']);
        
      this.showProgress = false;
    },
    error => {
      this.showProgress = false;
      if (error.status == 401) {
        this.alertService.warn("Invalid username or password");
        return;
      }

      if (error.status == 400) {
        this.alertService.error(error.error.reason);
        return;
      }

      if (error.status == 404) {
        this.alertService.info(error.error.reason);
        return;
      }

      this.alertService.error("Oops! Something goes wrong");
    });
  }

  redirectToRegistration() {
    this.router.navigate(['register']);
  }

  onBackRegistration(event: any) {
    this.showRegistration = false;
  }

}
