import React from 'react';
import type { EmotionDto } from '../types';
import { EMOTION_EMOJIS, EMOTION_COLORS, NAME_TO_EMOTION_TYPE, type EmotionType } from '../types';

interface EmotionBarProps {
  emotion: EmotionDto;
}

export const EmotionBar: React.FC<EmotionBarProps> = ({ emotion }) => {
  const typeKey = (NAME_TO_EMOTION_TYPE[emotion.type] || 1) as EmotionType;
  const emoji = EMOTION_EMOJIS[typeKey] || '🎭';
  const color = EMOTION_COLORS[typeKey] || '#64748b';

  return (
    <div style={{ width: '100%' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '4px' }}>
        <span style={{ fontSize: '14px', fontWeight: 600, color: '#334155', display: 'flex', alignItems: 'center', gap: '6px' }}>
          <span>{emoji}</span>
          <span>{emotion.type}</span>
        </span>
        <span style={{ fontSize: '13px', fontWeight: 700, color }}>
          {Math.round(emotion.percentage)}%
        </span>
      </div>
      <div
        style={{
          width: '100%',
          height: '8px',
          backgroundColor: '#e2e8f0',
          borderRadius: '9999px',
          overflow: 'hidden',
        }}
      >
        <div
          style={{
            width: `${Math.min(Math.max(emotion.percentage, 0), 100)}%`,
            height: '100%',
            backgroundColor: color,
            borderRadius: '9999px',
            transition: 'width 0.4s ease',
          }}
        />
      </div>
    </div>
  );
};

