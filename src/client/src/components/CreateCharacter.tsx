import React, { useState } from 'react';
import { charactersApi } from '../services/api';
import { Character, CharacterClass, CreateCharacterRequest } from '../types';

interface Props {
  onCharacterCreated: (character: Character) => void;
}

const CreateCharacter: React.FC<Props> = ({ onCharacterCreated }) => {
  const [name, setName] = useState('');
  const [selectedClass, setSelectedClass] = useState<CharacterClass>(CharacterClass.Warrior);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const classes = [
    { value: CharacterClass.Warrior, label: '⚔️ Warrior', description: 'Strong melee fighter' },
    { value: CharacterClass.Mage, label: '🔮 Mage', description: 'Powerful spellcaster' },
    { value: CharacterClass.Rogue, label: '🗡️ Rogue', description: 'Stealthy and agile' },
    { value: CharacterClass.Cleric, label: '✨ Cleric', description: 'Healer and support' },
    { value: CharacterClass.Ranger, label: '🏹 Ranger', description: 'Skilled archer' },
  ];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const data: CreateCharacterRequest = {
        name,
        class: selectedClass,
      };
      const character = await charactersApi.create(data);
      onCharacterCreated(character);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create character');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.container}>
      <h2 style={styles.title}>Create Your Character</h2>
      <form onSubmit={handleSubmit} style={styles.form}>
        <div style={styles.field}>
          <label style={styles.label}>Character Name</label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
            style={styles.input}
            placeholder="Enter character name"
          />
        </div>

        <div style={styles.field}>
          <label style={styles.label}>Choose Your Class</label>
          <div style={styles.classGrid}>
            {classes.map((cls) => (
              <button
                key={cls.value}
                type="button"
                onClick={() => setSelectedClass(cls.value)}
                style={{
                  ...styles.classButton,
                  ...(selectedClass === cls.value ? styles.classButtonSelected : {}),
                }}
              >
                <div style={styles.classLabel}>{cls.label}</div>
                <div style={styles.classDescription}>{cls.description}</div>
              </button>
            ))}
          </div>
        </div>

        {error && <div style={styles.error}>{error}</div>}

        <button type="submit" disabled={loading} style={styles.submitButton}>
          {loading ? 'Creating...' : 'Create Character'}
        </button>
      </form>
    </div>
  );
};

const styles = {
  container: {
    maxWidth: '600px',
  },
  title: {
    marginBottom: '1.5rem',
    color: '#00ff00',
    fontSize: '0.85rem',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '0 0 10px #00ff00',
  },
  form: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '1.5rem',
  },
  field: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '0.5rem',
  },
  label: {
    fontWeight: 'bold' as const,
    color: '#00ffff',
    fontSize: '0.7rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  input: {
    padding: '0.75rem',
    border: '2px solid #00ff00',
    background: '#0f0f0f',
    color: '#00ff00',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  classGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(2, 1fr)',
    gap: '0.75rem',
  },
  classButton: {
    padding: '1rem',
    border: '2px solid #00ff00',
    background: '#0f0f0f',
    cursor: 'pointer',
    transition: 'all 0.2s',
    textAlign: 'left' as const,
    color: '#00ff00',
  },
  classButtonSelected: {
    borderColor: '#00ffff',
    background: 'rgba(0, 255, 255, 0.1)',
    boxShadow: '0 0 15px rgba(0, 255, 255, 0.5)',
  },
  classLabel: {
    fontSize: '0.7rem',
    fontWeight: 'bold' as const,
    marginBottom: '0.25rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ffff',
  },
  classDescription: {
    fontSize: '0.6rem',
    color: '#00ff00',
    fontFamily: "'Press Start 2P', monospace",
    lineHeight: '1.4',
  },
  error: {
    color: '#ff0000',
    fontSize: '0.65rem',
    textShadow: '0 0 5px #ff0000',
    padding: '0.5rem',
    border: '2px solid #ff0000',
    background: 'rgba(255, 0, 0, 0.1)',
  },
  submitButton: {
    padding: '1rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    fontSize: '0.75rem',
    fontWeight: 'bold' as const,
    cursor: 'pointer',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 4px 0 #00aa00',
  },
};

export default CreateCharacter;
