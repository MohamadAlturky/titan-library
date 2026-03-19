import { useState } from 'react';
import { toast } from 'sonner';
import { BookOpen, AlertCircle } from 'lucide-react';
import { useAuth } from '@/hooks/useAuth';
import { mockBorrows } from '@/data/mockBorrows';
import { Button } from '@/components/ui/Button';
import { PageHeader } from '@/components/layout/PageHeader';
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
  const isOverdue = selectedBorrow?.status === 'overdue';

  return (
    <div>
      <PageHeader title="Return a Book" description="Return a borrowed book to the library" />
      {activeBorrows.length === 0 ? (
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-10 text-center">
          <p className="text-gray-500 dark:text-zinc-400">You have no active borrows to return.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Left: Borrow Selector */}
          <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg p-6 space-y-4">
            <h2 className="text-base font-semibold text-gray-900 dark:text-zinc-100">Select a Borrow</h2>
            <div>
              <label className="block text-sm font-medium text-gray-700 dark:text-zinc-300 mb-1">
                Active Borrows
              </label>
              <select
                value={selectedBorrowId}
                onChange={e => setSelectedBorrowId(e.target.value)}
                className="w-full px-3 py-2 rounded border border-gray-200 dark:border-zinc-800 bg-white dark:bg-zinc-800 text-gray-900 dark:text-zinc-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              >
                <option value="">-- Select a borrow --</option>
                {activeBorrows.map(b => (
                  <option key={b.id} value={b.id}>
                    {b.bookTitle} (Due: {b.dueDate})
                  </option>
                ))}
              </select>
            </div>
            <Button onClick={handleReturn} disabled={!selectedBorrowId} className="w-full">
              Confirm Return
            </Button>
          </div>

          {/* Right: Borrow Details */}
          <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-lg overflow-hidden">
            {selectedBorrow ? (
              <>
                <div className={`p-6 border-b border-gray-200 dark:border-zinc-800 ${isOverdue ? 'bg-red-50 dark:bg-red-950/30' : 'bg-indigo-50 dark:bg-indigo-950/30'}`}>
                  <div className="flex items-start justify-between gap-2">
                    <div>
                      <h3 className="text-lg font-bold text-gray-900 dark:text-zinc-100">{selectedBorrow.bookTitle}</h3>
                      <div className="mt-2">
                        <Badge variant={selectedBorrow.status}>{selectedBorrow.status.charAt(0).toUpperCase() + selectedBorrow.status.slice(1)}</Badge>
                      </div>
                    </div>
                    {isOverdue && <AlertCircle className="text-red-500 flex-shrink-0 mt-1" size={22} />}
                  </div>
                </div>
                <div className="p-6 space-y-2">
                  <div className="flex justify-between items-center py-2 border-b border-gray-100 dark:border-zinc-800 text-sm">
                    <span className="text-gray-400 dark:text-zinc-500">Borrowed On</span>
                    <span className="font-medium text-gray-900 dark:text-zinc-100">{selectedBorrow.borrowDate}</span>
                  </div>
                  <div className="flex justify-between items-center py-2 text-sm">
                    <span className="text-gray-400 dark:text-zinc-500">Due Date</span>
                    <span className={`font-medium ${isOverdue ? 'text-red-500' : 'text-gray-900 dark:text-zinc-100'}`}>
                      {selectedBorrow.dueDate}{isOverdue && ' (Overdue)'}
                    </span>
                  </div>
                  {isOverdue && (
                    <div className="flex items-center gap-2 p-3 bg-red-50 dark:bg-red-950/30 rounded text-sm text-red-600 dark:text-red-400 mt-2">
                      <AlertCircle size={16} />
                      This book is overdue. Please return it as soon as possible.
                    </div>
                  )}
                </div>
              </>
            ) : (
              <div className="h-full min-h-48 flex flex-col items-center justify-center p-10 text-center">
                <BookOpen className="text-gray-300 dark:text-zinc-700 mb-3" size={64} />
                <p className="text-gray-400 dark:text-zinc-500 text-sm">Select a borrow to see its details</p>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
