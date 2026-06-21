<template>
	<q-layout view="lHh lpR fFf">
		<q-header bordered class="bg-dark text-white">
			<q-toolbar>
				<q-btn flat dense round icon="menu" @click="isSidebarMini = !isSidebarMini" />
				<q-space></q-space>
				<div v-show="authStore.isAuthenticated" class="row items-center q-gutter-md">
					<span>{{ authStore.user?.name }}</span>
					<q-avatar size="2rem">
						<img :src="authStore.user?.profilePictureUrl">
					</q-avatar>
					<q-btn push color="primary" text-color="dark" label="Logout" @click="deauthenticate" />
				</div>
			</q-toolbar>
			<q-toolbar>
				<q-breadcrumbs active-color="primary">
					<q-breadcrumbs-el v-for="(breadcrumbLink, index) in breadcrumbLinks" :key="index"
						:label="breadcrumbLink?.label" :icon="breadcrumbLink?.icon" />
				</q-breadcrumbs>
			</q-toolbar>
		</q-header>

		<q-drawer bordered class="bg-dark text-white" :width="320" :mini="isSidebarMini" v-model="isSidebarOpen"
			show-if-above :breakpoint="500">
			<div class="text-h5 q-pa-md text-uppercase" v-show="!isSidebarMini">
				EventAssist
			</div>
			<SidebarLinks :items="accessibleSidebarLinks"></SidebarLinks>
		</q-drawer>

		<q-page-container class="full-height">
			<router-view />
			<q-page-sticky v-if="!authStore.user?.roles.includes('HelpdeskAgent')" position="bottom-right"
				:offset="[24, 24]">
				<q-fab v-if="!isChatOpen" push v-model="isChatOpen" label-position="top" color="primary"
					text-color="dark" icon="support_agent" direction="up" />
				<customer-support-chat v-show="isChatOpen" @close="isChatOpen = false" />
			</q-page-sticky>
		</q-page-container>
	</q-layout>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import type { SidebarLink } from 'src/models/SidebarLink';
import SidebarLinks from 'src/components/SidebarLinks.vue';
import { useAuthStore } from 'src/stores/auth-store';
import { useRoute, useRouter } from 'vue-router';
import CustomerSupportChat from 'src/components/CustomerSupportChat.vue';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const sidebarLinks: SidebarLink[] = [
	{
		label: 'Home',
		url: '/',
		icon: 'home',
		separator: true,
	},
	{
		label: 'Events',
		url: '/events',
		icon: 'event',
		separator: true,
	},
	{
		label: 'Agent Dashboard',
		url: '/agent-dashboard',
		icon: 'support_agent',
		roles: ['HelpdeskAgent'],
		separator: false,
	},
	{
		label: 'Admin Dashboard',
		url: '/admin-dashboard',
		icon: 'dashboard',
		roles: ['HelpdeskAgent'],
		separator: true,
	},
	{
		label: 'Settings',
		url: '/settings',
		icon: 'settings',
		separator: false,
	}
];

const accessibleSidebarLinks = computed(() => {
	const userRoles = authStore.user?.roles ?? [];
	return sidebarLinks.filter(sidebarLink => !sidebarLink.roles || sidebarLink.roles.length === 0 || userRoles.some(userRole => sidebarLink.roles!.includes(userRole)))
})

const deauthenticate = async () => {
	authStore.deauthenticateUser();
	await router.push('auth/login');
}

const breadcrumbLinks = computed(() => {
	const parts = route.path.split('/').filter(Boolean);
	const breadcrumbs = [];
	let currentPath = '';
	for (const part of parts) {
		currentPath += '/' + part;
		const link = sidebarLinks.find(l => l.url === currentPath);
		if (link) {
			breadcrumbs.push(link);
		}
	}
	return [
		sidebarLinks[0],
		...breadcrumbs.filter(c => c.url !== '/')
	];
});

const isSidebarOpen = ref(false);
const isSidebarMini = ref(false);
const isChatOpen = ref(true);
</script>
