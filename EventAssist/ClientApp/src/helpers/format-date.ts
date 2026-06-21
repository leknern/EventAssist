import { fromZonedTime } from 'date-fns-tz';
import { date as quasarDate } from 'quasar';

export function formatDate(dateString: string, isUtc: boolean = false) {
	const date = isUtc ? fromZonedTime(dateString.replace('Z', ''), 'UTC') : new Date(dateString);
	return quasarDate.formatDate(date, 'YYYY-MM-DD HH:mm:ss');
}
