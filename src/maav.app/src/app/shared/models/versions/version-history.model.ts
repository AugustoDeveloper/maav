import { KeyBranchVersion } from './keybranch-version.model';

export class VersionHistory {
    public id: string;
    public organisationId: string;
    public teamId: string;
    public applicationId: string;
    public keyBranchNane: string;
    public lastHistoryId: string;
    public versionHistory: Array<KeyBranchVersion>;
}