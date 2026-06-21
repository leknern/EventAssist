<template>
	<q-page class="flex flex-center">
		<q-card flat style="width: 360px; background-color: transparent;">
			<q-card-section>
				<div class="text-h5 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
					Welcome back
				</div>
				<div class="text-center text-grey-7">
					Enter your email and password to access your account.
				</div>
			</q-card-section>
			<q-card-section>
				<q-form @submit="onLoginSubmit" class="container" ref="loginFormRef">
					<q-input outlined stack-label placeholder="john.doe@example.com" type="email"
						v-model.trim="loginForm.email" label="Email" :rules="isEmailCorrect" lazy-rules />
					<q-input outlined stack-label placeholder="Password" :type="isPassword ? 'password' : 'text'"
						v-model.trim="loginForm.password" label="Password" :rules="isPasswordCorrect" lazy-rules>
						<template v-slot:append>
							<q-icon :name="isPassword ? 'visibility_off' : 'visibility'" class="cursor-pointer"
								@click="isPassword = !isPassword" />
						</template>
					</q-input>
					<div class="text-right">
						<q-btn flat dense color="primary" label="Forgot your password?"
							@click="showForgotPasswordModal = true" />
					</div>
				</q-form>
			</q-card-section>
			<q-card-actions align="center">
				<q-btn class="full-width" push color="primary" text-color="dark" label="Login" type="submit"
					@click="loginFormRef?.submit()" :loading="isLoginLoading" />
			</q-card-actions>
		</q-card>
	</q-page>

	<q-dialog v-model="showForgotPasswordModal">
		<forgot-password @close="showForgotPasswordModal = false"></forgot-password>
	</q-dialog>
	<q-dialog v-model="showTwoFactorModal" position="standard" persistent>
		<two-factor-auth :show="showTwoFactorModal"></two-factor-auth>
	</q-dialog>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { QForm, useQuasar } from 'quasar';
import { useRouter } from 'vue-router';
import TwoFactorAuth from 'src/components/TwoFactorAuth.vue';
import ForgotPassword from 'src/components/ForgotPassword.vue';
import type { LoginRequest } from 'src/models/LoginRequest';
import { useAuthStore } from 'src/stores/auth-store';

const authStore = useAuthStore();
const $q = useQuasar();
const router = useRouter();

const isLoginLoading = ref(false);
const showTwoFactorModal = ref(false);
const showForgotPasswordModal = ref(false);

const loginFormRef = ref<InstanceType<typeof QForm> | null>(null);
const loginForm = reactive<LoginRequest>({
	email: '',
	password: '',
});
const isPassword = ref(true);
const isEmailCorrect = [
	(val: string) => !!val || 'Please enter your email address',
	(val: string) => /^\S+@\S+\.\S+$/.test(val) || 'Please enter a valid email address',
];
const isPasswordCorrect = [(val: string) => !!val || 'Please enter your password'];

const onLoginSubmit = async () => {
	if (!(await loginFormRef.value?.validate())) {
		return;
	}

	isLoginLoading.value = true;
	try {
		await authStore.authenticateUser({ ...loginForm });
		isLoginLoading.value = false;

		if (authStore.isTwoFactorAuthRequired) {
			showTwoFactorModal.value = true;
			return;
		}

		await router.push('/');
	} catch (error) {
		isLoginLoading.value = false;
		$q.notify({
			type: 'negative',
			message: (error as Error).message,
			position: 'bottom',
		});
	}
};
</script>

<style scoped>
.container {
	display: flex;
	flex-direction: column;
	gap: 0.5rem;
}
</style>
