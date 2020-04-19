import { BranchVersion } from './branch-version.model';
import { BranchMap } from './branch-map.model';
import { Team } from '../team/team.model';

export class Application {    
    public name: string = '';
    public initialVersion: string = '';
    public formatVersion: string = '';
    public branchVersions: Array<BranchVersion> = []
    public map: Array<BranchMap> = []
    public team: Team = new Team();
}