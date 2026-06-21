<template>
	<q-dialog ref="dialogRef" @hide="onDialogHide">
		<q-card class="q-dialog-plugin" flat>
			<q-card-section>
				<div class="text-h6 text-uppercase text-center q-mt-sm q-mb-sm">
					{{ isEditMode ? 'Edit event' : 'Add new event' }}</div>
			</q-card-section>
			<q-card-section>
				<q-form @submit="onSubmit" ref="eventFormRef">
					<q-input outlined class="q-mb-sm" v-model.trim="eventForm.name" label="Name" stack-label
						:disable="isEditMode" :rules="[
							(val: string) => !!val || 'Name is required',
							(val: string) =>
								val.length <= 200 || 'Name must be maximum 200 characters',
						]" />
					<q-input outlined class="q-mb-sm" v-model.trim="eventForm.description" label="Description"
						stack-label :rules="[
							(val: string | null) =>
								!val ||
								val.length <= 500 ||
								'Description must be maximum 500 characters',
						]" />
					<q-input outlined class="q-mb-sm" v-model="eventForm.occurrence" label="Occurrence" stack-label
						:disable="isEditMode" mask="####-##-##" :rules="[
							(val: string) => !!val || 'Date is required',
							(val: string) =>
								/^\d{4}-\d{2}-\d{2}$/.test(val) || 'Invalid date format',
						]">
						<template v-slot:append>
							<q-icon name="event" class="cursor-pointer">
								<q-popup-proxy class="no-box-shadow" cover transition-show="scale"
									transition-hide="scale">
									<q-date text-color="dark" flat v-model="eventForm.occurrence" mask="YYYY-MM-DD">
										<div class="row items-center justify-end">
											<q-btn v-close-popup label="Close" color="primary" flat />
										</div>
									</q-date>
								</q-popup-proxy>
							</q-icon>
						</template>
					</q-input>
				</q-form>
			</q-card-section>
			<q-card-actions align="right">
				<q-btn flat color="primary" label="Cancel" @click="onDialogCancel" />
				<q-btn push color="primary" text-color="dark" label="Save" @click="eventFormRef?.submit()"
					:loading="isLoading" />
			</q-card-actions>
		</q-card>
	</q-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import { QForm, useDialogPluginComponent, useQuasar } from 'quasar';
import { useEventStore } from 'src/stores/event-store';
import type { UserEvent } from 'src/models/UserEvent';

const props = defineProps<{ event?: UserEvent }>();

defineEmits([...useDialogPluginComponent.emits]);
const { dialogRef, onDialogHide, onDialogCancel } = useDialogPluginComponent();

const $q = useQuasar();
const eventStore = useEventStore();
const isLoading = ref(false);

const isEditMode = computed(() => !!props.event);

const eventFormRef = ref<InstanceType<typeof QForm> | null>(null);
const eventForm = reactive({
	name: '',
	description: null as string | null,
	occurrence: '',
});

async function onSubmit() {
	if (!eventFormRef.value?.validate()) {
		return;
	}

	isLoading.value = true;
	try {
		if (isEditMode.value && props.event) {
			await eventStore.update({
				id: props.event.id,
				description: eventForm.description,
			});
		} else {
			await eventStore.add({ ...eventForm });
		}
		dialogRef.value?.hide();
	} catch (error) {
		isLoading.value = false;
		$q.notify({
			type: 'negative',
			message: (error as Error).message,
			position: 'bottom',
		});
	}
}

onMounted(() => {
	if (props.event) {
		eventForm.name = props.event.name;
		eventForm.description = props.event.description;
		eventForm.occurrence = props.event.occurrence;
	}
});
</script>