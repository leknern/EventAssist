import axios from 'axios';

export function standardizeError(error: unknown) {
	if (axios.isAxiosError(error)) {
		return new Error(error.response?.data || error.message || 'Unknown error');
	}

	if (error instanceof Error) {
		return error;
	}

	return new Error('Unknown error');
}
