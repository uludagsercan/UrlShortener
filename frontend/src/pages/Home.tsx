import { useState } from 'react';
import { Link } from 'react-router-dom';
import { createShortUrl, createShortUrlAnonymous } from '../api/urls';
import type { ShortUrl } from '../api/urls';
import { useAuthStore } from '../store/authStore';

export default function Home() {
  const { isAuthenticated } = useAuthStore();
  const [originalUrl, setOriginalUrl] = useState('');
  const [customAlias, setCustomAlias] = useState('');
  const [result, setResult] = useState<ShortUrl | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [copied, setCopied] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setResult(null);
    try {
      // Giriş yapmışsa authorized endpoint, yoksa anonymous
      const data = isAuthenticated()
        ? await createShortUrl(originalUrl, customAlias || undefined)
        : await createShortUrlAnonymous(originalUrl, customAlias || undefined);

      setResult(data);
    } catch {
      setError('Link kısaltılamadı. Tekrar dene.');
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = () => {
    if (result) {
      navigator.clipboard.writeText(result.shortUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      {/* Navbar */}
      <nav className="bg-white border-b border-gray-100 px-6 py-4">
        <div className="max-w-2xl mx-auto flex items-center justify-between">
          <span className="text-lg font-bold text-blue-600">URL Kısaltıcı</span>
          <div className="flex items-center gap-3">
            {isAuthenticated() ? (
              <Link
                to="/dashboard"
                className="text-sm text-blue-600 hover:underline"
              >
                Dashboard →
              </Link>
            ) : (
              <>
                <Link to="/login" className="text-sm text-gray-600 hover:text-gray-800">
                  Giriş Yap
                </Link>
                <Link
                  to="/register"
                  className="text-sm bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
                >
                  Kayıt Ol
                </Link>
              </>
            )}
          </div>
        </div>
      </nav>

      <div className="max-w-2xl mx-auto pt-16 px-4">
        <div className="text-center mb-10">
          <h1 className="text-4xl font-bold text-gray-800 mb-3">URL Kısaltıcı</h1>
          <p className="text-gray-500">Uzun linkleri saniyeler içinde kısalt</p>
        </div>

        <div className="bg-white rounded-2xl shadow-lg p-8">
          {/* Giriş yapmamış kullanıcıya bilgi */}
          {!isAuthenticated() && (
            <div className="bg-blue-50 text-blue-700 p-3 rounded-lg mb-4 text-sm">
              💡 Linklerini kaydetmek ve yönetmek için{' '}
              <Link to="/register" className="font-medium underline">kayıt ol</Link>
            </div>
          )}

          {error && (
            <div className="bg-red-50 text-red-600 p-3 rounded-lg mb-4 text-sm">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Uzun URL
              </label>
              <input
                type="url"
                value={originalUrl}
                onChange={(e) => setOriginalUrl(e.target.value)}
                placeholder="https://example.com/cok-uzun-bir-url"
                className="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Özel alias <span className="text-gray-400">(opsiyonel)</span>
              </label>
              <div className="flex items-center border border-gray-300 rounded-lg overflow-hidden focus-within:ring-2 focus-within:ring-blue-500">
                <span className="bg-gray-50 px-3 py-3 text-gray-500 text-sm border-r border-gray-300">
                  localhost:8080/
                </span>
                <input
                  type="text"
                  value={customAlias}
                  onChange={(e) => setCustomAlias(e.target.value)}
                  placeholder="benim-linklerim"
                  className="flex-1 px-3 py-3 focus:outline-none"
                />
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 disabled:opacity-50 font-medium text-lg"
            >
              {loading ? 'Kısaltılıyor...' : 'Kısalt'}
            </button>
          </form>

          {result && (
            <div className="mt-6 p-4 bg-green-50 rounded-xl border border-green-200">
              <p className="text-sm text-green-700 font-medium mb-2">✅ Link hazır!</p>
              <div className="flex items-center gap-2">
                <a
                  href={result.shortUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-blue-600 hover:underline font-medium flex-1 truncate"
                >
                  {result.shortUrl}
                </a>
                <button
                  onClick={handleCopy}
                  className="bg-white border border-gray-300 px-3 py-1 rounded-lg text-sm hover:bg-gray-50"
                >
                  {copied ? '✅ Kopyalandı' : 'Kopyala'}
                </button>
              </div>
              {/* Giriş yapmamışsa kayıt olmayı öner */}
              {!isAuthenticated() && (
                <p className="text-xs text-gray-400 mt-2">
                  Bu link kaydedilmedi.{' '}
                  <Link to="/register" className="text-blue-500 underline">
                    Kayıt ol
                  </Link>{' '}
                  ve linklerini yönet.
                </p>
              )}
            </div>
          )}
        </div>

        <div className="text-center mt-6">
          {isAuthenticated() && (
            <Link to="/dashboard" className="text-blue-600 hover:underline text-sm">
              Linklerimi gör →
            </Link>
          )}
        </div>
      </div>
    </div>
  );
}