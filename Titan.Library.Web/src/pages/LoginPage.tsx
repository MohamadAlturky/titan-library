// import { useState } from 'react';
// import { Link, useNavigate } from 'react-router-dom';
// import { Library } from 'lucide-react';
// import { toast } from 'sonner';
// import { useAuth } from '@/hooks/useAuth';
// import { Button } from '@/components/ui/Button';
// import { userAuthenticationService } from '@/services/userAuthenticationService';
// import { setTokenCookie } from '@/lib/api';
// import type { UserRole } from '@/types';

// function mapUserType(userType: string): UserRole {
//   if (userType === 'admin') return 'admin';
//   if (userType === 'author') return 'author';
//   return 'customer';
// }

// export function LoginPage() {
//   const [email, setEmail] = useState('');
//   const [password, setPassword] = useState('');
//   const [loading, setLoading] = useState(false);
//   const { login } = useAuth();
//   const navigate = useNavigate();

//   const handleLogin = async (e: React.FormEvent) => {
//     e.preventDefault();
//     setLoading(true);
//     try {
//       const tokenRes = await userAuthenticationService.login({ email, password });
//       const { token, userId, userType } = tokenRes.data;

//       setTokenCookie(token);

//       const role = mapUserType(userType);
//       login({ id: String(userId), name: email, email, role, token });

//       if (role === 'admin') navigate('/admin/books');
//       else if (role === 'author') navigate('/author/my-books');
//       else navigate('/customer/books');
//     } catch (err: unknown) {
//       const message =
//         (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
//         'Invalid email or password';
//       toast.error(message);
//     } finally {
//       setLoading(false);
//     }
//   };

//   return (
//     <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center">
//       <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-200 dark:border-gray-700 p-8 w-full max-w-md">
//         <div className="flex flex-col items-center mb-8">
//           <div className="w-14 h-14 bg-indigo-600 rounded-2xl flex items-center justify-center mb-4">
//             <Library className="text-white" size={28} />
//           </div>
//           <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Titan Library</h1>
//           <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">Sign in to your account</p>
//         </div>

//         <form onSubmit={handleLogin} className="space-y-4">
//           <div>
//             <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
//               Email
//             </label>
//             <input
//               type="email"
//               value={email}
//               onChange={e => setEmail(e.target.value)}
//               required
//               placeholder="you@example.com"
//               className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
//             />
//           </div>

//           <div>
//             <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
//               Password
//             </label>
//             <input
//               type="password"
//               value={password}
//               onChange={e => setPassword(e.target.value)}
//               required
//               placeholder="••••••••"
//               className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500"
//             />
//           </div>

//           <Button type="submit" disabled={loading} className="w-full">
//             {loading ? 'Signing in…' : 'Sign In'}
//           </Button>
//         </form>

//         <p className="mt-6 text-center text-sm text-gray-500 dark:text-gray-400">
//           Don't have an account?{' '}
//           <Link to="/register" className="text-indigo-600 hover:underline font-medium">
//             Register
//           </Link>
//         </p>
//       </div>
//     </div>
//   );
// }

import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Library, ArrowRight, ShieldCheck } from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui/Button';
import { userAuthenticationService } from '@/services/userAuthenticationService';
import { setTokenCookie } from '@/lib/api';
import type { UserRole } from '@/types';

function mapUserType(userType: string): UserRole {
  if (userType.toLowerCase() === 'admin') return 'admin';
  if (userType.toLowerCase() === 'author') return 'author';
  return 'customer';
}

export function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const tokenRes = await userAuthenticationService.login({ email, password });
      const { token, userId, userType } = tokenRes.data;
      console.log(tokenRes.data);

      setTokenCookie(token);

      const role = mapUserType(userType);
      login({ id: String(userId), name: email, email, role, token });

      if (role.toLowerCase() === 'admin') navigate('/admin/books');
      else if (role.toLowerCase() === 'author') navigate('/author/my-books');
      else navigate('/customer/books');
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        'Invalid email or password';
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 flex items-center justify-center p-4">
      <div className="flex flex-col lg:flex-row w-full max-w-5xl bg-white dark:bg-gray-800 rounded-3xl shadow-2xl overflow-hidden border border-gray-200 dark:border-gray-700">

        {/* Form Side */}
        <div className="w-full lg:w-1/2 p-8 lg:p-12">
          <div className="flex items-center gap-3 mb-12">
            <div className="w-10 h-10 bg-indigo-600 rounded-xl flex items-center justify-center">
              <Library className="text-white" size={20} />
            </div>
            <h1 className="text-xl font-bold text-gray-900 dark:text-gray-100">Titan Library</h1>
          </div>

          <div className="mb-8">
            <h2 className="text-3xl font-bold text-gray-900 dark:text-white">Welcome Back</h2>
            <p className="text-gray-500 dark:text-gray-400 mt-2">Enter your credentials to access your library.</p>
          </div>

          <form onSubmit={handleLogin} className="space-y-5">
            <div>
              <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
                Email Address
              </label>
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
              <div className="flex justify-between items-center mb-1.5">
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                  Password
                </label>
                <a href="#" className="text-xs text-indigo-600 hover:underline">Forgot password?</a>
              </div>
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
              {loading ? 'Signing in...' : 'Sign In'}
            </Button>
          </form>

          <p className="mt-8 text-center text-sm text-gray-500 dark:text-gray-400">
            Don't have an account?{' '}
            <Link to="/register" className="text-indigo-600 hover:underline font-semibold">
              Create an account
            </Link>
          </p>
        </div>

        {/* Info Side (Blue Theme) */}
        <div className="hidden lg:flex w-1/2 bg-indigo-600 p-12 text-white flex-col justify-center relative overflow-hidden">
          {/* Decorative Elements */}
          <div className="absolute -bottom-24 -left-24 w-64 h-64 bg-indigo-500 rounded-full opacity-50" />
          <div className="absolute top-10 right-10 opacity-10">
            <Library size={200} />
          </div>

          <div className="relative z-10">
            <ShieldCheck className="mb-6 opacity-80" size={48} />
            <h3 className="text-4xl font-bold leading-tight mb-6">
              Your personal gateway to knowledge.
            </h3>
            <p className="text-indigo-100 text-lg mb-8">
              Log in to sync your reading progress, manage your publications, or oversee library operations across all your devices.
            </p>

            <div className="space-y-4">
              <div className="flex items-center gap-3 text-indigo-100">
                <div className="w-8 h-8 rounded-full bg-indigo-500 flex items-center justify-center flex-shrink-0">
                  <ArrowRight size={16} />
                </div>
                <span>Secure multi-factor authentication</span>
              </div>
              <div className="flex items-center gap-3 text-indigo-100">
                <div className="w-8 h-8 rounded-full bg-indigo-500 flex items-center justify-center flex-shrink-0">
                  <ArrowRight size={16} />
                </div>
                <span>Access to 50,000+ digital titles</span>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  );
}