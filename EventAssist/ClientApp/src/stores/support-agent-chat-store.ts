import axios from 'axios';
import { acceptHMRUpdate, defineStore } from 'pinia';
import { standardizeError } from 'src/helpers/standardize-error';
import type { Chat } from 'src/models/Chat';
import { useAuthStore } from './auth-store';
import type { Message } from 'src/models/Message';
import { ChatStatus } from 'src/models/ChatStatus';
import { compareAsc } from 'date-fns';
import type { MessageSender } from 'src/models/MessageSender';
import { MessageType } from 'src/models/message-type';

export const useSupportAgentChatStore = defineStore('supportAgentChatStore', {
	state: () => ({
		chats: [] as Chat[],
		chatId: null as number | null,
		typingMembers: [] as MessageSender[],
		messages: [] as Message[],
		lastEvent: '',
	}),
	getters: {
		textMessages: (state) =>
			state.messages.filter((message) => message.type === MessageType.Text),
		openedChat: (state) => {
			if (state.chats.length === 0 || !state.chatId) {
				return null;
			}

			return state.chats.find((chat) => chat.id === state.chatId);
		},
		myChats: (state) => {
			const authStore = useAuthStore();
			if (!authStore.user) {
				return [] as Chat[];
			}

			return state.chats
				.filter(
					(chat) =>
						chat.lastMessage &&
						chat.status === ChatStatus.OperatorAssigned &&
						chat.customerSupportAgent?.id === authStore.user!.id,
				)
				.sort((a, b) => {
					const sentDateA = new Date(a.lastMessage!.sentDate);
					const sentDateB = new Date(b.lastMessage!.sentDate);

					return compareAsc(sentDateA, sentDateB);
				});
		},
		allChats: (state) => {
			return state.chats
				.filter((chat) => chat.lastMessage && chat.status === ChatStatus.Opened)
				.sort((a, b) => {
					if (a.humanSupportRequired !== b.humanSupportRequired) {
						return a.humanSupportRequired ? -1 : 1;
					}

					const sentDateA = new Date(a.lastMessage!.sentDate);
					const sentDateB = new Date(b.lastMessage!.sentDate);

					return compareAsc(sentDateA, sentDateB);
				});
		},
		closedChats: (state) => {
			const authStore = useAuthStore();
			if (!authStore.user) {
				return [] as Chat[];
			}
			return state.chats
				.filter(
					(chat) =>
						chat.lastMessage &&
						chat.status === ChatStatus.Closed &&
						chat.customerSupportAgent?.id === authStore.user?.id,
				)
				.sort((a, b) => {
					const sentDateA = new Date(a.lastMessage!.sentDate);
					const sentDateB = new Date(b.lastMessage!.sentDate);

					return compareAsc(sentDateA, sentDateB);
				});
		},
		humanAgentRequestsCount: (state) => {
			return state.chats.filter(
				(chat) => chat.status === ChatStatus.Opened && chat.humanSupportRequired,
			).length;
		},
	},
	actions: {
		async init() {
			try {
				const response = await axios.get('/api/Chat/GetChats');
				this.chats = response.data;
			} catch (error) {
				standardizeError(error);
			}
		},
		addChat(newChat: Chat) {
			this.chats.push(newChat);
		},
		tryUpdateChat(updatedChat: Chat) {
			const index = this.chats.findIndex((chat) => chat.id === updatedChat.id);
			if (index !== -1) {
				this.chats[index] = updatedChat;
			}
		},
		selectChat(chatId: number) {
			if (this.chatId !== chatId) {
				this.chatId = chatId;
				this.messages = [];
			}
		},
		clearSelectedChat() {
			this.messages = [];
			this.chatId = null;
		},
		async closeChat() {
			try {
				await axios.get('/api/Chat/CloseChat', {
					params: {
						chatId: this.chatId,
					},
				});
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async takeOverChat() {
			try {
				await axios.get('/api/Chat/TakeOverChat', {
					params: {
						chatId: this.chatId,
					},
				});
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async updateCustomerSupportComment(comment: string) {
			try {
				await axios.put('/api/Chat/UpdateCustomerSupportComment', {
					chatId: this.chatId,
					comment,
				});
			} catch (error) {
				throw standardizeError(error);
			}
		},
		trySetAsLastMessage(chatId: number, message: Message) {
			const chat = this.chats.find((chat) => chat.id === chatId);
			if (chat) {
				chat.lastMessage = message;
			}
		},
		tryAppendMessage(chatId: number, message: Message) {
			if (this.chatId === chatId) {
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
						chatId: this.openedChat!.id,
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
			for (const chat of this.chats) {
				if (chat.user.id === userId) {
					chat.user.isOnline = isOnline;
				}
			}
		},
		trySetMemberAsTyping(chatId: number, sender: MessageSender) {
			if (this.chatId === chatId && !this.typingMembers.includes(sender)) {
				this.typingMembers.push(sender);
			}
		},
		trySetMemberAsNotTyping(chatId: number, sender: MessageSender) {
			if (this.chatId === chatId) {
				this.typingMembers = this.typingMembers.filter((member) => member !== sender);
			}
		},
		reset() {
			this.$reset();
		},
	},
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useSupportAgentChatStore, import.meta.hot));
}
