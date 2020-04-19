import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Registration } from '../shared/models/registration.model';
import { style, trigger, state, transition, animate } from '@angular/animations';
import { CommonPatterns } from '../shared/utils/common-patterns.utils';
import { OrganisationService } from '../shared/services-model/organisation.service';
import { Router } from '@angular/router';
import { AlertManagementService } from '../shared/services/alert-management.service';
import { Alert } from '../shared/services/shared/alert';
import { NgForm } from '@angular/forms';
import { SessionService } from '../shared/services/session.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
  animations: [
    trigger('fadeInOut', [
      state('void', style({
        opacity: 0
      })),
      transition('void <=> *', animate(1000)),
    ]),
  ]
})
export class RegistrationComponent implements OnInit {
  public showProgress: boolean = false;
  public registration: Registration = new Registration();
  public confirmAdminPassword: string = '';
  public validationPatterns = CommonPatterns;

  constructor(private orgService: OrganisationService,
              private router: Router,
              private session: SessionService,
              public alertService: AlertManagementService) { }

  ngOnInit(): void {
    this.session.destroy();    
  }

  closeAlert(alert: Alert) {
    this.alertService.close(alert);
  }

  onSubmit(form: any) {
    if (form.password.value && form.confirmPassword.value && 
      form.orgId && form.orgName && form.adminName && form.adminLastName
      && form.adminUsername && form.password) {
        return;
      }
      
    if (form.password.value !== form.confirmPassword.value) {
      this.alertService.error('Passwords not match');
      return;
    }

    this.showProgress = true;
    this.orgService.register(this.registration).subscribe(response => {
      this.session.organisationId = response.id;
      this.router.navigate(['login']);
      this.showProgress = false;
    },
    error => {
      if (error.status == 400) {
        this.alertService.error(error.error.reason);
      } else {
        this.alertService.error('Oops! Something\'s goes wrong.');
      }

      this.showProgress = false;
    })
  }
}
