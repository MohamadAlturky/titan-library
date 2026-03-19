import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout';
import { RoleGuard } from '@/components/auth/RoleGuard';
import { LoginPage } from '@/pages/LoginPage';
import { NotFoundPage } from '@/pages/NotFoundPage';
import { UnauthorizedPage } from '@/pages/UnauthorizedPage';
import { AdminBooksPage } from '@/pages/admin/AdminBooksPage';
import { AdminBorrowsPage } from '@/pages/admin/AdminBorrowsPage';
import { AdminUsersPage } from '@/pages/admin/AdminUsersPage';
import { AdminAuthorsPage } from '@/pages/admin/AdminAuthorsPage';
import { CustomerBooksPage } from '@/pages/customer/CustomerBooksPage';
import { CustomerMyBorrowsPage } from '@/pages/customer/CustomerMyBorrowsPage';
import { BorrowBookPage } from '@/pages/customer/BorrowBookPage';
import { ReturnBookPage } from '@/pages/customer/ReturnBookPage';
import { AuthorMyBooksPage } from '@/pages/author/AuthorMyBooksPage';
import { AuthorMyBorrowedPage } from '@/pages/author/AuthorMyBorrowedPage';
import { useAuth } from '@/hooks/useAuth';

function RootRedirect() {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  if (user.role === 'admin') return <Navigate to="/admin/books" replace />;
  if (user.role === 'customer') return <Navigate to="/customer/books" replace />;
  return <Navigate to="/author/my-books" replace />;
}

export const router = createBrowserRouter([
  { path: '/login', element: <LoginPage /> },
  { path: '/unauthorized', element: <UnauthorizedPage /> },
  {
    path: '/admin',
    element: <RoleGuard allowedRole="admin" />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { index: true, element: <Navigate to="/admin/books" replace /> },
          { path: 'books', element: <AdminBooksPage /> },
          { path: 'borrows', element: <AdminBorrowsPage /> },
          { path: 'users', element: <AdminUsersPage /> },
          { path: 'authors', element: <AdminAuthorsPage /> },
        ],
      },
    ],
  },
  {
    path: '/customer',
    element: <RoleGuard allowedRole="customer" />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { index: true, element: <Navigate to="/customer/books" replace /> },
          { path: 'books', element: <CustomerBooksPage /> },
          { path: 'my-borrows', element: <CustomerMyBorrowsPage /> },
          { path: 'borrow', element: <BorrowBookPage /> },
          { path: 'return', element: <ReturnBookPage /> },
        ],
      },
    ],
  },
  {
    path: '/author',
    element: <RoleGuard allowedRole="author" />,
    children: [
      {
        element: <AppLayout />,
        children: [
          { index: true, element: <Navigate to="/author/my-books" replace /> },
          { path: 'my-books', element: <AuthorMyBooksPage /> },
          { path: 'my-borrowed', element: <AuthorMyBorrowedPage /> },
        ],
      },
    ],
  },
  { path: '/', element: <RootRedirect /> },
  { path: '*', element: <NotFoundPage /> },
]);
