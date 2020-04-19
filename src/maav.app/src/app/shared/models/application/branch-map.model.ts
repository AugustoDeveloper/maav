import { IncrementMode } from './increment-mode.model';

export class BranchMap {
    public name: string = '';
    public pattern: string = '';
    public allowBumpMajorVersion: boolean = false;
    public incrementMode: IncrementMode = IncrementMode.None;
    public suffixFormat: string = '';
    public isKeyBranch: boolean = false;
    public inheritedFrom: string = '';
}