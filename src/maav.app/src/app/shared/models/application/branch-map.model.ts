import { IncrementMode } from './increment-mode.model';
import { KeyBranch } from './key-branch.model';

export class BranchMap {
    public name: string = '';
    public branchPattern: string = '';
    public allowBumpMajor: boolean = false;
    public bumpMajorText: string = '';
    public formatVersion: string = '';
    public increment: number = 0;
    public get incrementMode(): string {
        switch (this.increment) {
            case 3: return "Major";
            case 2: return "Minor";
            case 1: return "Patch";
            default: return "None";
        }
    }
    public inheritedFrom: KeyBranch;
}