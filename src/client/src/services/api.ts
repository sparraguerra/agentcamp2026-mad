import axios from 'axios';
import { LoginRequest, RegisterRequest, LoginResponse, Character, CreateCharacterRequest, DiceRollRequest, DiceRollResponse, StartGameRequest, StartGameResponse, GameActionRequest, GameActionResponse } from '../types';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5101/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to requests if available
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auth API
export const authApi = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/auth/login', data);
    return response.data;
  },
  
  register: async (data: RegisterRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>('/auth/register', data);
    return response.data;
  }
};

// Characters API
export const charactersApi = {
  getAll: async (): Promise<Character[]> => {
    const response = await api.get<Character[]>('/characters');
    return response.data;
  },
  
  getById: async (id: number): Promise<Character> => {
    const response = await api.get<Character>(`/characters/${id}`);
    return response.data;
  },
  
  create: async (data: CreateCharacterRequest): Promise<Character> => {
    const response = await api.post<Character>('/characters', data);
    return response.data;
  }
};

// Dice API
export const diceApi = {
  roll: async (data: DiceRollRequest): Promise<DiceRollResponse> => {
    const response = await api.post<DiceRollResponse>('/dice/roll', data);
    return response.data;
  }
};

// Game API
export const gameApi = {
  startGame: async (data: StartGameRequest): Promise<StartGameResponse> => {
    const response = await api.post<StartGameResponse>('/game/start', data);
    return response.data;
  },
  
  performAction: async (data: GameActionRequest): Promise<GameActionResponse> => {
    const response = await api.post<GameActionResponse>('/game/action', data);
    return response.data;
  },
  
  getSession: async (sessionId: number): Promise<any> => {
    const response = await api.get(`/game/session/${sessionId}`);
    return response.data;
  },
  
  endSession: async (sessionId: number): Promise<void> => {
    await api.post(`/game/end/${sessionId}`);
  }
};

export default api;
