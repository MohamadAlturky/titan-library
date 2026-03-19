import { useState } from 'react';
import { toast } from 'sonner';
import { mockBooks } from '@/data/mockBooks';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';

export function BorrowBookPage() {
  const [selectedBookId, setSelectedBookId] = useState('');
  const availableBooks = mockBooks.filter(b => b.availableCopies > 0);

  const handleBorrow = () => {
    if (!selectedBookId) return;
    const book = availableBooks.find(b => b.id === selectedBookId);
    toast.success(`Successfully borrowed "${book?.title}"!`);
    setSelectedBookId('');
  };

  const selectedBook = availableBooks.find(b => b.id === selectedBookId);

  return (
    <div>
      <PageHeader title="Borrow a Book" description="Select a book to borrow" />
      <Card>
        <div className="p-6 space-y-4 max-w-lg">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Select Book
            </label>
            <select
              value={selectedBookId}
              onChange={e => setSelectedBookId(e.target.value)}
              className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">-- Select a book --</option>
              {availableBooks.map(b => (
                <option key={b.id} value={b.id}>
                  {b.title} by {b.authorName} ({b.availableCopies} available)
                </option>
              ))}
            </select>
          </div>

          {selectedBook && (
            <div className="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <h3 className="font-semibold text-gray-900 dark:text-gray-100">{selectedBook.title}</h3>
              <p className="text-sm text-gray-500 dark:text-gray-400">Author: {selectedBook.authorName}</p>
              <p className="text-sm text-gray-500 dark:text-gray-400">Genre: {selectedBook.genre}</p>
              <div className="mt-2">
                <Badge variant={selectedBook.status}>{selectedBook.status}</Badge>
              </div>
            </div>
          )}

          <Button onClick={handleBorrow} disabled={!selectedBookId}>
            Confirm Borrow
          </Button>
        </div>
      </Card>
    </div>
  );
}
