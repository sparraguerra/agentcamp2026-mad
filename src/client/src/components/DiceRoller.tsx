import React, { useState } from 'react';
import { diceApi } from '../services/api';
import { DiceRollResponse } from '../types';

const DiceRoller: React.FC = () => {
  const [notation, setNotation] = useState('2d6');
  const [result, setResult] = useState<DiceRollResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const commonRolls = ['1d4', '1d6', '1d8', '1d10', '1d12', '1d20', '2d6', '3d6'];

  const handleRoll = async (customNotation?: string) => {
    const rollNotation = customNotation || notation;
    setError('');
    setLoading(true);

    try {
      const response = await diceApi.roll({ notation: rollNotation });
      setResult(response);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to roll dice');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.container}>
      <h3 style={styles.title}>🎲 Dice Roller</h3>
      
      <div style={styles.inputGroup}>
        <input
          type="text"
          value={notation}
          onChange={(e) => setNotation(e.target.value)}
          placeholder="e.g., 2d6, 1d20+5"
          style={styles.input}
        />
        <button
          onClick={() => handleRoll()}
          disabled={loading}
          style={styles.rollButton}
        >
          Roll
        </button>
      </div>

      <div style={styles.quickRolls}>
        {commonRolls.map((roll) => (
          <button
            key={roll}
            onClick={() => handleRoll(roll)}
            style={styles.quickButton}
          >
            {roll}
          </button>
        ))}
      </div>

      {error && <div style={styles.error}>{error}</div>}

      {result && (
        <div style={styles.result}>
          <div style={styles.resultHeader}>Result</div>
          <div style={styles.rolls}>
            {result.rolls.map((roll, index) => (
              <span key={index} style={styles.diceValue}>
                {roll}
              </span>
            ))}
          </div>
          {result.modifier !== 0 && (
            <div style={styles.modifier}>
              {result.modifier > 0 ? '+' : ''}{result.modifier}
            </div>
          )}
          <div style={styles.total}>Total: {result.total}</div>
        </div>
      )}
    </div>
  );
};

const styles = {
  container: {
    background: '#1a1a1a',
    border: '3px solid #00ff00',
    padding: '1.5rem',
    boxShadow: '0 0 20px rgba(0, 255, 0, 0.3), inset 0 0 20px rgba(0, 255, 0, 0.05)',
  },
  title: {
    marginTop: 0,
    marginBottom: '1rem',
    color: '#00ff00',
    fontSize: '0.75rem',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '0 0 10px #00ff00',
  },
  inputGroup: {
    display: 'flex',
    gap: '0.5rem',
    marginBottom: '1rem',
  },
  input: {
    flex: 1,
    padding: '0.75rem',
    border: '2px solid #00ff00',
    background: '#0f0f0f',
    color: '#00ff00',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  rollButton: {
    padding: '0.75rem 1.5rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    cursor: 'pointer',
    fontWeight: 'bold' as const,
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 4px 0 #00aa00',
  },
  quickRolls: {
    display: 'grid',
    gridTemplateColumns: 'repeat(4, 1fr)',
    gap: '0.5rem',
    marginBottom: '1rem',
  },
  quickButton: {
    padding: '0.5rem',
    background: '#0f0f0f',
    border: '2px solid #00ffff',
    color: '#00ffff',
    cursor: 'pointer',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    transition: 'all 0.2s',
  },
  error: {
    color: '#ff0000',
    fontSize: '0.65rem',
    marginBottom: '0.5rem',
    textShadow: '0 0 5px #ff0000',
    padding: '0.5rem',
    border: '2px solid #ff0000',
    background: 'rgba(255, 0, 0, 0.1)',
  },
  result: {
    background: '#0f0f0f',
    border: '3px solid #00ff00',
    padding: '1rem',
    textAlign: 'center' as const,
    boxShadow: '0 0 15px rgba(0, 255, 0, 0.3)',
  },
  resultHeader: {
    fontSize: '0.6rem',
    color: '#00ffff',
    marginBottom: '0.5rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  rolls: {
    display: 'flex',
    justifyContent: 'center',
    gap: '0.5rem',
    marginBottom: '0.5rem',
    flexWrap: 'wrap' as const,
  },
  diceValue: {
    display: 'inline-block',
    width: '50px',
    height: '50px',
    lineHeight: '50px',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '3px solid #00aa00',
    fontWeight: 'bold' as const,
    fontSize: '1rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 0 10px rgba(0, 255, 0, 0.5), inset 0 0 10px rgba(0, 0, 0, 0.3)',
  },
  modifier: {
    color: '#00ffff',
    fontSize: '0.75rem',
    marginBottom: '0.5rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  total: {
    fontSize: '1.2rem',
    fontWeight: 'bold' as const,
    color: '#00ff00',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '0 0 10px #00ff00',
  },
};

export default DiceRoller;
