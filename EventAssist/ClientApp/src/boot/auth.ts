import { defineBoot } from '@quasar/app-vite/wrappers';
import { useAuthStore } from 'src/stores/auth-store';

export default defineBoot(() => {
	const authStore = useAuthStore();
	authStore.initFromCookie();
});
