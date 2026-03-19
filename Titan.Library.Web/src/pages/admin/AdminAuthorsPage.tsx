import type { ColumnDef } from '@tanstack/react-table';
import { mockAuthors } from '@/data/mockAuthors';
import type { Author } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<Author, unknown>[] = [
  { accessorKey: 'name', header: 'Name' },
  { accessorKey: 'email', header: 'Email' },
  { accessorKey: 'bio', header: 'Bio' },
  { accessorKey: 'booksCount', header: 'Books' },
];

export function AdminAuthorsPage() {
  return (
    <div>
      <PageHeader title="Authors" description="All library authors" />
      <Card>
        <div className="p-4">
          <DataTable
            data={mockAuthors}
            columns={columns}
            searchKey="name"
            searchPlaceholder="Search authors..."
          />
        </div>
      </Card>
    </div>
  );
}
