export class Role {
    private internalRoleName: string = '';
    
    public teamId: string;
    
    public get name(): string {
        return this.internalRoleName;
    }

    constructor(roleName: string) {
        this.internalRoleName = roleName;
    }

    public static user: Role = new Role("user");
    public static admin: Role = new Role("admin");
    public static teamLeader: Role = new Role("team-leader");
    public static developer: Role = new Role("developer");
    public static integration: Role = new Role("integration");
}