import { SemanticVersion } from '../application/semantic-version.model';
import { BranchActionRequest } from './branch-action-request.model';

export class KeyBranchVersion {
    public id: string;
    public previousId: string;
    public createdAt: string;
    public formatVersion: string;
    public version: SemanticVersion;
    public request: BranchActionRequest;
}