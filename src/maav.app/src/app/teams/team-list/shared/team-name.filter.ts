import { ClrDatagridStringFilterInterface } from '@clr/angular';
import { Team } from 'src/app/shared/models/team/team.model';

export class TeamNameFilter implements ClrDatagridStringFilterInterface<Team> {
    accepts(team: Team, search: string) {
        return team.name === search || team.name.toLowerCase().indexOf(search) >= 0;
    }
}