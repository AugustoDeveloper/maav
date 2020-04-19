import { User } from '../user/user.model';
import { Application } from '../application/application.model';

export class Team {
    public createdAt: string;
    public name: string = '';
    public id: string = '';
    public users: Array<User> = [];
    public applications: Array<Application> = [];
}