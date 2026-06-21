import { defineRouter } from '#q-app/wrappers';
import {
	createMemoryHistory,
	createRouter,
	createWebHashHistory,
	createWebHistory,
} from 'vue-router';
import routes from './routes';
import { useAuthStore } from 'src/stores/auth-store';

/*
 * If not building with SSR mode, you can
 * directly export the Router instantiation;
 *
 * The function below can be async too; either use
 * async/await or return a Promise which resolves
 * with the Router instance.
 */

export default defineRouter(function (/* { store, ssrContext } */) {
	const createHistory = process.env.SERVER
		? createMemoryHistory
		: process.env.VUE_ROUTER_MODE === 'history'
			? createWebHistory
			: createWebHashHistory;

	const Router = createRouter({
		scrollBehavior: () => ({ left: 0, top: 0 }),
		routes,
		history: createHistory(process.env.VUE_ROUTER_BASE),
	});

	Router.beforeEach((to, from, next) => {
		const authStore = useAuthStore();
		const requiresAuth = to.meta.requiresAuth;
		const roles = to.meta.roles as string[] | undefined;
		const userRoles = authStore.user?.roles || [];

		if (to.path === '/auth/login' && authStore.isAuthenticated) {
			next({ path: '/' });
			return;
		}

		if (!requiresAuth) {
			next();
			return;
		}

		if (!authStore.isAuthenticated) {
			next({ path: '/auth/login' });
			return;
		}

		if (!roles) {
			next();
			return;
		}

		if (!userRoles.some((role) => roles.includes(role))) {
			next({ path: '/' });
			return;
		}

		next();
	});

	return Router;
});
