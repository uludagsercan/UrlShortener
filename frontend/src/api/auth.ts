import client from './client';

export interface AuthResponse {
  token: string;
  email: string;
}

export const register = async (email: string, password: string): Promise<AuthResponse> => {
  const { data } = await client.post('/api/auth/register', { email, password });
  return data;
};

export const login = async (email: string, password: string): Promise<AuthResponse> => {
  const { data } = await client.post('/api/auth/login', { email, password });
  return data;
};