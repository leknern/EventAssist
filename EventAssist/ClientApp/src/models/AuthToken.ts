export interface AuthToken {
	nameid: number;
	unique_name: string;
	email: string;
	role: string[] | string | undefined;
	exp: number;
	profilePictureUrl: string;
	isTwoFactorAuthEnabled: string;
	isTempToken: string;
}
