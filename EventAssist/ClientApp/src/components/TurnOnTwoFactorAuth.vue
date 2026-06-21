<template>
	<q-dialog ref="dialogRef" @hide="onDialogHide">
		<q-card class="q-dialog-plugin" flat>
			<q-card-section>
				<div class="text-h6 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
					Two-Factor Authentication
				</div>
				<div class="text-center text-grey-7">
					To protect your account, scan the QR code with Google Authenticator, then enter
					the generated code to complete the process.
				</div>
			</q-card-section>
			<q-separator />
			<q-card-section>
				<div class="flex flex-center q-mb-md" v-if="isLoading">
					<q-spinner-oval color="primary" size="4rem" />
				</div>
				<div v-else class="flex flex-center column">
					<img v-if="displayQrCode" :src="qrCodeUrl" alt="QR code" class="q-mb-md" style="max-width: 256px" />
					<div v-else class="q-mb-lg q-mt-md text-body2">
						{{ inputKey }}
					</div>
					<q-btn flat color="primary" class="q-mb-md"
						:label="displayQrCode ? 'Problems scanning?' : 'Show QR code instead'"
						@click="displayQrCode = !displayQrCode" />
				</div>
				<q-input v-model="authenticatorCode" label="Authenticator Code" outlined stack-label maxlength="6"
					placeholder="TOTP code (6 digits)" :disable="isLoading" />
			</q-card-section>
			<q-separator />
			<q-card-actions align="right">
				<q-btn flat color="primary" label="Cancel" @click="onDialogCancel" />
				<q-btn fpush @click="onSubmit" color="primary" text-color="dark" label="Verify"
					:disable="isLoading || authenticatorCode.length !== 6" :loading="isTwoFactorAuthLoading" />
			</q-card-actions>
		</q-card>
	</q-dialog>
</template>

<script setup lang="ts">
import axios from 'axios';
import { ref, onMounted } from 'vue';
import { useDialogPluginComponent, useQuasar } from 'quasar';
import type { QrCodeResponse } from 'src/models/QrCodeResponse';
import { useAuthStore } from 'src/stores/auth-store';
import { standardizeError } from 'src/helpers/standardize-error';

defineEmits([...useDialogPluginComponent.emits]);
const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent();

const $q = useQuasar();
const authStore = useAuthStore();

const isLoading = ref(false);
const isTwoFactorAuthLoading = ref(false);
const displayQrCode = ref(true);

const qrCodeUrl = ref('');
const inputKey = ref('');

const authenticatorCode = ref('');

const onSubmit = async () => {
	isTwoFactorAuthLoading.value = true;
	try {
		await authStore.turnOnTwoFactorAuth(authenticatorCode.value);
		isTwoFactorAuthLoading.value = false;
		onDialogOK(true);
	} catch (error) {
		isTwoFactorAuthLoading.value = false;
		$q.notify({
			type: 'negative',
			message: standardizeError(error).message,
		});
	}
};

onMounted(async () => {
	isLoading.value = true;
	try {
		const response = await axios.get<QrCodeResponse>('api/Auth/GetTwoFactorQrCode');

		qrCodeUrl.value = response.data.qrCodeUrl;
		inputKey.value = response.data.inputKey;
	} catch (error) {
		$q.notify({
			type: 'negative',
			message: standardizeError(error).message,
			position: 'bottom',
		});
	} finally {
		isLoading.value = false;
	}
});
</script>
