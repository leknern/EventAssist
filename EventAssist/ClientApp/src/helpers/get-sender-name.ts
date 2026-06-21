import type { Chat } from 'src/models/Chat';
import { MessageSender } from 'src/models/MessageSender';
import { useAuthStore } from 'src/stores/auth-store';

export function getSenderName(sender: MessageSender, chat?: Chat) {
	if (!chat) {
		return 'Unknown';
	}

	if (sender === MessageSender.Model) {
		return 'Bot';
	}

	const authStore = useAuthStore();
	const senderUserId =
		sender === MessageSender.User ? chat.user.id : chat.customerSupportAgent!.id;

	if (senderUserId === authStore.user?.id) {
		return 'Me';
	}

	if (sender === MessageSender.User) {
		return chat.user.name;
	}

	if (sender === MessageSender.CustomerSupportAgent) {
		return chat.customerSupportAgent!.name;
	}
}
