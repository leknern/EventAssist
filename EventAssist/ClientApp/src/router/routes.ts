import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
	{
		path: '/',
		component: () => import('layouts/MainLayout.vue'),
		meta: { requiresAuth: true },
		children: [
			{
				path: '',
				component: () => import('pages/IndexPage.vue'),
				meta: { requiresAuth: true },
			},
			{
				path: 'events',
				component: () => import('pages/EventsPage.vue'),
				meta: { requiresAuth: true },
			},
			{
				path: 'agent-dashboard',
				component: () => import('pages/AgentDashboardPage.vue'),
				meta: { requiresAuth: true, roles: ['HelpdeskAgent'] },
			},
			{
				path: 'admin-dashboard',
				component: () => import('pages/AdminDashboardPage.vue'),
				meta: { requiresAuth: true, roles: ['HelpdeskAgent'] },
			},
			{
				path: 'settings',
				component: () => import('pages/SettingsPage.vue'),
				meta: { requiresAuth: true },
			},
		],
	},
	{
		path: '/auth',
		component: () => import('layouts/AuthLayout.vue'),
		children: [
			{
				path: 'login',
				component: () => import('pages/LoginPage.vue'),
			},
			{
				path: 'reset-password/:token',
				component: () => import('pages/ResetPasswordPage.vue'),
			},
		],
	},
	{
		path: '/:catchAll(.*)*',
		component: () => import('pages/ErrorNotFound.vue'),
	},
];

export default routes;
