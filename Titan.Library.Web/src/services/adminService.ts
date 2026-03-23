import { apiClient } from '@/lib/api';
import type { PaginatedResult } from '@/services/authorBookService';

// ─── Users ────────────────────────────────────────────────────────────────────

export interface AdminUserDto {
  id: number;
  name: string;
  email: string;
  userType: number; // 1 = customer, 3 = author
  createdAt: string;
  isActive: boolean;
}

export interface GetAdminUsersParams {
  search?: string;
  userType?: number;
  sortBy?: 'id' | 'name' | 'email' | 'createdAt' | 'isActive';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

// ─── Books ────────────────────────────────────────────────────────────────────

export interface AdminBookDto {
  id: number;
  title: string;
  isbn: string;
  description: string;
  authorId: number;
  authorName: string;
  authorEmail: string;
  isAvailable: boolean;
  createdAt: string;
}

export interface GetAdminBooksParams {
  authorName?: string;
  search?: string;
  isAvailable?: boolean;
  sortBy?: 'id' | 'title' | 'isbn' | 'isAvailable';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

// ─── Messages ─────────────────────────────────────────────────────────────────

export interface AdminMessageDto {
  id: number;
  key: string;
  value: string;
  createdAt: string;
}

export interface GetAdminMessagesParams {
  search?: string;
  sortBy?: 'id' | 'key' | 'value' | 'createdAt';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface UpdateMessageRequest {
  key: string;
  value: string;
}

// ─── Service ──────────────────────────────────────────────────────────────────

export interface AdminBorrowDto {
  id: number;
  customerId: number;
  bookId: number;
  customerName: string;
  isReturned: boolean;
  returnedAt: string | null;
  createdAt: string;
}

export interface GetAdminBorrowsParams {
  sortBy?: 'id' | 'customerName' | 'createdAt' | 'returnedAt' | 'isReturned';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export const adminService = {
  getUsers: (params?: GetAdminUsersParams) =>
    apiClient.get<PaginatedResult<AdminUserDto>>('/Admin/users', { params }),

  getBooks: (params?: GetAdminBooksParams) =>
    apiClient.get<PaginatedResult<AdminBookDto>>('/Admin/books', { params }),

  getBookBorrows: (bookId: number, params?: GetAdminBorrowsParams) =>
    apiClient.get<PaginatedResult<AdminBorrowDto>>(`/Admin/books/${bookId}/borrows`, { params }),

  getMessages: (params?: GetAdminMessagesParams) =>
    apiClient.get<PaginatedResult<AdminMessageDto>>('/Messages', { params }),

  updateMessage: (id: number, data: UpdateMessageRequest) =>
    apiClient.put<AdminMessageDto>(`/Messages/${id}`, data),
};
