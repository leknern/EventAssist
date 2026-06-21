<template>
	<q-page class="q-pa-md">
		<q-table flat color="primary" :columns="columns" :rows="supportAgentChatStore.chats" separator="vertical">
			<template v-slot:top>
				<q-input class="q-mr-sm custom-input-size" outlined dense stack-label v-model="search" label="Search">
					<template v-slot:append>
						<q-icon name="search" />
					</template>
				</q-input>
				<q-space></q-space>
				<q-btn color="primary" text-color="dark" push label="Export" @click="exportMessagesToCsv" />
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
					<q-btn color="secondary" flat round dense icon="visibility" @click="openChatHistory(props.row)" />
				</q-td>
			</template>
			<template v-slot:body-cell-status="props">
				<q-td :props="props">
					<q-badge v-if="props.row.status === ChatStatus.Opened" label="Opened" color="info"
						text-color="dark"></q-badge>
					<q-badge v-if="props.row.status === ChatStatus.OperatorAssigned" label="Operator Assigned"
						color="secondary" text-color="white"></q-badge>
					<q-badge v-if="props.row.status === ChatStatus.Closed" label="Closed" color="negative"
						text-color="white"></q-badge>
				</q-td>
			</template>
		</q-table>
	</q-page>
</template>
<script setup lang="ts">
import { exportFile, useQuasar } from 'quasar';
import ChatHistory from 'src/components/ChatHistory.vue';
import { formatDate } from 'src/helpers/format-date';
import type { Chat } from 'src/models/Chat';
import { ChatStatus } from 'src/models/ChatStatus';
import { useSupportAgentChatStore } from 'src/stores/support-agent-chat-store';
import { ref } from 'vue';

const columns = [
	{
		name: 'id',
		label: 'Id',
		field: (chat: Chat) => chat.id,
		sortable: true
	},
	{
		name: 'user',
		label: 'Customer',
		field: (chat: Chat) => chat.user.name,
	},
	{
		name: 'agent',
		label: 'Human Helpdesk Agent',
		field: (chat: Chat) => chat.customerSupportAgent?.name,
		format: (val: string | undefined) => val ? val : '',
	},
	{
		name: 'created',
		label: 'Created',
		field: (chat: Chat) => chat.created,
		format: (val: string) => formatDate(val),
		sortable: true
	},
	{
		name: 'status',
		label: 'Status',
		field: (chat: Chat) => chat.status,
	},
	{
		name: 'comment',
		label: 'Comment',
		format: (val: string | null) => val ? val : '',
		field: (chat: Chat) => chat.internalNote
	},
	{
		name: 'actions',
		label: 'Actions',
		field: 'actions',
	},
]

const supportAgentChatStore = useSupportAgentChatStore();
const search = ref('');
const $q = useQuasar();

function exportMessagesToCsv() {
	const exportColumns = columns.filter(col => col.name !== 'actions');
	const rows = [
		exportColumns.map(col => col.label),
		...supportAgentChatStore.chats.map((chat: Chat) =>
			exportColumns.map(col => {
				if (col.format) {
					return col.format(col.field(chat)!);
				}
				return typeof col.field === 'function' ? col.field(chat) : '';
			})
		)
	];
	const content = rows.map(row => row.map(field => `"${String(field ?? '').replace(/"/g, '""')}"`).join(';')).join('\r\n');
	exportFile('chats_export.csv', content, 'text/csv');
}

const openChatHistory = (chat: Chat) => {
	$q.dialog({
		component: ChatHistory,
		componentProps: {
			persistent: true,
			chat,
		},
	});
};
</script>
