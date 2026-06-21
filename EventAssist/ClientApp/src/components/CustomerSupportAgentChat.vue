<template>
	<q-card v-if="!supportAgentChatStore.openedChat" class="full-height no-top-bottom-border" square flat bordered>
		<q-card-section class="text-center">
			<p class="text-grey-7">You don't have any open chats.</p>
		</q-card-section>
	</q-card>
	<q-card v-else class="full-height no-top-bottom-border" square flat bordered>
		<q-item>
			<q-item-section avatar>
				<q-avatar>
					<img :src="supportAgentChatStore.openedChat?.user.profilePictureUrl" />
					<q-badge :color="supportAgentChatStore.openedChat?.user.isOnline ? 'green' : 'grey'" rounded
						class="custom-avatar-status" floating />
				</q-avatar>
			</q-item-section>
			<q-item-section>
				<q-item-label>{{ supportAgentChatStore.openedChat?.user.name }}</q-item-label>
				<q-item-label caption>
					{{ supportAgentChatStore.openedChat?.user.isOnline ? 'Online' : 'Offline' }}
				</q-item-label>
			</q-item-section>
			<q-item-section side>
				<div class="row items-center">
					<q-btn push color="secondary"
						:disable="supportAgentChatStore.openedChat.status === ChatStatus.Closed || !supportAgentChatStore.openedChat.customerSupportAgent || supportAgentChatStore.openedChat.customerSupportAgent.id !== authStore.user?.id"
						@click="closeChat" label="Close" icon="check" />
				</div>
			</q-item-section>
		</q-item>
		<q-separator></q-separator>
		<q-bar class="float-bar bg-negative" dense v-if="supportAgentChatStore.openedChat.status === ChatStatus.Closed">
			<div class="text-caption">You cannot reply to this chat because it has been closed.</div>
		</q-bar>
		<q-card-section class="q-pa-none">
			<div id="customer-support-agent-chat-scroll-target-id" style="height: 556px; overflow: auto;"
				ref="chatScroll">
				<q-infinite-scroll scroll-target="#customer-support-agent-chat-scroll-target-id"
					:key="supportAgentChatStore.openedChat.id" class="q-pr-lg q-pl-lg" @load="onLoad" :offset="556"
					:initial-index="-1" :reverse="true">
					<q-chat-message v-for="(message, index) in supportAgentChatStore.textMessages" :key="index"
						:name="getSenderName(message.sender, supportAgentChatStore.openedChat)" :text="[message.text]"
						:avatar="getSenderAvatar(message.sender, supportAgentChatStore.openedChat)"
						:bg-color="message.sender === MessageSender.CustomerSupportAgent ? 'primary' : 'grey-9'"
						:text-color="message.sender === MessageSender.CustomerSupportAgent ? 'dark' : 'white'"
						:stamp="formatSentDate(message.sentDate)"
						:sent="message.sender === MessageSender.CustomerSupportAgent" />
					<template v-for="(typingMember, index) in supportAgentChatStore.typingMembers" :key="index">
						<q-chat-message v-if="typingMember !== MessageSender.CustomerSupportAgent" bg-color="grey-9"
							text-color="white" :name="getSenderName(typingMember, supportAgentChatStore.openedChat)"
							:avatar="getSenderAvatar(typingMember, supportAgentChatStore.openedChat)">
							<q-spinner-dots size="2rem" />
						</q-chat-message>
					</template>
					<template v-slot:loading>
						<div class="row justify-center q-my-md">
							<q-spinner-oval color="primary" size="2rem" />
						</div>
					</template>
				</q-infinite-scroll>
			</div>
		</q-card-section>
		<q-card-section class="q-pa-xs">
			<q-btn class="q-mr-xs" v-if="!supportAgentChatStore.openedChat.customerSupportAgent" @click="takeOverChat"
				outline color="primary" label="take over chat" />
			<q-btn outline color="primary" label="Report an issue"></q-btn>
		</q-card-section>
		<q-card-section class="q-pa-none">
			<q-tabs v-model="tab" dense class="text-grey" active-color="primary" indicator-color="primary" align="left">
				<q-tab name="reply" label="Reply" />
				<q-tab name="internal_note" label="Internal note" />
			</q-tabs>
			<q-separator></q-separator>
			<q-tab-panels v-model="tab">
				<q-tab-panel name="reply">
					<q-input @focus="onStartTyping" @blur="onStopTyping" dense
						:disable="supportAgentChatStore.openedChat.customerSupportAgent?.id !== authStore.user?.id"
						autogrow class="q-mb-sm" outlined v-model="text" placeholder="Enter your message">
					</q-input>
					<div class="row justify-end">
						<q-btn @click="onSend"
							:disable="text.length === 0 || supportAgentChatStore.openedChat.customerSupportAgent?.id !== authStore.user?.id || supportAgentChatStore.openedChat.status === ChatStatus.Closed"
							push icon="send" label="Send" color="primary" text-color="dark" />
					</div>
				</q-tab-panel>
				<q-tab-panel name="internal_note">
					<q-input :disable="supportAgentChatStore.openedChat.customerSupportAgent?.id !== authStore.user?.id"
						dense autogrow outlined v-model="comment" class="q-mb-sm"
						placeholder="Write your internal note here">
					</q-input>
					<div class="row justify-end">
						<q-btn @click="onUpdateCustomerSupportComment"
							:disable="comment.length === 0 || supportAgentChatStore.openedChat.customerSupportAgent?.id !== authStore.user?.id"
							push label="Save" color="primary" text-color="dark" />
					</div>
				</q-tab-panel>
			</q-tab-panels>
		</q-card-section>
	</q-card>
</template>
<script setup lang="ts">
import { MessageSender } from 'src/models/MessageSender';
import { useSupportAgentChatStore } from 'src/stores/support-agent-chat-store';
import { nextTick, ref, watch } from 'vue';
import { QInfiniteScroll } from 'quasar';
import { useAuthStore } from 'src/stores/auth-store';
import { ChatStatus } from 'src/models/ChatStatus';
import { storeToRefs } from 'pinia';
import { formatSentDate } from 'src/helpers/format-sent-date';
import { ChatHubService } from 'src/services/chat-hub-service';
import { getSenderName } from 'src/helpers/get-sender-name';
import { getSenderAvatar } from 'src/helpers/get-sender-avatar';

const supportAgentChatStore = useSupportAgentChatStore();
const authStore = useAuthStore();
const chatScroll = ref<HTMLDivElement | null>(null);

const tab = ref('reply');
const text = ref('');
const comment = ref('');

const onSend = async () => {
	try {
		await ChatHubService.connection?.invoke('SendMessage', supportAgentChatStore.chatId, text.value);
		text.value = '';
	} catch (error) {
		console.log(error);
	}
};

const onStartTyping = async () => {
	if (supportAgentChatStore.chatId) {
		await ChatHubService.connection.invoke('StartTyping', supportAgentChatStore.chatId, MessageSender.CustomerSupportAgent);
	}
}

const onStopTyping = async () => {
	if (supportAgentChatStore.chatId) {
		await ChatHubService.connection.invoke('StopTyping', supportAgentChatStore.chatId, MessageSender.CustomerSupportAgent);
	}
}

const onUpdateCustomerSupportComment = async () => {
	try {
		await supportAgentChatStore.updateCustomerSupportComment(comment.value);
	} catch (error) {
		console.log(error)
	}
}

const takeOverChat = async () => {
	try {
		await supportAgentChatStore.takeOverChat();
	} catch (error) {
		console.log(error)
	}
}

const closeChat = async () => {
	try {
		await supportAgentChatStore.closeChat();
	} catch (error) {
		console.log(error)
	}
}

const onLoad = async (index: number, done: (stop?: boolean) => void) => {
	try {
		const isLast = await supportAgentChatStore.loadMessages(index, 20);
		done(isLast);
	} catch (error) {
		console.log(error);
	}
}

const { openedChat } = storeToRefs(supportAgentChatStore);

watch(openedChat, (newChat) => {
	if (newChat) {
		comment.value = newChat.internalNote ?? '';
	}
})

watch(
	[() => supportAgentChatStore.textMessages, () => supportAgentChatStore.typingMembers],
	async ([messages]) => {
		if (!chatScroll.value) {
			return;
		}

		const scrollContainer = chatScroll.value;

		const isScrolledToBottom =
			scrollContainer.scrollHeight - scrollContainer.clientHeight <=
			scrollContainer.scrollTop + 1;

		const lastMessageSender = messages[messages.length - 1]?.sender
		const isLastMessageFromUser = lastMessageSender === MessageSender.CustomerSupportAgent && supportAgentChatStore.lastEvent === 'receive_message'
		if (isScrolledToBottom || isLastMessageFromUser) {
			await nextTick();
			scrollContainer.scrollTop = scrollContainer.scrollHeight;
		}
	},
	{ deep: true },
);
</script>
<style scoped lang="scss">
.custom-avatar-status {
	bottom: -4px;
	top: auto;
	border: 3px solid $dark;
	width: 16px;
	height: 16px;
	padding: 0;
}

.float-bar {
	position: absolute;
	width: 100%;
	z-index: 2;
}

.no-top-bottom-border {
	border-top: none;
	border-bottom: none;
}
</style>