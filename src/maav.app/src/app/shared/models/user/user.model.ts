import { Role } from './role.model';
import { TeamPermission } from '../team/team-permission.model';

export class User {
    public firstName: string = '';
    public lastName: string = '';
    public username: string = '';
    public password: string = '';
    public teamsPermissions: Array<TeamPermission> = [];
    public roles: Array<string> = []
}