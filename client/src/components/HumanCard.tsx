import React from 'react';
import { Link } from 'react-router-dom';
import type { HumanDto } from '../types';
import { ShowColorBadge } from './ShowColorBadge';
import { EMOTION_EMOJIS, NAME_TO_EMOTION_TYPE, type EmotionType } from '../types';

interface HumanCardProps {
  human: HumanDto;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export const HumanCard: React.FC<HumanCardProps> = ({ human, onDelete, isDeleting = false }) => {
  return (
    <div
      style={{
        backgroundColor: '#ffffff',
        borderRadius: '12px',
        border: '1px solid #e2e8f0',
        padding: '20px',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.04)',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        transition: 'transform 0.15s ease, box-shadow 0.15s ease',
      }}
    >
      <div>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '14px' }}>
          <div>
            <Link
              to={`/${human.id}`}
              style={{
                fontSize: '18px',
                fontWeight: 700,
                color: '#0f172a',
                textDecoration: 'none',
              }}
            >
              {human.name}
            </Link>
            <div style={{ fontSize: '12px', color: '#64748b', marginTop: '2px' }}>
              Added: {new Date(human.createdAtUtc).toLocaleDateString()}
            </div>
          </div>
          <ShowColorBadge color={human.showColor} score={human.emotionScorePercentage} size="md" />
        </div>

        <div style={{ marginBottom: '16px' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '13px', color: '#475569', marginBottom: '4px' }}>
            <span>Total Emotion Score</span>
            <span style={{ fontWeight: 700 }}>{human.emotionScorePercentage.toFixed(1)}%</span>
          </div>
          <div style={{ width: '100%', height: '6px', backgroundColor: '#f1f5f9', borderRadius: '4px', overflow: 'hidden' }}>
            <div
              style={{
                width: `${human.emotionScorePercentage}%`,
                height: '100%',
                backgroundColor: '#10b981',
                borderRadius: '4px',
              }}
            />
          </div>
        </div>

        {/* Emotions badges preview */}
        <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px', marginBottom: '18px' }}>
          {human.emotions.length === 0 ? (
            <span style={{ fontSize: '12px', color: '#94a3b8', fontStyle: 'italic' }}>
              No emotions added yet
            </span>
          ) : (
            human.emotions.map((emotion) => {
              const typeKey = (NAME_TO_EMOTION_TYPE[emotion.type] || 1) as EmotionType;
              const emoji = EMOTION_EMOJIS[typeKey] || '🎭';
              return (
                <span
                  key={emotion.id}
                  style={{
                    backgroundColor: '#f8fafc',
                    border: '1px solid #e2e8f0',
                    borderRadius: '20px',
                    padding: '3px 10px',
                    fontSize: '12px',
                    color: '#334155',
                    display: 'inline-flex',
                    alignItems: 'center',
                    gap: '4px',
                  }}
                >
                  <span>{emoji}</span>
                  <span style={{ fontWeight: 600 }}>{emotion.type}</span>
                  <span style={{ color: '#64748b' }}>{Math.round(emotion.percentage)}%</span>
                </span>
              );
            })
          )}
        </div>
      </div>

      <div style={{ display: 'flex', gap: '8px', borderTop: '1px solid #f1f5f9', paddingTop: '14px' }}>
        <Link
          to={`/${human.id}/emotions`}
          style={{
            flex: 1,
            backgroundColor: '#0f172a',
            color: '#ffffff',
            textAlign: 'center',
            padding: '8px 12px',
            borderRadius: '6px',
            textDecoration: 'none',
            fontSize: '13px',
            fontWeight: 600,
          }}
        >
          Manage Emotions
        </Link>
        <Link
          to={`/${human.id}`}
          style={{
            backgroundColor: '#f1f5f9',
            color: '#334155',
            padding: '8px 12px',
            borderRadius: '6px',
            textDecoration: 'none',
            fontSize: '13px',
            fontWeight: 600,
          }}
        >
          View
        </Link>
        <button
          onClick={() => {
            if (confirm(`Are you sure you want to delete ${human.name}?`)) {
              onDelete(human.id);
            }
          }}
          disabled={isDeleting}
          style={{
            backgroundColor: '#fee2e2',
            color: '#dc2626',
            border: 'none',
            padding: '8px 12px',
            borderRadius: '6px',
            cursor: 'pointer',
            fontSize: '13px',
            fontWeight: 600,
          }}
        >
          Delete
        </button>
      </div>
    </div>
  );
};

