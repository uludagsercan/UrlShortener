import { create } from 'zustand';


interface AuthState {
  token: string | null;
  email: string | null;
  setAuth: (token: string, email: string) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  token: localStorage.getItem('token'),
  email: localStorage.getItem('email'),

  setAuth: (token, email) => {
    localStorage.setItem('token', token);
    localStorage.setItem('email', email);
    set({ token, email });
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    set({ token: null, email: null });
  },

  isAuthenticated: () => !!get().token,
}));