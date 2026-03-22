import type { ColumnDef } from '@tanstack/react-table';
import { useAuth } from '@/hooks/useAuth';
import { mockBooks } from '@/data/mockBooks';
import { mockAuthors } from '@/data/mockAuthors';
import type { Book } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<Book, unknown>[] = [
  { accessorKey: 'title', header: 'Title' },
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
];

export function AuthorMyBooksPage() {
  const { user } = useAuth();
  const author = mockAuthors.find(a => a.email === user?.email);
  const myBooks = author ? mockBooks.filter(b => b.authorId === author.id) : [];
  return (
    <div>
      <PageHeader
        title="My Books"
        description={author ? `Books authored by ${author.name}` : 'No author profile found'}
      />
      <Card>
        <div className="p-4">
          <DataTable
            data={myBooks}
            columns={columns}
            searchKey="title"
            searchPlaceholder="Search my books..."
            emptyMessage="No books found for your author profile."
          />
        </div>
      </Card>
    </div>
  );
}
