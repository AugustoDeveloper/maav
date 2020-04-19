import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/shared/models/user/user.model';
import { UserService } from 'src/app/shared/services-model/user.service';
import { SessionService } from 'src/app/shared/services/session.service';
import { Roles } from 'src/app/teams/shared/roles.model';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  public persistencyMode: string = 'view'
  public persistentUser: User = new User();
  public userRoles: Roles;
  public users: Array<User> = [];
  public showDeleteModal: boolean = false;
  public showProgress: boolean = false;

  constructor(private userService: UserService,
              private session: SessionService) { 
    this.userRoles = Roles.from(this.session.currentUser.roles);
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.showProgress = true;
    this.userService.loadAllUsers().subscribe(response => {
      this.users = response;
      this.showProgress = false;
    });
  }

  finish(event: any) {
    if (event.finished) {
      this.loadUsers();
    }

    this.persistencyMode = 'view';
  }

  selectionChanged(event: any) {
    if (event == null) {
      return;
    }

    if (event == true) {
      return;
    }

    this.persistentUser = event;  
  }

  performAddUser() {
    this.persistentUser = new User();
    this.persistencyMode = 'add';
  }
  

  editUser(user: User) {
    this.persistentUser = user;
    this.persistencyMode = 'edit';
  }

  deleteUser(user: User) {
    this.persistentUser = user;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    this.showProgress = true;
    this.userService.delete(this.persistentUser.username).subscribe(r => {
      this.loadUsers();
      this.showDeleteModal = false;
    })
  }

  cancelDelete() {
    this.showDeleteModal = false;
  }


}
