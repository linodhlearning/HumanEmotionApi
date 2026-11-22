import React, { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useHuman } from '../hooks/useHumans';
import { useEmotions, useAddEmotion, useUpdateEmotion, useDeleteEmotion } from '../hooks/useEmotions';
import { ShowColorBadge } from '../components/ShowColorBadge';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { ErrorMessage } from '../components/ErrorMessage';
import { HtmxEmotionsView } from './HtmxEmotionsView';
import {
  EMOTION_TYPES,
  EMOTION_LABELS,
  EMOTION_EMOJIS,
  EMOTION_COLORS,
  NAME_TO_EMOTION_TYPE,
  type EmotionType,
} from '../types';

export const EmotionsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [activeTab, setActiveTab] = useState<'react' | 'htmx'>('react');

  const { data: human, isLoading: isHumanLoading, error: humanError, refetch: refetchHuman } = useHuman(id);

  if (isHumanLoading) return <LoadingSpinner message="Loading emotions control center..." />;
  if (humanError || !human || !id) {
    return (
      <ErrorMessage
        message={`Could not load human profile: ${humanError ? (humanError as any).message : 'Not found'}`}
        onRetry={() => refetchHuman()}
      />
    );
  }

  return (
    <div>
      {/* Breadcrumb Navigation */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <div style={{ display: 'flex', gap: '12px', fontSize: '14px' }}>
          <Link to="/" style={{ color: '#64748b', textDecoration: 'none' }}>
            All Humans
          </Link>
          <span style={{ color: '#cbd5e1' }}>/</span>
          <Link to={`/${human.id}`} style={{ color: '#64748b', textDecoration: 'none' }}>
            {human.name}
          </Link>
          <span style={{ color: '#cbd5e1' }}>/</span>
          <span style={{ color: '#0f172a', fontWeight: 600 }}>Emotions</span>
        </div>

        {/* View Switcher Toggle */}
        <div
          style={{
            display: 'inline-flex',
            backgroundColor: '#e2e8f0',
            borderRadius: '10px',
            padding: '4px',
            gap: '4px',
          }}
        >
          <button
            onClick={() => setActiveTab('react')}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '6px',
              padding: '8px 16px',
              borderRadius: '8px',
              border: 'none',
              cursor: 'pointer',
              fontWeight: 700,
              fontSize: '13px',
              backgroundColor: activeTab === 'react' ? '#0f172a' : 'transparent',
              color: activeTab === 'react' ? '#ffffff' : '#475569',
              transition: 'all 0.2s ease',
            }}
          >
            <span>⚛️</span>
            <span>React Mode</span>
          </button>
          <button
            onClick={() => setActiveTab('htmx')}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: '6px',
              padding: '8px 16px',
              borderRadius: '8px',
              border: 'none',
              cursor: 'pointer',
              fontWeight: 700,
              fontSize: '13px',
              backgroundColor: activeTab === 'htmx' ? '#2563eb' : 'transparent',
              color: activeTab === 'htmx' ? '#ffffff' : '#475569',
              transition: 'all 0.2s ease',
            }}
          >
            <span>⚡</span>
            <span>HTMX Mode</span>
          </button>
        </div>
      </div>

      {/* Render selected view */}
      {activeTab === 'react' ? (
        <ReactEmotionsView humanId={id} />
      ) : (
        <HtmxEmotionsView humanId={id} />
      )}
    </div>
  );
};

// ==========================================
// React Implementation View Component
// ==========================================
interface ReactEmotionsViewProps {
  humanId: string;
}

const ReactEmotionsView: React.FC<ReactEmotionsViewProps> = ({ humanId }) => {
  const { data: human, refetch: refetchHuman } = useHuman(humanId);
  const { data: emotions, isLoading, error, refetch: refetchEmotions } = useEmotions(humanId);

  const addMutation = useAddEmotion(humanId);
  const updateMutation = useUpdateEmotion(humanId);
  const deleteMutation = useDeleteEmotion(humanId);

  // Local state for sliders so user can drag smoothly before saving
  const [sliderValues, setSliderValues] = useState<Record<number, number>>({});

  if (isLoading) return <LoadingSpinner message="Loading emotions state..." />;
  if (error || !human) {
    return (
      <ErrorMessage
        message={`Failed to load emotions: ${(error as any)?.message}`}
        onRetry={() => {
          refetchHuman();
          refetchEmotions();
        }}
      />
    );
  }

  const getPercentageForType = (type: EmotionType): number => {
    if (sliderValues[type] !== undefined) return sliderValues[type];
    const existing = emotions?.find(
      (e) => (NAME_TO_EMOTION_TYPE[e.type] || 0) === type || e.type === EMOTION_LABELS[type]
    );
    return existing ? existing.percentage : 50;
  };

  const handleSliderChange = (type: EmotionType, value: number) => {
    setSliderValues((prev) => ({ ...prev, [type]: value }));
  };

  const handleAdd = async (type: EmotionType) => {
    const percentage = getPercentageForType(type);
    try {
      await addMutation.mutateAsync({ type, percentage });
    } catch (err: any) {
      alert(err.response?.data?.detail || err.message || 'Failed to add emotion.');
    }
  };

  const handleUpdate = async (type: EmotionType) => {
    const percentage = getPercentageForType(type);
    try {
      await updateMutation.mutateAsync({ type, percentage });
    } catch (err: any) {
      alert(err.response?.data?.detail || err.message || 'Failed to update emotion.');
    }
  };

  const handleDelete = async (type: EmotionType) => {
    const label = EMOTION_LABELS[type];
    if (confirm(`Remove ${label} from ${human.name}?`)) {
      try {
        await deleteMutation.mutateAsync(type);
      } catch (err: any) {
        alert(err.response?.data?.detail || err.message || 'Failed to remove emotion.');
      }
    }
  };

  return (
    <div>
      {/* Overview Card */}
      <div
        style={{
          backgroundColor: '#ffffff',
          borderRadius: '16px',
          border: '1px solid #e2e8f0',
          padding: '28px',
          textAlign: 'center',
          boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.04)',
          marginBottom: '24px',
        }}
      >
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <ShowColorBadge color={human.showColor} score={human.emotionScorePercentage} size="lg" />
          <h2 style={{ fontSize: '24px', fontWeight: 800, color: '#0f172a', margin: '14px 0 4px 0' }}>
            {human.name}
          </h2>
          <div
            style={{
              display: 'inline-block',
              backgroundColor: '#f1f5f9',
              padding: '4px 12px',
              borderRadius: '20px',
              fontFamily: 'monospace',
              fontSize: '14px',
              color: '#334155',
              fontWeight: 600,
              marginTop: '4px',
            }}
          >
            ShowColor: {human.showColor}
          </div>
          <p style={{ color: '#64748b', fontSize: '13px', margin: '8px 0 0 0' }}>
            White (#FFFFFF) → Green (#008000) spectrum • Current Score: <strong>{human.emotionScorePercentage.toFixed(1)}%</strong>
          </p>
        </div>
      </div>

      {/* Emotion Sliders Section */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '14px' }}>
        {EMOTION_TYPES.map((type) => {
          const label = EMOTION_LABELS[type];
          const emoji = EMOTION_EMOJIS[type];
          const color = EMOTION_COLORS[type];

          const existing = emotions?.find(
            (e) => (NAME_TO_EMOTION_TYPE[e.type] || 0) === type || e.type === label
          );
          const currentPercentage = getPercentageForType(type);
          const isPending =
            addMutation.isPending || updateMutation.isPending || deleteMutation.isPending;

          return (
            <div
              key={type}
              style={{
                backgroundColor: existing ? '#ffffff' : '#f8fafc',
                borderRadius: '12px',
                border: existing ? '1px solid #e2e8f0' : '1px dashed #cbd5e1',
                padding: '20px',
                boxShadow: existing ? '0 2px 4px rgba(0, 0, 0, 0.03)' : 'none',
                transition: 'all 0.2s ease',
              }}
            >
              <div
                style={{
                  display: 'flex',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  marginBottom: '12px',
                }}
              >
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                  <span style={{ fontSize: '26px' }}>{emoji}</span>
                  <div>
                    <h3 style={{ margin: 0, fontSize: '17px', fontWeight: 700, color: '#0f172a' }}>
                      {label}
                    </h3>
                    <span style={{ fontSize: '12px', color: existing ? '#10b981' : '#94a3b8', fontWeight: 600 }}>
                      {existing ? '● Added to Human' : '○ Not added yet'}
                    </span>
                  </div>
                </div>

                <div style={{ textAlign: 'right' }}>
                  <span
                    style={{
                      fontSize: '20px',
                      fontWeight: 800,
                      color: existing ? color : '#94a3b8',
                    }}
                  >
                    {Math.round(currentPercentage)}%
                  </span>
                </div>
              </div>

              {/* Slider & progress bar */}
              <div style={{ marginBottom: '16px' }}>
                <input
                  type="range"
                  min={0}
                  max={100}
                  step={1}
                  value={currentPercentage}
                  onChange={(e) => handleSliderChange(type, Number(e.target.value))}
                  style={{
                    width: '100%',
                    height: '8px',
                    accentColor: color,
                    cursor: 'pointer',
                  }}
                />
              </div>

              {/* Action Buttons */}
              <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '10px' }}>
                {existing ? (
                  <>
                    <button
                      onClick={() => handleUpdate(type)}
                      disabled={isPending}
                      style={{
                        backgroundColor: '#3b82f6',
                        color: '#ffffff',
                        border: 'none',
                        padding: '8px 18px',
                        borderRadius: '6px',
                        cursor: isPending ? 'not-allowed' : 'pointer',
                        fontSize: '13px',
                        fontWeight: 600,
                      }}
                    >
                      Update Percentage
                    </button>
                    <button
                      onClick={() => handleDelete(type)}
                      disabled={isPending}
                      style={{
                        backgroundColor: '#fee2e2',
                        color: '#dc2626',
                        border: 'none',
                        padding: '8px 14px',
                        borderRadius: '6px',
                        cursor: isPending ? 'not-allowed' : 'pointer',
                        fontSize: '13px',
                        fontWeight: 600,
                      }}
                    >
                      Remove
                    </button>
                  </>
                ) : (
                  <button
                    onClick={() => handleAdd(type)}
                    disabled={isPending}
                    style={{
                      backgroundColor: '#10b981',
                      color: '#ffffff',
                      border: 'none',
                      padding: '8px 18px',
                      borderRadius: '6px',
                      cursor: isPending ? 'not-allowed' : 'pointer',
                      fontSize: '13px',
                      fontWeight: 600,
                    }}
                  >
                    + Add {label} at {Math.round(currentPercentage)}%
                  </button>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

