import React from 'react';

interface ShowColorBadgeProps {
  color: string;
  score: number;
  size?: 'sm' | 'md' | 'lg';
  showHex?: boolean;
}

export const ShowColorBadge: React.FC<ShowColorBadgeProps> = ({
  color,
  score,
  size = 'md',
  showHex = false,
}) => {
  const getDimensions = () => {
    switch (size) {
      case 'sm':
        return { width: 28, height: 28, fontSize: 10, borderWidth: 2 };
      case 'lg':
        return { width: 120, height: 120, fontSize: 20, borderWidth: 4 };
      case 'md':
      default:
        return { width: 44, height: 44, fontSize: 12, borderWidth: 2.5 };
    }
  };

  const { width, height, fontSize, borderWidth } = getDimensions();

  // Determine contrasting text color based on perceived brightness or subtle shadow
  return (
    <div style={{ display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
      <div
        title={`ShowColor: ${color} (${score.toFixed(0)}%)`}
        style={{
          width: `${width}px`,
          height: `${height}px`,
          backgroundColor: color,
          borderRadius: '50%',
          border: `${borderWidth}px solid #cbd5e1`,
          boxShadow: '0 2px 6px rgba(0, 0, 0, 0.1)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          fontWeight: 700,
          fontSize: `${fontSize}px`,
          color: score > 60 ? '#ffffff' : '#1e293b',
          textShadow: score > 60 ? '0 1px 2px rgba(0,0,0,0.4)' : '0 1px 1px rgba(255,255,255,0.7)',
          transition: 'background-color 0.4s ease, transform 0.2s ease',
          flexShrink: 0,
        }}
      >
        {size !== 'sm' && `${Math.round(score)}%`}
      </div>
      {showHex && (
        <span style={{ fontFamily: 'monospace', fontSize: '13px', color: '#475569', fontWeight: 600 }}>
          {color}
        </span>
      )}
    </div>
  );
};
