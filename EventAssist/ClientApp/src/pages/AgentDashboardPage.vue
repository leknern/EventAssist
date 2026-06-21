<template>
	<q-page class="row full-height">
		<div class="col-4">
			<q-btn-toggle push class="q-ma-md" color="dark" text-color="white" toggle-color="primary"
				toggle-text-color="dark" v-model="currentTab" :options="[
					{ label: 'My Chats', value: 'myChats', slot: 'myChats' },
					{ label: 'All Chats', value: 'allChats', slot: 'allChats' },
					{ label: 'Closed', value: 'closedChats', slot: 'closedChats' },
				]">
				<template v-slot:allChats>
					<q-badge v-if="supportAgentChatStore.humanAgentRequestsCount !== 0" color="negative" floating>{{
						supportAgentChatStore.humanAgentRequestsCount
					}}</q-badge>
				</template>
			</q-btn-toggle>

			<q-list v-show="currentTab === 'myChats'">
				<chat-list-item :important="false" :chat="chat" v-for="chat in supportAgentChatStore.myChats"
					:key="chat.id" @click="selectChat(chat.id)"></chat-list-item>
			</q-list>
			<q-list v-show="currentTab === 'allChats'">
				<chat-list-item :important="chat.humanSupportRequired" :chat="chat"
					v-for="chat in supportAgentChatStore.allChats" :key="chat.id"
					@click="selectChat(chat.id)"></chat-list-item>
			</q-list>
			<q-list v-show="currentTab === 'closedChats'">
				<chat-list-item :important="false" :chat="chat" v-for="chat in supportAgentChatStore.closedChats"
					:key="chat.id" @click="selectChat(chat.id)"></chat-list-item>
			</q-list>
		</div>
		<div class="col-8">
			<customer-support-agent-chat></customer-support-agent-chat>
		</div>
	</q-page>
</template>

<script setup lang="ts">
import ChatListItem from 'src/components/ChatListItem.vue';
import CustomerSupportAgentChat from 'src/components/CustomerSupportAgentChat.vue';
import { useSupportAgentChatStore } from 'src/stores/support-agent-chat-store';
import { onBeforeUnmount, ref } from 'vue';

const supportAgentChatStore = useSupportAgentChatStore();
const currentTab = ref('myChats');

const selectChat = (chatId: number) => {
	supportAgentChatStore.selectChat(chatId);
}

onBeforeUnmount(() => {
	supportAgentChatStore.clearSelectedChat();
})
</script>
<style scoped lang="scss">
.custom-border {
	border: 1px solid $primary;
}
</style>
