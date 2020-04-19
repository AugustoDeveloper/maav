import { User } from 'src/app/shared/models/user/user.model';
import { ClrDatagridStringFilterInterface } from '@clr/angular';

export class UsernameFilter implements ClrDatagridStringFilterInterface<User> {
    accepts(user: User, search: string) {
        return user.username === search || user.username.toLowerCase().indexOf(search) >= 0;
    }
}