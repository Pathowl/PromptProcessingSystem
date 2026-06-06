export default function StatusBadge({ status }) {
  const s = String(status || '').toLowerCase();

 const styles = {
    completed:  { background: '#e9fedb', color: '#50c21b', border: '1px solid #92f58e' },
    processing: { background: '#dbeafe', color: '#1d4ed8', border: '1px solid #bfdbfe' }, 
    pending:    { background: '#fef3c7', color: '#b45309', border: '1px solid #fde68a' },
    failed:     { background: '#fee2e2', color: '#b91c1c', border: '1px solid #fecaca' },
  };

  // error handling
  const style = styles[s] || { background: '#f3f4f6', color: '#6b7280', border: '1px solid #e5e7eb' };
  
  return (
    <span style={{
      display: 'inline-block', fontSize: 11, fontFamily: 'monospace',
      padding: '3px 8px', borderRadius: 4, whiteSpace: 'nowrap', fontWeight: 500, ...style
    }}>
      {status || 'queued'}
    </span>
  );
}