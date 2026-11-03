'use client';
import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from 'react';
import { api, json } from '@/lib/api';
import type { User } from '@/lib/types';
interface AuthContext { user: User | null; loading: boolean; login: (path: string, body: unknown) => Promise<void>; logout: () => Promise<void>; reload: () => Promise<void> }
const Auth = createContext<AuthContext | null>(null);
const Toast = createContext<(message: string) => void>(() => {});
export const useAuth = () => useContext(Auth)!;
export const useToast = () => useContext(Toast);
export function Providers({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState('');
  const reload = useCallback(async () => { const data = await api<User>('/auth/me'); setUser(data); }, []);
  useEffect(() => { reload().catch(() => setUser(null)).finally(() => setLoading(false)); }, [reload]);
  useEffect(() => { if (!message) return; const t = setTimeout(() => setMessage(''), 4500); return () => clearTimeout(t); }, [message]);
  const login = async (path: string, body: unknown) => { await api(path, { method: 'POST', body: json(body) }); await reload(); };
  const logout = async () => { await api('/auth/logout', { method: 'POST', body: '{}' }); setUser(null); };
  return <Auth.Provider value={{ user, loading, login, logout, reload }}><Toast.Provider value={setMessage}>
    {children}<div className={`toast ${message ? 'visible' : ''}`} role="status" aria-live="polite">{message}</div>
  </Toast.Provider></Auth.Provider>;
}
