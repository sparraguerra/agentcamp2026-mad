import React from 'react';
import { render } from '@testing-library/react';
import CharacterAvatar from './CharacterAvatar';
import { CharacterClass } from '../types';

describe('CharacterAvatar', () => {
  it('renders without crashing for Warrior', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Warrior} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders without crashing for Mage', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Mage} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders without crashing for Rogue', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Rogue} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders without crashing for Cleric', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Cleric} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders without crashing for Ranger', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Ranger} />);
    expect(container.firstChild).toBeInTheDocument();
  });

  it('renders with custom size', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Warrior} size={100} />);
    const avatar = container.firstChild as HTMLElement;
    expect(avatar).toHaveStyle({ width: '100px', height: '100px' });
  });

  it('renders with default size when not specified', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Warrior} />);
    const avatar = container.firstChild as HTMLElement;
    expect(avatar).toHaveStyle({ width: '64px', height: '64px' });
  });

  it('renders 256 pixels (16x16 grid)', () => {
    const { container } = render(<CharacterAvatar characterClass={CharacterClass.Warrior} />);
    const avatar = container.firstChild as HTMLElement;
    const pixels = avatar.querySelectorAll('div');
    expect(pixels.length).toBe(256); // 16x16 = 256 pixels
  });
});
