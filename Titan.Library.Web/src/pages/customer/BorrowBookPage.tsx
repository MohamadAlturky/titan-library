import { useState } from 'react';
import { toast } from 'sonner';
import { BookOpen } from 'lucide-react';
import { mockBooks } from '@/data/mockBooks';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
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
      <PageHeader title="Borrow a Book" description="Select a book to borrow from our collection" />
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Left: Book Selector */}
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-6 space-y-4">
          <h2 className="text-base font-semibold text-gray-900 dark:text-zinc-100">Select a Book</h2>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-zinc-300 mb-1">
              Available Books
            </label>
            <select
              value={selectedBookId}
              onChange={e => setSelectedBookId(e.target.value)}
              className="w-full px-3 py-2 rounded border border-gray-200 dark:border-zinc-800 bg-white dark:bg-zinc-800 text-gray-900 dark:text-zinc-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">-- Select a book --</option>
              {availableBooks.map(b => (
                <option key={b.id} value={b.id}>
                  {b.title} by {b.authorName} ({b.availableCopies} available)
                </option>
              ))}
            </select>
          </div>
          <Button onClick={handleBorrow} disabled={!selectedBookId} className="w-full">
            Confirm Borrow
          </Button>
        </div>

        {/* Right: Book Preview */}
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg overflow-hidden">
          {selectedBook ? (
            <>
              <div className="bg-indigo-50 dark:bg-indigo-950/30 p-10 flex items-center justify-center border-b border-gray-200 dark:border-zinc-800">
                <BookOpen className="text-indigo-500 dark:text-indigo-400" size={64} />
              </div>
              <div className="p-6 space-y-3">
                <h3 className="text-lg font-bold text-gray-900 dark:text-zinc-100">{selectedBook.title}</h3>
                <p className="text-sm text-gray-500 dark:text-zinc-400">by {selectedBook.authorName}</p>
                <div className="grid grid-cols-2 gap-3 text-sm pt-2">
                  <div>
                    <span className="text-gray-400 dark:text-zinc-500 text-xs uppercase tracking-wide">Genre</span>
                    <p className="font-medium text-gray-900 dark:text-zinc-100 mt-0.5">{selectedBook.genre}</p>
                  </div>
                  <div>
                    <span className="text-gray-400 dark:text-zinc-500 text-xs uppercase tracking-wide">Year</span>
                    <p className="font-medium text-gray-900 dark:text-zinc-100 mt-0.5">{selectedBook.publishedYear}</p>
                  </div>
                  <div>
                    <span className="text-gray-400 dark:text-zinc-500 text-xs uppercase tracking-wide">Available Copies</span>
                    <p className="font-medium text-gray-900 dark:text-zinc-100 mt-0.5">{selectedBook.availableCopies}</p>
                  </div>
                  <div>
                    <span className="text-gray-400 dark:text-zinc-500 text-xs uppercase tracking-wide">Status</span>
                    <div className="mt-1">
                      <Badge variant={selectedBook.status}>{selectedBook.status.charAt(0).toUpperCase() + selectedBook.status.slice(1)}</Badge>
                    </div>
                  </div>
                </div>
              </div>
            </>
          ) : (
            <div className="h-full min-h-48 flex flex-col items-center justify-center p-10 text-center">
              <BookOpen className="text-gray-300 dark:text-zinc-700 mb-3" size={64} />
              <p className="text-gray-400 dark:text-zinc-500 text-sm">Select a book to see its details</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
