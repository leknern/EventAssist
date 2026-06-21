import { HubConnectionBuilder } from '@microsoft/signalr';
import { useAuthStore } from 'src/stores/auth-store';

export class ChatHubService {
	static connection = new HubConnectionBuilder()
		.withUrl('https://localhost:7181/chatHub', {
			accessTokenFactory: () => useAuthStore().token!,
		})
		.withAutomaticReconnect()
		.build();
}
