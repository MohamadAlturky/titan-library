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
  ChevronUp, ChevronDown, ChevronsUpDown,
  ChevronLeft, ChevronRight,
  Search, X, Filter, MessageSquare, Pencil, MoreHorizontal,
} from 'lucide-react';
import { toast } from 'sonner';
import {
  adminService,
  type AdminMessageDto,
  type UpdateMessageRequest,
} from '@/services/adminService';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';
import { Button } from '@/components/ui/Button';
import { Modal } from '@/components/ui/Modal';

// ─── Types ────────────────────────────────────────────────────────────────────

interface DraftFilters {
  search: string;
}

interface AppliedFilters {
  search?: string;
}

type MessageFormData = { key: string; value: string };
const emptyDraft: DraftFilters = { search: '' };
const emptyForm: MessageFormData = { key: '', value: '' };
const PAGE_SIZE = 10;

const col = createColumnHelper<AdminMessageDto>();

// ─── Row action menu ──────────────────────────────────────────────────────────

interface RowMenuProps {
  message: AdminMessageDto;
  onEdit: (message: AdminMessageDto) => void;
}

function RowMenu({ message, onEdit }: RowMenuProps) {
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
            onClick={() => { setOpen(false); onEdit(message); }}
            className="w-full flex items-center gap-2 px-3 py-2 text-sm text-gray-700 dark:text-zinc-200 hover:bg-gray-100 dark:hover:bg-zinc-700 transition-colors"
          >
            <Pencil size={14} />
            Edit
          </button>
        </div>
      )}
    </>
  );
}

// ─── Page ─────────────────────────────────────────────────────────────────────

export function AdminMessagesPage() {
  const [messages, setMessages] = useState<AdminMessageDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);

  const [filtersOpen, setFiltersOpen] = useState(false);
  const [draft, setDraft] = useState<DraftFilters>(emptyDraft);
  const [applied, setApplied] = useState<AppliedFilters>({});

  const [page, setPage] = useState(1);
  const [sorting, setSorting] = useState<SortingState>([]);

  const [editTarget, setEditTarget] = useState<AdminMessageDto | null>(null);
  const [form, setForm] = useState<MessageFormData>(emptyForm);
  const [submitting, setSubmitting] = useState(false);

  const fetchMessages = useCallback(
    async (filters: AppliedFilters, currentPage: number, sort: SortingState) => {
      setIsLoading(true);
      try {
        const sortField = sort[0];
        const res = await adminService.getMessages({
          search: filters.search || undefined,
          sortBy: sortField ? (sortField.id as 'id' | 'key' | 'value' | 'createdAt') : undefined,
          sortDirection: sortField ? (sortField.desc ? 'desc' : 'asc') : undefined,
          page: currentPage,
          pageSize: PAGE_SIZE,
        });
        setMessages(res.data.items);
        setTotalCount(res.data.totalCount);
        setTotalPages(res.data.totalPages);
        setFiltersOpen(false);
      } catch {
        toast.error('Failed to load messages.');
      } finally {
        setIsLoading(false);
      }
    },
    [],
  );

  useEffect(() => {
    fetchMessages(applied, page, sorting);
  }, [applied, page, sorting, fetchMessages]);

  const handleApply = () => {
    setApplied({ search: draft.search.trim() || undefined });
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

  const openEdit = (message: AdminMessageDto) => {
    setEditTarget(message);
    setForm({ key: message.key, value: message.value });
  };

  const closeEdit = () => {
    if (submitting) return;
    setEditTarget(null);
    setForm(emptyForm);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editTarget || !form.key.trim() || !form.value.trim()) return;
    setSubmitting(true);
    try {
      const payload: UpdateMessageRequest = { key: form.key.trim(), value: form.value.trim() };
      await adminService.updateMessage(editTarget.id, payload);
      toast.success('Message updated successfully.');
      setMessages(prev =>
        prev.map(m => m.id === editTarget.id ? { ...m, key: payload.key, value: payload.value } : m),
      );
      closeEdit();
    } catch {
      toast.error('Failed to update message.');
    } finally {
      setSubmitting(false);
    }
  };

  const columns = [
    col.accessor('id', { header: 'ID', size: 60 }),
    col.accessor('key', { header: 'Key' }),
    col.accessor('value', {
      header: 'Value',
      cell: ({ getValue }) => (
        <span className="block max-w-xs truncate" title={getValue()}>
          {getValue()}
        </span>
      ),
    }),
    col.accessor('createdAt', {
      header: 'Created',
      cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
    }),
    col.display({
      id: 'actions',
      size: 32,
      enableSorting: false,
      header: () => null,
      cell: ({ row }) => (
        <RowMenu message={row.original} onEdit={openEdit} />
      ),
    }),
  ];

  const table = useReactTable({
    data: messages,
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
      <PageHeader title="Messages" description="Manage localization message keys and values" />

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
                1
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
                    onChange={e => setDraft({ search: e.target.value })}
                    onKeyDown={e => e.key === 'Enter' && handleApply()}
                    placeholder="Key or value..."
                    className="w-full pl-10 pr-4 py-2.5 bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm shadow-sm focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
                  />
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
            {isLoading ? 'Loading…' : `${totalCount} message${totalCount !== 1 ? 's' : ''} found`}
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
                        <MessageSquare size={40} strokeWidth={1.2} />
                        <p className="text-sm font-medium text-gray-600 dark:text-zinc-300">No messages found</p>
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

      {/* ── Edit modal ───────────────────────────────────────────────────────── */}
      <Modal
        isOpen={!!editTarget}
        onClose={closeEdit}
        title="Edit Message"
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Key
            </label>
            <input
              type="text"
              value={form.key}
              onChange={e => setForm(prev => ({ ...prev, key: e.target.value }))}
              required
              autoFocus
              className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              placeholder="MESSAGE_KEY"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Value
            </label>
            <textarea
              value={form.value}
              onChange={e => setForm(prev => ({ ...prev, value: e.target.value }))}
              required
              rows={4}
              className="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
              placeholder="Human readable message..."
            />
          </div>
          <div className="flex justify-end gap-3 pt-2">
            <Button type="button" variant="secondary" onClick={closeEdit} disabled={submitting}>
              Cancel
            </Button>
            <Button type="submit" disabled={submitting || !form.key.trim() || !form.value.trim()}>
              {submitting ? 'Saving…' : 'Save Changes'}
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
