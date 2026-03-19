import type { Book } from '@/types';

export const mockBooks: Book[] = [
  { id: 'b1', title: 'The Silent Shadow', authorId: 'a1', authorName: 'Henry Author', genre: 'Mystery', publishedYear: 2019, isbn: '978-1-111-11111-1', status: 'available', totalCopies: 3, availableCopies: 2 },
  { id: 'b2', title: 'Dark Corridors', authorId: 'a1', authorName: 'Henry Author', genre: 'Mystery', publishedYear: 2020, isbn: '978-1-111-11112-8', status: 'borrowed', totalCopies: 2, availableCopies: 0 },
  { id: 'b3', title: 'Whispers in the Night', authorId: 'a1', authorName: 'Henry Author', genre: 'Thriller', publishedYear: 2021, isbn: '978-1-111-11113-5', status: 'available', totalCopies: 4, availableCopies: 3 },
  { id: 'b4', title: 'Galactic Minds', authorId: 'a2', authorName: 'Iris Author', genre: 'Sci-Fi', publishedYear: 2018, isbn: '978-2-222-22221-1', status: 'available', totalCopies: 3, availableCopies: 1 },
  { id: 'b5', title: 'Beyond the Stars', authorId: 'a2', authorName: 'Iris Author', genre: 'Sci-Fi', publishedYear: 2022, isbn: '978-2-222-22222-8', status: 'borrowed', totalCopies: 2, availableCopies: 0 },
  { id: 'b6', title: 'Quantum Dreams', authorId: 'a2', authorName: 'Iris Author', genre: 'Sci-Fi', publishedYear: 2023, isbn: '978-2-222-22223-5', status: 'available', totalCopies: 5, availableCopies: 5 },
  { id: 'b7', title: 'Empire of Dust', authorId: 'a3', authorName: 'Jack Author', genre: 'Historical', publishedYear: 2017, isbn: '978-3-333-33331-1', status: 'available', totalCopies: 2, availableCopies: 2 },
  { id: 'b8', title: 'The Bronze Age', authorId: 'a3', authorName: 'Jack Author', genre: 'Historical', publishedYear: 2019, isbn: '978-3-333-33332-8', status: 'reserved', totalCopies: 3, availableCopies: 0 },
  { id: 'b9', title: 'Hearts Ablaze', authorId: 'a4', authorName: 'Karen Writer', genre: 'Romance', publishedYear: 2021, isbn: '978-4-444-44441-1', status: 'available', totalCopies: 4, availableCopies: 3 },
  { id: 'b10', title: 'Love in Paris', authorId: 'a4', authorName: 'Karen Writer', genre: 'Romance', publishedYear: 2022, isbn: '978-4-444-44442-8', status: 'borrowed', totalCopies: 3, availableCopies: 1 },
  { id: 'b11', title: 'Autumn Sonnets', authorId: 'a5', authorName: 'Leo Scribe', genre: 'Poetry', publishedYear: 2020, isbn: '978-5-555-55551-1', status: 'available', totalCopies: 2, availableCopies: 2 },
  { id: 'b12', title: 'The Modern World', authorId: 'a6', authorName: 'Mia Novelist', genre: 'Fiction', publishedYear: 2023, isbn: '978-6-666-66661-1', status: 'available', totalCopies: 3, availableCopies: 2 },
  { id: 'b13', title: 'City of Lights', authorId: 'a6', authorName: 'Mia Novelist', genre: 'Fiction', publishedYear: 2022, isbn: '978-6-666-66662-8', status: 'borrowed', totalCopies: 2, availableCopies: 0 },
  { id: 'b14', title: 'Tech Horizons', authorId: 'a7', authorName: 'Nathan Prose', genre: 'Non-Fiction', publishedYear: 2021, isbn: '978-7-777-77771-1', status: 'available', totalCopies: 5, availableCopies: 4 },
  { id: 'b15', title: 'Future of Code', authorId: 'a7', authorName: 'Nathan Prose', genre: 'Non-Fiction', publishedYear: 2023, isbn: '978-7-777-77772-8', status: 'available', totalCopies: 3, availableCopies: 3 },
  { id: 'b16', title: 'Dragon Tales', authorId: 'a8', authorName: 'Olivia Story', genre: 'Children', publishedYear: 2020, isbn: '978-8-888-88881-1', status: 'available', totalCopies: 6, availableCopies: 5 },
  { id: 'b17', title: 'Rainbow Friends', authorId: 'a8', authorName: 'Olivia Story', genre: 'Children', publishedYear: 2021, isbn: '978-8-888-88882-8', status: 'available', totalCopies: 4, availableCopies: 4 },
  { id: 'b18', title: 'The Crystal Cave', authorId: 'a8', authorName: 'Olivia Story', genre: 'Children', publishedYear: 2022, isbn: '978-8-888-88883-5', status: 'borrowed', totalCopies: 3, availableCopies: 1 },
  { id: 'b19', title: 'Midnight Riddles', authorId: 'a1', authorName: 'Henry Author', genre: 'Mystery', publishedYear: 2023, isbn: '978-1-111-11114-2', status: 'available', totalCopies: 2, availableCopies: 2 },
  { id: 'b20', title: 'Nebula Rising', authorId: 'a2', authorName: 'Iris Author', genre: 'Sci-Fi', publishedYear: 2024, isbn: '978-2-222-22224-2', status: 'available', totalCopies: 4, availableCopies: 4 },
];
