<template>
	<q-item :class="{ 'bg-info': important }" clickable :focused="chat.id === supportAgentChatStore.chatId">
		<q-item-section top avatar>
			<q-avatar>
				<img :src="props.chat.user.profilePictureUrl">
				<q-badge :color="props.chat.user.isOnline ? 'green' : 'grey'" rounded floating
					class="custom-avatar-status" />
			</q-avatar>
		</q-item-section>
		<q-item-section>
			<q-item-label :class="{ 'text-dark': important }">{{ props.chat.user.name }} ({{ props.chat.id
			}})</q-item-label>
			<q-item-label :class="{ 'text-dark': important }" caption lines="2">
				{{ message }}
			</q-item-label>
		</q-item-section>
		<q-item-section side top>
			<q-item-label caption :class="{ 'text-dark': important }">{{ displaySentDate }}</q-item-label>
		</q-item-section>
	</q-item>
</template>
<script setup lang="ts">
import type { Chat } from 'src/models/Chat';
import { MessageSender } from 'src/models/MessageSender';
import { useSupportAgentChatStore } from 'src/stores/support-agent-chat-store';
import { computed } from 'vue';
import { formatSentDate } from 'src/helpers/format-sent-date';

const props = defineProps<{ chat: Chat, important: boolean }>();
const supportAgentChatStore = useSupportAgentChatStore();

const displaySentDate = computed(() => {
	const date = props.chat.lastMessage ? props.chat.lastMessage.sentDate : props.chat.created
	return formatSentDate(date);
})

const message = computed(() => {
	if (!props.chat.lastMessage) {
		return 'No messages yet.';
	}

	let sender = '';
	switch (props.chat.lastMessage.sender) {
		case MessageSender.User:
			sender = props.chat.user.name;
			break;
		case MessageSender.Model:
			sender = 'Bot'
			break;
		case MessageSender.CustomerSupportAgent:
			sender = 'You'
			break;
	}

	return `${sender}: ${props.chat.lastMessage.text}`
})

</script>
<style scoped lang="scss">
.custom-avatar-status {
	bottom: -4px;
	top: auto;
	border: 3px solid #0F0F0F;
	width: 16px;
	height: 16px;
	padding: 0;
}
</style>