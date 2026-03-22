import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import {
  Library, BookOpen, BookMarked, BookCopy, RotateCcw,
  Sun, Moon, LogOut,
} from 'lucide-react';
import { useAuth } from '@/hooks/useAuth';
import { useTheme } from '@/hooks/useTheme';
import { cn } from '@/lib/utils';

const navLinks = [
  { to: '/customer/books',      label: 'Browse',     icon: BookOpen   },
  { to: '/customer/my-borrows', label: 'My Library', icon: BookMarked },
  { to: '/customer/borrow',     label: 'Borrow',     icon: BookCopy   },
  { to: '/customer/return',     label: 'Return',     icon: RotateCcw  },
];

export function CustomerLayout() {
  const { user, logout } = useAuth();
  const { theme, toggleTheme } = useTheme();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-zinc-950">

      {/* ── Navbar ──────────────────────────────────────────────────────────── */}
      <header className="sticky top-0 z-50 w-full border-b border-gray-200/80 dark:border-zinc-800/80 bg-white/80 dark:bg-zinc-950/80 backdrop-blur-md">
        <div className="max-w-5xl mx-auto px-4 h-16 flex items-center justify-between gap-4">

          {/* Left — Wordmark */}
          <div className="flex items-center gap-2 shrink-0">
            <div className="p-1.5 bg-indigo-600 rounded-lg">
              <Library size={18} className="text-white" />
            </div>
            <span className="text-base font-bold text-gray-900 dark:text-zinc-100 hidden sm:block">
              Titan Library
            </span>
          </div>

          {/* Center — Nav links */}
          <nav className="flex items-center gap-1">
            {navLinks.map(({ to, label, icon: Icon }) => (
              <NavLink
                key={to}
                to={to}
                className={({ isActive }) => cn(
                  'flex items-center gap-1.5 px-3 py-2 rounded-full text-sm font-medium transition-all duration-150',
                  isActive
                    ? 'bg-indigo-600 text-white shadow-sm shadow-indigo-500/30'
                    : 'text-gray-500 dark:text-zinc-400 hover:bg-gray-100 dark:hover:bg-zinc-800 hover:text-gray-900 dark:hover:text-zinc-100'
                )}
              >
                <Icon size={16} />
                <span className="hidden md:block">{label}</span>
              </NavLink>
            ))}
          </nav>

          {/* Right — Controls */}
          <div className="flex items-center gap-2 shrink-0">
            {/* Theme toggle */}
            <button
              onClick={toggleTheme}
              className="p-2 rounded-full text-gray-500 dark:text-zinc-400 hover:bg-gray-100 dark:hover:bg-zinc-800 hover:text-gray-900 dark:hover:text-zinc-100 transition-colors"
              aria-label="Toggle theme"
            >
              {theme === 'light' ? <Moon size={18} /> : <Sun size={18} />}
            </button>

            {/* Avatar + name */}
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white text-xs font-bold shrink-0 shadow-sm">
                {user?.name.charAt(0).toUpperCase()}
              </div>
              <span className="hidden lg:block text-sm font-medium text-gray-700 dark:text-zinc-300">
                {user?.name}
              </span>
            </div>

            {/* Logout */}
            <button
              onClick={handleLogout}
              className="flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm font-medium text-gray-500 dark:text-zinc-400 hover:bg-gray-100 dark:hover:bg-zinc-800 hover:text-gray-900 dark:hover:text-zinc-100 transition-colors"
            >
              <LogOut size={16} />
              <span className="hidden sm:block">Logout</span>
            </button>
          </div>

        </div>
      </header>

      {/* ── Page content ────────────────────────────────────────────────────── */}
      <main className="max-w-5xl mx-auto px-4 py-8">
        <Outlet />
      </main>

    </div>
  );
}
