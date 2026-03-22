import { apiClient } from '@/lib/api';

export interface CustomerBookDto {
  id: number;
  title: string;
  isbn: string;
  authorId: number;
  authorName: string;
  authorEmail: string;
  isAvailable: boolean;
  createdAt: string;
}

export interface CursorPaginatedResult<T> {
  items: T[];
  hasMore: boolean;
  nextCursor: number | null;
}

export interface GetCustomerBooksParams {
  search?: string;
  isAvailable?: boolean;
  cursor?: number;
  pageSize?: number;
}

export const customerBookService = {
  getBooks: (params?: GetCustomerBooksParams) =>
    apiClient.get<CursorPaginatedResult<CustomerBookDto>>('/CustomerBooks', { params }),

  getBookById: (id: number) =>
    apiClient.get<CustomerBookDto>(`/CustomerBooks/${id}`),
};
