import { User } from './user/user.model';

    export class Authorization {
        public accessToken: string;
        public expiration: string;
        public roleNames: Array<string> = [];
        public user: User;
        public organisationId: string;
    }