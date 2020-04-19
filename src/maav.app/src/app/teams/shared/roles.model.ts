export class Roles {
    public isUser: boolean = false;
    public isAdmin: boolean = false;
    public isTeamLeader: boolean = false;
    public isDeveloper: boolean = false;
    public isIntegration: boolean = false;

    public static user = ("user");
    public static admin = ("admin");
    public static teamLeader = ("team-leader");
    public static developer = ("developer");
    public static integration = ("integration");
    
    static from(roles: string[]): Roles {
      var newRoles = new Roles();
      for (const role of roles) {
        if (role == 'admin') {
            newRoles.isAdmin = true;
        }
        if (role == 'developer') {
            newRoles.isDeveloper = true;
        }
        if (role == 'integration') {
            newRoles.isIntegration = true;
        }
        if (role == 'team-leader') {
            newRoles.isTeamLeader = true;
        }
        if (role == 'user') {
            newRoles.isUser = true;
        }
      }
      return newRoles;
    }

    public to(): string[] {
        var roles: Array<string> = []
        if (this.isUser) {
            roles.push(Roles.user);
        }

        if (this.isAdmin) {
            roles.push(Roles.admin);
        }

        if (this.isDeveloper) {
            roles.push(Roles.developer);
        }

        if (this.isIntegration) {
            roles.push(Roles.integration);
        }

        if (this.isTeamLeader) {
            roles.push(Roles.teamLeader);
        }

        return roles;
    }
}
