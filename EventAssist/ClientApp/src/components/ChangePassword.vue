<template>
	<q-dialog ref="dialogRef" @hide="onDialogHide">
		<q-card class="q-dialog-plugin" flat>
			<q-card-section>
				<div class="text-h6 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
					Change Your Password
				</div>
				<div class="text-center text-grey-7">
					For your security, you will be signed out after a successful password change.
				</div>
			</q-card-section>
			<q-card-section>
				<q-form @submit="onSubmit" class="container" ref="formRef">
					<q-input outlined stack-label label="Password" placeholder="Password"
						:type="isPassword ? 'password' : 'text'" v-model.trim="password" :rules="isPasswordCorrect"
						lazy-rules>
						<template v-slot:append>
							<q-icon :name="isPassword ? 'visibility_off' : 'visibility'" class="cursor-pointer"
								@click="isPassword = !isPassword" />
						</template>
					</q-input>
					<q-input outlined stack-label placeholder="New Password"
						:type="isPasswordSecond ? 'password' : 'text'" v-model.trim="newPassword" label="New Password"
						:rules="isNewPasswordCorrect" lazy-rules>
						<template v-slot:append>
							<q-icon :name="isPasswordSecond ? 'visibility_off' : 'visibility'" class="cursor-pointer"
								@click="isPasswordSecond = !isPasswordSecond" />
						</template>
					</q-input>
					<q-input outlined v-model.trim="confirmNewPassword" type="password" stack-label
						placeholder="Confirm New Password" label="Confirm New Password" lazy-rules
						:rules="isConfirmNewPasswordCorrect" />
				</q-form>
			</q-card-section>
			<q-card-actions align="right">
				<q-btn flat color="primary" label="Cancel" @click="onDialogCancel" />
				<q-btn push @click="formRef?.submit()" color="primary" text-color="dark" label="Change"
					:loading="isLoading" />
			</q-card-actions>
		</q-card>
	</q-dialog>
</template>
<script setup lang="ts">
import { QForm, useDialogPluginComponent, useQuasar } from 'quasar';
import { ref } from 'vue';
import axios from 'axios';
import { useAuthStore } from 'src/stores/auth-store';
import { standardizeError } from 'src/helpers/standardize-error';
import { useRouter } from 'vue-router';

defineEmits([...useDialogPluginComponent.emits]);

const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent();

const $q = useQuasar();
const authStore = useAuthStore();
const router = useRouter();

const isLoading = ref(false);
const isPassword = ref(true);
const isPasswordSecond = ref(true);
const formRef = ref<InstanceType<typeof QForm> | null>(null);

const password = ref('');
const newPassword = ref('');
const confirmNewPassword = ref('');

const isPasswordCorrect = [(val: string) => !!val || 'Please enter your password'];
const isNewPasswordCorrect = [
	(val: string) => !!val || 'Please enter your new password',
	(val: string) => val.length >= 8 || 'New password must be at least 8 characters',
	(val: string) => val.length <= 32 || 'New password must be at most 32 characters',
	(val: string) => /[A-Z]/.test(val) || 'Must contain at least one uppercase letter',
	(val: string) => /[a-z]/.test(val) || 'Must contain at least one lowercase letter',
	(val: string) => /[0-9]/.test(val) || 'Must contain at least one number',
];

const isConfirmNewPasswordCorrect = [
	(val: string) => !!val || 'Please confirm your new password',
	(val: string) => val === newPassword.value || 'New passwords do not match',
];

const onSubmit = async () => {
	isLoading.value = true;
	try {
		isLoading.value = false;

		await axios.post('/api/Auth/ChangePassword', {
			password: password.value,
			newPassword: newPassword.value,
		});
		onDialogOK();
		authStore.deauthenticateUser();
		await router.push('auth/login');
	} catch (error) {
		isLoading.value = false;

		$q.notify({
			type: 'negative',
			message: standardizeError(error).message,
		});
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
