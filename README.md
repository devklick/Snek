<h1 align="center">
    Snek
</h1>

<p align="center">
    A terminal version of the classic mobile game, Snake.
</p>
<br/>
<br/>
<br/>
<br/>

# Installation

Download the [latest release](https://github.com/devklick/Snek/releases) zip file for your operating system and extract it to a folder of your choice.

## Invoking the executable

### Windows

You can either-double click the `Snek.exe` file, or open a command prompt, `cd` folder you extracted the zip file to (e.g. `cd %HOMEPATH%\\Downloads\\Snek`) and invoke the executable by calling `Snek.exe`).

### Linux

Open a terminal, navigate to the folder you extracted the zip file to (e.g. `cd ~/Downloads/Snek`) and invoke the executable by calling `./Snek`.

### OSX

# Key Bindings

| Keys           | Description             |
| -------------- | ----------------------- |
| Up Arrow, W    | Face up/north           |
| Left Arrow, A  | Face left/west          |
| Right Arrow, D | Face right/east         |
| Down Arrow, S  | Face down/south         |
| Escape         | Toggle pause            |
| R              | Replay (when game over) |
| Q              | Quit (when game over)   |

# Audio

The sound effects are annoying. Love 'em or hate 'em. Currently there's no way to disable them, but an option will be added in future.

# Game options

There are a number of options that can can be configured when starting a game, however the only way to change this at the moment is via the code.

| Keys                          | Type    | Description                                                                                                                                                                                                                                          |
| ----------------------------- | ------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Width`                         | Number  | The number of cells across the horizontal axis                                                                                                                                                                                                       |
| `Height`                        | Number  | The number of cells across the vertical axis                                                                                                                                                                                                         |
| `InitialTicksPerSecond`         | Number  | The number of times per second the snake will move. This may increase depending on the `IncreaseSpeedOnEnemyDestroyed` setting                                                                                                                       |
| `IncreaseSpeedOnEnemyDestroyed` | Boolean | Whether or not the snake should get faster every time it destroys an enemy                                                                                                                                                                           |
| `WallCollisionBehavior`         | Enum    | `GameOver` - The game ends when the snake collides with a wall <br/>`Rebound` - The players snake is reversed and continues in the opposite direction it was facing when it collided with the wall <br/>`Portal` - Allows the player to travel through walls |

## Setup UI (future enhancement)

In the future I plan to add a small UI that allows you to select the options you want to play the game with. Until then, the only way to try out these different settings is to clone the repo, tweak the code and run it locally. 