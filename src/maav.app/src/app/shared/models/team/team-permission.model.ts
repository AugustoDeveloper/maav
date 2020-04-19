export class TeamPermission {
    public teamId: string;
    public isOwner: boolean;
    public isReader: boolean;
    public isWriter: boolean;

    public static from(optionPermission: string, teamId: string): TeamPermission {
        var teamPermission: TeamPermission = new TeamPermission();

        teamPermission.isOwner = optionPermission == 'owner';
        teamPermission.isWriter = optionPermission == 'writer';
        teamPermission.isReader = optionPermission == 'reader';
        teamPermission.teamId = teamId;
        return teamPermission;
    }
}