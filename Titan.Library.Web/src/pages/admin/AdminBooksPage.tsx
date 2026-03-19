import type { ColumnDef } from '@tanstack/react-table';
import { toast } from 'sonner';
import { mockBooks } from '@/data/mockBooks';
import type { Book } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<Book, unknown>[] = [
  { accessorKey: 'title', header: 'Title' },
  { accessorKey: 'authorName', header: 'Author' },
  { accessorKey: 'genre', header: 'Genre' },
  { accessorKey: 'publishedYear', header: 'Year' },
  { accessorKey: 'isbn', header: 'ISBN' },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ getValue }) => {
      const v = getValue() as Book['status'];
      return <Badge variant={v}>{v.charAt(0).toUpperCase() + v.slice(1)}</Badge>;
    },
  },
  {
    accessorKey: 'availableCopies',
    header: 'Available',
    cell: ({ row }) => `${row.original.availableCopies} / ${row.original.totalCopies}`,
  },
  {
    id: 'actions',
    header: 'Actions',
    cell: () => (
      <Button variant="ghost" size="sm" onClick={() => toast.info('Edit functionality coming soon!')}>
        Edit
      </Button>
    ),
  },
];

export function AdminBooksPage() {
  return (
    <div>
      <PageHeader
        title="Books"
        description="Manage all library books"
        actions={<Button onClick={() => toast.success('Add book modal would open here!')}>Add Book</Button>}
      />
      <Card>
        <div className="p-4">
          <DataTable
            data={mockBooks}
            columns={columns}
            searchKey="title"
            searchPlaceholder="Search books..."
            pageSize={8}
          />
        </div>
      </Card>
    </div>
  );
}
