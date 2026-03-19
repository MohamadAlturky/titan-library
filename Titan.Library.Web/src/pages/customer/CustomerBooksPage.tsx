import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { BookOpen } from 'lucide-react';
import { mockBooks } from '@/data/mockBooks';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';

export function CustomerBooksPage() {
  const navigate = useNavigate();
  const [search, setSearch] = useState('');

  const filtered = mockBooks.filter(b =>
    b.title.toLowerCase().includes(search.toLowerCase()) ||
    b.authorName.toLowerCase().includes(search.toLowerCase()) ||
    b.genre.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div>
      <PageHeader title="Browse Books" description="Find and borrow books from our collection" />
      <div className="mb-4">
        <input
          value={search}
          onChange={e => setSearch(e.target.value)}
          placeholder="Search by title, author, or genre..."
          className="w-full max-w-sm px-3 py-2 text-sm rounded border border-gray-200 dark:border-zinc-800 bg-white dark:bg-zinc-800 text-gray-900 dark:text-zinc-100 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
        />
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {filtered.map(book => (
          <div
            key={book.id}
            className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg overflow-hidden hover:shadow-md transition-shadow"
          >
            <div className="bg-indigo-50 dark:bg-indigo-950/30 p-6 flex items-center justify-center border-b border-gray-200 dark:border-zinc-800">
              <BookOpen className="text-indigo-500 dark:text-indigo-400" size={48} />
            </div>
            <div className="p-4 space-y-2">
              <h3 className="font-semibold text-gray-900 dark:text-zinc-100 text-sm leading-tight line-clamp-2">
                {book.title}
              </h3>
              <p className="text-xs text-gray-500 dark:text-zinc-400">{book.authorName}</p>
              <div className="flex items-center gap-2 flex-wrap">
                <span className="text-xs bg-gray-100 dark:bg-zinc-800 text-gray-600 dark:text-zinc-400 px-2 py-0.5 rounded">
                  {book.genre}
                </span>
                <span className="text-xs text-gray-400 dark:text-zinc-500">{book.publishedYear}</span>
              </div>
              <div className="flex items-center justify-between pt-1">
                <Badge variant={book.status}>{book.status.charAt(0).toUpperCase() + book.status.slice(1)}</Badge>
                <span className="text-xs text-gray-500 dark:text-zinc-400">{book.availableCopies} avail.</span>
              </div>
              <Button
                variant="primary"
                size="sm"
                className="w-full mt-2"
                disabled={book.availableCopies === 0}
                onClick={() => navigate('/customer/borrow')}
              >
                Borrow
              </Button>
            </div>
          </div>
        ))}
        {filtered.length === 0 && (
          <div className="col-span-full text-center py-12 text-gray-500 dark:text-zinc-400">
            No books found matching your search.
          </div>
        )}
      </div>
    </div>
  );
}
