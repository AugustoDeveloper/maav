import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/shared/models/user/user.model';
import { Roles } from 'src/app/teams/shared/roles.model';
import { UserService } from 'src/app/shared/services-model/user.service';
import { SessionService } from 'src/app/shared/services/session.service';
import { AlertManagementService } from 'src/app/shared/services/alert-management.service';

@Component({
  selector: 'app-user-persistency',
  templateUrl: './user-persistency.component.html',
  styleUrls: ['./user-persistency.component.scss']
})
export class UserPersistencyComponent implements OnInit {
  @Input('modelUser') public user: User;
  @Input() public isAdmin: boolean;
  @Input() public persistencyMode: string;
  @Output() public finished: EventEmitter<any> = new EventEmitter<any>(null);

  public confirmPassword: string;
  public showProgress: boolean = false;
  public resetPassword: boolean = false;
  private lastUpdated: boolean = false;
  public internalRoles: Roles;
  
  constructor(private userService: UserService,
    public alertService: AlertManagementService,
    private session: SessionService) {
      
  }

  public get buttonTitle(): string {
    if (this.persistencyMode == 'add') 
      return 'Create';

    return'Apply';
  }
    
  public get roles(): Roles {
    if (this.persistencyMode === 'view') {
      this.internalRoles = Roles.from(this.user.roles);
      this.lastUpdated = false;
    } else {
      if (!this.lastUpdated) {
        this.lastUpdated = true;
        this.internalRoles = Roles.from(this.user.roles);
      }
    }
  
    return this.internalRoles;
  }

  ngOnInit(): void {
    this.resetPassword = false;
    if (this.persistencyMode !== 'add') {
      this.confirmPassword = this.user.password;
    } else {
      this.roles.isUser = true;
    }
  }

  public close(finished: boolean) {
    this.showProgress = false;
    this.finished.emit({finished, model: this.user});
  }

  performResetPassword() {
    this.resetPassword = true;
  }

  onSubmit(form: any) {
    this.alertService.clear();
    this.showProgress = true;
    if (this.session.currentUser.username === this.user.username) {
      this.internalRoles = Roles.from(this.session.currentUser.roles);
    }

    this.user.roles = this.internalRoles.to();
    if (this.persistencyMode !== 'add') {
      if (this.resetPassword) {
        this.executeResetPassword();
      } else {
        this.updateUser();
      }
    } else {
      this.addUser();
    }
  }

  private addUser() {
    this.userService.add(this.user).subscribe( response => {
      this.user = response;
      this.close(true);
    }, error => {
      this.showProgress = false;
    })
  }

  private updateUser() {
    this.userService.update(this.user).subscribe( response => {
      this.user = response;
      this.close(true);
    }, error => {
      this.showProgress = false;
    });
  }

  private executeResetPassword() {
    if (this.user.password !== this.confirmPassword) {
      this.alertService.warn('The password is not match');
      this.showProgress = false;
      return;
    }

    this.userService.resetPassword(this.user).subscribe(response => {
      this.showProgress = false;
      this.resetPassword = false;
      this.close(true);
    }, error => {
      this.alertService.error('Oops...');
      this.showProgress = false;
      console.error(error);
    })
  }

  cancel() {
    this.user  = new User();
    this.close(false);
  }

}
