
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Library, User, PenTool } from 'lucide-react';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/Button';
import { userAuthenticationService } from '@/services/userAuthenticationService';
import { setTokenCookie } from '@/lib/api';
import type { UserRole } from '@/types';

function mapUserType(userType: string): UserRole {
  if (userType === 'author') return 'author';
  return 'customer';
}

export function RegisterPage() {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [userType, setUserType] = useState<'customer' | 'author'>('customer');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const tokenRes = await userAuthenticationService.register({ name, email, password, userType });
      const { token, userId } = tokenRes.data;

      setTokenCookie(token);
      const role = mapUserType(userType);
      login({ id: String(userId), name, email, role, token });

      if (role === 'author') navigate('/author/my-books');
      else navigate('/customer/books');
    } catch {
      // nothing to do
    } finally {
      setLoading(false);
    }
  };

  const isAuthor = userType === 'author';

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex flex-col">
      {/* Role Toggle Switcher */}
      <div className="flex justify-center p-6">
        <div className="bg-gray-200 dark:bg-gray-800 p-1 rounded-xl flex items-center gap-1 border border-gray-300 dark:border-gray-700">
          <button
            onClick={() => setUserType('customer')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-all flex items-center gap-2 ${!isAuthor ? 'bg-white dark:bg-gray-700 shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700'
              }`}
          >
            <User size={16} /> Customer
          </button>
          <button
            onClick={() => setUserType('author')}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-all flex items-center gap-2 ${isAuthor ? 'bg-white dark:bg-gray-700 shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700'
              }`}
          >
            <PenTool size={16} /> Author
          </button>
        </div>
      </div>

      <div className="flex-1 flex items-center justify-center p-4">
        <div className={`flex flex-col lg:flex-row w-full max-w-5xl bg-white dark:bg-gray-800 rounded-3xl shadow-2xl overflow-hidden border border-gray-200 dark:border-gray-700 transition-all duration-500 ${isAuthor ? 'lg:flex-row-reverse' : ''}`}>

          {/* Form Side */}
          <div className="w-full lg:w-1/2 p-8 lg:p-12">
            <div className="flex items-center gap-3 mb-8">
              <div className="w-10 h-10 bg-indigo-600 rounded-xl flex items-center justify-center">
                <Library className="text-white" size={20} />
              </div>
              <h1 className="text-xl font-bold text-gray-900 dark:text-gray-100">Titan Library</h1>
            </div>

            <div className="mb-8">
              <h2 className="text-3xl font-bold text-gray-900 dark:text-white">Join as {isAuthor ? 'an Author' : 'a Reader'}</h2>
              <p className="text-gray-500 dark:text-gray-400 mt-2">Start your journey with Titan Library today.</p>
            </div>

            <form onSubmit={handleRegister} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Full Name</label>
                <input
                  type="text"
                  value={name}
                  onChange={e => setName(e.target.value)}
                  required
                  placeholder="John Doe"
                  className="w-full px-4 py-3 rounded-xl border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-indigo-500 outline-none transition-all"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Email</label>
                <input
                  type="email"
                  value={email}
                  onChange={e => setEmail(e.target.value)}
                  required
                  placeholder="you@example.com"
                  className="w-full px-4 py-3 rounded-xl border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-indigo-500 outline-none transition-all"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Password</label>
                <input
                  type="password"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                  required
                  placeholder="••••••••"
                  className="w-full px-4 py-3 rounded-xl border border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-indigo-500 outline-none transition-all"
                />
              </div>

              <Button type="submit" disabled={loading} className="w-full py-6 text-lg rounded-xl mt-4">
                {loading ? 'Processing...' : 'Create Account'}
              </Button>
            </form>

            <p className="mt-8 text-center text-sm text-gray-500 dark:text-gray-400">
              Already have an account?{' '}
              <Link to="/login" className="text-indigo-600 hover:underline font-semibold">Sign In</Link>
            </p>
          </div>

          {/* Info Side (Blue Background) */}
          <div className="hidden lg:flex w-1/2 bg-indigo-600 p-12 text-white flex-col justify-center relative overflow-hidden">
            {/* Decorative background circle */}
            <div className="absolute -top-24 -right-24 w-64 h-64 bg-indigo-500 rounded-full opacity-50" />

            <div className="relative z-10">
              {isAuthor ? (
                <>
                  <h3 className="text-4xl font-bold leading-tight mb-6">Share your stories with the world.</h3>
                  <ul className="space-y-4 text-indigo-100">
                    <li className="flex items-center gap-3">✓ Publish and manage your digital library</li>
                    <li className="flex items-center gap-3">✓ Reach thousands of avid readers</li>
                    <li className="flex items-center gap-3">✓ Real-time analytics on book performance</li>
                  </ul>
                </>
              ) : (
                <>
                  <h3 className="text-4xl font-bold leading-tight mb-6">Discover your next favorite book.</h3>
                  <ul className="space-y-4 text-indigo-100">
                    <li className="flex items-center gap-3">✓ Access thousands of exclusive titles</li>
                    <li className="flex items-center gap-3">✓ Follow your favorite authors</li>
                    <li className="flex items-center gap-3">✓ Build your personal digital bookshelf</li>
                  </ul>
                </>
              )}
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}