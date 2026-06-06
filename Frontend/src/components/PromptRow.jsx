import { useRef } from 'react';

const MAX_LENGTH = 100; // Definiujemy limit znaków

export default function PromptRow({ prompt, index, onRemove, onChange, canRemove }) {
  const textareaRef = useRef(null);

  // Zmienne sprawdzające limity
  const atLimit = prompt.value.length >= MAX_LENGTH;
  const showCounter = prompt.value.length > MAX_LENGTH - 20;

  const handleInput = (e) => {
    onChange(prompt.id, e.target.value);
    e.target.style.height = 'auto';
    e.target.style.height = e.target.scrollHeight + 'px';
  };

  return (
    <div style={{ marginBottom: 12 }}>
      <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10 }}>
        
        {/* Numerek wiersza */}
        <span style={{ fontSize: 14, color: 'gray', minWidth: 25, paddingTop: 8, userSelect: 'none', fontFamily: 'monospace' }}>
          {String(index + 1).padStart(2, '0')}
        </span>
        
        {/* Pole tekstowe (z czerwonym borderem na limicie 100 zankow) */}
        <textarea
          ref={textareaRef}
          value={prompt.value}
          onChange={handleInput}
          placeholder={`Prompt ${index + 1}...`}
          maxLength={MAX_LENGTH} 
          rows={1}
          style={{
            flex: 1, 
            padding: '8px 12px', 
            border: `1px solid ${atLimit ? '#fca5a5' : '#ccc'}`, 
            borderRadius: 6,
            fontSize: 14, 
            fontFamily: 'inherit',
            resize: 'none', 
            minHeight: 36,
            boxSizing: 'border-box',
            lineHeight: 1.5,
            outline: 'none'
          }}
        />
        
        {/* Przycisk usuwania (zawsze szary, tylko zanika) */}
        <button
          onClick={() => onRemove(prompt.id)}
          disabled={!canRemove}
          title="Remove prompt"
          style={{
            background: 'none', 
            border: 'none', 
            cursor: canRemove ? 'pointer' : 'default',
            color: '#888', 
            opacity: canRemove ? 1 : 0.3,
            fontSize: 18, 
            padding: '6px 4px',
            fontWeight: 'bold'
          }}
        >
          ✕
        </button>
      </div>

      {/* Licznik jest mniej niz 20 znakow */}
      {showCounter && (
        <div style={{ 
          textAlign: 'right', 
          fontSize: 12, 
          marginTop: 4, 
          paddingRight: 32, 
          color: atLimit ? '#ef4444' : 'gray', 
          fontFamily: 'monospace' 
        }}>
          {prompt.value.length}/{MAX_LENGTH}
        </div>
      )}
    </div>
  );
}