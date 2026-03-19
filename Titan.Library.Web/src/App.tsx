import { RouterProvider } from 'react-router-dom';
import { Toaster } from 'sonner';
import { router } from '@/router';
import { useTheme } from '@/hooks/useTheme';

function AppContent() {
  const { theme } = useTheme();
  return (
    <>
      <RouterProvider router={router} />
      <Toaster position="top-right" richColors theme={theme} />
    </>
  );
}

export default function App() {
  return <AppContent />;
}
