import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SessionService } from './session.service';
import { AlertManagementService } from './alert-management.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CommonRestService {

  constructor(private http: HttpClient,
              private session: SessionService,
              private alert: AlertManagementService,
              private router: Router) { }

  private get commonHeaders():  HttpHeaders {
    return new HttpHeaders()
      .set('Authorization', `Bearer ${ this.session.authToken }`)
      .set('Accept', 'application/json');
  }
  
  get<T>(url: string, headers?: HttpHeaders): Observable<T> {
    const httpHeaders = this.commonHeaders;
    return this.http.get<T>(url, { headers: httpHeaders })
    .pipe(catchError(this.handleError));
  }

  post<T>(url: string, body: any, manualCheckError: boolean = false): Observable<T> {
    var result = this.http.post<T>(url, body, { headers: this.commonHeaders })
    if (!manualCheckError)
      return result.pipe(catchError(this.handleError));
    return result;
  }

  put<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
    const httpHeaders = this.commonHeaders;
    return this.http.put<T>(url, body, { headers: httpHeaders })
    .pipe(catchError(this.handleError));
  }

  patch<T>(url: string, body: any, headers?: HttpHeaders): Observable<T> {
    const httpHeaders = this.commonHeaders;
    return this.http.patch<T>(url, body, { headers: httpHeaders })
    .pipe(catchError(this.handleError));
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(url, { headers: this.commonHeaders })
    .pipe(catchError(this.handleError));
  }

  private handleError = (response: Response) => {
    switch(response.status){
      case ResponseHttpStatus.Unauthorized:
        this.onUnauthorized();
        break;
      case ResponseHttpStatus.Forbidden:
        this.onForbidden();
        break;
      default:
        return throwError(response);
    }
  }

  private onUnauthorized(){
    this.alert.warn('Session expired.');

    const navigationExtras = {
      queryParams: this.router.url !== '/' ? { redirect: this.router.url } : null
    };
    this.router.navigate([ 'login' ], navigationExtras);
  }

  private onForbidden(){
    const navigationExtras = {
      queryParams: this.router.url !== '/' ? { redirect: this.router.url } : null
    };
    this.router.navigate([ 'login' ], navigationExtras);
  }

}

export enum ResponseHttpStatus {
  Unauthorized = 401,
  NotFound = 404,
  MethodNotAllowed = 405,
  BadRequest = 400,
  Forbidden = 403,
  InternalServerError = 500
}
