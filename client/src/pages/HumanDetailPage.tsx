import React, { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useHuman, useUpdateHuman, useDeleteHuman } from '../hooks/useHumans';
import { ShowColorBadge } from '../components/ShowColorBadge';
import { EmotionBar } from '../components/EmotionBar';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { ErrorMessage } from '../components/ErrorMessage';

export const HumanDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { data: human, isLoading, error, refetch } = useHuman(id);
  const updateMutation = useUpdateHuman(id || '');
  const deleteMutation = useDeleteHuman();

  const [isEditing, setIsEditing] = useState(false);
  const [editName, setEditName] = useState('');

  const handleStartEdit = () => {
    if (human) {
      setEditName(human.name);
      setIsEditing(true);
    }
  };

  const handleSaveEdit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editName.trim()) return;
    try {
      await updateMutation.mutateAsync({ name: editName.trim() });
      setIsEditing(false);
    } catch (err: any) {
      alert(err.response?.data?.detail || err.message || 'Failed to update name.');
    }
  };

  const handleDelete = async () => {
    if (!human || !id) return;
    if (confirm(`Are you sure you want to delete ${human.name}?`)) {
      try {
        await deleteMutation.mutateAsync(id);
        navigate('/');
      } catch (err: any) {
        alert(err.response?.data?.detail || err.message || 'Failed to delete human.');
      }
    }
  };

  if (isLoading) return <LoadingSpinner message="Loading human profile..." />;
  if (error || !human) {
    return (
      <ErrorMessage
        message={`Could not load human: ${error ? (error as any).message : 'Not found'}`}
        onRetry={() => refetch()}
      />
    );
  }

  return (
    <div>
      {/* Navigation breadcrumb */}
      <div style={{ marginBottom: '20px' }}>
        <Link
          to="/"
          style={{
            color: '#3b82f6',
            textDecoration: 'none',
            fontSize: '14px',
            display: 'inline-flex',
            alignItems: 'center',
            gap: '4px',
            fontWeight: 500,
          }}
        >
          ← Back to All Humans
        </Link>
      </div>

      {/* Main hero card */}
      <div
        style={{
          backgroundColor: '#ffffff',
          borderRadius: '16px',
          border: '1px solid #e2e8f0',
          padding: '32px',
          boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.05)',
          marginBottom: '28px',
        }}
      >
        <div
          style={{
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'center',
            gap: '32px',
            flexWrap: 'wrap',
          }}
        >
          {/* Large ShowColor Swatch */}
          <div style={{ textAlign: 'center' }}>
            <ShowColorBadge color={human.showColor} score={human.emotionScorePercentage} size="lg" />
            <div style={{ marginTop: '10px' }}>
              <span
                style={{
                  fontFamily: 'monospace',
                  fontSize: '14px',
                  backgroundColor: '#f1f5f9',
                  padding: '4px 10px',
                  borderRadius: '6px',
                  color: '#334155',
                  fontWeight: 600,
                }}
              >
                {human.showColor}
              </span>
            </div>
            <div style={{ fontSize: '12px', color: '#64748b', marginTop: '4px' }}>
              ShowColor Property
            </div>
          </div>

          {/* Details & editing */}
          <div style={{ flex: 1, minWidth: '240px' }}>
            {isEditing ? (
              <form onSubmit={handleSaveEdit} style={{ display: 'flex', gap: '8px', alignItems: 'center', marginBottom: '12px' }}>
                <input
                  type="text"
                  value={editName}
                  onChange={(e) => setEditName(e.target.value)}
                  style={{
                    fontSize: '22px',
                    fontWeight: 700,
                    padding: '4px 10px',
                    border: '2px solid #3b82f6',
                    borderRadius: '6px',
                    outline: 'none',
                  }}
                  autoFocus
                />
                <button
                  type="submit"
                  disabled={updateMutation.isPending}
                  style={{
                    backgroundColor: '#10b981',
                    color: '#ffffff',
                    border: 'none',
                    borderRadius: '6px',
                    padding: '8px 14px',
                    cursor: 'pointer',
                    fontWeight: 600,
                  }}
                >
                  Save
                </button>
                <button
                  type="button"
                  onClick={() => setIsEditing(false)}
                  style={{
                    backgroundColor: '#f1f5f9',
                    color: '#475569',
                    border: 'none',
                    borderRadius: '6px',
                    padding: '8px 14px',
                    cursor: 'pointer',
                  }}
                >
                  Cancel
                </button>
              </form>
            ) : (
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '8px' }}>
                <h1 style={{ fontSize: '28px', fontWeight: 800, color: '#0f172a', margin: 0 }}>
                  {human.name}
                </h1>
                <button
                  onClick={handleStartEdit}
                  style={{
                    background: 'none',
                    border: '1px solid #cbd5e1',
                    borderRadius: '6px',
                    padding: '4px 8px',
                    cursor: 'pointer',
                    fontSize: '12px',
                    color: '#64748b',
                  }}
                >
                  ✏️ Edit Name
                </button>
              </div>
            )}

            <div style={{ color: '#64748b', fontSize: '13px', marginBottom: '16px' }}>
              ID: <span style={{ fontFamily: 'monospace' }}>{human.id}</span>
            </div>

            <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap', marginBottom: '20px' }}>
              <div style={{ backgroundColor: '#f8fafc', padding: '10px 16px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                <div style={{ fontSize: '12px', color: '#64748b' }}>Emotion Score</div>
                <div style={{ fontSize: '20px', fontWeight: 800, color: '#0f172a' }}>
                  {human.emotionScorePercentage.toFixed(1)}%
                </div>
              </div>

              <div style={{ backgroundColor: '#f8fafc', padding: '10px 16px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                <div style={{ fontSize: '12px', color: '#64748b' }}>Emotions Added</div>
                <div style={{ fontSize: '20px', fontWeight: 800, color: '#0f172a' }}>
                  {human.emotions.length} / 5
                </div>
              </div>
            </div>

            {/* Quick Actions */}
            <div style={{ display: 'flex', gap: '10px' }}>
              <Link
                to={`/${human.id}/emotions`}
                style={{
                  backgroundColor: '#0f172a',
                  color: '#ffffff',
                  padding: '10px 20px',
                  borderRadius: '8px',
                  textDecoration: 'none',
                  fontWeight: 600,
                  fontSize: '14px',
                  display: 'inline-flex',
                  alignItems: 'center',
                  gap: '6px',
                }}
              >
                🎭 Manage Emotions (React ↔ HTMX)
              </Link>
              <button
                onClick={handleDelete}
                style={{
                  backgroundColor: '#fee2e2',
                  color: '#dc2626',
                  border: 'none',
                  padding: '10px 16px',
                  borderRadius: '8px',
                  fontWeight: 600,
                  fontSize: '14px',
                  cursor: 'pointer',
                }}
              >
                Delete Profile
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Emotions Breakdown section */}
      <div
        style={{
          backgroundColor: '#ffffff',
          borderRadius: '16px',
          border: '1px solid #e2e8f0',
          padding: '24px',
        }}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '18px' }}>
          <h2 style={{ fontSize: '18px', fontWeight: 700, color: '#0f172a', margin: 0 }}>
            Current Emotions Breakdown
          </h2>
          <Link
            to={`/${human.id}/emotions`}
            style={{ fontSize: '13px', color: '#3b82f6', textDecoration: 'none', fontWeight: 600 }}
          >
            Adjust Values →
          </Link>
        </div>

        {human.emotions.length === 0 ? (
          <div style={{ textAlign: 'center', padding: '32px', color: '#64748b' }}>
            <p style={{ margin: '0 0 12px 0' }}>No emotions have been assigned to this human yet.</p>
            <Link
              to={`/${human.id}/emotions`}
              style={{
                backgroundColor: '#10b981',
                color: '#ffffff',
                padding: '8px 16px',
                borderRadius: '6px',
                textDecoration: 'none',
                fontWeight: 600,
                fontSize: '13px',
                display: 'inline-block',
              }}
            >
              Add First Emotion
            </Link>
          </div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
            {human.emotions.map((emotion) => (
              <div key={emotion.id} style={{ backgroundColor: '#f8fafc', padding: '14px', borderRadius: '8px' }}>
                <EmotionBar emotion={emotion} />
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

