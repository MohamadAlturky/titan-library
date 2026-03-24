import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  useReactTable,
  getCoreRowModel,
  getSortedRowModel,
  flexRender,
  createColumnHelper,
  type SortingState,
} from '@tanstack/react-table';
import {
  ChevronUp, ChevronDown, ChevronsUpDown,
  ChevronLeft, ChevronRight,
  Search, X, Filter, BookOpen, History,
} from 'lucide-react';
import {
  adminService,
  type AdminBookDto,
} from '@/services/adminService';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';

// ─── Types ────────────────────────────────────────────────────────────────────

type AvailabilityFilter = 'all' | 'true' | 'false';

interface DraftFilters {
  search: string;
  isAvailable: AvailabilityFilter;
  authorName: string;
}

interface AppliedFilters {
  search?: string;
  isAvailable?: boolean;
  authorName?: string;
}

const emptyDraft: DraftFilters = { search: '', isAvailable: 'all', authorName: '' };
const PAGE_SIZE = 10;

const col = createColumnHelper<AdminBookDto>();

// ─── Page ─────────────────────────────────────────────────────────────────────

export function AdminBooksPage() {
  const navigate = useNavigate();

  const [books, setBooks] = useState<AdminBookDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  const [filtersOpen, setFiltersOpen] = useState(false);
  const [draft, setDraft] = useState<DraftFilters>(emptyDraft);
  const [applied, setApplied] = useState<AppliedFilters>({});

  const [page, setPage] = useState(1);
  const [sorting, setSorting] = useState<SortingState>([]);

  const fetchBooks = useCallback(
    async (filters: AppliedFilters, currentPage: number, sort: SortingState) => {
      setIsLoading(true);
      try {
        const sortField = sort[0];
        const res = await adminService.getBooks({
          search: filters.search || undefined,
          isAvailable: filters.isAvailable,
          authorName: filters.authorName || undefined,
          sortBy: sortField ? (sortField.id as 'id' | 'title' | 'isbn' | 'isAvailable') : undefined,
          sortDirection: sortField ? (sortField.desc ? 'desc' : 'asc') : undefined,
          page: currentPage,
          pageSize: PAGE_SIZE,
        });
        setBooks(res.data.items);
        setTotalCount(res.data.totalCount);
        setTotalPages(res.data.totalPages);
        setFiltersOpen(false);
      } catch {
        // nothing to do
      } finally {
        setIsLoading(false);
      }
    },
    [],
  );

  useEffect(() => {
    fetchBooks(applied, page, sorting);
  }, [applied, page, sorting, fetchBooks]);

  const handleApply = () => {
    setApplied({
      search: draft.search.trim() || undefined,
      isAvailable: draft.isAvailable === 'all' ? undefined : draft.isAvailable === 'true',
      authorName: draft.authorName.trim() || undefined,
    });
    setPage(1);
  };

  const handleCancel = () => {
    setDraft(emptyDraft);
    setApplied({});
    setPage(1);
  };

  const handleSortChange = (updater: SortingState | ((prev: SortingState) => SortingState)) => {
    const next = typeof updater === 'function' ? updater(sorting) : updater;
    setSorting(next);
    setPage(1);
  };

  const columns = [
    col.accessor('id', { header: 'ID', size: 60 }),
    col.accessor('title', { header: 'Title' }),
    col.accessor('isbn', { header: 'ISBN' }),
    col.accessor('authorName', { header: 'Author', enableSorting: false }),
    col.accessor('isAvailable', {
      header: 'Availability',
      cell: ({ getValue }) => (
        <Badge variant={getValue() ? 'available' : 'borrowed'}>
          {getValue() ? 'Available' : 'Borrowed'}
        </Badge>
      ),
    }),
    col.accessor('createdAt', {
      header: 'Created',
      enableSorting: false,
      cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
    }),
    col.display({
      id: 'actions',
      size: 32,
      enableSorting: false,
      header: () => null,
      cell: ({ row }) => (
        <button
          onClick={() => navigate(`/admin/books/${row.original.id}/borrows`, { state: { bookTitle: row.original.title } })}
          className="p-1.5 rounded-md text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 dark:hover:text-indigo-400 dark:hover:bg-indigo-900/20 transition-colors"
          title="Borrow history"
        >
          <History size={16} />
        </button>
      ),
    }),
  ];

  const table = useReactTable({
    data: books,
    columns,
    state: { sorting },
    onSortingChange: handleSortChange,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    manualSorting: true,
    manualPagination: true,
    pageCount: totalPages,
  });

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

  const hasActiveFilters = Object.values(applied).some(v => v !== undefined);

  return (
    <div className="space-y-6">
      <PageHeader title="Books" description="All books in the library" />

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
                    placeholder="Title or ISBN..."
                    className="w-full pl-10 pr-4 py-2.5 bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm shadow-sm focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400">
                  Author Name
                </label>
                <input
                  type="text"
                  value={draft.authorName}
                  onChange={e => setDraft(prev => ({ ...prev, authorName: e.target.value }))}
                  onKeyDown={e => e.key === 'Enter' && handleApply()}
                  placeholder="Author name..."
                  className="w-44 px-3 py-2.5 bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm shadow-sm focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                />
              </div>

              <div className="space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400">
                  Availability
                </label>
                <div className="inline-flex p-1 bg-gray-200/50 dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl">
                  {([
                    { value: 'all', label: 'All' },
                    { value: 'true', label: 'Available' },
                    { value: 'false', label: 'Borrowed' },
                  ] as { value: AvailabilityFilter; label: string }[]).map(opt => (
                    <button
                      key={opt.value}
                      onClick={() => setDraft(prev => ({ ...prev, isAvailable: opt.value }))}
                      className={`px-4 py-1.5 text-sm font-medium rounded-lg transition-all duration-200 ${
                        draft.isAvailable === opt.value
                          ? 'bg-white dark:bg-zinc-700 text-indigo-600 dark:text-indigo-400 shadow-sm'
                          : 'text-gray-500 hover:text-gray-700 dark:hover:text-gray-300'
                      }`}
                    >
                      {opt.label}
                    </button>
                  ))}
                </div>
              </div>

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
          <p className="text-sm text-gray-500 dark:text-zinc-400">
            {isLoading ? 'Loading…' : `${totalCount} book${totalCount !== 1 ? 's' : ''} found`}
          </p>

          <div className="overflow-x-auto border border-gray-200 dark:border-zinc-700 rounded-lg">
            <table className="w-full text-sm border-collapse">
              <thead className="bg-gray-100 dark:bg-zinc-800">
                {table.getHeaderGroups().map(hg => (
                  <tr key={hg.id}>
                    {hg.headers.map(header => {
                      const canSort = header.column.getCanSort();
                      const sorted = header.column.getIsSorted();
                      return (
                        <th
                          key={header.id}
                          onClick={canSort ? header.column.getToggleSortingHandler() : undefined}
                          style={{ width: header.column.columnDef.size }}
                          className={`px-4 py-3 text-left text-xs font-semibold text-gray-600 dark:text-zinc-300 uppercase tracking-wider border-b border-gray-200 dark:border-zinc-700 ${canSort ? 'cursor-pointer select-none hover:bg-gray-200 dark:hover:bg-zinc-700' : ''}`}
                        >
                          <div className="flex items-center gap-1">
                            {flexRender(header.column.columnDef.header, header.getContext())}
                            {canSort && (
                              <span className="text-gray-400 dark:text-zinc-500">
                                {sorted === 'asc'
                                  ? <ChevronUp size={13} />
                                  : sorted === 'desc'
                                    ? <ChevronDown size={13} />
                                    : <ChevronsUpDown size={13} />}
                              </span>
                            )}
                          </div>
                        </th>
                      );
                    })}
                  </tr>
                ))}
              </thead>

              <tbody>
                {isLoading ? (
                  Array.from({ length: PAGE_SIZE }).map((_, i) => (
                    <tr key={i} className={i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'}>
                      {columns.map((_, j) => (
                        <td key={j} className="px-4 py-3 border-b border-gray-100 dark:border-zinc-800">
                          <div className="h-4 bg-gray-200 dark:bg-zinc-700 rounded animate-pulse" />
                        </td>
                      ))}
                    </tr>
                  ))
                ) : table.getRowModel().rows.length === 0 ? (
                  <tr>
                    <td colSpan={columns.length} className="px-4 py-16 text-center">
                      <div className="flex flex-col items-center gap-3 text-gray-400 dark:text-zinc-500">
                        <BookOpen size={40} strokeWidth={1.2} />
                        <p className="text-sm font-medium text-gray-600 dark:text-zinc-300">No books found</p>
                        {hasActiveFilters && (
                          <Button variant="secondary" size="sm" onClick={handleCancel}>
                            Clear filters
                          </Button>
                        )}
                      </div>
                    </td>
                  </tr>
                ) : (
                  table.getRowModel().rows.map((row, i) => (
                    <tr
                      key={row.id}
                      className={`transition-colors hover:bg-indigo-50 dark:hover:bg-zinc-700/40 ${i % 2 === 0 ? 'bg-white dark:bg-zinc-900' : 'bg-gray-50 dark:bg-zinc-800/40'}`}
                    >
                      {row.getVisibleCells().map(cell => (
                        <td key={cell.id} className="px-4 py-3 text-gray-900 dark:text-zinc-100 border-b border-gray-100 dark:border-zinc-800">
                          {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </td>
                      ))}
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
