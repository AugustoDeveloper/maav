import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService {
  constructor(private router: Router, private session: SessionService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.session.isLoggedIn() && state.url.indexOf('/login') !== 0 && state.url.indexOf('/register') !== 0 ) {
      this.session.destroy();
      const navigationExtras = {
        queryParams: state && state.url !== '/' ? { redirect: state.url } : null
      };
      this.router.navigate([ 'login' ], navigationExtras);

      return false;
    }

    if (this.session.isLoggedIn() && (state.url.indexOf('/login') === 0 || state.url.indexOf('/register') === 0 )) {

      this.router.navigate(['']);
    }

    return true;
  }
}
