export interface User {
  id: number;
  username: string;
  email: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  username: string;
}

export enum CharacterClass {
  Warrior = 0,
  Mage = 1,
  Rogue = 2,
  Cleric = 3,
  Ranger = 4
}

export interface Character {
  id: number;
  name: string;
  class: CharacterClass;
  level: number;
  experience: number;
  hitPoints: number;
  maxHitPoints: number;
  strength: number;
  dexterity: number;
  constitution: number;
  intelligence: number;
  wisdom: number;
  charisma: number;
  gold: number;
}

export interface CreateCharacterRequest {
  name: string;
  class: CharacterClass;
}

export interface DiceRollRequest {
  notation: string;
}

export interface DiceRollResponse {
  rolls: number[];
  modifier: number;
  total: number;
  description: string;
}

export interface GameMessage {
  role: string;
  content: string;
  timestamp: string;
}

export interface StartGameRequest {
  characterId: number;
}

export interface StartGameResponse {
  sessionId: number;
  welcomeMessage: string;
  characterName: string;
  characterClass: string;
  availableActions: string[];
  gameState: {
    location: string;
    characterHp: number;
    characterMaxHp: number;
    gold: number;
    isInCombat?: boolean;
  };
}

export interface GameActionRequest {
  sessionId: number;
  action: string;
  playerRollTotal?: number;
  playerRollNotation?: string;
}

export interface GameActionResponse {
  response: string;
  availableActions: string[];
  subActions?: string[];
  selectedParentAction?: string;
  gameState: {
    location: string;
    characterHp: number;
    characterMaxHp: number;
    gold: number;
    isInCombat?: boolean;
  };
}
