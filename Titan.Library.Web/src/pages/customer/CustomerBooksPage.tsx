import type { ColumnDef } from '@tanstack/react-table';
import { useNavigate } from 'react-router-dom';
import { mockBooks } from '@/data/mockBooks';
import type { Book } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

export function CustomerBooksPage() {
  const navigate = useNavigate();

  const columns: ColumnDef<Book, unknown>[] = [
    { accessorKey: 'title', header: 'Title' },
    { accessorKey: 'authorName', header: 'Author' },
    { accessorKey: 'genre', header: 'Genre' },
    { accessorKey: 'publishedYear', header: 'Year' },
    {
      accessorKey: 'status',
      header: 'Status',
      cell: ({ getValue }) => {
        const v = getValue() as Book['status'];
        return <Badge variant={v}>{v.charAt(0).toUpperCase() + v.slice(1)}</Badge>;
      },
    },
    { accessorKey: 'availableCopies', header: 'Available Copies' },
    {
      id: 'actions',
      header: 'Actions',
      cell: ({ row }) => (
        <Button
          variant="primary"
          size="sm"
          disabled={row.original.availableCopies === 0}
          onClick={() => navigate('/customer/borrow')}
        >
          Borrow
        </Button>
      ),
    },
  ];

  return (
    <div>
      <PageHeader title="Browse Books" description="Find and borrow books from our collection" />
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
