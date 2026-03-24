import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  BookOpen, Search, X, Share2, ArrowRight
} from 'lucide-react';
import { toast } from 'sonner';
import {
  customerBookService,
  type CustomerBookDto,
} from '@/services/customerBookService';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';

// ─── Types ────────────────────────────────────────────────────────────────────

type AvailabilityFilter = 'all' | 'true' | 'false';

interface DraftFilters {
  search: string;
  isAvailable: AvailabilityFilter;
}

interface AppliedFilters {
  search?: string;
  isAvailable?: boolean;
}

const emptyDraft: DraftFilters = { search: '', isAvailable: 'all' };
const PAGE_SIZE = 12;

// ─── Helpers ──────────────────────────────────────────────────────────────────

const COVER_GRADIENTS = [
  'from-indigo-500 to-purple-600',
  'from-rose-500 to-pink-600',
  'from-amber-500 to-orange-500',
  'from-emerald-500 to-teal-600',
  'from-sky-500 to-blue-600',
  'from-violet-500 to-fuchsia-600',
  'from-lime-500 to-green-600',
  'from-red-500 to-rose-600',
];

function coverGradient(id: number) {
  return COVER_GRADIENTS[id % COVER_GRADIENTS.length];
}

// ─── Skeleton card ────────────────────────────────────────────────────────────

function SkeletonCard() {
  return (
    <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl overflow-hidden animate-pulse flex flex-col h-full">
      {/* Cover Skeleton */}
      <div className="h-48 w-full bg-gray-200 dark:bg-zinc-800 shrink-0" />

      {/* Body Skeleton */}
      <div className="p-5 flex flex-col flex-1 gap-4">
        <div className="flex items-center justify-between gap-3">
          <div className="flex items-center gap-2.5 w-full">
            <div className="w-8 h-8 rounded-full bg-gray-200 dark:bg-zinc-700 shrink-0" />
            <div className="space-y-1.5 flex-1">
              <div className="h-3 bg-gray-200 dark:bg-zinc-700 rounded w-2/3" />
              <div className="h-2.5 bg-gray-100 dark:bg-zinc-800 rounded w-1/3" />
            </div>
          </div>
          <div className="w-16 h-5 bg-gray-100 dark:bg-zinc-800 rounded-full shrink-0" />
        </div>
        <div className="space-y-2 mt-2">
          <div className="h-3 bg-gray-100 dark:bg-zinc-800 rounded w-full" />
          <div className="h-3 bg-gray-100 dark:bg-zinc-800 rounded w-5/6" />
        </div>
      </div>

      {/* Footer Skeleton */}
      <div className="px-5 py-4 border-t border-gray-100 dark:border-zinc-800/50 flex items-center justify-between">
        <div className="h-6 w-6 bg-gray-100 dark:bg-zinc-800 rounded-full" />
        <div className="h-4 w-20 bg-gray-100 dark:bg-zinc-800 rounded" />
      </div>
    </div>
  );
}

// ─── Book card ────────────────────────────────────────────────────────────────

interface BookCardProps {
  book: CustomerBookDto;
  onDetails: (id: number) => void;
}

function BookCard({ book, onDetails }: BookCardProps) {

  const handleShare = async (e: React.MouseEvent) => {
    e.stopPropagation(); // Prevents card click when just sharing
    const url = `${window.location.origin}/customer/books/${book.id}`;
    try {
      await navigator.clipboard.writeText(url);
      toast.success('Link copied to clipboard!');
    } catch {
      toast.error('Could not copy to clipboard.');
    }
  };

  return (
    <article
      onClick={() => onDetails(book.id)}
      className="group cursor-pointer bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl overflow-hidden hover:shadow-xl hover:shadow-indigo-500/10 hover:border-indigo-400 dark:hover:border-indigo-500/50 transition-all duration-300 flex flex-col h-full"
    >
      {/* ── Expressive Cover ── */}
      <div className={`relative h-48 w-full bg-gradient-to-br ${coverGradient(book.id)} p-6 flex flex-col items-center justify-center text-center overflow-hidden shrink-0`}>
        {/* Subtle overlay for depth */}
        <div className="absolute inset-0 bg-black/10 mix-blend-overlay"></div>
        <BookOpen size={32} className="text-white/80 mb-3 drop-shadow-sm shrink-0" strokeWidth={1.5} />
        <h3 className="text-xl font-bold text-white leading-snug drop-shadow-md line-clamp-3 relative z-10">
          {book.title}
        </h3>
      </div>

      {/* ── Body Content ── */}
      <div className="p-5 flex flex-col flex-1 gap-4">
        {/* Author & Badge */}
        <div className="flex items-center justify-between gap-3">
          <div className="flex items-center gap-2.5 min-w-0">
            <div className={`w-8 h-8 rounded-full bg-gradient-to-br ${coverGradient(book.authorId)} flex items-center justify-center text-white text-xs font-bold shrink-0 shadow-sm`}>
              {book.authorName.charAt(0).toUpperCase()}
            </div>
            <div className="truncate">
              <p className="text-sm font-semibold text-gray-900 dark:text-zinc-100 truncate">
                {book.authorName}
              </p>
              <p className="text-xs text-gray-500 dark:text-zinc-400 font-mono mt-0.5 truncate">
                ISBN: {book.isbn}
              </p>
            </div>
          </div>
          <Badge variant={book.isAvailable ? 'available' : 'borrowed'} className="shrink-0">
            {book.isAvailable ? 'Available' : 'Borrowed'}
          </Badge>
        </div>

        {/* Description */}
        {book.description ? (
          <p className="text-sm text-gray-600 dark:text-zinc-400 line-clamp-3 leading-relaxed flex-1 mt-1">
            {book.description}
          </p>
        ) : (
          <p className="text-sm text-gray-400 dark:text-zinc-600 italic flex-1 mt-1">
            No description available.
          </p>
        )}
      </div>

      {/* ── Action Footer ── */}
      <div className="px-5 py-3.5 border-t border-gray-100 dark:border-zinc-800/50 flex items-center justify-between bg-gray-50/50 dark:bg-zinc-800/20 group-hover:bg-indigo-50/50 dark:group-hover:bg-indigo-500/10 transition-colors">
        <button
          onClick={handleShare}
          className="p-2 -ml-2 rounded-full text-gray-400 hover:text-indigo-600 hover:bg-indigo-100 dark:hover:text-indigo-400 dark:hover:bg-indigo-900/50 transition-colors"
          aria-label="Share"
          title="Share Book"
        >
          <Share2 size={18} />
        </button>
        <div className="text-sm font-medium text-gray-500 dark:text-zinc-400 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 flex items-center gap-1 transition-colors">
          View Details
          <ArrowRight size={16} className="transform group-hover:translate-x-1 transition-transform" />
        </div>
      </div>
    </article>
  );
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export function CustomerBooksPage() {
  const navigate = useNavigate();

  // ── Data state ──────────────────────────────────────────────────────────────
  const [books, setBooks] = useState<CustomerBookDto[]>([]);
  const [hasMore, setHasMore] = useState(false);
  const [nextCursor, setNextCursor] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isLoadingMore, setIsLoadingMore] = useState(false);

  // ── Filter state ────────────────────────────────────────────────────────────
  const [draft, setDraft] = useState<DraftFilters>(emptyDraft);
  const [applied, setApplied] = useState<AppliedFilters>({});

  // ── Fetch ────────────────────────────────────────────────────────────────────
  const fetchBooks = useCallback(async (filters: AppliedFilters) => {
    setIsLoading(true);
    try {
      const res = await customerBookService.getBooks({
        search: filters.search,
        isAvailable: filters.isAvailable,
        pageSize: PAGE_SIZE,
      });
      setBooks(res.data.items);
      setHasMore(res.data.hasMore);
      setNextCursor(res.data.nextCursor);
    } catch {
      // nothing to do

    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchBooks(applied);
  }, [applied, fetchBooks]);

  const handleLoadMore = async () => {
    if (!hasMore || nextCursor === null || isLoadingMore) return;
    setIsLoadingMore(true);
    try {
      const res = await customerBookService.getBooks({
        search: applied.search,
        isAvailable: applied.isAvailable,
        cursor: nextCursor,
        pageSize: PAGE_SIZE,
      });
      setBooks(prev => [...prev, ...res.data.items]);
      setHasMore(res.data.hasMore);
      setNextCursor(res.data.nextCursor);
    } catch {
      // nothing to do
    } finally {
      setIsLoadingMore(false);
    }
  };

  // ── Filter handlers ──────────────────────────────────────────────────────────
  const handleApply = () => {
    setApplied({
      search: draft.search.trim() || undefined,
      isAvailable: draft.isAvailable === 'all' ? undefined : draft.isAvailable === 'true',
    });
  };

  const handleCancel = () => {
    setDraft(emptyDraft);
    setApplied({});
  };

  const hasActiveFilters = Object.values(applied).some(v => v !== undefined);

  // ─────────────────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-8">

      {/* ── Filter bar ────────────────────────────────────────────────────────── */}
      <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-4 shadow-sm">
        <div className="flex flex-col lg:flex-row gap-4 items-center">

          {/* Search */}
          <div className="relative w-full lg:flex-1 group">
            <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-500 transition-colors" />
            <input
              type="text"
              value={draft.search}
              onChange={e => setDraft(prev => ({ ...prev, search: e.target.value }))}
              onKeyDown={e => e.key === 'Enter' && handleApply()}
              placeholder="Search by title or ISBN..."
              className="w-full pl-10 pr-4 py-2.5 bg-gray-50 dark:bg-zinc-800/50 border border-transparent hover:border-gray-200 dark:hover:border-zinc-700 rounded-xl text-sm focus:bg-white dark:focus:bg-zinc-900 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
            />
          </div>

          <div className="flex w-full lg:w-auto items-center gap-3 justify-between lg:justify-end">
            {/* Availability toggle */}
            <div className="inline-flex p-1 bg-gray-100 dark:bg-zinc-800 rounded-xl">
              {(
                [
                  { value: 'all', label: 'All' },
                  { value: 'true', label: 'Available' },
                  { value: 'false', label: 'Borrowed' },
                ] as { value: AvailabilityFilter; label: string }[]
              ).map(opt => {
                const isActive = draft.isAvailable === opt.value;
                return (
                  <button
                    key={opt.value}
                    onClick={() => setDraft(prev => ({ ...prev, isAvailable: opt.value }))}
                    className={`px-4 py-1.5 text-sm font-medium rounded-lg transition-all duration-200 ${isActive
                      ? 'bg-white dark:bg-zinc-700 text-indigo-600 dark:text-indigo-400 shadow-sm'
                      : 'text-gray-500 hover:text-gray-700 dark:hover:text-gray-300'
                      }`}
                  >
                    {opt.label}
                  </button>
                );
              })}
            </div>

            {/* Buttons */}
            <div className="flex items-center gap-2">
              <Button
                onClick={handleApply}
                disabled={isLoading}
                className="h-[38px] px-6 rounded-xl shadow-md shadow-indigo-500/20 active:scale-95 transition-transform"
              >
                Search
              </Button>
              {hasActiveFilters && (
                <Button
                  variant="secondary"
                  onClick={handleCancel}
                  disabled={isLoading}
                  className="h-[38px] px-3 rounded-xl active:scale-95 transition-transform"
                  title="Clear filters"
                >
                  <X size={16} />
                </Button>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* ── Grid ──────────────────────────────────────────────────────────────── */}
      <div>
        {!isLoading && (
          <div className="flex items-center justify-between mb-5">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-zinc-100">Library Collection</h2>
            <p className="text-sm text-gray-500 dark:text-zinc-400">
              {books.length === 0 ? 'No results' : `${books.length} book${books.length !== 1 ? 's' : ''}`}
            </p>
          </div>
        )}

        {/* Note: I adjusted the grid-cols slightly to account for the taller, more expressive cards */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-3 gap-6">
          {isLoading
            ? Array.from({ length: PAGE_SIZE }).map((_, i) => <SkeletonCard key={i} />)
            : books.map(book => (
              <BookCard key={book.id} book={book} onDetails={id => navigate(`/customer/books/${id}`)} />
            ))
          }
        </div>

        {/* Empty state */}
        {!isLoading && books.length === 0 && (
          <div className="flex flex-col items-center justify-center text-center gap-4 py-20 px-4 bg-gray-50 dark:bg-zinc-900/50 border border-dashed border-gray-200 dark:border-zinc-800 rounded-2xl mt-4">
            <div className="w-16 h-16 bg-gray-100 dark:bg-zinc-800 rounded-full flex items-center justify-center text-gray-400">
              <Search size={28} />
            </div>
            <div>
              <p className="text-base font-semibold text-gray-900 dark:text-zinc-100">No books found</p>
              <p className="text-sm text-gray-500 dark:text-zinc-400 mt-1 max-w-sm">
                {hasActiveFilters
                  ? "We couldn't find any books matching your filters. Try adjusting them."
                  : 'The library currently has no books available.'}
              </p>
            </div>
            {hasActiveFilters && (
              <Button variant="secondary" onClick={handleCancel} className="mt-2">
                Clear all filters
              </Button>
            )}
          </div>
        )}

        {/* Load more */}
        {hasMore && !isLoading && (
          <div className="flex justify-center mt-12">
            <Button
              variant="secondary"
              onClick={handleLoadMore}
              disabled={isLoadingMore}
              className="px-8 rounded-full"
            >
              {isLoadingMore ? 'Loading...' : 'Load more books'}
            </Button>
          </div>
        )}

        {/* Load more skeletons */}
        {isLoadingMore && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mt-6">
            {Array.from({ length: 4 }).map((_, i) => <SkeletonCard key={i} />)}
          </div>
        )}
      </div>

    </div>
  );
}