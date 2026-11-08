import React from 'react';
import { Routes, Route, Link, Navigate } from 'react-router-dom';
import { HumansPage } from './pages/HumansPage';
import { HumanDetailPage } from './pages/HumanDetailPage';
import { EmotionsPage } from './pages/EmotionsPage';

export const App: React.FC = () => {
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', backgroundColor: '#f8fafc' }}>
      {/* Top Navbar */}
      <header
        style={{
          backgroundColor: '#0f172a',
          color: '#ffffff',
          padding: '0 24px',
          height: '64px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '24px' }}>
          <Link
            to="/"
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '10px',
              color: '#ffffff',
              textDecoration: 'none',
              fontWeight: 800,
              fontSize: '18px',
            }}
          >
            <span style={{ fontSize: '24px' }}>🧠</span>
            <span>Human Emotion API</span>
          </Link>

          <nav style={{ display: 'flex', gap: '16px' }}>
            <Link
              to="/"
              style={{
                color: '#cbd5e1',
                textDecoration: 'none',
                fontSize: '14px',
                fontWeight: 500,
                padding: '6px 12px',
                borderRadius: '6px',
                transition: 'color 0.15s ease',
              }}
            >
              All Humans
            </Link>
          </nav>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <a
            href="http://localhost:5050/swagger"
            target="_blank"
            rel="noreferrer"
            style={{
              color: '#94a3b8',
              textDecoration: 'none',
              fontSize: '13px',
              display: 'inline-flex',
              alignItems: 'center',
              gap: '6px',
              backgroundColor: '#1e293b',
              padding: '6px 12px',
              borderRadius: '6px',
            }}
          >
            <span>📖</span>
            <span>Swagger Docs</span>
          </a>
          <span
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '6px',
              backgroundColor: '#064e3b',
              color: '#6ee7b7',
              fontSize: '12px',
              padding: '4px 10px',
              borderRadius: '20px',
              fontWeight: 600,
            }}
          >
            <span
              style={{
                width: '8px',
                height: '8px',
                backgroundColor: '#10b981',
                borderRadius: '50%',
              }}
            />
            .NET 10 API
          </span>
        </div>
      </header>

      {/* Main Container */}
      <main style={{ flex: 1, maxWidth: '1000px', width: '100%', margin: '0 auto', padding: '32px 16px' }}>
        <Routes>
          <Route path="/" element={<HumansPage />} />
          <Route path="/:id" element={<HumanDetailPage />} />
          <Route path="/:id/emotions" element={<EmotionsPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>

      {/* Footer */}
      <footer
        style={{
          borderTop: '1px solid #e2e8f0',
          backgroundColor: '#ffffff',
          padding: '20px 24px',
          textAlign: 'center',
          fontSize: '13px',
          color: '#64748b',
        }}
      >
        <div>
          Built with <strong>React 19</strong>, <strong>HTMX</strong>, and <strong>.NET 10 LTS</strong> with CQRS + MediatR
        </div>
      </footer>
    </div>
  );
};

export default App;
