<template>
	<router-view />
</template>

<script setup lang="ts">
import { useQuasar } from 'quasar'
import { useAuthStore } from './stores/auth-store';
import { storeToRefs } from 'pinia';
import { watch } from 'vue';
import { useEventStore } from './stores/event-store';
import { useSupportAgentChatStore } from './stores/support-agent-chat-store';
import { useSupportChatStore } from './stores/support-chat-store';
import type { Chat } from './models/Chat';
import type { Message } from './models/Message';
import { ChatHubService } from './services/chat-hub-service';
import type { MessageSender } from './models/MessageSender';
import { EventHubService } from './services/event-hub-service';
import type { UserEvent } from './models/UserEvent';
import { HubConnectionState } from '@microsoft/signalr';

const $q = useQuasar()
$q.dark.set(true)

const authStore = useAuthStore();
const { isAuthenticated } = storeToRefs(authStore);

const eventStore = useEventStore();
const supportAgentChatStore = useSupportAgentChatStore();
const supportChatStore = useSupportChatStore();

watch(isAuthenticated, async (loggedIn) => {
	if (loggedIn) {
		try {
			await eventStore.init();
			await supportAgentChatStore.init();
			await supportChatStore.init();

			if (ChatHubService.connection.state === HubConnectionState.Connected) {
				ChatHubService.connection.off('UserConnected');
				ChatHubService.connection.off('UserDisconnected');
				ChatHubService.connection.off('ChatAdded');
				ChatHubService.connection.off('ChatUpdated');
				ChatHubService.connection.off('ReceiveMessage');
				ChatHubService.connection.off('StartTyping');
				ChatHubService.connection.off('StopTyping');
				await ChatHubService.connection.stop();
			}

			if (EventHubService.connection.state === HubConnectionState.Connected) {
				EventHubService.connection.off('ReceiveNewMessage');
				await EventHubService.connection.stop();
			}

			ChatHubService.connection.on('UserConnected', (userId: number) => {
				supportAgentChatStore.tryUpdateUserConnection(userId, true);
				supportChatStore.tryUpdateUserConnection(userId, true);
			});

			ChatHubService.connection.on('UserDisconnected', (userId: number) => {
				supportAgentChatStore.tryUpdateUserConnection(userId, false);
				supportChatStore.tryUpdateUserConnection(userId, false);
			});

			ChatHubService.connection.on('ChatAdded', async (chat: Chat) => {
				supportAgentChatStore.addChat(chat);
				await ChatHubService.connection.invoke('JoinChatGroup', chat.id);
			});

			ChatHubService.connection.on('ChatUpdated', (chat: Chat) => {
				supportAgentChatStore.tryUpdateChat(chat);
				supportChatStore.tryUpdateChat(chat);
			});

			ChatHubService.connection.on('ReceiveMessage', (chatId: number, message: Message) => {
				supportAgentChatStore.tryAppendMessage(chatId, message);
				supportAgentChatStore.trySetAsLastMessage(chatId, message);
				supportChatStore.tryAppendMessage(chatId, message);
			});

			ChatHubService.connection.on('StartTyping', (chatId: number, sender: MessageSender) => {
				supportAgentChatStore.trySetMemberAsTyping(chatId, sender);
				supportChatStore.trySetMemberAsTyping(chatId, sender);
			});

			ChatHubService.connection.on('StopTyping', (chatId: number, sender: MessageSender) => {
				supportAgentChatStore.trySetMemberAsNotTyping(chatId, sender);
				supportChatStore.trySetMemberAsNotTyping(chatId, sender);
			});

			EventHubService.connection.on('ReceiveNewEvent', (event: UserEvent) => {
				eventStore.appendEvent(event);
			});

			await ChatHubService.connection.start();
			await EventHubService.connection.start();
		} catch (error) {
			console.log(error);
		}
	} else {
		ChatHubService.connection.off('UserConnected');
		ChatHubService.connection.off('UserDisconnected');
		ChatHubService.connection.off('ChatAdded');
		ChatHubService.connection.off('ChatUpdated');
		ChatHubService.connection.off('ReceiveMessage');
		ChatHubService.connection.off('StartTyping');
		ChatHubService.connection.off('StopTyping');
		EventHubService.connection.off('ReceiveNewMessage');

		await ChatHubService.connection.stop();
		await EventHubService.connection.stop();

		eventStore.reset();
		supportAgentChatStore.reset();
		supportChatStore.reset();
	}
}, { immediate: true })
</script>
