import { IncrementMode } from '../models/application/increment-mode.model';

export class IncrementModeExtension {
    static to(increment: IncrementMode):number {
        switch (increment) {
            case IncrementMode.Major: return 3;
            case IncrementMode.Minor: return 2;
            case IncrementMode.Patch: return 1;
            default: return 0;
        }
    }

    static from(increment: string):number {
        switch (increment) {
            case "Major": return 3;
            case "Minor": return 2;
            case "Patch": return 1;
            default: return 0;
        }
    }
}