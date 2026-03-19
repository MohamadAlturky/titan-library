import type { ColumnDef } from '@tanstack/react-table';
import { mockBorrows } from '@/data/mockBorrows';
import type { BorrowRecord } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<BorrowRecord, unknown>[] = [
  { accessorKey: 'bookTitle', header: 'Book' },
  { accessorKey: 'userName', header: 'Borrower' },
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

export function AdminBorrowsPage() {
  return (
    <div>
      <PageHeader title="Borrows" description="All borrow records" />
      <Card>
        <div className="p-4">
          <DataTable
            data={mockBorrows}
            columns={columns}
            searchKey="bookTitle"
            searchPlaceholder="Search borrows..."
            pageSize={8}
          />
        </div>
      </Card>
    </div>
  );
}
