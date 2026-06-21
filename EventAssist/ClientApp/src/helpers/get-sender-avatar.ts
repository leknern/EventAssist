import type { Chat } from 'src/models/Chat';
import { MessageSender } from 'src/models/MessageSender';

export function getSenderAvatar(sender: MessageSender, chat?: Chat) {
	switch (sender) {
		case MessageSender.User:
			return chat?.user.profilePictureUrl;
		case MessageSender.Model:
			return 'https://play-lh.googleusercontent.com/8OlMPEnfATLiCN4OmAHGbTX0dp7_HxpPrYiKGh-0kdPxgyoqcB6L6XQvgL2Lq4Dxwg';
		case MessageSender.CustomerSupportAgent:
			return chat?.customerSupportAgent?.profilePictureUrl;
		default:
			return 'Unknown';
	}
}
