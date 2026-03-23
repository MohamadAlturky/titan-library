import axios from 'axios';
import { toast } from 'sonner';

export const BASE_URL = 'http://localhost:8080';

export function setTokenCookie(token: string) {
  document.cookie = `titan_token=${token}; path=/; SameSite=Strict`;
}

export function getTokenCookie(): string | null {
  const match = document.cookie.match(/(?:^|;\s*)titan_token=([^;]+)/);
  return match ? match[1] : null;
}

export function removeTokenCookie() {
  document.cookie = 'titan_token=; path=/; max-age=0';
}

export const apiClient = axios.create({
  baseURL: `${BASE_URL}/api`,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = getTokenCookie();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Unwrap the backend envelope: { success, message, data: T } → response.data = T
apiClient.interceptors.response.use(
  (response) => {
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      response.data = response.data.data;
    }
    return response;
  },
  (error) => {
    const status = error?.response?.status;
    if (status === 401 || status === 403) {
      removeTokenCookie();
      toast.error(status === 403 ? 'You do not have permission to perform this action.' : 'Your session has expired. Please log in again.');
      setTimeout(() => { window.location.href = '/login'; }, 1500);
    }
    return Promise.reject(error);
  },
);
