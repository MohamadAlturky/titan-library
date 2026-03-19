import type { BorrowRecord } from '@/types';

export const mockBorrows: BorrowRecord[] = [
  { id: 'br1', bookId: 'b2', bookTitle: 'Dark Corridors', userId: 'u3', userName: 'Carol Customer', borrowDate: '2026-03-01', dueDate: '2026-03-15', status: 'overdue' },
  { id: 'br2', bookId: 'b5', bookTitle: 'Beyond the Stars', userId: 'u4', userName: 'Dave Customer', borrowDate: '2026-03-10', dueDate: '2026-03-24', status: 'active' },
  { id: 'br3', bookId: 'b10', bookTitle: 'Love in Paris', userId: 'u5', userName: 'Eve Customer', borrowDate: '2026-03-12', dueDate: '2026-03-26', status: 'active' },
  { id: 'br4', bookId: 'b13', bookTitle: 'City of Lights', userId: 'u6', userName: 'Frank Customer', borrowDate: '2026-03-05', dueDate: '2026-03-19', status: 'overdue' },
  { id: 'br5', bookId: 'b18', bookTitle: 'The Crystal Cave', userId: 'u7', userName: 'Grace Customer', borrowDate: '2026-03-15', dueDate: '2026-03-29', status: 'active' },
  { id: 'br6', bookId: 'b1', bookTitle: 'The Silent Shadow', userId: 'u3', userName: 'Carol Customer', borrowDate: '2026-02-01', dueDate: '2026-02-15', returnDate: '2026-02-14', status: 'returned' },
  { id: 'br7', bookId: 'b4', bookTitle: 'Galactic Minds', userId: 'u4', userName: 'Dave Customer', borrowDate: '2026-02-10', dueDate: '2026-02-24', returnDate: '2026-02-22', status: 'returned' },
  { id: 'br8', bookId: 'b7', bookTitle: 'Empire of Dust', userId: 'u5', userName: 'Eve Customer', borrowDate: '2026-02-15', dueDate: '2026-03-01', returnDate: '2026-02-28', status: 'returned' },
  { id: 'br9', bookId: 'b9', bookTitle: 'Hearts Ablaze', userId: 'u6', userName: 'Frank Customer', borrowDate: '2026-02-20', dueDate: '2026-03-06', returnDate: '2026-03-05', status: 'returned' },
  { id: 'br10', bookId: 'b11', bookTitle: 'Autumn Sonnets', userId: 'u7', userName: 'Grace Customer', borrowDate: '2026-03-01', dueDate: '2026-03-15', returnDate: '2026-03-14', status: 'returned' },
  { id: 'br11', bookId: 'b14', bookTitle: 'Tech Horizons', userId: 'u3', userName: 'Carol Customer', borrowDate: '2026-03-16', dueDate: '2026-03-30', status: 'active' },
  { id: 'br12', bookId: 'b16', bookTitle: 'Dragon Tales', userId: 'u4', userName: 'Dave Customer', borrowDate: '2026-02-25', dueDate: '2026-03-11', returnDate: '2026-03-10', status: 'returned' },
  { id: 'br13', bookId: 'b19', bookTitle: 'Midnight Riddles', userId: 'u5', userName: 'Eve Customer', borrowDate: '2026-03-08', dueDate: '2026-03-22', status: 'active' },
  { id: 'br14', bookId: 'b3', bookTitle: 'Whispers in the Night', userId: 'u8', userName: 'Henry Author', borrowDate: '2026-03-01', dueDate: '2026-03-15', status: 'overdue' },
  { id: 'br15', bookId: 'b12', bookTitle: 'The Modern World', userId: 'u9', userName: 'Iris Author', borrowDate: '2026-03-17', dueDate: '2026-03-31', status: 'active' },
];
