import { useCallback, useEffect, useRef, useState } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  getSortedRowModel,
  flexRender,
  createColumnHelper,
  type SortingState,
} from '@tanstack/react-table';
import {
  Plus, Pencil, Trash2,
  ChevronUp, ChevronDown, ChevronsUpDown,
  ChevronLeft, ChevronRight,
  BookOpen, Search, X, Filter, MoreHorizontal,
  AlertTriangle,
} from 'lucide-react';
import { toast } from 'sonner';
import {
  authorBookService,
  type AuthorBookDto,
  type CreateBookRequest,
  type UpdateBookRequest,
} from '@/services/authorBookService';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Modal } from '@/components/ui/Modal';

// ─── Types ────────────────────────────────────────────────────────────────────

type BookFormData = { title: string; isbn: string; description: string };
type AvailabilityFilter = 'all' | 'true' | 'false';

interface DraftFilters {
  search: string;
  isAvailable: AvailabilityFilter;
}

interface AppliedFilters {
  search?: string;
  isAvailable?: boolean;
}

const emptyForm: BookFormData = { title: '', isbn: '', description: '' };
const emptyDraft: DraftFilters = { search: '', isAvailable: 'all' };
const PAGE_SIZE = 10;

const col = createColumnHelper<AuthorBookDto>();

// ─── Row action menu ──────────────────────────────────────────────────────────

interface RowMenuProps {
  book: AuthorBookDto;
  onEdit: (book: AuthorBookDto) => void;
  onDelete: (book: AuthorBookDto) => void;
}

function RowMenu({ book, onEdit, onDelete }: RowMenuProps) {
  const [open, setOpen] = useState(false);
  const [pos, setPos] = useState({ top: 0, left: 0 });
  const btnRef = useRef<HTMLButtonElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);

  const toggle = () => {
    if (!open && btnRef.current) {
      const rect = btnRef.current.getBoundingClientRect();
      setPos({ top: rect.bottom + 4, left: rect.right - 144 });
    }
    setOpen(v => !v);
  };

  // Close on outside click or scroll
  useEffect(() => {
    if (!open) return;
    const close = (e: MouseEvent) => {
      if (
        menuRef.current &&
        !menuRef.current.contains(e.target as Node) &&
        !btnRef.current?.contains(e.target as Node)
      ) {
        setOpen(false);
      }
    };
    const closeOnScroll = () => setOpen(false);
    document.addEventListener('mousedown', close);
    document.addEventListener('scroll', closeOnScroll, true);
    return () => {
      document.removeEventListener('mousedown', close);
      document.removeEventListener('scroll', closeOnScroll, true);
    };
  }, [open]);

  return (
    <>
      <button
        ref={btnRef}
        onClick={toggle}
        className="p-1.5 rounded-md text-gray-400 hover:text-gray-700 hover:bg-gray-100 dark:hover:text-zinc-200 dark:hover:bg-zinc-700 transition-colors"
        aria-label="Row actions"
      >
        <MoreHorizontal size={16} />
      </button>

      {open && (
        <div
          ref={menuRef}
          style={{ top: pos.top, left: pos.left }}
          className="fixed z-50 w-36 rounded-lg border border-gray-200 dark:border-zinc-700 bg-white dark:bg-zinc-800 shadow-lg py-1"
        >
          <button
            onClick={() => { setOpen(false); onEdit(book); }}
            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-zinc-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors"
          >
            <Pencil size={14} />
            Edit
          </button>
          <button
            onClick={() => { setOpen(false); onDelete(book); }}
            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
          >
            <Trash2 size={14} />
            Delete
          </button>
        </div>
      )}
    </>
  );
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export function AuthorMyBooksPage() {
  // ── Data state ──────────────────────────────────────────────────────────────
  const [books, setBooks] = useState<AuthorBookDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  // ── Filter state ────────────────────────────────────────────────────────────
  const [filtersOpen, setFiltersOpen] = useState(false);
  const [draft, setDraft] = useState<DraftFilters>(emptyDraft);
  const [applied, setApplied] = useState<AppliedFilters>({});

  // ── Pagination & sort state ─────────────────────────────────────────────────
  const [page, setPage] = useState(1);
  const [sorting, setSorting] = useState<SortingState>([]);

  // ── Form modal state ─────────────────────────────────────────────────────────
  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<AuthorBookDto | null>(null);
  const [form, setForm] = useState<BookFormData>(emptyForm);
  const [submitting, setSubmitting] = useState(false);

  // ── Delete modal state ───────────────────────────────────────────────────────
  const [deleteTarget, setDeleteTarget] = useState<AuthorBookDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  // ── Fetch ───────────────────────────────────────────────────────────────────
  const fetchBooks = useCallback(
    async (filters: AppliedFilters, currentPage: number, sort: SortingState) => {
      setIsLoading(true);
      try {
        const sortField = sort[0];
        const res = await authorBookService.getBooks({
          search: filters.search || undefined,
          isAvailable: filters.isAvailable,
          sortBy: sortField
            ? (sortField.id as 'id' | 'title' | 'isbn' | 'isAvailable')
            : undefined,
          sortDirection: sortField ? (sortField.desc ? 'desc' : 'asc') : undefined,
          page: currentPage,
          pageSize: PAGE_SIZE,
        });
        setBooks(res.data.items);
        setTotalCount(res.data.totalCount);
        setTotalPages(res.data.totalPages);
        setFiltersOpen(false);
      } catch {
        toast.error('Failed to load books.');
      } finally {
        setIsLoading(false);
      }
    },
    [],
  );

  useEffect(() => {
    fetchBooks(applied, page, sorting);
  }, [applied, page, sorting, fetchBooks]);

  // ── Filter handlers ─────────────────────────────────────────────────────────
  const handleApply = () => {
    setApplied({
      search: draft.search.trim() || undefined,
      isAvailable: draft.isAvailable === 'all' ? undefined : draft.isAvailable === 'true',
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

  // ── Form modal helpers ───────────────────────────────────────────────────────
  const openCreate = () => {
    setEditTarget(null);
    setForm(emptyForm);
    setFormOpen(true);
  };

  const openEdit = (book: AuthorBookDto) => {
    setEditTarget(book);
    setForm({ title: book.title, isbn: book.isbn, description: book.description });
    setFormOpen(true);
  };

  const closeForm = () => {
    setFormOpen(false);
    setEditTarget(null);
    setForm(emptyForm);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.title.trim() || !form.isbn.trim() || form.description.trim().length < 50) return;
    setSubmitting(true);
    try {
      if (editTarget) {
        const payload: UpdateBookRequest = { title: form.title, isbn: form.isbn, description: form.description };
        await authorBookService.updateBook(editTarget.id, payload);
        toast.success('Book updated successfully.');
      } else {
        const payload: CreateBookRequest = { title: form.title, isbn: form.isbn, description: form.description };
        await authorBookService.createBook(payload);
        toast.success('Book created successfully.');
      }
      closeForm();
      fetchBooks(applied, page, sorting);
    } catch {
      toast.error(editTarget ? 'Failed to update book.' : 'Failed to create book.');
    } finally {
      setSubmitting(false);
    }
  };

  // ── Delete modal helpers ─────────────────────────────────────────────────────
  const openDelete = (book: AuthorBookDto) => setDeleteTarget(book);
  const closeDelete = () => { if (!deleting) setDeleteTarget(null); };

  const handleConfirmDelete = async () => {
    if (!deleteTarget) return;
    setDeleting(true);
    try {
      await authorBookService.deleteBook(deleteTarget.id);
      toast.success('Book deleted.');
      setBooks(prev => prev.filter(b => b.id !== deleteTarget.id));
      setTotalCount(prev => prev - 1);
      setDeleteTarget(null);
    } catch {
      toast.error('Failed to delete book.');
    } finally {
      setDeleting(false);
    }
  };

  // ── Columns ──────────────────────────────────────────────────────────────────
  const columns = [
    col.accessor('id', { header: 'ID', size: 60 }),
    col.accessor('title', { header: 'Title' }),
    col.accessor('isbn', { header: 'ISBN' }),
    col.accessor('isAvailable', {
      header: 'Availability',
      cell: ({ getValue }) => {
        const available = getValue();
        return (
          <Badge variant={available ? 'available' : 'borrowed'}>
            {available ? 'Available' : 'Borrowed'}
          </Badge>
        );
      },
    }),
    col.display({
      id: 'actions',
      size: 32,
      enableSorting: false,
      header: () => null,
      cell: ({ row }) => (
        <RowMenu
          book={row.original}
          onEdit={openEdit}
          onDelete={openDelete}
        />
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

  // ── Pagination page numbers ──────────────────────────────────────────────────
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

  const hasActiveFilters = Object.values(applied).some(v => v !== undefined);

  // ─────────────────────────────────────────────────────────────────────────────

  return (
    <div className="space-y-6">
      <PageHeader title="My Books" description="Manage books you have authored" />

      {/* ── Filters card ────────────────────────────────────────────────────── */}
      <Card>
        {/* Collapsible header */}
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

        {/* Collapsible body */}
        {filtersOpen && (
          <div className="px-6 py-5 border-t border-gray-100 dark:border-zinc-700/60 bg-gray-50/50 dark:bg-zinc-800/30">
            <div className="flex flex-col md:flex-row md:items-end gap-6">

              {/* Search Section */}
              <div className="flex-1 space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400">
                  Search Library
                </label>
                <div className="relative group">
                  <Search
                    size={18}
                    className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 group-focus-within:text-indigo-500 transition-colors"
                  />
                  <input
                    type="text"
                    value={draft.search}
                    onChange={e => setDraft(prev => ({ ...prev, search: e.target.value }))}
                    onKeyDown={e => e.key === 'Enter' && handleApply()}
                    placeholder="Title, author, or ISBN..."
                    className="w-full pl-10 pr-4 py-2.5 bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm shadow-sm focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  />
                </div>
              </div>

              {/* Availability "Square" Toggle Section */}
              <div className="space-y-2">
                <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-gray-400 mr-2">
                  Status
                </label>
                <div className="inline-flex p-1 bg-gray-200/50 dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl">
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
                        className={`
                        px-4 py-1.5 text-sm font-medium rounded-lg transition-all duration-200
                        ${isActive
                            ? 'bg-white dark:bg-zinc-700 text-indigo-600 dark:text-indigo-400 shadow-sm'
                            : 'text-gray-500 hover:text-gray-700 dark:hover:text-gray-300'
                          }
                      `}
                      >
                        {opt.label}
                      </button>
                    );
                  })}
                </div>
              </div>

              {/* Action Buttons */}
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
          // <div className="px-4 pb-4 border-t border-gray-100 dark:border-zinc-700/60 pt-4 space-y-4">
          //   <div className="flex flex-wrap items-end gap-4">

          //     {/* Search */}
          //     <div className="flex-1 min-w-52">
          //       <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
          //         Search
          //       </label>
          //       <div className="relative">
          //         <Search size={14} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          //         <input
          //           type="text"
          //           value={draft.search}
          //           onChange={e => setDraft(prev => ({ ...prev, search: e.target.value }))}
          //           onKeyDown={e => e.key === 'Enter' && handleApply()}
          //           placeholder="Search by title or ISBN…"
          //           className="w-full pl-9 pr-3 py-2 text-sm rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          //         />
          //       </div>
          //     </div>

          //     {/* Availability radios */}
          //     <div>
          //       <p className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
          //         Availability
          //       </p>
          //       <div className="flex items-center gap-4">
          //         {(
          //           [
          //             { value: 'all', label: 'All' },
          //             { value: 'true', label: 'Available' },
          //             { value: 'false', label: 'Borrowed' },
          //           ] as { value: AvailabilityFilter; label: string }[]
          //         ).map(opt => (
          //           <label
          //             key={opt.value}
          //             className="flex items-center gap-1.5 text-sm text-gray-700 dark:text-gray-300 cursor-pointer select-none"
          //           >
          //             <input
          //               type="radio"
          //               name="availability"
          //               value={opt.value}
          //               checked={draft.isAvailable === opt.value}
          //               onChange={() => setDraft(prev => ({ ...prev, isAvailable: opt.value }))}
          //               className="accent-indigo-600"
          //             />
          //             {opt.label}
          //           </label>
          //         ))}
          //       </div>
          //     </div>

          //     {/* Apply / Cancel */}
          //     <div className="flex items-center gap-2 ml-auto">
          //       <Button onClick={handleApply} disabled={isLoading}>
          //         <Search size={14} />
          //         Apply
          //       </Button>
          //       <Button variant="secondary" onClick={handleCancel} disabled={isLoading}>
          //         <X size={14} />
          //         Clear
          //       </Button>
          //     </div>
          //   </div>
          // </div>
        )}
      </Card>

      {/* ── Table card ──────────────────────────────────────────────────────── */}
      <Card>
        <div className="p-4 space-y-4">
          {/* Header */}
          <div className="flex items-center justify-between">
            <p className="text-sm text-gray-500 dark:text-zinc-400">
              {isLoading ? 'Loading…' : `${totalCount} book${totalCount !== 1 ? 's' : ''} found`}
            </p>
            <Button onClick={openCreate}>
              <Plus size={16} />
              New Book
            </Button>
          </div>

          {/* Table */}
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
                        <p className="text-xs">
                          {hasActiveFilters
                            ? 'Try adjusting your filters or clearing them.'
                            : 'Get started by creating your first book.'}
                        </p>
                        {hasActiveFilters ? (
                          <Button variant="secondary" size="sm" onClick={handleCancel}>
                            Clear filters
                          </Button>
                        ) : (
                          <Button size="sm" onClick={openCreate}>
                            <Plus size={14} />
                            Create book
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
                        <td
                          key={cell.id}
                          className="px-4 py-3 text-gray-900 dark:text-zinc-100 border-b border-gray-100 dark:border-zinc-800"
                        >
                          {flexRender(cell.column.columnDef.cell, cell.getContext())}
                        </td>
                      ))}
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

      {/* ── Create / Edit modal ──────────────────────────────────────────────── */}
      <Modal
        isOpen={formOpen}
        onClose={closeForm}
        title={editTarget ? 'Edit Book' : 'Create Book'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Title
            </label>
            <input
              type="text"
              value={form.title}
              onChange={e => setForm(prev => ({ ...prev, title: e.target.value }))}
              required
              autoFocus
              className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="Book title"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              ISBN
            </label>
            <input
              type="text"
              value={form.isbn}
              onChange={e => setForm(prev => ({ ...prev, isbn: e.target.value }))}
              required
              className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="978-x-xxx-xxxxx-x"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Description
              <span className="ml-1 text-xs text-gray-400 dark:text-gray-500 font-normal">
                ({form.description.trim().length}/50 min)
              </span>
            </label>
            <textarea
              value={form.description}
              onChange={e => setForm(prev => ({ ...prev, description: e.target.value }))}
              required
              rows={4}
              className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
              placeholder="Write a description for this book (at least 50 characters)..."
            />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="secondary" onClick={closeForm} disabled={submitting}>
              Cancel
            </Button>
            <Button type="submit" disabled={submitting}>
              {submitting ? 'Saving…' : editTarget ? 'Save Changes' : 'Create Book'}
            </Button>
          </div>
        </form>
      </Modal>

      {/* ── Delete confirmation modal ────────────────────────────────────────── */}
      <Modal
        isOpen={!!deleteTarget}
        onClose={closeDelete}
        title="Delete Book"
      >
        <div className="space-y-4">
          <div className="flex items-start gap-3">
            <div className="flex-shrink-0 flex items-center justify-center w-10 h-10 rounded-full bg-red-100 dark:bg-red-900/30">
              <AlertTriangle size={20} className="text-red-600 dark:text-red-400" />
            </div>
            <div>
              <p className="text-sm text-gray-700 dark:text-gray-300">
                Are you sure you want to delete{' '}
                <span className="font-semibold">"{deleteTarget?.title}"</span>?
              </p>
              <p className="mt-1 text-xs text-gray-500 dark:text-zinc-400">
                This action cannot be undone.
              </p>
            </div>
          </div>
          <div className="flex justify-end gap-3">
            <Button variant="secondary" onClick={closeDelete} disabled={deleting}>
              Cancel
            </Button>
            <Button variant="danger" onClick={handleConfirmDelete} disabled={deleting}>
              {deleting ? 'Deleting…' : 'Delete'}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
