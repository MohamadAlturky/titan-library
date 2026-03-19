import type { ColumnDef } from '@tanstack/react-table';
import { useAuth } from '@/hooks/useAuth';
import { mockBorrows } from '@/data/mockBorrows';
import type { BorrowRecord } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { BookOpen, BookMarked, CheckCircle } from 'lucide-react';

const columns: ColumnDef<BorrowRecord, unknown>[] = [
  { accessorKey: 'bookTitle', header: 'Book Title' },
  { accessorKey: 'borrowDate', header: 'Borrowed On' },
  { accessorKey: 'dueDate', header: 'Due Date' },
  { accessorKey: 'returnDate', header: 'Returned On', cell: ({ getValue }) => (getValue() as string) || '—' },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ getValue }) => {
      const v = getValue() as BorrowRecord['status'];
      return <Badge variant={v}>{v.charAt(0).toUpperCase() + v.slice(1)}</Badge>;
    },
  },
];

export function CustomerMyBorrowsPage() {
  const { user } = useAuth();
  const myBorrows = mockBorrows.filter(b => b.userId === user?.id);

  const totalBorrowed = myBorrows.length;
  const activeBorrows = myBorrows.filter(b => b.status === 'active' || b.status === 'overdue').length;
  const returned = myBorrows.filter(b => b.status === 'returned').length;

  return (
    <div className="space-y-6">
      <PageHeader title="My Library" description="Track your reading history and active loans" />

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-4 flex items-center gap-4">
          <div className="p-3 bg-indigo-50 dark:bg-indigo-950/30 rounded-lg">
            <BookOpen className="text-indigo-500" size={24} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{totalBorrowed}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Total Borrowed</p>
          </div>
        </div>
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-4 flex items-center gap-4">
          <div className="p-3 bg-amber-50 dark:bg-amber-950/30 rounded-lg">
            <BookMarked className="text-amber-500" size={24} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{activeBorrows}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Active</p>
          </div>
        </div>
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-4 flex items-center gap-4">
          <div className="p-3 bg-green-50 dark:bg-green-950/30 rounded-lg">
            <CheckCircle className="text-green-500" size={24} />
          </div>
          <div>
            <p className="text-2xl font-bold text-gray-900 dark:text-zinc-100">{returned}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400">Returned</p>
          </div>
        </div>
      </div>

      <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-4">
        <DataTable
          data={myBorrows}
          columns={columns}
          searchKey="bookTitle"
          searchPlaceholder="Search my borrows..."
          emptyMessage="You haven't borrowed any books yet."
        />
      </div>
    </div>
  );
}
