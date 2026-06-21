<template>
	<q-card flat style="width: 400px">
		<q-card-section>
			<div class="text-h6 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
				Enter security code
			</div>
			<div class="text-center text-grey-7">
				Enter the 6-digit verification code displayed in the Google Authenticator app.
			</div>
		</q-card-section>

		<q-card-section class="q-pt-none">
			<q-form @submit="onSubmit" class="row q-col-gutter-sm" ref="twoFactorLoginFormRef">
				<q-input class="col-2" outlined :autofocus="i === 1" v-for="i in 6" :key="i" ref="twoFactorDigitInputs"
					v-model="twoFactorDigits[i - 1]" maxlength="1" mask="#"
					@update:model-value="(digit) => onUpdate(digit, i - 1)"
					@keydown.backspace="twoFactorDigits[i - 1] || onBackspacePressed(i - 1)" @paste.prevent="onPaste"
					input-class="text-center" />
			</q-form>
		</q-card-section>

		<q-card-actions align="center">
			<q-btn class="full-width" push :label="submitButtonLabel" color="primary" text-color="dark"
				@click="twoFactorLoginFormRef?.submit()" :disable="missingDigitsCount !== 0" :loading="isLoading" />
		</q-card-actions>
	</q-card>
</template>

<script setup lang="ts">
import { QForm, QInput, useQuasar } from 'quasar';
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from 'src/stores/auth-store';

const authStore = useAuthStore();
const $q = useQuasar();
const router = useRouter();

const isLoading = ref(false);

const twoFactorLoginFormRef = ref<InstanceType<typeof QForm> | null>(null);
const twoFactorDigitInputs = ref<InstanceType<typeof QInput>[]>([]);

const twoFactorDigits = ref<string[]>([]);
const missingDigitsCount = computed(() => {
	return 6 - twoFactorDigits.value.filter((digit) => digit).length;
});
const submitButtonLabel = computed(() => {
	return missingDigitsCount.value === 0
		? 'Verification'
		: `Still missing ${missingDigitsCount.value} digits`;
});

const onUpdate = (digit: string | number | null, index: number) => {
	if (!digit) {
		return;
	}

	const nextIndex = index + 1;
	if (nextIndex < 6) {
		twoFactorDigitInputs.value[nextIndex]?.focus();
	}
};

const onBackspacePressed = (index: number) => {
	const nextIndex = index - 1;
	if (nextIndex >= 0) {
		twoFactorDigits.value[nextIndex] = '';
		twoFactorDigitInputs.value[nextIndex]?.focus();
	}
};

const onPaste = (event: ClipboardEvent) => {
	twoFactorDigits.value = [];
	const text = event.clipboardData?.getData('text') || '';
	const maxSixDigits = text.replace(/\D/g, '').slice(0, 6);

	for (let i = 0; i < maxSixDigits.length; i++) {
		twoFactorDigits.value[i] = maxSixDigits[i] || '';
	}

	const nextIndex = Math.min(maxSixDigits.length, 5);
	twoFactorDigitInputs.value[nextIndex]?.focus();
};

const onSubmit = async () => {
	isLoading.value = true;

	try {
		const twoFactorAuthCode = [...twoFactorDigits.value].join('');
		await authStore.checkTwoFactorAuthentication(twoFactorAuthCode);
		isLoading.value = false;

		resetForm();
		await router.push('/');
	} catch (error) {
		isLoading.value = false;
		$q.notify({
			type: 'negative',
			message: (error as Error).message,
			position: 'bottom',
		});
	}
};

const resetForm = () => {
	twoFactorDigits.value = ['', '', '', '', '', ''];
	twoFactorLoginFormRef.value?.resetValidation();
};
</script>
