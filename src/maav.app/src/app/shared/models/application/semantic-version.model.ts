import { BranchActionRequest } from '../versions/branch-action-request.model';

export class SemanticVersion {
    public major: number;
    public minor: number;
    public patch: number;
    public preRelease: string;
    public build: string;
    
    public static format(version: SemanticVersion): string {
        let preRelease = version.preRelease && version.preRelease !== '' ? '-'+version.preRelease : '';
        let build = version.build && version.build !== '' ? '+'+version.build : '';
        return `${version.major}.${version.minor}.${version.patch}${preRelease}${build}`;
    }

    public static formatFrom(version: SemanticVersion, formatVersion: string): string {
        let major = version.major.toString();
        let minor = version.minor.toString();
        let patch = version.patch.toString();
        return "v"+formatVersion
            .replace('{major}', major)
            .replace('{minor}', minor)
            .replace('{patch}', patch)
            .replace('{prerelease}', version.preRelease)
            .replace('{build}', version.build);
      }
}