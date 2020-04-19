import { Injectable } from '@angular/core';
import { Alert } from './shared/alert';
import { v1 as uuid } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class AlertManagementService {
  public alerts: Array<Alert> = [];  
  constructor() { }
  
  public warn(message: string) {
    var alert = new Alert();
    alert.id = uuid();
    alert.message = message;
    alert.type = 'warning';
    
    this.alerts.push(alert);
  }
  
  info(message: string) {
    var alert = new Alert();
    alert.id = uuid();
    alert.message = message;
    alert.type = 'info';
    
    this.alerts.push(alert);
  }
  
  success(message: string) {
    var alert = new Alert();
    alert.id = uuid();
    alert.message = message;
    alert.type = 'success';
    
    this.alerts.push(alert);
  }
  
  error(message: string) {
    var alert = new Alert();
    alert.id = uuid();
    alert.message = message;
    alert.type = 'danger';
    
    this.alerts.push(alert);
  }

  clear() {
    this.alerts = [];
  }

  public close(alert: Alert) {
    this.alerts = this.alerts.filter(a => a.id !== alert.id);
  }
}
