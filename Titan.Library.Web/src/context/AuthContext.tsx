// eslint-disable-next-line react-refresh/only-export-components
import { createContext, useState, type ReactNode } from 'react';
import type { AuthUser } from '@/types';
import { getTokenCookie, removeTokenCookie, setTokenCookie } from '@/lib/api';

interface AuthContextType {
  user: AuthUser | null;
  login: (user: AuthUser) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

export const AuthContext = createContext<AuthContextType>({
  user: null,
  login: () => {},
  logout: () => {},
  isAuthenticated: false,
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem('titan_user');
    if (!stored) return null;
    const parsed = JSON.parse(stored) as AuthUser;
    // sync token cookie in case it was cleared
    if (parsed.token && !getTokenCookie()) setTokenCookie(parsed.token);
    return parsed;
  });

  const login = (u: AuthUser) => {
    setUser(u);
    localStorage.setItem('titan_user', JSON.stringify(u));
    setTokenCookie(u.token);
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('titan_user');
    removeTokenCookie();
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, isAuthenticated: !!user }}>
      {children}
    </AuthContext.Provider>
  );
}
