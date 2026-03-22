import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  BookOpen, Search, X,
  Heart, Share2, Info,
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

// Seeded fake like count so it looks consistent between renders
function seedLikes(id: number) {
  return ((id * 31 + 17) % 120) + 4;
}


// ─── Skeleton card ────────────────────────────────────────────────────────────

function SkeletonCard() {
  return (
    <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl overflow-hidden animate-pulse">
      {/* Header */}
      <div className="flex items-center gap-2.5 px-4 pt-4 pb-3">
        <div className="w-8 h-8 rounded-full bg-gray-200 dark:bg-zinc-700" />
        <div className="flex-1 space-y-1.5">
          <div className="h-3 bg-gray-200 dark:bg-zinc-700 rounded w-1/2" />
          <div className="h-2.5 bg-gray-100 dark:bg-zinc-800 rounded w-1/3" />
        </div>
        <div className="w-16 h-5 bg-gray-100 dark:bg-zinc-800 rounded-full" />
      </div>
      {/* Cover */}
      <div className="h-52 bg-gray-200 dark:bg-zinc-800 mx-4 rounded-xl" />
      {/* Actions */}
      <div className="px-4 py-3 flex items-center gap-4">
        <div className="h-5 w-12 bg-gray-100 dark:bg-zinc-800 rounded" />
        <div className="h-5 w-8 bg-gray-100 dark:bg-zinc-800 rounded" />
        <div className="ml-auto h-5 w-16 bg-gray-100 dark:bg-zinc-800 rounded" />
      </div>
      {/* Caption */}
      <div className="px-4 pb-4 space-y-1.5">
        <div className="h-3.5 bg-gray-200 dark:bg-zinc-700 rounded w-3/4" />
        <div className="h-3 bg-gray-100 dark:bg-zinc-800 rounded w-1/2" />
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
  const [liked, setLiked] = useState(false);
  const [likeCount, setLikeCount] = useState(seedLikes(book.id));

  const handleLike = () => {
    setLiked(v => !v);
    setLikeCount(c => liked ? c - 1 : c + 1);
  };

  const handleShare = async () => {
    const url = `${window.location.origin}/customer/books/${book.id}`;
    try {
      await navigator.clipboard.writeText(url);
      toast.success('Link copied to clipboard!');
    } catch {
      toast.error('Could not copy to clipboard.');
    }
  };

  return (
    <article className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl overflow-hidden hover:shadow-lg hover:shadow-black/5 dark:hover:shadow-black/30 transition-all duration-200">

      {/* ── Post header ── */}
      <div className="flex items-center gap-2.5 px-4 pt-4 pb-3">
        {/* Author avatar */}
        <div className={`w-8 h-8 rounded-full bg-gradient-to-br ${coverGradient(book.authorId)} flex items-center justify-center text-white text-xs font-bold shrink-0`}>
          {book.authorName.charAt(0).toUpperCase()}
        </div>
        <div className="flex-1 min-w-0">
          <p className="text-sm font-semibold text-gray-900 dark:text-zinc-100 truncate leading-tight">
            {book.authorName}
          </p>
          <p className="text-xs text-gray-400 dark:text-zinc-500 leading-tight">Author</p>
        </div>
        <Badge variant={book.isAvailable ? 'available' : 'borrowed'}>
          {book.isAvailable ? 'Available' : 'Borrowed'}
        </Badge>
      </div>

      {/* ── Cover "photo" ── */}
      <div className={`mx-4 rounded-xl h-52 bg-gradient-to-br ${coverGradient(book.id)} flex flex-col items-center justify-center gap-3 relative overflow-hidden`}>
        {/* Decorative rings */}
        <div className="absolute inset-0 flex items-center justify-center opacity-10">
          <div className="w-64 h-64 rounded-full border-[32px] border-white" />
        </div>
        <div className="absolute inset-0 flex items-center justify-center opacity-5">
          <div className="w-96 h-96 rounded-full border-[48px] border-white" />
        </div>
        <BookOpen size={44} className="text-white/90" strokeWidth={1.5} />
        <p className="text-white/90 text-sm font-semibold text-center px-6 line-clamp-2 leading-snug drop-shadow">
          {book.title}
        </p>
      </div>

      {/* ── Action bar ── */}
      <div className="flex items-center gap-1 px-3 pt-3 pb-1">
        {/* Love */}
        <button
          onClick={handleLike}
          className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-full hover:bg-rose-50 dark:hover:bg-rose-950/30 transition-colors group"
          aria-label={liked ? 'Unlike' : 'Like'}
        >
          <Heart
            size={20}
            className={liked
              ? 'text-rose-500 fill-rose-500 scale-110 transition-transform'
              : 'text-gray-400 dark:text-zinc-500 group-hover:text-rose-400 transition-colors'
            }
          />
          <span className={`text-sm font-medium tabular-nums ${liked ? 'text-rose-500' : 'text-gray-500 dark:text-zinc-400'}`}>
            {likeCount}
          </span>
        </button>

        {/* Share */}
        <button
          onClick={handleShare}
          className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-full hover:bg-indigo-50 dark:hover:bg-indigo-950/30 transition-colors group"
          aria-label="Share"
        >
          <Share2 size={18} className="text-gray-400 dark:text-zinc-500 group-hover:text-indigo-500 transition-colors" />
          <span className="text-sm font-medium text-gray-500 dark:text-zinc-400 group-hover:text-indigo-500 transition-colors">Share</span>
        </button>

        {/* Details */}
        <button
          onClick={() => onDetails(book.id)}
          className="flex items-center gap-1.5 px-2.5 py-1.5 rounded-full hover:bg-gray-100 dark:hover:bg-zinc-800 transition-colors group ml-auto"
          aria-label="See details"
        >
          <Info size={18} className="text-gray-400 dark:text-zinc-500 group-hover:text-gray-700 dark:group-hover:text-zinc-200 transition-colors" />
          <span className="text-sm font-medium text-gray-500 dark:text-zinc-400 group-hover:text-gray-700 dark:group-hover:text-zinc-200 transition-colors">Details</span>
        </button>
      </div>

      {/* ── Caption ── */}
      <div className="px-4 pb-4 pt-1">
        <p className="text-sm text-gray-900 dark:text-zinc-100 font-semibold leading-snug line-clamp-1">
          {book.title}
        </p>
        <p className="text-xs text-gray-400 dark:text-zinc-500 font-mono mt-0.5">
          {book.isbn}
        </p>
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
      toast.error('Failed to load books.');
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
      toast.error('Failed to load more books.');
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
                    className={`px-4 py-1.5 text-sm font-medium rounded-lg transition-all duration-200 ${
                      isActive
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

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
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
          <div className="flex justify-center mt-10">
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
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5 mt-5">
            {Array.from({ length: 4 }).map((_, i) => <SkeletonCard key={i} />)}
          </div>
        )}
      </div>

    </div>
  );
}
