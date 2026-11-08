import React, { useState } from 'react';
import { useHumans, useCreateHuman, useDeleteHuman } from '../hooks/useHumans';
import { HumanCard } from '../components/HumanCard';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { ErrorMessage } from '../components/ErrorMessage';

export const HumansPage: React.FC = () => {
  const { data: humans, isLoading, error, refetch } = useHumans();
  const createMutation = useCreateHuman();
  const deleteMutation = useDeleteHuman();

  const [name, setName] = useState('');
  const [formError, setFormError] = useState<string | null>(null);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim()) {
      setFormError('Please enter a name.');
      return;
    }
    setFormError(null);
    try {
      await createMutation.mutateAsync({ name: name.trim() });
      setName('');
    } catch (err: any) {
      setFormError(err.response?.data?.detail || err.message || 'Failed to create human.');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteMutation.mutateAsync(id);
    } catch (err: any) {
      alert(err.response?.data?.detail || err.message || 'Failed to delete human.');
    }
  };

  return (
    <div>
      {/* Header section */}
      <div style={{ marginBottom: '24px' }}>
        <h1 style={{ fontSize: '28px', fontWeight: 800, color: '#0f172a', margin: '0 0 8px 0' }}>
          Human Emotion System
        </h1>
        <p style={{ color: '#64748b', fontSize: '15px', margin: 0 }}>
          Manage humans and track their emotional spectrum. As emotions are added, the{' '}
          <strong style={{ color: '#059669' }}>ShowColor</strong> dynamically shifts from white (#FFFFFF) to deep green (#008000).
        </p>
      </div>

      {/* Create form card */}
      <div
        style={{
          backgroundColor: '#ffffff',
          borderRadius: '12px',
          padding: '20px',
          border: '1px solid #e2e8f0',
          marginBottom: '28px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.03)',
        }}
      >
        <h3 style={{ margin: '0 0 12px 0', fontSize: '16px', fontWeight: 700, color: '#1e293b' }}>
          + Create New Human
        </h3>
        <form onSubmit={handleCreate} style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
          <input
            type="text"
            placeholder="Enter human name (e.g. Alice, Bob, Charlie)..."
            value={name}
            onChange={(e) => setName(e.target.value)}
            disabled={createMutation.isPending}
            style={{
              flex: '1 1 280px',
              padding: '10px 14px',
              border: '1px solid #cbd5e1',
              borderRadius: '8px',
              fontSize: '14px',
              outline: 'none',
            }}
          />
          <button
            type="submit"
            disabled={createMutation.isPending}
            style={{
              backgroundColor: '#10b981',
              color: '#ffffff',
              border: 'none',
              borderRadius: '8px',
              padding: '10px 20px',
              fontWeight: 600,
              cursor: createMutation.isPending ? 'not-allowed' : 'pointer',
              fontSize: '14px',
            }}
          >
            {createMutation.isPending ? 'Creating...' : 'Create Human'}
          </button>
        </form>
        {formError && (
          <p style={{ color: '#dc2626', fontSize: '13px', margin: '8px 0 0 0' }}>{formError}</p>
        )}
      </div>

      {/* Loading state */}
      {isLoading && <LoadingSpinner message="Fetching humans and emotional spectrums..." />}

      {/* Error state */}
      {error && (
        <ErrorMessage
          message={`Failed to load humans: ${(error as any).message}`}
          onRetry={() => refetch()}
        />
      )}

      {/* List content */}
      {humans && (
        <>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '16px' }}>
            <h2 style={{ fontSize: '18px', fontWeight: 700, color: '#334155', margin: 0 }}>
              All Humans ({humans.length})
            </h2>
            <div style={{ fontSize: '13px', color: '#64748b' }}>
              Color spectrum: <span style={{ fontFamily: 'monospace', background: '#f1f5f9', padding: '2px 6px', borderRadius: '4px' }}>#FFFFFF → #008000</span>
            </div>
          </div>

          {humans.length === 0 ? (
            <div
              style={{
                backgroundColor: '#ffffff',
                borderRadius: '12px',
                padding: '48px 24px',
                textAlign: 'center',
                border: '1px dashed #cbd5e1',
                color: '#64748b',
              }}
            >
              <div style={{ fontSize: '40px', marginBottom: '8px' }}>👤</div>
              <h3 style={{ margin: '0 0 4px 0', color: '#1e293b' }}>No humans created yet</h3>
              <p style={{ margin: 0, fontSize: '14px' }}>
                Use the form above to add your first human and start tracking their emotions!
              </p>
            </div>
          ) : (
            <div
              style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
                gap: '20px',
              }}
            >
              {humans.map((human) => (
                <HumanCard
                  key={human.id}
                  human={human}
                  onDelete={handleDelete}
                  isDeleting={deleteMutation.isPending}
                />
              ))}
            </div>
          )}
        </>
      )}
    </div>
  );
};
