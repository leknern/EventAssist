<template>
	<q-card style="width: 400px" flat>
		<q-item class="bg-primary" v-if="supportChatStore.chat?.customerSupportAgent">
			<q-item-section avatar>
				<q-avatar>
					<img :src="supportChatStore.chat.customerSupportAgent.profilePictureUrl" />
					<q-badge :color="supportChatStore.chat.customerSupportAgent.isOnline ? 'green' : 'grey'"
						class="custom-avatar-status" rounded floating />
				</q-avatar>
			</q-item-section>
			<q-item-section>
				<q-item-label class="text-dark">{{ supportChatStore.chat.customerSupportAgent.name
				}}</q-item-label>
				<q-item-label caption class="text-dark">
					{{ supportChatStore.chat.customerSupportAgent.isOnline ? 'Online' : 'Offline' }}
				</q-item-label>
			</q-item-section>
			<q-item-section side>
				<div class="row items-center">
					<q-btn round flat color="dark" icon="close" dense @click="$emit('close')" />
				</div>
			</q-item-section>
		</q-item>
		<q-item class="bg-primary" v-else>
			<q-item-section avatar>
				<q-avatar>
					<img
						src="https://play-lh.googleusercontent.com/8OlMPEnfATLiCN4OmAHGbTX0dp7_HxpPrYiKGh-0kdPxgyoqcB6L6XQvgL2Lq4Dxwg" />
					<q-badge color="green" class="custom-avatar-status" rounded floating />
				</q-avatar>
			</q-item-section>
			<q-item-section>
				<q-item-label class="text-dark">Customer Support</q-item-label>
				<q-item-label caption class="text-dark">
					Online
				</q-item-label>
			</q-item-section>
			<q-item-section side>
				<div class="row items-center">
					<q-btn round flat color="dark" icon="close" dense @click="$emit('close')" />
				</div>
			</q-item-section>
		</q-item>
		<q-bar class="bg-negative" dense v-if="supportChatStore.chat?.status === ChatStatus.Closed">
			<div class="text-caption">You cannot reply to this chat because it has been closed.</div>
		</q-bar>
		<q-bar class="bg-info" dense
			v-if="supportChatStore.chat?.humanSupportRequired && !supportChatStore.chat.customerSupportAgent">
			<q-spinner-rings color="dark" size="1.5rem" />
			<div class="text-caption text-dark">Waiting for a human operator</div>
		</q-bar>
		<q-card-section class="q-pa-none" style="height: 400px;">
			<div id=" customer-support-chat-scroll-target-id" style="height: 400px; overflow: auto;"
				v-if="supportChatStore.chat" ref="chatScroll">
				<q-infinite-scroll scroll-target="#customer-support-chat-scroll-target-id" class="q-pr-lg q-pl-lg"
					@load="onLoad" :offset="400" :initial-index="-1" reverse :key="supportChatStore.chat.id">
					<q-chat-message :name="getSenderName(message.sender, supportChatStore.chat)" :text="[message.text]"
						:bg-color="message.sender === MessageSender.User ? 'primary' : 'grey-9'"
						:text-color="message.sender === MessageSender.User ? 'dark' : 'white'"
						:stamp="formatSentDate(message.sentDate)" :sent="message.sender === MessageSender.User"
						:avatar="getSenderAvatar(message.sender, supportChatStore.chat)"
						v-for="(message) in supportChatStore.textMessages" :key="message.id" />
					<template v-for="(typingMember, index) in supportChatStore.typingMembers" :key="index">
						<q-chat-message v-if="typingMember !== MessageSender.User" bg-color="grey-9" text-color="white"
							:name="getSenderName(typingMember, supportChatStore.chat)"
							:avatar="getSenderAvatar(typingMember, supportChatStore.chat)">
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
			<div v-else class="q-pa-md text-center text-grey-7">
				There is no active or existing conversation. Please open a new chat to continue.
			</div>
		</q-card-section>
		<q-card-section class="q-pa-xs">
			<q-btn class="q-mr-xs" v-if="!supportChatStore.chat || supportChatStore.chat.status === ChatStatus.Closed"
				:loading="isChatOpeningInProgress" @click="onOpenChat" outline color="primary" label="Open new chat" />
			<q-btn class="q-mr-xs"
				v-if="supportChatStore.chat && !supportChatStore.chat.customerSupportAgent && supportChatStore.chat.status === ChatStatus.Opened"
				outline color="primary" @click="onRequestHumanSupport" label="Talk to a human" />
			<q-btn outline color="primary" label="Report an issue"></q-btn>
		</q-card-section>
		<q-card-actions>
			<q-input @focus="onStartTyping" @blur="onStopTyping" autogrow rounded outlined class="full-width"
				placeholder="Enter your message" v-model.trim="text" dense>
				<template v-slot:after>
					<q-btn
						:disable="!supportChatStore.chat || text.length === 0 || supportChatStore.chat.status === ChatStatus.Closed"
						@click="onSend" unelevated dense round icon="send" color="primary" text-color="dark" />
				</template>
			</q-input>
		</q-card-actions>
	</q-card>
</template>

<script setup lang="ts">
import { MessageSender } from 'src/models/MessageSender';
import { useSupportChatStore } from 'src/stores/support-chat-store';
import { nextTick, ref, watch } from 'vue';
// import type { QScrollArea } from 'quasar';
import { ChatStatus } from 'src/models/ChatStatus';
import { formatSentDate } from 'src/helpers/format-sent-date';
import { ChatHubService } from 'src/services/chat-hub-service';
import { getSenderName } from 'src/helpers/get-sender-name';
import { getSenderAvatar } from 'src/helpers/get-sender-avatar';

defineEmits(['close']);
const supportChatStore = useSupportChatStore();
const chatScroll = ref<HTMLDivElement | null>(null);

const text = ref('');
const isChatOpeningInProgress = ref(false);

const onOpenChat = async () => {
	isChatOpeningInProgress.value = true;

	try {
		await supportChatStore.openChat();
	} catch (error) {
		console.log(error);
	} finally {
		isChatOpeningInProgress.value = false;
	}
};

const onSend = async () => {
	try {
		const message = text.value;
		text.value = '';

		await ChatHubService.connection?.invoke('SendMessage', supportChatStore.chat?.id, message);
	} catch (error) {
		console.log(error);
	}
};

const onStartTyping = async () => {
	if (supportChatStore.chat) {
		await ChatHubService.connection.invoke('StartTyping', supportChatStore.chat?.id, MessageSender.User);
	}
}

const onStopTyping = async () => {
	if (supportChatStore.chat) {
		await ChatHubService.connection.invoke('StopTyping', supportChatStore.chat?.id, MessageSender.User);
	}
}

const onLoad = async (index: number, done: (stop?: boolean) => void) => {
	try {
		const isLast = await supportChatStore.loadMessages(index, 10);
		done(isLast);
	} catch (error) {
		console.log(error);
	}
};

const onRequestHumanSupport = async () => {
	try {
		await supportChatStore.requestHumanSupport();
	} catch (error) {
		console.log(error);
	}
};

watch(
	[() => supportChatStore.textMessages, () => supportChatStore.typingMembers],
	async ([messages]) => {
		if (!chatScroll.value) {
			return;
		}

		const scrollContainer = chatScroll.value;

		const isScrolledToBottom =
			scrollContainer.scrollHeight - scrollContainer.clientHeight <=
			scrollContainer.scrollTop + 1;

		const lastMessageSender = messages[messages.length - 1]?.sender
		const isLastMessageFromUser = lastMessageSender === MessageSender.User && supportChatStore.lastEvent === 'receive_message'
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
	border: 3px solid $primary;
	width: 16px;
	height: 16px;
	padding: 0;
}
</style>
