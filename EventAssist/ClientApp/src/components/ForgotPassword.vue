<template>
	<q-card flat style="width: 400px">
		<q-card-section>
			<h5 class="text-h5 text-weight-medium text-uppercase text-center q-mt-sm q-mb-sm">
				Forgot Password
			</h5>
			<div class="text-center text-grey-7">
				Enter your registered email address and we'll send you a link to reset your
				password.
			</div>
		</q-card-section>
		<q-card-section>
			<q-form @submit="onSubmit" ref="formRef">
				<q-input outlined stack-label placeholder="john.doe@example.com" type="email" v-model.trim="email"
					label="Email" :rules="isEmailCorrect" lazy-rules />
			</q-form>
		</q-card-section>
		<q-card-actions>
			<q-btn @click="formRef?.submit()" class="full-width" color="primary" text-color="dark" push
				label="Send Link" :loading="isLoading" />
		</q-card-actions>
	</q-card>
</template>

<script setup lang="ts">
import axios from 'axios';
import { ref } from 'vue';
import { QForm, useQuasar } from 'quasar';
import { standardizeError } from 'src/helpers/standardize-error';

const $q = useQuasar();
const emit = defineEmits(['close']);

const isLoading = ref(false);
const formRef = ref<InstanceType<typeof QForm> | null>(null);

const email = ref('');
const isEmailCorrect = [
	(email: string) => !!email || 'Please enter your email address',
	(email: string) => /^\S+@\S+\.\S+$/.test(email) || 'Please enter a valid email address',
];

const onSubmit = async () => {
	if (!(await formRef.value?.validate())) {
		return;
	}

	isLoading.value = true;
	try {
		await axios.get('/api/Auth/ForgotPassword', {
			params: {
				email: email.value,
			},
		});

		$q.notify({
			type: 'positive',
			message: 'Email sent! Please check your inbox.',
			position: 'bottom',
		});

		email.value = '';
		formRef.value?.resetValidation();
		emit('close');
	} catch (error) {
		$q.notify({
			type: 'negative',
			message: standardizeError(error).message,
			position: 'bottom',
		});
	} finally {
		isLoading.value = false;
	}
};
</script>
