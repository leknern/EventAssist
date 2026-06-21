<template>
	<q-page class="flex flex-center">
		<q-card flat style="background-color: transparent;">
			<q-card-section>
				<div class="text-h5 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
					Create new password
				</div>
			</q-card-section>
			<q-card-section>
				<q-form @submit="onSubmit" class="container" ref="formRef">
					<q-input outlined stack-label placeholder="New Password" :type="isPassword ? 'password' : 'text'"
						v-model.trim="password" label="New Password" :rules="isPasswordCorrect" lazy-rules>
						<template v-slot:append>
							<q-icon :name="isPassword ? 'visibility_off' : 'visibility'" class="cursor-pointer"
								@click="isPassword = !isPassword" />
						</template>
					</q-input>
					<q-input outlined v-model.trim="confirmPassword" type="password" stack-label
						placeholder="Confirm New Password" label="Confirm New Password" lazy-rules
						:rules="isConfirmPasswordCorrect" />
				</q-form>
			</q-card-section>
			<q-card-actions align="center">
				<q-btn class="full-width" push color="primary" text-color="dark" label="Reset Password"
					@click="formRef?.submit()" :loading="isLoading" />
			</q-card-actions>
		</q-card>
	</q-page>
</template>

<script setup lang="ts">
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { QForm, useQuasar } from 'quasar';
import { useRoute, useRouter } from 'vue-router';

const $q = useQuasar();
const route = useRoute();
const router = useRouter();

const isLoading = ref(false);
const isPassword = ref(true);
const formRef = ref<InstanceType<typeof QForm> | null>(null);
const passwordResetToken = ref('');

const password = ref('');
const confirmPassword = ref('');

onMounted(() => {
	passwordResetToken.value = route.params.token as string;
});

const isPasswordCorrect = [
	(val: string) => !!val || 'Please enter your new password',
	(val: string) => val.length >= 8 || 'New password must be at least 8 characters',
	(val: string) => val.length <= 32 || 'New password must be at most 32 characters',
	(val: string) => /[A-Z]/.test(val) || 'Must contain at least one uppercase letter',
	(val: string) => /[a-z]/.test(val) || 'Must contain at least one lowercase letter',
	(val: string) => /[0-9]/.test(val) || 'Must contain at least one number',
];

const isConfirmPasswordCorrect = [
	(val: string) => !!val || 'Please confirm your new password',
	(val: string) => val === password.value || 'New passwords do not match',
];

const onSubmit = async () => {
	if (!(await formRef.value?.validate())) {
		return;
	}

	if (!passwordResetToken.value) {
		$q.notify({
			type: 'negative',
			message: 'Missing or invalid token.',
			position: 'bottom',
		});
		return;
	}

	isLoading.value = true;
	try {
		await axios.post('/api/Auth/ResetPassword', {
			password: password.value,
			passwordResetToken: passwordResetToken.value,
		});

		$q.notify({
			type: 'positive',
			message: 'Password successfully reset!',
			position: 'bottom',
		});

		password.value = '';
		confirmPassword.value = '';
		formRef.value?.resetValidation();

		await router.push('/auth/login');
	} catch (error) {
		let message = '';

		if (axios.isAxiosError(error)) {
			message = error.response!.data;
		} else if (error instanceof Error) {
			message = error.message;
		}

		$q.notify({
			type: 'negative',
			message,
			position: 'bottom',
		});
	} finally {
		isLoading.value = false;
	}
};
</script>

<style>
.container {
	display: flex;
	flex-direction: column;
	gap: 0.5rem;
}
</style>
