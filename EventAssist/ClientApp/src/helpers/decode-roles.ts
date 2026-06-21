import type { AuthToken } from 'src/models/AuthToken';

export function decodeRoles(authToken: AuthToken): string[] {
	let roles = [] as string[];
	if (authToken.role) {
		if (Array.isArray(authToken.role)) {
			roles = authToken.role;
		} else {
			roles = [authToken.role];
		}
	}
	return roles;
}
