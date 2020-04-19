import { Component } from '@angular/core';
import { SessionService} from './shared/services/session.service'
import { Router } from '@angular/router';
import { logging } from 'protractor';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'app';
  constructor(private session: SessionService,
              private router: Router) {
  }

  isLoggedIn() : boolean {
    var result = this.session.isLoggedIn();

    return result;
  }

  isNotLoggedInAndLogin() {
    let isLogin = this.router.url.indexOf('login') > -1;
    let result = !this.isLoggedIn() && isLogin;
    return result;
  }

  isNotLoggedInAndRegistration() {
    let isRegiter = this.router.url.indexOf('register') > -1;
    let result = !this.isLoggedIn() && isRegiter;

    return !this.isLoggedIn() && isRegiter;
  }
}
