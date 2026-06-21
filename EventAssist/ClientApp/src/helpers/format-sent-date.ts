import { differenceInHours, formatDistance, formatRelative } from 'date-fns';
import { fromZonedTime } from 'date-fns-tz';

export function formatSentDate(dateString: string) {
	const current = new Date();
	const date = fromZonedTime(dateString.replace('Z', ''), 'UTC');

	const hoursDiff = differenceInHours(current, date);

	if (hoursDiff < 24) {
		return formatDistance(current, date);
	}

	return formatRelative(date, current);
}
