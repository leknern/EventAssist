import { HubConnectionBuilder } from '@microsoft/signalr';
import { useAuthStore } from 'src/stores/auth-store';

export class EventHubService {
	static connection = new HubConnectionBuilder()
		.withUrl('https://localhost:7181/eventHub', {
			accessTokenFactory: () => useAuthStore().token!,
		})
		.withAutomaticReconnect()
		.build();
}
