import { acceptHMRUpdate, defineStore } from 'pinia';
import { standardizeError } from 'src/helpers/standardize-error';
import axios from 'axios';
import type { UserEvent } from 'src/models/UserEvent';
import type { UserEventRequest } from 'src/models/UserEventRequest';
import type { EventDescriptionRequest } from 'src/models/EventDescriptionRequest';

export const useEventStore = defineStore('event', {
	state: () => ({
		isLoaded: false,
		events: [] as UserEvent[],
	}),
	actions: {
		async init() {
			try {
				const response = await axios.get<UserEvent[]>('/api/Event/GetEvents');
				this.isLoaded = true;
				this.events = response.data;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		reset() {
			this.$reset();
		},
		async add(request: UserEventRequest) {
			try {
				const response = await axios.post<UserEvent>('/api/Event/AddEvent', request);
				this.events.push(response.data);
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async update(request: EventDescriptionRequest) {
			try {
				const response = await axios.patch<UserEvent>('/api/Event/UpdateEvent', request);
				const index = this.events.findIndex((e) => e.id === request.id);
				this.events[index] = response.data;
			} catch (error) {
				throw standardizeError(error);
			}
		},
		async remove(eventId: number) {
			try {
				await axios.delete('/api/Event/RemoveEvent', {
					params: {
						eventId,
					},
				});

				this.events = this.events.filter((e) => e.id !== eventId);
			} catch (error) {
				throw standardizeError(error);
			}
		},
		appendEvent(event: UserEvent) {
			this.events.push(event);
		},
	},
});

if (import.meta.hot) {
	import.meta.hot.accept(acceptHMRUpdate(useEventStore, import.meta.hot));
}
