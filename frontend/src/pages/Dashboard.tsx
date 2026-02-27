import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { getUserUrls, deleteUrl } from '../api/urls';
import type { ShortUrl } from '../api/urls';
import { useAuthStore } from '../store/authStore';

export default function Dashboard() {
  const navigate = useNavigate();
  const { email, logout, isAuthenticated } = useAuthStore();
  const [urls, setUrls] = useState<ShortUrl[]>([]);
  const [loading, setLoading] = useState(true);
  const [copiedId, setCopiedId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login');
      return;
    }
    fetchUrls();
  }, []);

  const fetchUrls = async () => {
    try {
      const data = await getUserUrls();
      setUrls(data);
    } catch {
      console.error('Linkler yüklenemedi');
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = (url: ShortUrl) => {
    navigator.clipboard.writeText(url.shortUrl);
    setCopiedId(url.id);
    setTimeout(() => setCopiedId(null), 2000);
  };

  const handleDelete = async (id: string) => {
    if (!confirm('Bu linki silmek istediğine emin misin?')) return;
    setDeletingId(id);
    try {
      await deleteUrl(id);
      setUrls(prev => prev.filter(u => u.id !== id));
    } catch {
      alert('Silme işlemi başarısız.');
    } finally {
      setDeletingId(null);
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Navbar */}
      <nav className="bg-white border-b border-gray-200 px-6 py-4">
        <div className="max-w-5xl mx-auto flex items-center justify-between">
          <Link to="/" className="text-xl font-bold text-blue-600">
            URL Kısaltıcı
          </Link>
          <div className="flex items-center gap-4">
            <span className="text-sm text-gray-500">{email}</span>
            <button
              onClick={handleLogout}
              className="text-sm text-red-500 hover:text-red-700"
            >
              Çıkış Yap
            </button>
          </div>
        </div>
      </nav>

      <div className="max-w-5xl mx-auto px-6 py-8">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-2xl font-bold text-gray-800">Linklerim</h1>
          <Link
            to="/"
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 text-sm font-medium"
          >
            + Yeni Link
          </Link>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-2 gap-4 mb-8">
          <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
            <p className="text-sm text-gray-500">Toplam Link</p>
            <p className="text-3xl font-bold text-gray-800 mt-1">{urls.length}</p>
          </div>
          <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
            <p className="text-sm text-gray-500">Toplam Tıklanma</p>
            <p className="text-3xl font-bold text-gray-800 mt-1">
              {urls.reduce((sum, u) => sum + u.clickCount, 0)}
            </p>
          </div>
        </div>

        {/* Link Listesi */}
        {loading ? (
          <div className="text-center py-20 text-gray-400">Yükleniyor...</div>
        ) : urls.length === 0 ? (
          <div className="text-center py-20">
            <p className="text-gray-400 mb-4">Henüz hiç link oluşturmadın.</p>
            <Link to="/" className="text-blue-600 hover:underline text-sm">
              İlk linkini oluştur →
            </Link>
          </div>
        ) : (
          <div className="space-y-3">
            {urls.map(url => (
              <div
                key={url.id}
                className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 flex items-center gap-4"
              >
                {/* Link Bilgileri */}
                <div className="flex-1 min-w-0">
                  <a
                    href={url.shortUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-blue-600 font-medium hover:underline"
                  >
                    {url.shortUrl}
                  </a>
                  <p className="text-sm text-gray-400 truncate mt-1">
                    {url.originalUrl}
                  </p>
                  <p className="text-xs text-gray-400 mt-1">
                    {new Date(url.createdAt).toLocaleDateString('tr-TR')}
                  </p>
                </div>

                {/* Tıklanma */}
                <div className="text-center min-w-16">
                  <p className="text-2xl font-bold text-gray-700">{url.clickCount}</p>
                  <p className="text-xs text-gray-400">tıklanma</p>
                </div>

                {/* Aksiyonlar */}
                <div className="flex items-center gap-2">
                  <button
                    onClick={() => handleCopy(url)}
                    className="bg-gray-100 hover:bg-gray-200 px-3 py-2 rounded-lg text-sm text-gray-600"
                  >
                    {copiedId === url.id ? '✅' : '📋'}
                  </button>
                  <button
                    onClick={() => handleDelete(url.id)}
                    disabled={deletingId === url.id}
                    className="bg-red-50 hover:bg-red-100 px-3 py-2 rounded-lg text-sm text-red-500 disabled:opacity-50"
                  >
                    {deletingId === url.id ? '...' : '🗑️'}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}