import { createBrowserRouter, Navigate } from 'react-router-dom';
import { AppLayout } from '@/components/layout/AppLayout';
import { CustomerLayout } from '@/components/layout/CustomerLayout';
import { RoleGuard } from '@/components/auth/RoleGuard';
import { LoginPage } from '@/pages/LoginPage';
import { RegisterPage } from '@/pages/RegisterPage';
import { NotFoundPage } from '@/pages/NotFoundPage';
import { UnauthorizedPage } from '@/pages/UnauthorizedPage';
import { AdminBooksPage } from '@/pages/admin/AdminBooksPage';
import { AdminBorrowsPage } from '@/pages/admin/AdminBorrowsPage';
import { AdminUsersPage } from '@/pages/admin/AdminUsersPage';
import { AdminAuthorsPage } from '@/pages/admin/AdminAuthorsPage';
import { CustomerBooksPage } from '@/pages/customer/CustomerBooksPage';
import { CustomerBookDetailPage } from '@/pages/customer/CustomerBookDetailPage';
import { CustomerMyBorrowsPage } from '@/pages/customer/CustomerMyBorrowsPage';
import { AboutUsPage } from '@/pages/customer/AboutUsPage';
import { SendFeedbackPage } from '@/pages/customer/SendFeedbackPage';
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
  { path: '/register', element: <RegisterPage /> },
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
        element: <CustomerLayout />,
        children: [
          { index: true, element: <Navigate to="/customer/books" replace /> },
          { path: 'books', element: <CustomerBooksPage /> },
          { path: 'books/:id', element: <CustomerBookDetailPage /> },
          { path: 'my-borrows', element: <CustomerMyBorrowsPage /> },
          { path: 'about',     element: <AboutUsPage /> },
          { path: 'feedback',  element: <SendFeedbackPage /> },
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
