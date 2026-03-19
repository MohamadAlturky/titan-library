import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Library } from 'lucide-react';
import { mockUsers } from '@/data/mockUsers';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/Button';
import type { UserRole } from '@/types';

const roleGroups: UserRole[] = ['admin', 'customer', 'author'];

export function LoginPage() {
  const [selectedId, setSelectedId] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = () => {
    const user = mockUsers.find(u => u.id === selectedId);
    if (!user) return;
    login(user);
    if (user.role === 'admin') navigate('/admin/books');
    else if (user.role === 'customer') navigate('/customer/books');
    else navigate('/author/my-books');
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center">
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-200 dark:border-gray-700 p-8 w-full max-w-md">
        <div className="flex flex-col items-center mb-8">
          <div className="w-14 h-14 bg-indigo-600 rounded-2xl flex items-center justify-center mb-4">
            <Library className="text-white" size={28} />
          </div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Titan Library</h1>
          <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">Sign in to your account</p>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Select User
            </label>
            <select
              value={selectedId}
              onChange={e => setSelectedId(e.target.value)}
              className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">-- Choose a user --</option>
              {roleGroups.map(role => (
                <optgroup key={role} label={role.charAt(0).toUpperCase() + role.slice(1)}>
                  {mockUsers
                    .filter(u => u.role === role)
                    .map(u => (
                      <option key={u.id} value={u.id}>{u.name} ({u.email})</option>
                    ))}
                </optgroup>
              ))}
            </select>
          </div>

          <Button
            onClick={handleLogin}
            disabled={!selectedId}
            className="w-full"
          >
            Sign In
          </Button>
        </div>
      </div>
    </div>
  );
}
