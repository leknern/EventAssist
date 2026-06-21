<template>
	<q-dialog maximized ref="dialogRef" @hide="onDialogHide">
		<q-card flat>
			<q-card-section>
				<div class="text-h6 text-uppercase text-center q-mt-sm q-mb-sm">{{ props.chat?.id }}</div>
				<div class="text-center text-grey-7">
					Created: {{ formatDate(props.chat!.created, true) }}
				</div>
			</q-card-section>
			<q-separator></q-separator>
			<q-card-section class="q-pa-none">
				<q-table color="primary" :loading="isLoading" class="fixed-column-table full-section" flat
					:columns="columns" :rows="messages" row-key="id" virtual-scroll>
					<template v-slot:top>
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
					<template v-slot:body-cell-text="props">
						<q-td :props="props">
							<div class="wrap-text">
								{{ props.row.text }}
							</div>
						</q-td>
					</template>
				</q-table>
			</q-card-section>
			<q-separator></q-separator>
			<q-card-actions align="right">
				<q-btn flat color="primary" label="Close" @click="onDialogCancel" />
			</q-card-actions>
		</q-card>
	</q-dialog>
</template>
<script setup lang="ts">
import axios from 'axios';
import type { QTableColumn } from 'quasar';
import { exportFile } from 'quasar';
import { useDialogPluginComponent } from 'quasar';
import { formatDate } from 'src/helpers/format-date';
import { standardizeError } from 'src/helpers/standardize-error';
import type { Chat } from 'src/models/Chat';
import type { Message } from 'src/models/Message';
import { MessageType } from 'src/models/message-type';
import { MessageSender } from 'src/models/MessageSender';
import { onMounted, ref } from 'vue';

defineEmits([...useDialogPluginComponent.emits]);
const { dialogRef, onDialogHide, onDialogCancel } = useDialogPluginComponent();

const columns: QTableColumn[] = [
	{
		name: 'id',
		label: 'Id',
		field: (message: Message) => message.id,
	},
	{
		name: 'sender',
		label: 'Sender',
		field: (message: Message) => message.sender,
		format: (val: MessageSender) => {
			switch (val) {
				case MessageSender.User:
					return props.chat.user.name;
				case MessageSender.Model:
					return 'BOT';
				case MessageSender.CustomerSupportAgent:
					return props.chat.customerSupportAgent!.name;
				default:
					return 'Unknown';
			}
		}
	},
	{
		name: 'type',
		label: 'Type',
		field: (message: Message) => message.type,
		format: (val: MessageType) => {
			switch (val) {
				case MessageType.Text:
					return 'TEXT';
				case MessageType.FunctionCall:
					return 'FUNCTION CALL';
				case MessageType.FunctionResponse:
					return 'FUNCTION RESPONSE';
				default:
					return 'Unknown';
			}
		}
	},
	{
		name: 'text',
		label: 'Text',
		field: (message: Message) => message.text,
		align: "center",
	},
	{
		name: 'functionCall',
		label: 'Function Call',
		field: (message: Message) => message.functionCall,
	},
	{
		name: 'sentDate',
		label: 'Sent Date',
		field: (message: Message) => message.sentDate,
		format: (val: string) => formatDate(val, true),
	}
]

const isLoading = ref(true);

const props = defineProps<{ chat: Chat }>();
const messages = ref<Message[]>([]);

function exportMessagesToCsv() {
	const rows = [
		['Id', 'Sender', 'Text', 'Function Call', 'Sent Date'],
		...messages.value.map((msg: Message) => [
			msg.id,
			columns[1]!.format ? columns[1]!.format(msg.sender, msg.sender) : msg.sender,
			msg.text,
			msg.functionCall ?? '',
			formatDate(msg.sentDate, true)
		])
	];
	const content = rows.map(row => row.map(field => `"${String(field).replace(/"/g, '""')}"`).join(';')).join('\r\n');
	exportFile(`chat_${props.chat.id}_messages.csv`, content, 'text/csv');
}

onMounted(async () => {
	try {
		const response = await axios.get('/api/Message/GetMessages', {
			params: {
				chatId: props.chat?.id,
			}
		});
		messages.value = response.data;
	} catch (error) {
		console.log(standardizeError(error));
	} finally {
		isLoading.value = false;
	}
});
</script>

<style scoped lang="scss">
.full-section {
	height: calc(100vh - 155px);
}

.wrap-text {
	white-space: pre-wrap;
	overflow-wrap: break-word;
}

.fixed-column-table thead tr:first-child th:first-child {
	background-color: $dark;
}

.fixed-column-table :deep(td:first-child) {
	background-color: $dark;
}

.fixed-column-table th:first-child,
.fixed-column-table :deep(td:first-child) {
	position: sticky;
	left: 0;
	z-index: 1;
}
</style>