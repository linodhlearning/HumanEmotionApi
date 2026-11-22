import React, { useEffect, useRef } from 'react';

interface HtmxEmotionsViewProps {
  humanId: string;
}

export const HtmxEmotionsView: React.FC<HtmxEmotionsViewProps> = ({ humanId }) => {
  const containerRef = useRef<HTMLDivElement>(null);
  const fragmentUrl = `/api/humans/${humanId}/emotions/fragment`;

  const loadFragment = () => {
    const win = window as any;
    if (win.htmx) {
      win.htmx.config.selfRequestsOnly = false;
      win.htmx.ajax('GET', fragmentUrl, {
        target: '#htmx-emotions-container',
        swap: 'innerHTML',
      });
    }
  };

  useEffect(() => {
    // Disable self-requests-only restrictions and load the HTML fragment directly
    loadFragment();
  }, [humanId, fragmentUrl]);

  return (
    <div style={{ marginTop: '16px' }}>
      {/* HTMX Info Banner */}
      <div
        style={{
          backgroundColor: '#eff6ff',
          border: '1px solid #bfdbfe',
          borderRadius: '10px',
          padding: '16px',
          marginBottom: '20px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          flexWrap: 'wrap',
          gap: '12px',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'flex-start', gap: '12px' }}>
          <span style={{ fontSize: '24px' }}>⚡</span>
          <div>
            <div style={{ fontWeight: 700, color: '#1e40af', fontSize: '15px' }}>
              HTMX Server-Driven Mode Active
            </div>
            <div style={{ color: '#3b82f6', fontSize: '13px', marginTop: '4px', lineHeight: '1.5' }}>
              This panel is rendered and updated via <strong>HTMX</strong> by fetching and swapping
              server-rendered HTML fragments from{' '}
              <code style={{ background: '#dbeafe', padding: '1px 5px', borderRadius: '4px' }}>
                {fragmentUrl}
              </code>
              . Add, update, and remove actions swap the HTML in real-time with zero React state!
            </div>
          </div>
        </div>
        <button
          onClick={loadFragment}
          style={{
            backgroundColor: '#2563eb',
            color: '#ffffff',
            border: 'none',
            borderRadius: '6px',
            padding: '8px 14px',
            cursor: 'pointer',
            fontSize: '13px',
            fontWeight: 600,
            whiteSpace: 'nowrap',
          }}
        >
          🔄 Refresh HTMX Fragment
        </button>
      </div>

      {/* HTMX Mount Target */}
      <div ref={containerRef}>
        <div
          id="htmx-emotions-container"
          style={{ minHeight: '180px' }}
        >
          <div style={{ textAlign: 'center', padding: '40px', color: '#64748b' }}>
            <p>⚡ Loading server-rendered emotions fragment via HTMX...</p>
          </div>
        </div>
      </div>
    </div>
  );
};
