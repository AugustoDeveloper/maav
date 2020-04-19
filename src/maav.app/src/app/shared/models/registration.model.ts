import { User } from './user/user.model';

export class Registration {
    public id: string;
    public name: string;
    public adminUser: User = new User();
}