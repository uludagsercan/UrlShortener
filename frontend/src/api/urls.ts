import client from './client';

export interface ShortUrl {
  id: string;
  shortCode: string;
  shortUrl: string;
  originalUrl: string;
  clickCount: number;
  createdAt: string;
  expiresAt?: string;
}

// Giriş yapmış kullanıcı için
export const createShortUrl = async (
  originalUrl: string,
  customAlias?: string
): Promise<ShortUrl> => {
  const { data } = await client.post('/api/urls', { originalUrl, customAlias });
  return data;
};

// Anonim kullanıcı için
export const createShortUrlAnonymous = async (
  originalUrl: string,
  customAlias?: string
): Promise<ShortUrl> => {
  const { data } = await client.post('/api/urls/anonymous', { originalUrl, customAlias });
  return data;
};

export const getUserUrls = async (): Promise<ShortUrl[]> => {
  const { data } = await client.get('/api/urls');
  return data;
};

export const deleteUrl = async (id: string): Promise<void> => {
  await client.delete(`/api/urls/${id}`);
};