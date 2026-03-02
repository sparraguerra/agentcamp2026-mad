import React from 'react';
import { Character, CharacterClass } from '../types';
import CharacterAvatar from './CharacterAvatar';

interface Props {
  character: Character;
  onPlayClick?: (character: Character) => void;
}

const CharacterCard: React.FC<Props> = ({ character, onPlayClick }) => {
  const getClassColor = (charClass: CharacterClass): string => {
    const colors: Record<CharacterClass, string> = {
      [CharacterClass.Warrior]: '#e74c3c',
      [CharacterClass.Mage]: '#3498db',
      [CharacterClass.Rogue]: '#9b59b6',
      [CharacterClass.Cleric]: '#f39c12',
      [CharacterClass.Ranger]: '#27ae60',
    };
    return colors[charClass];
  };

  const getClassName = (charClass: CharacterClass): string => {
    return CharacterClass[charClass];
  };

  const hpPercentage = (character.hitPoints / character.maxHitPoints) * 100;

  return (
    <div style={styles.card}>
      <div style={{ ...styles.header, background: getClassColor(character.class) }}>
        <h3 style={styles.name}>{character.name}</h3>
        <span style={styles.class}>{getClassName(character.class)}</span>
      </div>
      
      <div style={styles.body}>
        <div style={styles.stat}>
          <span style={styles.label}>Level:</span>
          <span style={styles.value}>{character.level}</span>
        </div>
        
        <div style={styles.hpContainer}>
          <div style={styles.hpLabel}>
            HP: {character.hitPoints} / {character.maxHitPoints}
          </div>
          <div style={styles.hpBar}>
            <div 
              style={{
                ...styles.hpFill,
                width: `${hpPercentage}%`,
                background: hpPercentage > 50 ? '#27ae60' : hpPercentage > 25 ? '#f39c12' : '#e74c3c'
              }}
            />
          </div>
        </div>

        <div style={styles.stats}>
          <div style={styles.statRow}>
            <span>💪 STR: {character.strength}</span>
            <span>🤸 DEX: {character.dexterity}</span>
            <span>🛡️ CON: {character.constitution}</span>
          </div>
          <div style={styles.statRow}>
            <span>🧠 INT: {character.intelligence}</span>
            <span>🔮 WIS: {character.wisdom}</span>
            <span>💬 CHA: {character.charisma}</span>
          </div>
        </div>

        <div style={styles.footer}>
          <span>💰 Gold: {character.gold}</span>
          <span>⭐ XP: {character.experience}</span>
        </div>

        {onPlayClick && (
          <button
            onClick={(e) => {
              e.stopPropagation();
              onPlayClick(character);
            }}
            style={styles.playButton}
          >
            🎮 START GAME
          </button>
        )}
      </div>
    </div>
  );
};

const styles = {
  card: {
    border: '3px solid #00ff00',
    overflow: 'hidden',
    boxShadow: '0 0 15px rgba(0, 255, 0, 0.3)',
    transition: 'transform 0.2s, box-shadow 0.2s',
    cursor: 'pointer',
    background: '#1a1a1a',
  },
  header: {
    padding: '1rem',
    color: '#0f0f0f',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottom: '3px solid #000000',
  },
  name: {
    margin: 0,
    fontSize: '0.75rem',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '2px 2px 0 rgba(0, 0, 0, 0.5)',
  },
  class: {
    background: 'rgba(0, 0, 0, 0.3)',
    padding: '0.25rem 0.75rem',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    border: '2px solid rgba(0, 0, 0, 0.5)',
  },
  body: {
    padding: '1rem',
    background: '#1a1a1a',
    color: '#00ff00',
  },
  stat: {
    display: 'flex',
    justifyContent: 'space-between',
    marginBottom: '0.5rem',
    fontSize: '0.65rem',
  },
  label: {
    fontWeight: 'bold' as const,
    color: '#00ffff',
  },
  value: {
    color: '#00ff00',
  },
  hpContainer: {
    marginBottom: '1rem',
  },
  hpLabel: {
    fontSize: '0.65rem',
    marginBottom: '0.5rem',
    color: '#00ffff',
    fontFamily: "'Press Start 2P', monospace",
  },
  hpBar: {
    height: '20px',
    background: '#0f0f0f',
    border: '2px solid #00ff00',
    overflow: 'hidden',
    position: 'relative' as const,
  },
  hpFill: {
    height: '100%',
    transition: 'width 0.3s',
    boxShadow: 'inset 0 0 10px rgba(255, 255, 255, 0.3)',
  },
  stats: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '0.5rem',
    marginBottom: '1rem',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  statRow: {
    display: 'flex',
    justifyContent: 'space-between',
  },
  footer: {
    display: 'flex',
    justifyContent: 'space-between',
    paddingTop: '0.75rem',
    borderTop: '2px solid #00ff00',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  playButton: {
    marginTop: '1rem',
    width: '100%',
    padding: '0.75rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    cursor: 'pointer',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 4px 0 #00aa00',
    transition: 'all 0.2s',
  },
};

export default CharacterCard;
