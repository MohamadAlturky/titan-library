import { useCallback, useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import {
  ChevronUp, ChevronDown, ChevronsUpDown,
  ChevronLeft, ChevronRight,
  BookOpen, ArrowLeft,
} from 'lucide-react';
import { toast } from 'sonner';
import {
  adminService,
  type AdminBorrowDto,
  type GetAdminBorrowsParams,
} from '@/services/adminService';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

type BorrowSortKey = NonNullable<GetAdminBorrowsParams['sortBy']>;

const PAGE_SIZE = 10;

const BORROW_HEADERS: { key: BorrowSortKey; label: string }[] = [
  { key: 'id', label: '#' },
  { key: 'customerName', label: 'Customer Name' },
  { key: 'createdAt', label: 'Borrowed On' },
  { key: 'returnedAt', label: 'Returned On' },
  { key: 'isReturned', label: 'Status' },
];

const BORROW_COLS = BORROW_HEADERS.length;

export function AdminBookBorrowsPage() {
  const { bookId } = useParams<{ bookId: string }>();
  const location = useLocation();
  const navigate = useNavigate();

  const bookTitle: string = (location.state as { bookTitle?: string } | null)?.bookTitle ?? `Book #${bookId}`;

  const [borrows, setBorrows] = useState<AdminBorrowDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  const [page, setPage] = useState(1);
  const [sortKey, setSortKey] = useState<BorrowSortKey>('createdAt');
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('desc');

  const fetchBorrows = useCallback(
    async (currentPage: number, currentSortKey: BorrowSortKey, currentSortDir: 'asc' | 'desc') => {
      if (!bookId) return;
      setIsLoading(true);
      try {
        const res = await adminService.getBookBorrows(Number(bookId), {
          sortBy: currentSortKey,
          sortDirection: currentSortDir,
          page: currentPage,
          pageSize: PAGE_SIZE,
        });
        setBorrows(res.data.items);
        setTotalCount(res.data.totalCount);
        setTotalPages(res.data.totalPages);
      } catch {
        toast.error('Failed to load borrow history.');
      } finally {
        setIsLoading(false);
      }
    },
    [bookId],
  );

  useEffect(() => {
    fetchBorrows(page, sortKey, sortDir);
  }, [page, sortKey, sortDir, fetchBorrows]);

  const handleSort = (key: BorrowSortKey) => {
    if (sortKey === key) {
      setSortDir(d => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortKey(key);
      setSortDir('asc');
    }
    setPage(1);
  };

  const SortIcon = ({ k }: { k: BorrowSortKey }) => {
    if (sortKey !== k) return <ChevronsUpDown size={13} />;
    return sortDir === 'asc' ? <ChevronUp size={13} /> : <ChevronDown size={13} />;
  };

  const pageNumbers = (() => {
    const pages: (number | '...')[] = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      if (page > 4) pages.push('...');
      for (let i = Math.max(2, page - 1); i <= Math.min(totalPages - 1, page + 1); i++) pages.push(i);
      if (page < totalPages - 3) pages.push('...');
      pages.push(totalPages);
    }
    return pages;
  })();

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <button
          onClick={() => navigate('/admin/books')}
          className="p-1.5 rounded-md text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 dark:hover:text-indigo-400 dark:hover:bg-indigo-900/20 transition-colors"
          title="Back to books"
        >
          <ArrowLeft size={18} />
        </button>
        <PageHeader
          title={`Borrow History`}
          description={bookTitle}
        />
      </div>

      <Card>
        <div className="p-4 space-y-4">
          <p className="text-sm text-gray-500 dark:text-zinc-400">
            {isLoading ? 'Loading…' : `${totalCount} borrow record${totalCount !== 1 ? 's' : ''} found`}
          </p>

          <div className="overflow-x-auto border border-gray-200 dark:border-zinc-700 rounded-lg">
            <table className="w-full text-sm border-collapse">
              <thead className="bg-gray-100 dark:bg-zinc-800">
                <tr>
                  {BORROW_HEADERS.map(col => (
                    <th
                      key={col.key}
                      onClick={() => handleSort(col.key)}
                      className="px-4 py-3 text-left text-xs font-semibold text-gray-600 dark:text-zinc-300 uppercase tracking-wider border-b border-gray-200 dark:border-zinc-700 cursor-pointer select-none hover:bg-gray-200 dark:hover:bg-zinc-700"
                    >
                      <div className="flex items-center gap-1">
                        {col.label}
                        <span className="text-gray-400 dark:text-zinc-500">
                          <SortIcon k={col.key} />
                        </span>
                      </div>
                    </th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {isLoading ? (
                  Array.from({ length: PAGE_SIZE }).map((_, i) => (
                    <tr key={i} className={i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'}>
                      {Array.from({ length: BORROW_COLS }).map((_, j) => (
                        <td key={j} className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                          <div className="h-4 bg-gray-200 dark:bg-zinc-700 rounded animate-pulse" />
                        </td>
                      ))}
                    </tr>
                  ))
                ) : borrows.length === 0 ? (
                  <tr>
                    <td colSpan={BORROW_COLS} className="px-4 py-16 text-center">
                      <div className="flex flex-col items-center gap-3 text-gray-400 dark:text-zinc-500">
                        <BookOpen size={40} strokeWidth={1.2} />
                        <p className="text-sm font-medium text-gray-600 dark:text-zinc-300">No borrow records found</p>
                        <p className="text-xs">This book has never been borrowed.</p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  borrows.map((borrow, i) => (
                    <tr
                      key={borrow.id}
                      className={`transition-colors hover:bg-indigo-50 dark:hover:bg-zinc-700/40 ${i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'}`}
                    >
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <span className="font-mono text-xs text-gray-500 dark:text-zinc-400">#{borrow.id}</span>
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 font-medium text-gray-900 dark:text-zinc-100">
                        {borrow.customerName}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 text-gray-700 dark:text-zinc-300">
                        {new Date(borrow.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 text-gray-700 dark:text-zinc-300">
                        {borrow.returnedAt
                          ? new Date(borrow.returnedAt).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
                          : <span className="text-gray-400 dark:text-zinc-500">—</span>}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <Badge variant={borrow.isReturned ? 'returned' : 'borrowed'}>
                          {borrow.isReturned ? 'Returned' : 'Active'}
                        </Badge>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {totalPages > 1 && (
            <div className="flex items-center justify-between text-sm text-gray-600 dark:text-zinc-400">
              <span>Page {page} of {totalPages}</span>
              <div className="flex items-center gap-1">
                <button
                  onClick={() => setPage(p => Math.max(1, p - 1))}
                  disabled={page === 1 || isLoading}
                  className="p-1.5 rounded hover:bg-gray-100 dark:hover:bg-zinc-800 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronLeft size={16} />
                </button>
                {pageNumbers.map((n, i) =>
                  n === '...' ? (
                    <span key={`e-${i}`} className="px-2 text-gray-400">…</span>
                  ) : (
                    <button
                      key={n}
                      onClick={() => setPage(n as number)}
                      disabled={isLoading}
                      className={`min-w-[32px] px-2 py-1 rounded text-sm transition-colors ${page === n ? 'bg-indigo-600 text-white font-medium' : 'hover:bg-gray-100 dark:hover:bg-zinc-800'}`}
                    >
                      {n}
                    </button>
                  ),
                )}
                <button
                  onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages || isLoading}
                  className="p-1.5 rounded hover:bg-gray-100 dark:hover:bg-zinc-800 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronRight size={16} />
                </button>
              </div>
            </div>
          )}
        </div>
      </Card>
    </div>
  );
}
