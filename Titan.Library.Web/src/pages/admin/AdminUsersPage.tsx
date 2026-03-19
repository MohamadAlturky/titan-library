import type { ColumnDef } from '@tanstack/react-table';
import { mockUsers } from '@/data/mockUsers';
import type { AuthUser } from '@/types';
import { DataTable } from '@/components/ui/DataTable';
import { Badge } from '@/components/ui/Badge';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';

const columns: ColumnDef<AuthUser, unknown>[] = [
  { accessorKey: 'name', header: 'Name' },
  { accessorKey: 'email', header: 'Email' },
  {
    accessorKey: 'role',
    header: 'Role',
    cell: ({ getValue }) => {
      const v = getValue() as AuthUser['role'];
      return <Badge variant={v}>{v.charAt(0).toUpperCase() + v.slice(1)}</Badge>;
    },
  },
];

export function AdminUsersPage() {
  return (
    <div>
      <PageHeader title="Users" description="All registered users" />
      <Card>
        <div className="p-4">
          <DataTable
            data={mockUsers}
            columns={columns}
            searchKey="name"
            searchPlaceholder="Search users..."
          />
        </div>
      </Card>
    </div>
  );
}
