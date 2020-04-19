import { KeyBranch } from './key-branch.model';
import { BranchMap } from './branch-map.model';
import { SemanticVersion } from './semantic-version.model';

export class Application {    
    public id: string = '';
    public name: string = '';
    public teamId: string = '';
    public createdAt: string;
    public webHookEnabled: boolean;
    public githubSecretKey: string;
    public initialVersion: SemanticVersion = new SemanticVersion();
    public keyBranches: Array<KeyBranch> = [];
    public branches: Array<BranchMap> = [];
}