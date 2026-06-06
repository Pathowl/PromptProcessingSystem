import { useState, useEffect } from 'react';
import { apiClient } from './api/client';
import PromptRow from './components/PromptRow';
import StatusBadge from './components/StatusBadge';

const MAX_LENGTH = 100;
const newRow = () => ({ id: crypto.randomUUID(), value: '' });

const s = {
  card:   { background: '#fff', border: '1px solid #e5e7eb', borderRadius: 10, padding: 20 },
  mono:   { fontFamily: 'monospace', fontSize: 12 },
  label:  { fontFamily: 'monospace', fontSize: 11, fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.06em', color: '#9ca3af', margin: 0 },
  muted:  { color: '#9ca3af' },
  dimmed: { color: '#d1d5db' },
};

export default function App() {
  const [rows, setRows] = useState([newRow(), newRow(), newRow()]);
  const [prompts, setPrompts] = useState([]);
  const [sending, setSending] = useState(false);

  useEffect(() => {
    const fetch = async () => {
      try {
        const res = await apiClient.get('/prompts');
        setPrompts(res.data);
      } catch (err) {
        if (err.code === 'ERR_CONNECTION_REFUSED' || !err.response) return;
        console.error(err);
      }
    };
    fetch();
    const interval = setInterval(fetch, 1000);
    return () => clearInterval(interval);
  }, []);

  const handleSend = async () => {
    const toSend = rows.filter(r => r.value.trim());
    if (!toSend.length) return;
    
    setSending(true);
    try {
      // backend sending
      await Promise.all(
        toSend.map(r => apiClient.post('/prompts', { content: r.value.trim() }))
      );
      setRows([newRow(), newRow(), newRow()]);
    } catch (err) {
      console.error('Send failed', err);
    } finally {
      setSending(false);
    }
  };
  const filledCount = rows.filter(r => r.value.trim()).length;

  return (
    <div style={{ maxWidth: 800, margin: '60px auto', padding: '0 24px', fontFamily: 'system-ui, sans-serif' }}>
      <div style={{ marginBottom: 32, textAlign: 'center' }}>
        <h1 style={{ fontSize: 32, fontWeight: 600, margin: '0 0 6px', color: '#111' }}>
          Prompt Processing System
        </h1>
        <p style={{ ...s.mono, ...s.muted }}>RabbitMQ + MassTransit pipeline</p>
      </div>

      <div style={{ ...s.card, marginBottom: 16 }}>
        <p style={{ ...s.label, marginBottom: 14 }}>Batch input</p>

        {rows.map((row, i) => (
          <PromptRow
            key={row.id}
            prompt={row}
            index={i}
            onRemove={(id) => setRows(rows.filter(r => r.id !== id))}
            onChange={(id, val) => setRows(rows.map(r => r.id === id ? { ...r, value: val } : r))}
            canRemove={rows.length > 1}
          />
        ))}

        <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: 12, paddingTop: 12, borderTop: '1px solid #f3f4f6' }}>
          <button
            onClick={() => rows.length < 10 && setRows([...rows, newRow()])}
            disabled={rows.length >= 10}
            style={{ background: 'none', border: '1px solid #e5e7eb', borderRadius: 6, padding: '6px 14px', fontSize: 13, cursor: 'pointer', color: '#374151' }}
          >
            + Add prompt
          </button>
          <button
            onClick={handleSend}
            disabled={sending || filledCount === 0}
            style={{ background: '#111', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 20px', fontSize: 13, fontWeight: 500, cursor: sending || filledCount === 0 ? 'not-allowed' : 'pointer', opacity: sending || filledCount === 0 ? 0.4 : 1 }}
          >
            {sending ? 'Sending...' : `Send${filledCount > 0 ? ` (${filledCount})` : ''}`}
          </button>
        </div>
      </div>

      <div style={s.card}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14 }}>
          <p style={s.label}>Results</p>
          <div style={{ width: 8, height: 8, borderRadius: '50%', background: '#22c55e', animation: 'pulse 1.5s infinite' }} />
        </div>

        {prompts.length === 0 ? (
          <p style={{ textAlign: 'center', ...s.mono, ...s.muted, padding: '20px 0' }}>No results yet</p>
        ) : (
          <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 13 }}>
            <thead>
              <tr>
                {['#', 'Content', 'Status', 'Result'].map(h => (
                  <th key={h} style={{ ...s.label, textAlign: 'left', padding: '0 8px 10px', borderBottom: '1px solid #f3f4f6' }}>
                    {h}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {prompts.map((p, i) => (
                <tr key={p.id}>
                  <td style={{ padding: '10px 8px', borderBottom: '1px solid #f9fafb', ...s.mono, ...s.dimmed }}>
                    {String(i + 1).padStart(2, '0')}
                  </td>
                  <td style={{ padding: '10px 8px', borderBottom: '1px solid #f9fafb', ...s.mono, maxWidth: 200, wordBreak: 'break-word' }}>
                    {p.content}
                  </td>
                  <td style={{ padding: '10px 8px', borderBottom: '1px solid #f9fafb' }}>
                    <StatusBadge status={p.status} />
                  </td>
                  <td style={{ padding: '10px 8px', borderBottom: '1px solid #f9fafb', ...s.mono, color: '#6b7280', wordBreak: 'break-word' }}>
                    {p.result || p.errorMessage || '—'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}