import { defineStore, acceptHMRUpdate } from 'pinia';
import axios from 'axios';
import type { LoginRequest } from 'src/models/LoginRequest';
import type { User } from 'src/models/User';
import { standardizeError } from 'src/helpers/standardize-error';
import { jwtDecode } from 'jwt-decode';
import type { AuthToken } from 'src/models/AuthToken';
import { decodeRoles } from 'src/helpers/decode-roles';
import { Cookies } from 'quasar';

export const useAuthStore = defineStore('auth', {
	state: () => ({
		user: null as User | null,
		token: null as string | null,
		isAuthenticated: false,
		isTwoFactorAuthRequired: false,
	}),
	actions: {
		initFromCookie() {
			const token = Cookies.get('authToken');
			if (!token) {
				return;
			}
			const decodedAuthToken = jwtDecode<AuthToken>(token);
			const currentTime = Date.now() / 1000;

			if (decodedAuthToken.exp < currentTime) {
				Cookies.remove('authToken');
				return;
			}

			axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

			this.user = {
				id: +decodedAuthToken.nameid,
				name: decodedAuthToken.unique_name,
				profilePictureUrl: decodedAuthToken.profilePictureUrl,
				roles: decodeRoles(decodedAuthToken),
				isTwoFactorAuthEnabled: decodedAuthToken.isTwoFactorAuthEnabled === 'true',
			};
			this.token = token;
			this.isAuthenticated = true;
		},
		async authenticateUser(request: LoginRequest) {
			try {
				const response = await axios.post('/api/Auth/Login', request);
				const decodedAuthToken = jwtDecode<AuthToken>(response.data);
				const tokenExpiration = new Date(decodedAuthToken.exp * 1000);

				axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;

				if (decodedAuthToken.isTempToken === 'True') {
					this.isTwoFactorAuthRequired = true;

					return;
				}

				Cookies.set('authToken', response.data, { expires: tokenExpiration });

				this.user = {
					id: +decodedAuthToken.nameid,
					name: decodedAuthToken.unique_name,
					profilePictureUrl: decodedAuthToken.profilePictureUrl,
					roles: decodeRoles(decodedAuthToken),
					isTwoFactorAuthEnabled: decodedAuthToken.isTwoFactorAuthEnabled === 'True',
				};
				this.token = response.data;
				this.isAuthenticated = true;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async checkTwoFactorAuthentication(twoFactorAuthCode: string) {
			try {
				const response = await axios.get('/api/Auth/TwoFactorLogin', {
					params: { twoFactorAuthCode },
				});

				const decodedAuthToken = jwtDecode<AuthToken>(response.data);
				const tokenExpiration = new Date(decodedAuthToken.exp * 1000);
				Cookies.set('authToken', response.data, { expires: tokenExpiration });
				axios.defaults.headers.common['Authorization'] = `Bearer ${response.data}`;

				this.user = {
					id: +decodedAuthToken.nameid,
					name: decodedAuthToken.unique_name,
					profilePictureUrl: decodedAuthToken.profilePictureUrl,
					roles: decodeRoles(decodedAuthToken),
					isTwoFactorAuthEnabled: decodedAuthToken.isTwoFactorAuthEnabled === 'True',
				};
				this.isAuthenticated = true;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async turnOnTwoFactorAuth(twoFactorAuthCode: string) {
			try {
				await axios.get('/api/Auth/TurnOnTwoFactorAuth', {
					params: {
						twoFactorAuthCode,
					},
				});
				Cookies.remove('authToken');
				this.user!.isTwoFactorAuthEnabled = true;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async turnOffTwoFactorAuth() {
			try {
				await axios.get('/api/Auth/TurnOffTwoFactorAuth');
				Cookies.remove('authToken');
				this.user!.isTwoFactorAuthEnabled = true;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		deauthenticateUser() {
			Cookies.remove('authToken');
			delete axios.defaults.headers.common['Authorization'];
			this.user = null;
			this.isAuthenticated = false;
			this.isTwoFactorAuthRequired = false;
		},
	},
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useAuthStore, import.meta.hot));
}
