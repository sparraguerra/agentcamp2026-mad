import React, { useState, useEffect } from 'react';
import { charactersApi } from '../services/api';
import { Character } from '../types';
import CharacterCard from '../components/CharacterCard';
import CreateCharacter from '../components/CreateCharacter';
import DiceRoller from '../components/DiceRoller';
import Game from '../components/Game';

interface Props {
  username: string;
  onLogout: () => void;
}

const Dashboard: React.FC<Props> = ({ username, onLogout }) => {
  const [characters, setCharacters] = useState<Character[]>([]);
  const [showCreateCharacter, setShowCreateCharacter] = useState(false);
  const [loading, setLoading] = useState(true);
  const [selectedCharacter, setSelectedCharacter] = useState<Character | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);

  useEffect(() => {
    loadCharacters();
  }, []);

  const loadCharacters = async () => {
    try {
      const data = await charactersApi.getAll();
      setCharacters(data);
    } catch (error) {
      console.error('Failed to load characters:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCharacterCreated = (character: Character) => {
    setCharacters([...characters, character]);
    setShowCreateCharacter(false);
  };

  const handleStartGame = (character: Character) => {
    setSelectedCharacter(character);
    setIsPlaying(true);
  };

  const handleExitGame = () => {
    setIsPlaying(false);
    setSelectedCharacter(null);
  };

  if (isPlaying && selectedCharacter) {
    return <Game character={selectedCharacter} onExit={handleExitGame} />;
  }

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <h1 style={styles.title}>🎲 D&D Copilot</h1>
        <div style={styles.userInfo}>
          <span>Welcome, {username}!</span>
          <button onClick={onLogout} style={styles.logoutButton}>
            Logout
          </button>
        </div>
      </header>

      <div style={styles.content}>
        <div style={styles.mainSection}>
          <div style={styles.sectionHeader}>
            <h2>Your Characters</h2>
            <button
              onClick={() => setShowCreateCharacter(true)}
              style={styles.createButton}
            >
              + Create Character
            </button>
          </div>

          {loading ? (
            <p>Loading characters...</p>
          ) : characters.length === 0 ? (
            <div style={styles.emptyState}>
              <p>No characters yet. Create your first character to start your adventure!</p>
            </div>
          ) : (
            <div style={styles.charactersGrid}>
              {characters.map((character) => (
                <CharacterCard 
                  key={character.id} 
                  character={character}
                  onPlayClick={handleStartGame}
                />
              ))}
            </div>
          )}
        </div>

        <aside style={styles.sidebar}>
          <DiceRoller />
        </aside>
      </div>

      {showCreateCharacter && (
        <div style={styles.modal}>
          <div style={styles.modalContent}>
            <button
              onClick={() => setShowCreateCharacter(false)}
              style={styles.closeButton}
            >
              ×
            </button>
            <CreateCharacter onCharacterCreated={handleCharacterCreated} />
          </div>
        </div>
      )}
    </div>
  );
};

const styles = {
  container: {
    minHeight: '100vh',
    background: '#0f0f0f',
  },
  header: {
    background: '#1a1a1a',
    color: '#00ff00',
    padding: '1.5rem 2rem',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottom: '4px solid #00ff00',
    boxShadow: '0 0 20px rgba(0, 255, 0, 0.3)',
  },
  title: {
    margin: 0,
    fontSize: '1rem',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '0 0 10px #00ff00',
    letterSpacing: '2px',
  },
  userInfo: {
    display: 'flex',
    alignItems: 'center',
    gap: '1rem',
    fontSize: '0.65rem',
  },
  logoutButton: {
    padding: '0.5rem 1rem',
    background: '#ff0000',
    color: '#ffffff',
    border: '2px solid #ff0000',
    cursor: 'pointer',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 0 10px rgba(255, 0, 0, 0.5)',
  },
  content: {
    display: 'grid',
    gridTemplateColumns: '1fr 350px',
    gap: '2rem',
    padding: '2rem',
    maxWidth: '1400px',
    margin: '0 auto',
  },
  mainSection: {
    background: '#1a1a1a',
    border: '3px solid #00ff00',
    padding: '2rem',
    boxShadow: '0 0 20px rgba(0, 255, 0, 0.3), inset 0 0 20px rgba(0, 255, 0, 0.05)',
  },
  sectionHeader: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1.5rem',
    paddingBottom: '1rem',
    borderBottom: '2px solid #00ff00',
  },
  createButton: {
    padding: '0.75rem 1.5rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    cursor: 'pointer',
    fontSize: '0.65rem',
    fontWeight: 'bold' as const,
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 4px 0 #00aa00',
  },
  emptyState: {
    textAlign: 'center' as const,
    padding: '3rem',
    color: '#00ff00',
    fontSize: '0.75rem',
    border: '2px dashed #00ff00',
    background: 'rgba(0, 255, 0, 0.05)',
  },
  charactersGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))',
    gap: '1.5rem',
  },
  sidebar: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '1rem',
  },
  modal: {
    position: 'fixed' as const,
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    background: 'rgba(0, 0, 0, 0.9)',
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000,
  },
  modalContent: {
    background: '#1a1a1a',
    border: '4px solid #00ff00',
    padding: '2rem',
    maxWidth: '600px',
    width: '90%',
    position: 'relative' as const,
    boxShadow: '0 0 30px rgba(0, 255, 0, 0.5)',
  },
  closeButton: {
    position: 'absolute' as const,
    top: '1rem',
    right: '1rem',
    background: '#ff0000',
    border: '2px solid #ff0000',
    fontSize: '1.5rem',
    cursor: 'pointer',
    color: '#ffffff',
    width: '40px',
    height: '40px',
    fontFamily: "'Press Start 2P', monospace",
    lineHeight: '1',
  },
};

export default Dashboard;
