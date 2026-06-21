<template>
	<q-dialog ref="dialogRef" @hide="onDialogHide">
		<q-card class="q-dialog-plugin" flat>
			<q-card-section>
				<div class="text-h6 text-uppercase text-center q-mt-sm q-mb-sm">Confirm delete</div>
			</q-card-section>
			<q-card-section class="text-center">
				Are you sure you want to delete the event "{{ props.event.name }}"?
			</q-card-section>
			<q-card-actions align="center">
				<q-btn color="primary" flat label="Cancel" @click="onDialogCancel" />
				<q-btn color="negative" text-color="dark" push label="Delete" @click="onOKClick" />
			</q-card-actions>
		</q-card>
	</q-dialog>
</template>

<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar'
import type { UserEvent } from 'src/models/UserEvent';

const props = defineProps<{ event: UserEvent }>();

defineEmits([
	...useDialogPluginComponent.emits
])

const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent()

function onOKClick() {
	onDialogOK()
}
</script>