# Character Avatar Component

## Overview

The `CharacterAvatar` component renders 16-bit pixelated avatars for each D&D character class using CSS Grid.

## Character Classes

Each class has a unique 16x16 pixel art design:

### ⚔️ Warrior
- Red armor (#e74c3c)
- Silver sword
- Brown boots
- Strong and commanding appearance

### 🔮 Mage  
- Blue robes (#3498db)
- Wizard hat
- Wooden staff
- Mystical appearance

### 🗡️ Rogue
- Purple hooded cloak (#9b59b6)
- Silver dagger
- Stealthy appearance

### ✨ Cleric
- White robes with golden accents (#f39c12)
- Holy cross symbol
- Blessed appearance

### 🏹 Ranger
- Green cloak (#27ae60)
- Wooden bow
- Nature-inspired appearance

## Usage

```tsx
import CharacterAvatar from './CharacterAvatar';
import { CharacterClass } from '../types';

// Default size (64x64)
<CharacterAvatar characterClass={CharacterClass.Warrior} />

// Custom size
<CharacterAvatar characterClass={CharacterClass.Mage} size={80} />
```

## Technical Details

- **Grid**: 16x16 pixel grid
- **Rendering**: CSS Grid with `image-rendering: pixelated`
- **Colors**: Retro 16-bit color palette
- **Size**: Customizable via `size` prop (default: 64px)
- **Border**: Neon green (#00ff00) with glow effect

## Color Palette

| Code | Color | Usage |
|------|-------|-------|
| M    | #D4A574 | Skin/Face |
| E    | #000000 | Eyes |
| R    | #e74c3c | Warrior armor (red) |
| B    | #8B4513 | Boots/leather (brown) |
| D    | #2C2C2C | Shadows (dark gray) |
| G    | #C0C0C0 | Metal/sword (silver) |
| BL   | #3498db | Mage robes (blue) |
| W    | #FFFFFF | Cleric robes (white) |
| P    | #9b59b6 | Rogue cloak (purple) |
| Y    | #f39c12 | Holy symbols (gold) |
| GR   | #27ae60 | Ranger cloak (green) |
| BR   | #8B4513 | Wood/bow (brown) |
| S    | #C0C0C0 | Sword (silver) |

## Integration

The avatar is integrated into:

1. **CharacterCard**: Displays 80x80px avatar for each character
2. **CreateCharacter**: Shows 48x48px previews when selecting class

## Styling

The component matches the retro pixel-art aesthetic:
- Pixelated rendering for authentic retro feel
- Neon green borders with glow effects
- Press Start 2P font family for text
- Consistent with the overall D&D Copilot theme
