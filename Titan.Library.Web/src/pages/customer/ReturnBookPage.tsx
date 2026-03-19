import { useState } from 'react';
import { toast } from 'sonner';
import { useAuth } from '@/hooks/useAuth';
import { mockBorrows } from '@/data/mockBorrows';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
import { Card } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';

export function ReturnBookPage() {
  const { user } = useAuth();
  const [selectedBorrowId, setSelectedBorrowId] = useState('');
  const activeBorrows = mockBorrows.filter(
    b => b.userId === user?.id && (b.status === 'active' || b.status === 'overdue')
  );

  const handleReturn = () => {
    if (!selectedBorrowId) return;
    const borrow = activeBorrows.find(b => b.id === selectedBorrowId);
    toast.success(`Successfully returned "${borrow?.bookTitle}"!`);
    setSelectedBorrowId('');
  };

  const selectedBorrow = activeBorrows.find(b => b.id === selectedBorrowId);

  return (
    <div>
      <PageHeader title="Return a Book" description="Return a borrowed book" />
      <Card>
        <div className="p-6 space-y-4 max-w-lg">
          {activeBorrows.length === 0 ? (
            <p className="text-gray-500 dark:text-gray-400">You have no active borrows to return.</p>
          ) : (
            <>
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                  Select Borrow
                </label>
                <select
                  value={selectedBorrowId}
                  onChange={e => setSelectedBorrowId(e.target.value)}
                  className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Select a borrow --</option>
                  {activeBorrows.map(b => (
                    <option key={b.id} value={b.id}>
                      {b.bookTitle} (Due: {b.dueDate})
                    </option>
                  ))}
                </select>
              </div>

              {selectedBorrow && (
                <div className="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <h3 className="font-semibold text-gray-900 dark:text-gray-100">{selectedBorrow.bookTitle}</h3>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Borrowed: {selectedBorrow.borrowDate}</p>
                  <p className="text-sm text-gray-500 dark:text-gray-400">Due: {selectedBorrow.dueDate}</p>
                  <div className="mt-2">
                    <Badge variant={selectedBorrow.status}>{selectedBorrow.status}</Badge>
                  </div>
                </div>
              )}

              <Button onClick={handleReturn} disabled={!selectedBorrowId}>
                Confirm Return
              </Button>
            </>
          )}
        </div>
      </Card>
    </div>
  );
}
