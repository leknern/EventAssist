<template>
	<q-page class="q-pa-md">
		<q-table flat color="primary" :columns="columns" :rows="filteredRows" row-key="id" separator="vertical"
			:loading="!eventStore.isLoaded">
			<template v-slot:top>
				<q-input class="q-mr-sm custom-input-size" outlined dense stack-label v-model="search" label="Search">
					<template v-slot:append>
						<q-icon name="search" />
					</template>
				</q-input>
				<q-space></q-space>
				<q-btn @click="openManageEvent()" push color="primary" text-color="dark" icon="add" label="Add" />
			</template>
			<template v-slot:header="props">
				<q-tr :props="props">
					<q-th v-for="col in props.cols" :key="col.name" :props="props" class="text-uppercase">
						{{ col.label }}
					</q-th>
				</q-tr>
			</template>
			<template v-slot:body-cell-actions="props">
				<q-td :props="props">
					<q-btn color="secondary" flat round dense icon="edit" @click="openManageEvent(props.row)" />
					<q-btn color="negative" flat round dense icon="delete" @click="openConfirmDelete(props.row)" />
				</q-td>
			</template>
		</q-table>
	</q-page>
</template>

<script setup lang="ts">
import { useQuasar, date } from 'quasar';
import ConfirmDelete from 'src/components/ConfirmDelete.vue';
import ManageEvent from 'src/components/ManageEvent.vue';
import type { UserEvent } from 'src/models/UserEvent';
import { useEventStore } from 'src/stores/event-store';
import { computed, ref } from 'vue';

const columns = [
	{
		name: 'name',
		label: 'Name',
		sortable: true,
		field: 'name',
	},
	{
		name: 'description',
		label: 'Description',
		sortable: true,
		field: 'description',
		format: (val: string) => val ?? ''
	},
	{
		name: 'occurrence',
		label: 'Occurrence',
		sortable: true,
		field: 'occurrence',
		format: (val: string) => date.formatDate(val, 'YYYY-MM-DD'),
	},
	{
		name: 'actions',
		label: 'Actions',
		field: 'actions',
	},
];

const filteredRows = computed(() => {
	if (!search.value) {
		return eventStore.events;
	}

	const searchLower = search.value.toLowerCase();
	return eventStore.events.filter(
		(event) =>
			event.name.toLowerCase().includes(searchLower) ||
			event.description?.toLowerCase().includes(searchLower) ||
			event.occurrence.toLowerCase().includes(searchLower),
	);
});

const $q = useQuasar();
const eventStore = useEventStore();

const search = ref('');

const openManageEvent = (event?: UserEvent) => {
	$q.dialog({
		component: ManageEvent,
		componentProps: {
			persistent: true,
			event,
		},
	});
};

const openConfirmDelete = (event: UserEvent) => {
	$q.dialog({
		component: ConfirmDelete,
		componentProps: {
			persistent: true,
			event,
		},
	}).onOk(() => void deleteEvent(event.id));
};

const deleteEvent = async (eventId: number) => {
	try {
		await eventStore.remove(eventId);
		$q.notify({
			type: 'positive',
			message: 'Event deleted successfully',
			position: 'bottom',
		});
	} catch (error) {
		$q.notify({
			type: 'negative',
			message: (error as Error).message,
			position: 'bottom',
		});
	}
};
</script>
