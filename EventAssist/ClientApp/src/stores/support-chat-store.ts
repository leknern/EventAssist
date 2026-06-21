import type { HubConnection } from '@microsoft/signalr';
import { acceptHMRUpdate, defineStore } from 'pinia';
import { standardizeError } from 'src/helpers/standardize-error';
import type { Chat } from 'src/models/Chat';
import axios from 'axios';
import type { Message } from 'src/models/Message';
import type { MessageSender } from 'src/models/MessageSender';
import { MessageType } from 'src/models/message-type';

export const useSupportChatStore = defineStore('supportChatStore', {
	state: () => ({
		messages: [] as Message[],
		chat: null as Chat | null,
		typingMembers: [] as MessageSender[],
		connection: null as HubConnection | null,
		isConnected: false,
		lastEvent: '',
	}),
	getters: {
		textMessages: (state) =>
			state.messages.filter((message) => message.type === MessageType.Text),
	},
	actions: {
		async init() {
			try {
				const response = await axios.get('/api/Chat/TryGetChat');
				this.chat = response.data;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async openChat() {
			try {
				const response = await axios.get('/api/Chat/OpenChat');
				this.chat = response.data;
				this.messages = [];
			} catch (error) {
				throw standardizeError(error);
			}
		},
		tryUpdateChat(chat: Chat) {
			if (!this.chat || this.chat.id !== chat.id) {
				return;
			}

			this.chat = chat;
		},
		async requestHumanSupport() {
			try {
				await axios.get('/api/Chat/RequestHumanSupport', {
					params: {
						chatId: this.chat!.id,
					},
				});
			} catch (error) {
				throw standardizeError(error);
			}
		},
		tryAppendMessage(chatId: number, message: Message) {
			if (this.chat?.id === chatId) {
				this.lastEvent = 'receive_message';
				this.messages.push(message);
			}
		},
		async loadMessages(index: number, limit: number) {
			try {
				const timeout = index === 0 ? 0 : 1200;
				const minLoadTime = new Promise((resolve) => setTimeout(resolve, timeout));
				const request = axios.get('/api/Message/LoadMessages', {
					params: {
						chatId: this.chat!.id,
						index,
						limit,
					},
				});

				const response = await Promise.all([minLoadTime, request]).then(
					([, result]) => result,
				);
				this.lastEvent = 'load_messages';
				this.messages = [...response.data, ...this.messages];

				return response.data.length < limit;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		tryUpdateUserConnection(userId: number, isOnline: boolean) {
			if (
				!this.chat ||
				!this.chat.customerSupportAgent ||
				this.chat.customerSupportAgent.id !== userId
			) {
				return;
			}

			this.chat.customerSupportAgent.isOnline = isOnline;
		},
		trySetMemberAsTyping(chatId: number, sender: MessageSender) {
			if (this.chat?.id === chatId && !this.typingMembers.includes(sender)) {
				this.typingMembers.push(sender);
			}
		},
		trySetMemberAsNotTyping(chatId: number, sender: MessageSender) {
			if (this.chat?.id === chatId) {
				this.typingMembers = this.typingMembers.filter((member) => member !== sender);
			}
		},
		reset() {
			this.$reset();
		},
	},
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useSupportChatStore, import.meta.hot));
}
