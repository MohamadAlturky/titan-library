export type UserRole = 'admin' | 'customer' | 'author';

export interface AuthUser {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  token: string;
  avatarUrl?: string;
}

export interface Author {
  id: string;
  name: string;
  email: string;
  bio: string;
  booksCount: number;
}

export interface Book {
  id: string;
  title: string;
  authorId: string;
  authorName: string;
  genre: string;
  publishedYear: number;
  isbn: string;
  status: 'available' | 'borrowed' | 'reserved';
  totalCopies: number;
  availableCopies: number;
}

export interface BorrowRecord {
  id: string;
  bookId: string;
  bookTitle: string;
  userId: string;
  userName: string;
  borrowDate: string;
  dueDate: string;
  returnDate?: string;
  status: 'active' | 'returned' | 'overdue';
}
