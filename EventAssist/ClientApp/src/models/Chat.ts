import type { ChatMember } from './ChatMember';
import type { ChatStatus } from './ChatStatus';
import type { Message } from './Message';

export interface Chat {
	id: number;
	status: ChatStatus;
	user: ChatMember;
	customerSupportAgent?: ChatMember;
	humanSupportRequired: boolean;
	internalNote?: string;
	lastMessage?: Message;
	created: string;
}
