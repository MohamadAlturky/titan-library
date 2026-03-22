import { apiClient } from '@/lib/api';

export interface AuthorBookDto {
  id: number;
  title: string;
  isbn: string;
  authorId: number;
  isAvailable: boolean;
  createdAt: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  totalPages: number;
  page: number;
  pageSize: number;
}

export interface GetAuthorBooksParams {
  search?: string;
  isAvailable?: boolean;
  sortBy?: 'id' | 'title' | 'isbn' | 'isAvailable';
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface CreateBookRequest {
  isbn: string;
  title: string;
}

export interface UpdateBookRequest {
  isbn: string;
  title: string;
}

export const authorBookService = {
  getBooks: (params?: GetAuthorBooksParams) =>
    apiClient.get<PaginatedResult<AuthorBookDto>>('/AuthorBooks', { params }),

  createBook: (data: CreateBookRequest) =>
    apiClient.post<AuthorBookDto>('/AuthorBooks', data),

  updateBook: (id: number, data: UpdateBookRequest) =>
    apiClient.put<AuthorBookDto>(`/AuthorBooks/${id}`, data),

  deleteBook: (id: number) =>
    apiClient.delete<boolean>(`/AuthorBooks/${id}`),
};
