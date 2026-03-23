import { apiClient } from '@/lib/api';

export interface CustomerBookDto {
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

export interface BorrowDto {
  id: number;
  bookId: number;
  customerId: number;
  borrowedAt: string;
  returnedAt: string | null;
}

export interface CustomerBorrowDto {
  id: number;
  bookId: number;
  bookTitle: string;
  authorName: string;
  isReturned: boolean;
  returnedAt: string | null;
  createdAt: string;
}

export const customerBookService = {
  getBooks: (params?: GetCustomerBooksParams) =>
    apiClient.get<CursorPaginatedResult<CustomerBookDto>>('/CustomerBooks', { params }),

  getBookById: (id: number) =>
    apiClient.get<CustomerBookDto>(`/CustomerBooks/${id}`),

  borrowBook: (bookId: number) =>
    apiClient.post<BorrowDto>(`/Borrows/borrow/${bookId}`),

  getBorrowsByCustomer: () =>
    apiClient.get<CustomerBorrowDto[]>(`/Borrows/Mine`),

  returnBook: (bookId: number) =>
    apiClient.post<BorrowDto>(`/Borrows/return/${bookId}`),
};
