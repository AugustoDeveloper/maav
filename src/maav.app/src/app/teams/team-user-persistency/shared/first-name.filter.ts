import { User } from 'src/app/shared/models/user/user.model';
import { ClrDatagridStringFilterInterface } from '@clr/angular';

export class FirstNameFilter implements ClrDatagridStringFilterInterface<User> {
    accepts(user: User, search: string) {
        return user.firstName === search || user.firstName.toLowerCase().indexOf(search) >= 0;
    }
}