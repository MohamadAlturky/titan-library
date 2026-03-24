import { useEffect, useMemo, useState } from 'react';
import {
  ChevronUp, ChevronDown, ChevronsUpDown,
  ChevronLeft, ChevronRight,
  BookOpen, BookMarked, CheckCircle,
  Search, X, Filter, Undo2,
} from 'lucide-react';
import { toast } from 'sonner';
import { customerBookService, type CustomerBorrowDto } from '@/services/customerBookService';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { Card } from '@/components/ui/Card';
import { PageHeader } from '@/components/layout/PageHeader';

// ─── Types ────────────────────────────────────────────────────────────────────

type StatusFilter = 'all' | 'active' | 'returned';
type SortKey = keyof Pick<CustomerBorrowDto, 'id' | 'bookTitle' | 'authorName' | 'createdAt' | 'returnedAt' | 'isReturned'>;

interface DraftFilters {
  search: string;
  status: StatusFilter;
}

interface AppliedFilters {
  search?: string;
  status?: 'active' | 'returned';
}

const emptyDraft: DraftFilters = { search: '', status: 'all' };
const PAGE_SIZE = 10;
const COLS = 7;

// ─── Helpers ──────────────────────────────────────────────────────────────────

function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric',
  });
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export function CustomerMyBorrowsPage() {
  const [borrows, setBorrows] = useState<CustomerBorrowDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  // ── Filter state ─────────────────────────────────────────────────────────────
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [draft, setDraft] = useState<DraftFilters>(emptyDraft);
  const [applied, setApplied] = useState<AppliedFilters>({});

  // ── Sort state ───────────────────────────────────────────────────────────────
  const [sortKey, setSortKey] = useState<SortKey | null>(null);
  const [sortDir, setSortDir] = useState<'asc' | 'desc'>('asc');

  // ── Pagination ───────────────────────────────────────────────────────────────
  const [page, setPage] = useState(1);

  // ── Return state ─────────────────────────────────────────────────────────────
  const [returningBookId, setReturningBookId] = useState<number | null>(null);

  // ── Fetch ────────────────────────────────────────────────────────────────────
  useEffect(() => {
    // setIsLoading(true);
    customerBookService.getBorrowsByCustomer()
      .then(res => setBorrows(res.data))
      .finally(() => setIsLoading(false));
  }, []);

  // ── Return handler ───────────────────────────────────────────────────────────
  const handleReturn = async (bookId: number) => {
    setReturningBookId(bookId);
    try {
      await customerBookService.returnBook(bookId);
      toast.success('Book returned successfully!');
      setBorrows(prev =>
        prev.map(b =>
          b.bookId === bookId
            ? { ...b, isReturned: true, returnedAt: new Date().toISOString() }
            : b,
        ),
      );
    } catch {
      // nothing to do
    } finally {
      setReturningBookId(null);
    }
  };

  // ── Stats ─────────────────────────────────────────────────────────────────────
  const totalBorrowed = borrows.length;
  const activeBorrows = borrows.filter(b => !b.isReturned).length;
  const returnedCount = borrows.filter(b => b.isReturned).length;

  // ── Filter + sort + paginate ──────────────────────────────────────────────────
  const filtered = useMemo(() => {
    let result = [...borrows];

    if (applied.search) {
      const q = applied.search.toLowerCase();
      result = result.filter(b =>
        b.bookTitle.toLowerCase().includes(q) ||
        b.authorName.toLowerCase().includes(q),
      );
    }

    if (applied.status === 'active') result = result.filter(b => !b.isReturned);
    if (applied.status === 'returned') result = result.filter(b => b.isReturned);

    if (sortKey) {
      result.sort((a, b) => {
        const av = a[sortKey];
        const bv = b[sortKey];
        if (av == null && bv == null) return 0;
        if (av == null) return sortDir === 'asc' ? -1 : 1;
        if (bv == null) return sortDir === 'asc' ? 1 : -1;
        if (typeof av === 'string' && typeof bv === 'string') {
          return sortDir === 'asc' ? av.localeCompare(bv) : bv.localeCompare(av);
        }
        return sortDir === 'asc' ? (av > bv ? 1 : -1) : (av < bv ? 1 : -1);
      });
    }

    return result;
  }, [borrows, applied, sortKey, sortDir]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const paginated = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  // ── Sort handler ─────────────────────────────────────────────────────────────
  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortDir(d => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortKey(key);
      setSortDir('asc');
    }
    setPage(1);
  };

  const SortIcon = ({ k }: { k: SortKey }) => {
    if (sortKey !== k) return <ChevronsUpDown size={13} />;
    return sortDir === 'asc' ? <ChevronUp size={13} /> : <ChevronDown size={13} />;
  };

  // ── Filter handlers ───────────────────────────────────────────────────────────
  const handleApply = () => {
    setApplied({
      search: draft.search.trim() || undefined,
      status: draft.status === 'all' ? undefined : draft.status,
    });
    setPage(1);
  };

  const handleCancel = () => {
    setDraft(emptyDraft);
    setApplied({});
    setPage(1);
  };

  const hasActiveFilters = Object.values(applied).some(v => v !== undefined);

  // ── Page numbers ──────────────────────────────────────────────────────────────
  const pageNumbers = (() => {
    const pages: (number | '...')[] = [];
    if (totalPages <= 7) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      if (page > 4) pages.push('...');
      for (let i = Math.max(2, page - 1); i <= Math.min(totalPages - 1, page + 1); i++) {
        pages.push(i);
      }
      if (page < totalPages - 3) pages.push('...');
      pages.push(totalPages);
    }
    return pages;
  })();

  // ─────────────────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      <PageHeader title="My Library" description="Track your reading history and active loans" />

      {/* ── Stats ────────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-4 flex items-center gap-4 shadow-sm">
          <div className="p-3 bg-indigo-50 dark:bg-indigo-950/30 rounded-xl">
            <BookOpen className="text-indigo-500" size={22} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{totalBorrowed}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Total Borrowed</p>
          </div>
        </div>
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-4 flex items-center gap-4 shadow-sm">
          <div className="p-3 bg-amber-50 dark:bg-amber-950/30 rounded-xl">
            <BookMarked className="text-amber-500" size={22} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{activeBorrows}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Active</p>
          </div>
        </div>
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-4 flex items-center gap-4 shadow-sm">
          <div className="p-3 bg-green-50 dark:bg-green-950/30 rounded-xl">
            <CheckCircle className="text-green-500" size={22} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{returnedCount}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Returned</p>
          </div>
        </div>
      </div>

      {/* ── Filters card ─────────────────────────────────────────────────────── */}
      <Card>
        <button
          onClick={() => setFiltersOpen(v => !v)}
          className="w-full flex items-center justify-between px-4 py-3 text-sm font-semibold text-gray-700 dark:text-zinc-200 hover:bg-gray-50 dark:hover:bg-zinc-800/60 rounded-t-xl transition-colors"
        >
          <div className="flex items-center gap-2">
            <Filter size={15} className="text-indigo-500" />
            Filters
            {hasActiveFilters && (
              <span className="inline-flex items-center justify-center w-5 h-5 rounded-full bg-indigo-100 dark:bg-indigo-900/40 text-indigo-600 dark:text-indigo-400 text-xs font-bold">
                {Object.values(applied).filter(v => v !== undefined).length}
              </span>
            )}
          </div>
          {filtersOpen
            ? <ChevronUp size={16} className="text-gray-400" />
            : <ChevronDown size={16} className="text-gray-400" />}
        </button>

        {filtersOpen && (
          <div className="px-6 py-5 border-t border-gray-100 dark:border-zinc-700/60 bg-gray-50/50 dark:bg-zinc-800/30">
            <div className="flex flex-col md:flex-row md:items-end gap-6">

              {/* Search */}
              <div className="flex-1 space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400">
                  Search
                </label>
                <div className="relative group">
                  <Search size={18} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-500 transition-colors" />
                  <input
                    type="text"
                    value={draft.search}
                    onChange={e => setDraft(prev => ({ ...prev, search: e.target.value }))}
                    onKeyDown={e => e.key === 'Enter' && handleApply()}
                    placeholder="Book title or author..."
                    className="w-full pl-10 pr-4 py-2.5 bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm shadow-sm focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  />
                </div>
              </div>

              {/* Status toggle */}
              <div className="space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400">
                  Status
                </label>
                <div className="inline-flex p-1 bg-gray-200/50 dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl">
                  {(
                    [
                      { value: 'all', label: 'All' },
                      { value: 'active', label: 'Active' },
                      { value: 'returned', label: 'Returned' },
                    ] as { value: StatusFilter; label: string }[]
                  ).map(opt => {
                    const isActive = draft.status === opt.value;
                    return (
                      <button
                        key={opt.value}
                        onClick={() => setDraft(prev => ({ ...prev, status: opt.value }))}
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
              </div>

              {/* Action buttons */}
              <div className="flex items-center gap-2">
                <Button
                  onClick={handleApply}
                  disabled={isLoading}
                  className="h-[42px] px-6 rounded-xl shadow-md shadow-indigo-500/20 active:scale-95 transition-transform"
                >
                  <Search size={16} className="mr-2" />
                  Apply
                </Button>
                <Button
                  variant="secondary"
                  onClick={handleCancel}
                  disabled={isLoading}
                  className="h-[42px] px-4 rounded-xl active:scale-95 transition-transform"
                >
                  <X size={16} />
                </Button>
              </div>

            </div>
          </div>
        )}
      </Card>

      {/* ── Table card ───────────────────────────────────────────────────────── */}
      <Card>
        <div className="p-4 space-y-4">

          {/* Row count */}
          <p className="text-sm text-gray-500 dark:text-zinc-400">
            {isLoading ? 'Loading…' : `${filtered.length} borrow${filtered.length !== 1 ? 's' : ''} found`}
          </p>

          {/* Table */}
          <div className="overflow-x-auto border border-gray-200 dark:border-zinc-700 rounded-lg">
            <table className="w-full text-sm border-collapse">
              <thead className="bg-gray-100 dark:bg-zinc-800">
                <tr>
                  {(
                    [
                      { key: 'id' as SortKey, label: '#' },
                      { key: 'bookTitle' as SortKey, label: 'Book Title' },
                      { key: 'authorName' as SortKey, label: 'Author' },
                      { key: 'createdAt' as SortKey, label: 'Borrowed On' },
                      { key: 'returnedAt' as SortKey, label: 'Returned On' },
                      { key: 'isReturned' as SortKey, label: 'Status' },
                      { key: null, label: 'Action' },
                    ]
                  ).map(col => (
                    <th
                      key={col.label}
                      onClick={col.key ? () => handleSort(col.key!) : undefined}
                      className={`px-4 py-3 text-left text-xs font-semibold text-gray-600 dark:text-zinc-300 uppercase tracking-wider border-b border-gray-200 dark:border-zinc-700 ${col.key ? 'cursor-pointer select-none hover:bg-gray-200 dark:hover:bg-zinc-700' : ''
                        }`}
                    >
                      <div className="flex items-center gap-1">
                        {col.label}
                        {col.key && (
                          <span className="text-gray-400 dark:text-zinc-500">
                            <SortIcon k={col.key} />
                          </span>
                        )}
                      </div>
                    </th>
                  ))}
                </tr>
              </thead>

              <tbody>
                {isLoading ? (
                  Array.from({ length: PAGE_SIZE }).map((_, i) => (
                    <tr key={i} className={i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'}>
                      {Array.from({ length: COLS }).map((_, j) => (
                        <td key={j} className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                          <div className="h-4 bg-gray-200 dark:bg-zinc-700 rounded animate-pulse" />
                        </td>
                      ))}
                    </tr>
                  ))
                ) : paginated.length === 0 ? (
                  <tr>
                    <td colSpan={COLS} className="px-4 py-16 text-center">
                      <div className="flex flex-col items-center gap-3 text-gray-400 dark:text-zinc-500">
                        <BookOpen size={40} strokeWidth={1.2} />
                        <p className="text-sm font-medium text-gray-600 dark:text-zinc-300">No borrows found</p>
                        <p className="text-xs">
                          {hasActiveFilters
                            ? 'Try adjusting your filters or clearing them.'
                            : "You haven't borrowed any books yet."}
                        </p>
                        {hasActiveFilters && (
                          <Button variant="secondary" size="sm" onClick={handleCancel}>
                            Clear filters
                          </Button>
                        )}
                      </div>
                    </td>
                  </tr>
                ) : (
                  paginated.map((borrow, i) => (
                    <tr
                      key={borrow.id}
                      className={`transition-colors hover:bg-indigo-50 dark:hover:bg-zinc-700/40 ${i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'
                        }`}
                    >
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <span className="font-mono text-xs text-gray-500 dark:text-zinc-400">#{borrow.id}</span>
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <span className="font-semibold text-gray-900 dark:text-zinc-100">{borrow.bookTitle}</span>
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 text-gray-700 dark:text-zinc-300">
                        {borrow.authorName}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 text-gray-700 dark:text-zinc-300">
                        {formatDate(borrow.createdAt)}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800 text-gray-700 dark:text-zinc-300">
                        {borrow.returnedAt
                          ? formatDate(borrow.returnedAt)
                          : <span className="text-gray-400 dark:text-zinc-500">—</span>}
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <Badge variant={borrow.isReturned ? 'returned' : 'borrowed'}>
                          {borrow.isReturned ? 'Returned' : 'Active'}
                        </Badge>
                      </td>
                      <td className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                        <Button
                          size="sm"
                          variant="secondary"
                          disabled={borrow.isReturned || returningBookId === borrow.bookId}
                          onClick={() => handleReturn(borrow.bookId)}
                          className="rounded-full px-3 gap-1.5"
                        >
                          <Undo2 size={14} />
                          {returningBookId === borrow.bookId ? 'Returning…' : 'Return'}
                        </Button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between text-sm text-gray-600 dark:text-zinc-400">
              <span>Page {page} of {totalPages}</span>
              <div className="flex items-center gap-1">
                <button
                  onClick={() => setPage(p => Math.max(1, p - 1))}
                  disabled={page === 1}
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
                      className={`min-w-[32px] px-2 py-1 rounded text-sm transition-colors ${page === n ? 'bg-indigo-600 text-white font-medium' : 'hover:bg-gray-100 dark:hover:bg-zinc-800'
                        }`}
                    >
                      {n}
                    </button>
                  ),
                )}
                <button
                  onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
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
