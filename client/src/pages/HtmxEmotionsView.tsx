import React, { useEffect, useRef } from 'react';

interface HtmxEmotionsViewProps {
  humanId: string;
}

export const HtmxEmotionsView: React.FC<HtmxEmotionsViewProps> = ({ humanId }) => {
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    // Process HTMX attributes when component mounts or humanId changes
    const win = window as any;
    if (win.htmx && containerRef.current) {
      win.htmx.process(containerRef.current);
    }
  }, [humanId]);

  const fragmentUrl = `http://localhost:5050/api/humans/${humanId}/emotions/fragment`;

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
          alignItems: 'flex-start',
          gap: '12px',
        }}
      >
        <span style={{ fontSize: '24px' }}>⚡</span>
        <div>
          <div style={{ fontWeight: 700, color: '#1e40af', fontSize: '15px' }}>
            HTMX Mode Active
          </div>
          <div style={{ color: '#3b82f6', fontSize: '13px', marginTop: '4px', lineHeight: '1.5' }}>
            This entire UI panel is rendered and updated via <strong>HTMX</strong> by fetching and swapping
            server-rendered HTML fragments from{' '}
            <code style={{ background: '#dbeafe', padding: '1px 5px', borderRadius: '4px' }}>
              /api/humans/{humanId}/emotions/fragment
            </code>
            . No React state or client-side stores are involved in mutations here!
          </div>
        </div>
      </div>

      {/* HTMX Mount Target */}
      <div ref={containerRef}>
        <div
          id="htmx-emotions-container"
          {...{
            'hx-get': fragmentUrl,
            'hx-trigger': 'load',
            'hx-swap': 'innerHTML',
          }}
          style={{ minHeight: '180px' }}
        >
          <div style={{ textAlign: 'center', padding: '40px', color: '#64748b' }}>
            <p>⚡ Fetching server-rendered HTML fragment via HTMX...</p>
          </div>
        </div>
      </div>
    </div>
  );
};
