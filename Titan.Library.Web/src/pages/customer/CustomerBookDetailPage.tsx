import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  ArrowLeft, BookOpen, Share2,
  User, Mail, Hash, CalendarDays, BookCopy,
} from 'lucide-react';
import { toast } from 'sonner';
import { customerBookService, type CustomerBookDto } from '@/services/customerBookService';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';

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

// ─── Skeleton ─────────────────────────────────────────────────────────────────

function Skeleton() {
  return (
    <div className="animate-pulse space-y-6">
      {/* Back button placeholder */}
      <div className="h-9 w-24 bg-gray-200 dark:bg-zinc-800 rounded-full" />

      <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-3xl overflow-hidden">
        {/* Cover */}
        <div className="h-72 bg-gray-200 dark:bg-zinc-800" />
        {/* Header */}
        <div className="px-6 pt-5 pb-4 flex items-center gap-3 border-b border-gray-100 dark:border-zinc-800">
          <div className="w-10 h-10 rounded-full bg-gray-200 dark:bg-zinc-700" />
          <div className="space-y-1.5 flex-1">
            <div className="h-4 bg-gray-200 dark:bg-zinc-700 rounded w-1/3" />
            <div className="h-3 bg-gray-100 dark:bg-zinc-800 rounded w-1/4" />
          </div>
          <div className="w-20 h-6 bg-gray-100 dark:bg-zinc-800 rounded-full" />
        </div>
        {/* Actions */}
        <div className="px-6 py-4 flex gap-4 border-b border-gray-100 dark:border-zinc-800">
          <div className="h-8 w-20 bg-gray-100 dark:bg-zinc-800 rounded-full" />
          <div className="h-8 w-20 bg-gray-100 dark:bg-zinc-800 rounded-full" />
          <div className="ml-auto h-8 w-28 bg-gray-100 dark:bg-zinc-800 rounded-full" />
        </div>
        {/* Info */}
        <div className="px-6 py-6 space-y-4">
          <div className="h-6 bg-gray-200 dark:bg-zinc-700 rounded w-2/3" />
          <div className="grid grid-cols-2 gap-4">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="space-y-1.5">
                <div className="h-3 bg-gray-100 dark:bg-zinc-800 rounded w-1/3" />
                <div className="h-4 bg-gray-200 dark:bg-zinc-700 rounded w-2/3" />
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export function CustomerBookDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [book, setBook] = useState<CustomerBookDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);
  useEffect(() => {
    if (!id) return;
    customerBookService.getBookById(Number(id))
      .then(res => {
        setBook(res.data);
      })
      .catch(() => setNotFound(true))
      .finally(() => setIsLoading(false));
  }, [id]);

  const handleShare = async () => {
    try {
      await navigator.clipboard.writeText(window.location.href);
      toast.success('Link copied to clipboard!');
    } catch {
      toast.error('Could not copy to clipboard.');
    }
  };

  if (isLoading) return <Skeleton />;

  if (notFound || !book) {
    return (
      <div className="flex flex-col items-center justify-center gap-4 py-24 text-center">
        <div className="w-20 h-20 rounded-full bg-gray-100 dark:bg-zinc-800 flex items-center justify-center">
          <BookOpen size={36} className="text-gray-400" strokeWidth={1.5} />
        </div>
        <p className="text-lg font-semibold text-gray-900 dark:text-zinc-100">Book not found</p>
        <p className="text-sm text-gray-500 dark:text-zinc-400">This book doesn't exist or has been removed.</p>
        <Button variant="secondary" onClick={() => navigate('/customer/books')} className="mt-2">
          <ArrowLeft size={16} />
          Back to Browse
        </Button>
      </div>
    );
  }

  const gradient = coverGradient(book.id);
  const authorGradient = coverGradient(book.authorId);
  const addedDate = new Date(book.createdAt).toLocaleDateString('en-US', {
    year: 'numeric', month: 'long', day: 'numeric',
  });

  return (
    <div className="space-y-5 max-w-2xl mx-auto">

      {/* Back */}
      <button
        onClick={() => navigate('/customer/books')}
        className="flex items-center gap-1.5 text-sm font-medium text-gray-500 dark:text-zinc-400 hover:text-gray-900 dark:hover:text-zinc-100 transition-colors px-1"
      >
        <ArrowLeft size={16} />
        Browse
      </button>

      {/* ── Post card ─────────────────────────────────────────────────────────── */}
      <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-3xl overflow-hidden shadow-sm">

        {/* Cover "photo" */}
        <div className={`w-full h-72 bg-gradient-to-br ${gradient} flex flex-col items-center justify-center gap-3 relative overflow-hidden px-10`}>
          {/* Decorative rings */}
          <div className="absolute inset-0 flex items-center justify-center opacity-10 pointer-events-none">
            <div className="w-80 h-80 rounded-full border-[40px] border-white" />
          </div>
          <div className="absolute inset-0 flex items-center justify-center opacity-5 pointer-events-none">
            <div className="w-[500px] h-[500px] rounded-full border-[60px] border-white" />
          </div>
          <BookOpen size={52} className="text-white/90 shrink-0" strokeWidth={1.2} />
          <h1 className="text-white text-xl font-bold text-center leading-snug drop-shadow-sm line-clamp-2 w-full">
            {book.title}
          </h1>
          {book.description && (
            <p className="text-white/75 text-sm text-center line-clamp-3 leading-relaxed w-full">
              {book.description}
            </p>
          )}
        </div>

        {/* Post header */}
        <div className="flex items-center gap-3 px-6 pt-5 pb-4 border-b border-gray-100 dark:border-zinc-800">
          <div className={`w-10 h-10 rounded-full bg-gradient-to-br ${authorGradient} flex items-center justify-center text-white font-bold text-sm shrink-0`}>
            {book.authorName.charAt(0).toUpperCase()}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-semibold text-gray-900 dark:text-zinc-100 truncate">{book.authorName}</p>
            <p className="text-xs text-gray-400 dark:text-zinc-500">Author</p>
          </div>
          <Badge variant={book.isAvailable ? 'available' : 'borrowed'} className="shrink-0">
            {book.isAvailable ? 'Available' : 'Borrowed'}
          </Badge>
        </div>

        {/* Action bar */}
        <div className="flex items-center gap-1 px-4 py-3 border-b border-gray-100 dark:border-zinc-800">


          {/* Share */}
          <button
            onClick={handleShare}
            className="flex items-center gap-1.5 px-3 py-1.5 rounded-full hover:bg-indigo-50 dark:hover:bg-indigo-950/30 transition-colors group"
          >
            <Share2 size={18} className="text-gray-400 dark:text-zinc-500 group-hover:text-indigo-500 transition-colors" />
            <span className="text-sm font-medium text-gray-500 dark:text-zinc-400 group-hover:text-indigo-500 transition-colors">Share</span>
          </button>

          {/* Borrow CTA */}
          <div className="ml-auto">
            <Button
              onClick={() => navigate('/customer/borrow')}
              disabled={!book.isAvailable}
              className="rounded-full px-5"
              size="sm"
            >
              <BookCopy size={15} />
              {book.isAvailable ? 'Borrow this book' : 'Not available'}
            </Button>
          </div>
        </div>

        {/* Details */}
        <div className="px-6 py-6 space-y-5">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-gray-400 dark:text-zinc-500 mb-1">Title</p>
            <p className="text-xl font-bold text-gray-900 dark:text-zinc-100 leading-snug">{book.title}</p>
          </div>

          {book.description && (
            <div>
              <p className="text-xs font-semibold uppercase tracking-wider text-gray-400 dark:text-zinc-500 mb-1">Description</p>
              <p className="text-sm text-gray-700 dark:text-zinc-300 leading-relaxed">{book.description}</p>
            </div>
          )}

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-5">
            <div className="flex items-start gap-3">
              <div className="p-2 bg-indigo-50 dark:bg-indigo-950/30 rounded-lg shrink-0">
                <User size={16} className="text-indigo-500" />
              </div>
              <div>
                <p className="text-xs text-gray-400 dark:text-zinc-500 mb-0.5">Author</p>
                <p className="text-sm font-semibold text-gray-800 dark:text-zinc-200">{book.authorName}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <div className="p-2 bg-indigo-50 dark:bg-indigo-950/30 rounded-lg shrink-0">
                <Mail size={16} className="text-indigo-500" />
              </div>
              <div>
                <p className="text-xs text-gray-400 dark:text-zinc-500 mb-0.5">Contact</p>
                <p className="text-sm font-semibold text-gray-800 dark:text-zinc-200 break-all">{book.authorEmail}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <div className="p-2 bg-indigo-50 dark:bg-indigo-950/30 rounded-lg shrink-0">
                <Hash size={16} className="text-indigo-500" />
              </div>
              <div>
                <p className="text-xs text-gray-400 dark:text-zinc-500 mb-0.5">ISBN</p>
                <p className="text-sm font-mono font-semibold text-gray-800 dark:text-zinc-200">{book.isbn}</p>
              </div>
            </div>

            <div className="flex items-start gap-3">
              <div className="p-2 bg-indigo-50 dark:bg-indigo-950/30 rounded-lg shrink-0">
                <CalendarDays size={16} className="text-indigo-500" />
              </div>
              <div>
                <p className="text-xs text-gray-400 dark:text-zinc-500 mb-0.5">Added to library</p>
                <p className="text-sm font-semibold text-gray-800 dark:text-zinc-200">{addedDate}</p>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}
