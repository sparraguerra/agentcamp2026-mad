import React, { useState } from 'react';
import { authApi } from '../services/api';
import { LoginRequest, RegisterRequest } from '../types';

interface Props {
  onLogin: (token: string, userId: number, username: string) => void;
}

const Login: React.FC<Props> = ({ onLogin }) => {
  const [isLogin, setIsLogin] = useState(true);
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      if (isLogin) {
        const data: LoginRequest = { username, password };
        const response = await authApi.login(data);
        localStorage.setItem('token', response.token);
        onLogin(response.token, response.userId, response.username);
      } else {
        const data: RegisterRequest = { username, email, password };
        const response = await authApi.register(data);
        localStorage.setItem('token', response.token);
        onLogin(response.token, response.userId, response.username);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'An error occurred');
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={styles.title}>{isLogin ? '> LOGIN_' : '> REGISTER_'}</h2>
        <form onSubmit={handleSubmit} style={styles.form}>
          <input
            type="text"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            style={styles.input}
          />
          {!isLogin && (
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              style={styles.input}
            />
          )}
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            style={styles.input}
          />
          {error && <div style={styles.error}>{error}</div>}
          <button type="submit" style={styles.button}>
            {isLogin ? 'Login' : 'Register'}
          </button>
        </form>
        <p style={styles.toggle}>
          {isLogin ? "Don't have an account? " : 'Already have an account? '}
          <button
            onClick={() => setIsLogin(!isLogin)}
            style={styles.linkButton}
          >
            {isLogin ? 'Register' : 'Login'}
          </button>
        </p>
      </div>
    </div>
  );
};

const styles = {
  container: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    minHeight: '100vh',
    background: '#0f0f0f',
    padding: '20px',
  },
  card: {
    background: '#1a1a1a',
    padding: '2rem',
    border: '4px solid #00ff00',
    boxShadow: '0 0 20px rgba(0, 255, 0, 0.5), inset 0 0 20px rgba(0, 255, 0, 0.1)',
    width: '100%',
    maxWidth: '500px',
    position: 'relative' as const,
  },
  form: {
    display: 'flex',
    flexDirection: 'column' as const,
    gap: '1.5rem',
  },
  input: {
    padding: '1rem',
    border: '2px solid #00ff00',
    background: '#0f0f0f',
    color: '#00ff00',
    fontSize: '0.75rem',
    fontFamily: "'Press Start 2P', monospace",
    outline: 'none',
    transition: 'all 0.3s',
  },
  button: {
    padding: '1rem',
    background: '#00ff00',
    color: '#0f0f0f',
    border: '2px solid #00ff00',
    fontSize: '0.75rem',
    cursor: 'pointer',
    fontWeight: 'bold' as const,
    fontFamily: "'Press Start 2P', monospace",
    transition: 'all 0.3s',
    boxShadow: '0 4px 0 #00aa00',
    position: 'relative' as const,
    top: '0',
  },
  error: {
    color: '#ff0000',
    fontSize: '0.65rem',
    textShadow: '0 0 5px #ff0000',
    padding: '0.5rem',
    border: '2px solid #ff0000',
    background: 'rgba(255, 0, 0, 0.1)',
  },
  toggle: {
    marginTop: '1.5rem',
    textAlign: 'center' as const,
    fontSize: '0.65rem',
    color: '#00ff00',
  },
  linkButton: {
    background: 'none',
    border: 'none',
    color: '#00ffff',
    cursor: 'pointer',
    textDecoration: 'none',
    fontSize: '0.65rem',
    fontFamily: "'Press Start 2P', monospace",
    textShadow: '0 0 5px #00ffff',
    padding: '0.5rem',
  },
  title: {
    color: '#00ff00',
    textAlign: 'center' as const,
    marginBottom: '1.5rem',
    fontSize: '1.2rem',
    textShadow: '0 0 10px #00ff00, 0 0 20px #00ff00',
    letterSpacing: '2px',
  },
};

export default Login;
