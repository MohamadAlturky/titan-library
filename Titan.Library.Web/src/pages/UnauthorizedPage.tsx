import { Link } from 'react-router-dom';
import { ShieldX } from 'lucide-react';
import { Button } from '@/components/ui/Button';
import { useAuth } from '@/hooks/useAuth';

export function UnauthorizedPage() {
  const { user } = useAuth();
  const home = user?.role === 'admin' ? '/admin/books' : user?.role === 'customer' ? '/customer/books' : '/author/my-books';

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center">
      <div className="text-center">
        <ShieldX className="mx-auto text-red-500 mb-4" size={64} />
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">Unauthorized</h1>
        <p className="text-gray-500 dark:text-gray-400 mb-8">You don't have permission to access this page.</p>
        <Link to={home}><Button>Go to Dashboard</Button></Link>
      </div>
    </div>
  );
}
