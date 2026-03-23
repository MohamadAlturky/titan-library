import { NavLink } from 'react-router-dom';
import { BookOpen, Users, BookMarked, Library, BookCopy, RotateCcw, MessageSquare } from 'lucide-react';
import { useAuth } from '@/hooks/useAuth';
import { cn } from '@/lib/utils';

const adminLinks = [
  { to: '/admin/books', label: 'Books', icon: BookOpen },
  // { to: '/admin/borrows', label: 'Borrows', icon: BookMarked },
  { to: '/admin/users', label: 'Users', icon: Users },
  // { to: '/admin/authors', label: 'Authors', icon: UserSquare },
  { to: '/admin/messages', label: 'Messages', icon: MessageSquare },
];

const customerLinks = [
  { to: '/customer/books', label: 'Browse Books', icon: BookOpen },
  { to: '/customer/my-borrows', label: 'My Borrows', icon: BookMarked },
  { to: '/customer/borrow', label: 'Borrow a Book', icon: BookCopy },
  { to: '/customer/return', label: 'Return a Book', icon: RotateCcw },
];

const authorLinks = [
  { to: '/author/my-books', label: 'My Books', icon: BookOpen },
  { to: '/author/my-borrowed', label: 'My Borrowed', icon: BookMarked },
];

export function Sidebar() {
  const { user } = useAuth();

  const links = user?.role === 'admin'
    ? adminLinks
    : user?.role === 'customer'
    ? customerLinks
    : authorLinks;

  return (
    <aside className="w-64 bg-white dark:bg-zinc-900 border-r border-gray-200 dark:border-zinc-800 flex flex-col">
      <div className="px-6 py-5 border-b border-gray-200 dark:border-zinc-800">
        <div className="flex items-center gap-2">
          <Library className="text-indigo-600" size={24} />
          <span className="text-lg font-bold text-gray-900 dark:text-zinc-100">Titan Library</span>
        </div>
      </div>
      <nav className="flex-1 px-3 py-4 space-y-1">
        {links.map(({ to, label, icon: Icon }) => (
          <NavLink
            key={to}
            to={to}
            className={({ isActive }) => cn(
              'flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors',
              isActive
                ? 'bg-indigo-600 text-white'
                : 'text-gray-600 dark:text-zinc-400 hover:bg-gray-100 dark:hover:bg-zinc-800 hover:text-gray-900 dark:hover:text-zinc-100'
            )}
          >
            <Icon size={18} />
            {label}
          </NavLink>
        ))}
      </nav>
      <div className="px-6 py-4 border-t border-gray-200 dark:border-zinc-800">
        <div className="text-xs text-gray-400 dark:text-zinc-500 capitalize">
          Logged in as <span className="font-semibold text-gray-600 dark:text-zinc-400">{user?.role}</span>
        </div>
      </div>
    </aside>
  );
}
