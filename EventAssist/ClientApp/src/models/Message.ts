import type { MessageType } from './message-type';
import type { MessageSender } from './MessageSender';

export interface Message {
	id: number;
	type: MessageType;
	sender: MessageSender;
	text: string;
	functionCall?: string;
	functionResponse?: string;
	sentDate: string;
}
