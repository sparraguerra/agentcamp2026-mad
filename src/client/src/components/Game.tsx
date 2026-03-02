import React, { useState, useEffect, useRef } from 'react';
import { diceApi, gameApi } from '../services/api';
import { Character, DiceRollResponse } from '../types';

interface Props {
  character: Character;
  onExit: () => void;
}

interface Message {
  role: 'user' | 'assistant';
  content: string;
}

const D20Icon: React.FC<{ rolling: boolean }> = ({ rolling }) => (
  <svg viewBox="0 0 100 100" width="42" height="42" aria-label="d20 die" role="img">
    <polygon
      points="50,4 91,28 76,85 24,85 9,28"
      fill="#f7d80b"
      stroke="#111111"
      strokeWidth="5"
      className={rolling ? 'd20-glow' : ''}
    />
    <line x1="50" y1="4" x2="24" y2="85" stroke="#111111" strokeWidth="3" opacity="0.9" />
    <line x1="50" y1="4" x2="76" y2="85" stroke="#111111" strokeWidth="3" opacity="0.9" />
    <line x1="9" y1="28" x2="91" y2="28" stroke="#111111" strokeWidth="3" opacity="0.9" />
    <line x1="24" y1="85" x2="50" y2="48" stroke="#111111" strokeWidth="2.5" opacity="0.85" />
    <line x1="76" y1="85" x2="50" y2="48" stroke="#111111" strokeWidth="2.5" opacity="0.85" />
    <line x1="9" y1="28" x2="50" y2="48" stroke="#111111" strokeWidth="2.5" opacity="0.85" />
    <line x1="91" y1="28" x2="50" y2="48" stroke="#111111" strokeWidth="2.5" opacity="0.85" />
    <text x="27" y="36" fill="#111111" fontSize="10" fontFamily="'Press Start 2P', monospace">2</text>
    <text x="71" y="36" fill="#111111" fontSize="10" fontFamily="'Press Start 2P', monospace">14</text>
    <text x="47" y="21" fill="#111111" fontSize="9" fontFamily="'Press Start 2P', monospace">8</text>
    <text x="47" y="80" fill="#111111" fontSize="9" fontFamily="'Press Start 2P', monospace">8</text>
    <text x="14" y="58" fill="#111111" fontSize="8" fontFamily="'Press Start 2P', monospace">12</text>
    <text x="78" y="58" fill="#111111" fontSize="8" fontFamily="'Press Start 2P', monospace">9</text>
  </svg>
);

const Game: React.FC<Props> = ({ character, onExit }) => {
  const [sessionId, setSessionId] = useState<number | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputAction, setInputAction] = useState('');
  const [loading, setLoading] = useState(false);
  const [gameState, setGameState] = useState<any>(null);
  const [availableActions, setAvailableActions] = useState<string[]>([]);
  const [subActions, setSubActions] = useState<string[]>([]);
  const [selectedParentAction, setSelectedParentAction] = useState<string | null>(null);
  const [pendingRoll, setPendingRoll] = useState<DiceRollResponse | null>(null);
  const [queuedCombatAction, setQueuedCombatAction] = useState<string | null>(null);
  const [showDicePanel, setShowDicePanel] = useState(false);
  const [rolling, setRolling] = useState(false);
  const [diceFaceValue, setDiceFaceValue] = useState<number | string>('?');
  const [diceRotation, setDiceRotation] = useState(0);
  const [diceOffsetY, setDiceOffsetY] = useState(0);
  const [diceTilt, setDiceTilt] = useState(0);
  const [isSettling, setIsSettling] = useState(false);
  const [diceError, setDiceError] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const dicePanelRef = useRef<HTMLDivElement>(null);
  const rollAnimationRef = useRef<number | null>(null);
  const settleTimeoutRef = useRef<number | null>(null);
  const isInCombat = Boolean(gameState?.isInCombat);
  const shouldShowDicePanel = isInCombat && showDicePanel;
  const combatActionSet = new Set(['attack', 'defend', 'flee', 'run']);
  const diceNotation = '1d20';

  useEffect(() => {
    startNewGame();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    if (!isInCombat) {
      setShowDicePanel(false);
      setQueuedCombatAction(null);
      setPendingRoll(null);
    }
  }, [isInCombat]);

  useEffect(() => {
    return () => {
      if (rollAnimationRef.current !== null) {
        window.clearInterval(rollAnimationRef.current);
      }
      if (settleTimeoutRef.current !== null) {
        window.clearTimeout(settleTimeoutRef.current);
      }
    };
  }, []);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const focusDicePanel = () => {
    dicePanelRef.current?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  };

  const startNewGame = async () => {
    setLoading(true);
    try {
      const response = await gameApi.startGame({ characterId: character.id });
      setSessionId(response.sessionId);
      setMessages([
        {
          role: 'assistant',
          content: response.welcomeMessage
        }
      ]);
      setAvailableActions(response.availableActions ?? []);
      setGameState(response.gameState ?? null);
    } catch (error) {
      console.error('Failed to start game:', error);
    } finally {
      setLoading(false);
    }
  };

  const executeAction = async (action: string, rollForAction?: DiceRollResponse | null) => {
    if (!action.trim() || !sessionId) return;

    const rollTag = rollForAction
      ? ` [${diceNotation}=${rollForAction.total}]`
      : '';

    const userMessage: Message = {
      role: 'user',
      content: `${action}${rollTag}`
    };

    setMessages(prev => [...prev, userMessage]);
    setInputAction('');
    setLoading(true);

    try {
      const response = await gameApi.performAction({
        sessionId,
        action,
        playerRollTotal: rollForAction?.total,
        playerRollNotation: rollForAction ? diceNotation : undefined
      });

      const assistantMessage: Message = {
        role: 'assistant',
        content: response.response
      };

      setMessages(prev => [...prev, assistantMessage]);
      setAvailableActions(response.availableActions);
      setGameState(response.gameState);

      if (response.subActions && response.subActions.length > 0) {
        setSubActions(response.subActions);
        setSelectedParentAction(response.selectedParentAction || null);
      } else {
        setSubActions([]);
        setSelectedParentAction(null);
      }
    } catch (error) {
      console.error('Failed to perform action:', error);
      const errorMessage: Message = {
        role: 'assistant',
        content: '⚠️ An error occurred. Please try again.'
      };
      setMessages(prev => [...prev, errorMessage]);
      setSubActions([]);
      setSelectedParentAction(null);
    } finally {
      setPendingRoll(null);
      setLoading(false);
    }
  };

  const handleSubmitAction = async (action: string) => {
    if (!action.trim() || !sessionId) return;

    const normalizedAction = action.toLowerCase().trim();
    const requiresCombatRoll = isInCombat && combatActionSet.has(normalizedAction);

    if (requiresCombatRoll) {
      setShowDicePanel(true);
      setQueuedCombatAction(action);
      focusDicePanel();

      if (pendingRoll) {
        await executeAction(action, pendingRoll);
        return;
      }

      const blockMessage: Message = {
        role: 'assistant',
        content: `🎲 Roll required: roll the die to resolve '${action}'.`
      };
      setMessages(prev => [...prev, blockMessage]);
      return;
    }

    setShowDicePanel(false);
    setQueuedCombatAction(null);
    setPendingRoll(null);
    await executeAction(action, null);
  };

  const handleRollDice = async () => {
    setRolling(true);
    setIsSettling(false);
    setDiceError('');
    setPendingRoll(null);
    setDiceFaceValue('?');

    if (rollAnimationRef.current !== null) {
      window.clearInterval(rollAnimationRef.current);
    }

    rollAnimationRef.current = window.setInterval(() => {
      setDiceRotation(prev => prev + (30 + Math.random() * 70));
      setDiceOffsetY(-6 - Math.random() * 6);
      setDiceTilt((Math.random() - 0.5) * 30);
      setDiceFaceValue(Math.floor(Math.random() * 20) + 1);
    }, 60);

    try {
      const response = await diceApi.roll({ notation: diceNotation });
      setPendingRoll(response);
      setDiceFaceValue(response.total);
      setDiceRotation(prev => prev + 420);

      if (queuedCombatAction) {
        await executeAction(queuedCombatAction, response);
      }
    } catch (error: any) {
      setDiceError(error.response?.data?.message || 'Failed to roll dice');
      setPendingRoll(null);
    } finally {
      if (rollAnimationRef.current !== null) {
        window.clearInterval(rollAnimationRef.current);
        rollAnimationRef.current = null;
      }
      setIsSettling(true);
      setDiceOffsetY(0);
      setDiceTilt(0);
      if (settleTimeoutRef.current !== null) {
        window.clearTimeout(settleTimeoutRef.current);
      }
      settleTimeoutRef.current = window.setTimeout(() => {
        setIsSettling(false);
      }, 420);
      setRolling(false);
      setQueuedCombatAction(null);
    }
  };

  const handleEndGame = async () => {
    if (sessionId) {
      try {
        await gameApi.endSession(sessionId);
      } catch (error) {
        console.error('Failed to end session:', error);
      }
    }
    onExit();
  };

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <div style={styles.headerInfo}>
          <h2 style={styles.title}>🎮 {character.name}</h2>
          {gameState && (
            <div style={styles.stats}>
              <span>📍 {gameState.location}</span>
              <span>❤️ {gameState.characterHp}/{gameState.characterMaxHp}</span>
              <span>💰 {gameState.gold}</span>
            </div>
          )}
        </div>
        <button onClick={handleEndGame} style={styles.exitButton}>
          EXIT
        </button>
      </div>

      <div style={styles.messagesContainer}>
        {messages.map((message, index) => (
          <div
            key={index}
            style={{
              ...styles.message,
              ...(message.role === 'user' ? styles.userMessage : styles.assistantMessage)
            }}
          >
            <div style={styles.messageRole}>
              {message.role === 'user' ? '> YOU' : '> GAME MASTER'}
            </div>
            <div style={styles.messageContent}>
              {message.content.split('\n').map((line, i) => (
                <React.Fragment key={i}>
                  {line}
                  {i < message.content.split('\n').length - 1 && <br />}
                </React.Fragment>
              ))}
            </div>
          </div>
        ))}
        {loading && (
          <div style={styles.loadingMessage}>
            <span style={styles.loadingText}>Processing...</span>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {subActions.length > 0 && selectedParentAction && (
        <div style={styles.subActionsContainer}>
          <div style={styles.subActionsLabel}>📋 {selectedParentAction.toUpperCase()}:</div>
          <div style={styles.actionsGrid}>
            {subActions.map((subAction, index) => (
              <button
                key={index}
                onClick={() => {
                  setSubActions([]);
                  setSelectedParentAction(null);
                  handleSubmitAction(subAction);
                }}
                disabled={loading}
                style={{...styles.actionButton, ...styles.subActionButton}}
              >
                {subAction}
              </button>
            ))}
          </div>
        </div>
      )}

      {availableActions.length > 0 && (
        <div style={styles.actionsContainer}>
          <div style={styles.actionsLabel}>QUICK ACTIONS:</div>
          <div style={styles.actionsGrid}>
            {availableActions.map((action, index) => (
              <button
                key={index}
                onClick={() => handleSubmitAction(action)}
                disabled={loading}
                style={styles.actionButton}
              >
                {action}
              </button>
            ))}
          </div>
        </div>
      )}

      {shouldShowDicePanel && (
      <div style={styles.diceContainer} ref={dicePanelRef}>
        <div style={styles.diceTitle}>
          {isInCombat ? '🎲 COMBAT DICE (REQUIRED)' : '🎲 ACTION DICE'}
        </div>
        <div style={styles.diceControls}>
          <div style={styles.diceVisualizer}>
            <div
              style={{
                ...styles.diceFace,
                ...(rolling ? styles.diceFaceRolling : {}),
                ...(isSettling ? styles.diceFaceSettling : {}),
                transform: `translateY(${diceOffsetY}px) rotate(${diceRotation}deg) skew(${diceTilt}deg, ${-diceTilt / 3}deg)`
              }}
            >
              <D20Icon rolling={rolling} />
              <div
                style={{
                  ...styles.diceValueOverlay,
                  transform: `rotate(${-diceRotation}deg) skew(${-diceTilt}deg, ${diceTilt / 3}deg)`
                }}
              >
                {rolling ? '?' : diceFaceValue}
              </div>
            </div>
            <div style={styles.diceNotationLabel}>1d20</div>
          </div>
          <button
            onClick={handleRollDice}
            disabled={loading || rolling}
            style={styles.diceRollButton}
          >
            ROLL 1D20
          </button>
          <button
            onClick={() => setPendingRoll(null)}
            disabled={loading || rolling || !pendingRoll}
            style={styles.clearRollButton}
          >
            CLEAR
          </button>
        </div>
        {diceError && <div style={styles.diceError}>{diceError}</div>}
        <div style={styles.rollStatus}>
          {isInCombat
            ? (queuedCombatAction
              ? `Queued action: ${queuedCombatAction}. Roll to execute automatically.`
              : (pendingRoll
                ? `Roll ready (${diceNotation}=${pendingRoll.total}). Select a combat action.`
                : 'Choose a combat action to activate the die panel.'))
            : 'Roll first to influence your next action'}
        </div>
      </div>
      )}

      <div style={styles.inputContainer}>
        <input
          type="text"
          value={inputAction}
          onChange={(e) => setInputAction(e.target.value)}
          onKeyPress={(e) => {
            if (e.key === 'Enter' && !loading) {
              handleSubmitAction(inputAction);
            }
          }}
          placeholder="Enter your action..."
          disabled={loading}
          style={styles.input}
        />
        <button
          onClick={() => handleSubmitAction(inputAction)}
          disabled={loading || !inputAction.trim()}
          style={styles.submitButton}
        >
          SEND
        </button>
      </div>
    </div>
  );
};

const styles = {
  container: {
    display: 'flex',
    flexDirection: 'column' as const,
    height: '100vh',
    background: '#0f0f0f',
  },
  header: {
    background: '#1a1a1a',
    borderBottom: '3px solid #00ff00',
    padding: '1rem 2rem',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    boxShadow: '0 0 20px rgba(0, 255, 0, 0.3)',
  },
  headerInfo: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '0.5rem',
  },
  title: {
    margin: 0,
    fontSize: '1rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ff00',
    textShadow: '0 0 10px #00ff00',
  },
  stats: {
    display: 'flex',
    gap: '1.5rem',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ffff',
  },
  exitButton: {
    padding: '0.75rem 1.5rem',
    background: '#ff0000',
    color: '#ffffff',
    border: '2px solid #ff0000',
    cursor: 'pointer',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 0 10px rgba(255, 0, 0, 0.5)',
  },
  messagesContainer: {
    flex: 1,
    overflowY: 'auto' as const,
    padding: '1.5rem',
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '1rem',
  },
  message: {
    padding: '1rem',
    border: '2px solid',
    background: 'rgba(0, 0, 0, 0.5)',
  },
  userMessage: {
    borderColor: '#00ffff',
    alignSelf: 'flex-end',
    maxWidth: '70%',
    boxShadow: '0 0 10px rgba(0, 255, 255, 0.3)',
  },
  assistantMessage: {
    borderColor: '#00ff00',
    alignSelf: 'flex-start',
    maxWidth: '85%',
    boxShadow: '0 0 10px rgba(0, 255, 0, 0.3)',
  },
  messageRole: {
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    marginBottom: '0.5rem',
    opacity: 0.7,
  },
  messageContent: {
    fontSize: '0.7rem',
    fontFamily: "'Press Start 2P', monospace",
    lineHeight: '1.6',
    color: '#00ff00',
    whiteSpace: 'pre-wrap' as const,
  },
  loadingMessage: {
    padding: '1rem',
    border: '2px solid #00ff00',
    borderColor: '#00ff00',
    alignSelf: 'flex-start',
    background: 'rgba(0, 255, 0, 0.1)',
  },
  loadingText: {
    fontSize: '0.7rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ff00',
    animation: 'blink 1s infinite',
  },
  actionsContainer: {
    padding: '1rem 2rem',
    background: '#1a1a1a',
    borderTop: '2px solid #00ff00',
  },
  subActionsContainer: {
    padding: '1rem 2rem',
    background: '#1a2a1a',
    borderTop: '2px solid #00ff00',
    borderBottom: '1px solid #00ff00',
  },
  subActionsLabel: {
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ff00',
    marginBottom: '0.75rem',
  },
  actionsLabel: {
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#00ffff',
    marginBottom: '0.75rem',
  },
  actionsGrid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
    gap: '0.5rem',
  },
  actionButton: {
    padding: '0.5rem',
    background: '#0f0f0f',
    border: '2px solid #00ffff',
    color: '#00ffff',
    cursor: 'pointer',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    transition: 'all 0.2s',
  },
  subActionButton: {
    borderColor: '#00ff00',
    color: '#00ff00',
  },
  inputContainer: {
    padding: '1rem 2rem',
    background: '#1a1a1a',
    borderTop: '3px solid #00ff00',
    display: 'flex',
    gap: '1rem',
  },
  diceContainer: {
    padding: '0.8rem 2rem',
    background: '#1a1a12',
    borderTop: '2px solid #f7d80b',
  },
  diceTitle: {
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#f7d80b',
    marginBottom: '0.6rem',
    textShadow: '0 0 8px rgba(247, 216, 11, 0.45)',
  },
  diceControls: {
    display: 'grid',
    gridTemplateColumns: 'auto auto auto',
    gap: '0.5rem',
    alignItems: 'center',
    marginBottom: '0.5rem',
  },
  diceVisualizer: {
    display: 'flex',
    alignItems: 'center',
    gap: '0.5rem',
  },
  diceFace: {
    width: '42px',
    height: '42px',
    position: 'relative' as const,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    transition: 'transform 70ms linear, filter 120ms ease',
    transformOrigin: '50% 55%',
  },
  diceFaceRolling: {
    filter: 'drop-shadow(0 0 12px rgba(247, 216, 11, 0.95))',
  },
  diceFaceSettling: {
    transition: 'transform 420ms cubic-bezier(0.2, 1.2, 0.24, 1)',
  },
  diceValueOverlay: {
    position: 'absolute' as const,
    left: '50%',
    top: '50%',
    transform: 'translate(-50%, -50%)',
    color: '#050505',
    fontSize: '0.95rem',
    fontFamily: "'Press Start 2P', monospace",
    fontWeight: 'bold' as const,
    textShadow: '0 0 6px rgba(255, 255, 255, 0.55)',
    background: 'rgba(255, 245, 170, 0.85)',
    border: '2px solid #111111',
    borderRadius: '6px',
    minWidth: '26px',
    height: '22px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    padding: '0 2px',
    pointerEvents: 'none' as const,
  },
  diceNotationLabel: {
    fontSize: '0.55rem',
    color: '#f7d80b',
    fontFamily: "'Press Start 2P', monospace",
  },
  diceRollButton: {
    padding: '0.6rem 1rem',
    border: '2px solid #f7d80b',
    background: '#f7d80b',
    color: '#111111',
    cursor: 'pointer',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
    fontWeight: 'bold' as const,
  },
  clearRollButton: {
    padding: '0.6rem 1rem',
    border: '2px solid #f7d80b',
    background: '#1a1a1a',
    color: '#f7d80b',
    cursor: 'pointer',
    fontSize: '0.6rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  rollStatus: {
    fontSize: '0.55rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#ffe880',
  },
  diceError: {
    fontSize: '0.55rem',
    fontFamily: "'Press Start 2P', monospace",
    color: '#ff5555',
    marginBottom: '0.4rem',
  },
  input: {
    flex: 1,
    padding: '0.75rem',
    border: '2px solid #00ff00',
    background: '#0f0f0f',
    color: '#00ff00',
    fontSize: '0.7rem',
    fontFamily: "'Press Start 2P', monospace",
  },
  submitButton: {
    padding: '0.75rem 2rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    cursor: 'pointer',
    fontSize: '0.7rem',
    fontFamily: "'Press Start 2P', monospace",
    boxShadow: '0 4px 0 #00aa00',
  },
};

export default Game;
