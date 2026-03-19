import type { ColumnDef } from '@tanstack/react-table';
import { useAuth } from '@/hooks/useAuth';
import { mockBorrows } from '@/data/mockBorrows';
import type { BorrowRecord } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<BorrowRecord, unknown>[] = [
  { accessorKey: 'bookTitle', header: 'Book' },
  { accessorKey: 'borrowDate', header: 'Borrowed' },
  { accessorKey: 'dueDate', header: 'Due Date' },
  { accessorKey: 'returnDate', header: 'Returned', cell: ({ getValue }) => (getValue() as string) || '—' },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ getValue }) => {
      const v = getValue() as BorrowRecord['status'];
      return <Badge variant={v}>{v.charAt(0).toUpperCase() + v.slice(1)}</Badge>;
    },
  },
];

export function AuthorMyBorrowedPage() {
  const { user } = useAuth();
  const myBorrows = mockBorrows.filter(b => b.userId === user?.id);

  return (
    <div>
      <PageHeader title="My Borrowed Books" description="Books you have borrowed" />
      <Card>
        <div className="p-4">
          <DataTable
            data={myBorrows}
            columns={columns}
            searchKey="bookTitle"
            searchPlaceholder="Search..."
            emptyMessage="You haven't borrowed any books yet."
          />
        </div>
      </Card>
    </div>
  );
}
