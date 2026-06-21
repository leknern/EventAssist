export interface User {
	id: number;
	name: string;
	roles: string[];
	profilePictureUrl: string;
	isTwoFactorAuthEnabled: boolean;
}
