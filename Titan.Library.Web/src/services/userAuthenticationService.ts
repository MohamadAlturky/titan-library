import { apiClient } from '@/lib/api';

export interface AuthTokenDto {
  token: string;
  userId: number;
  userType: string;
}

export interface UserProfileDto {
  id: number;
  name: string;
  email: string;
  userType: string;
  createdAt: string;
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  userType: string;
}

export const userAuthenticationService = {
  login: (data: LoginRequest) =>
    apiClient.post<AuthTokenDto>('/auth/login', data),

  register: (data: RegisterRequest) =>
    apiClient.post<AuthTokenDto>('/auth/register', data),

  getProfile: () =>
    apiClient.get<UserProfileDto>('/auth/profile'),
};
