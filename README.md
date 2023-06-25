<h1 align="center">
    Snek
</h1>

<p align="center">
    A terminal version of the classic arcade/mobile game, Snake.
</p>
<br/>
<br/>
<br/>
<br/>

# Description
A cross-platform version of Snake written in C#. 

Features:

  - [x] Configurable window size
  - [x] Configurable speed
  - [x] Toggle whether or not the game should gradually speed up
  - [x] Toggle sound effects
  - [x] Different behavior when colliding with a wall
  - [x] Runs in console (Windows, MacOS, Linux)
  - [ ] Runs in Browser (Blazor)
# Gameplay

<details>
  <summary>Click to see short gifs of some gameplay.</summary>
  
  ### Default
  If you run the game without specifying any arguments, this is what you get.

  ![Default](/Images/Snek_Default.gif)

  ### Portal
  If you run the game and specify the Wall Collision Behavior (`-c`, `--collision`) as `Portal`, this is what you get.
  
  ![Portal](/Images/Snek_Portal.gif)

  ### Rebound
  If you run the game and specify the Wall Collision Behavior (`-c`, `--collision`) as `Rebound`, this is what you get.
  
  ![Rebound](/Images/Snek_Rebound.gif)
</details>

# Installation

Download the [latest release](https://github.com/devklick/Snek/releases) zip file for your operating system and extract it to a folder of your choice.

## Invoking the executable

### Windows

You can either-double click the `Snek.exe` file, or open a command prompt, `cd` folder you extracted the zip file to (e.g. `cd %HOMEPATH%\\Downloads\\Snek`) and invoke the executable by calling `Snek.exe`).

### Linux

Open a terminal, navigate to the folder you extracted the zip file to (e.g. `cd ~/Downloads/Snek`) and invoke the executable by calling `./Snek`.

### OSX

Open a terminal, navigate to the folder you extracted the zip file to (e.g. `cd ~/Downloads/Snek`) and invoke the executable by calling `./Snek`.

# Key Bindings

| Keys           | Description             |
| -------------- | ----------------------- |
| Up Arrow, W    | Face up/north           |
| Left Arrow, A  | Face left/west          |
| Right Arrow, D | Face right/east         |
| Down Arrow, S  | Face down/south         |
| Escape         | Toggle pause            |
| R              | Replay (when game over) |
| Escape         | Quit (when game over)   |

# Audio

The sound effects are annoying. Love 'em or hate 'em. If they're too much for you, you can disable them. See the [Supported arguments](#supported-arguments) for more info on this.

# Game options

There are a number of options that can can be configured when starting a game. The only way to specify these options at present is via command line arguments, so the game would have to be launched from the command line rather than by double-clicking the executable.

## Supported arguments

### Display Help

- Arguments: `--help`, `-h`
- Description: Shows this help information

### Game Width

- Arguments: `--width`, `-x`
- Description: The number of cells along the horizontal axis
- Type: Number (min 13, max 80)
- Default: 15

### Game Height

- Arguments: `--height`, `-y`
- Description: The number of cells along the vertical axis
- Type: Number (min 6, max 80)
- Default: 15

### Initial Game Speed

- Arguments: `--speed`, `-s`
- Description: The starting number of times per second the snake will move
- Type: Number (min 1, max 50)
- Default: 8

### Increase Speed on Enemy Destroyed

- Arguments: `--increase-speed`, `-i`
- Description: Whether or not the snake should get faster every time it destroys an enemy
- Type: Boolean (True, False)
- Default: False

### Wall Collision Behavior

- Arguments: `--collision`, `-c`
- Description: How the game should behave when the snake collides with a wall
- Type: Enum (GameOver, Rebound, Portal)
  - GameOver: The game is over
  - Rebound: The players snake is reversed and continues traveling in the opposite direction
  - Portal: The player travels through walls. They will continue moving in the same direction but will emerge from the wall opposite the one they collided with
- Default: GameOver

### Audio Enabled

- Arguments: `--audio`, `-a`
- Description: Whether or not sound effects should play
- Type: Boolean (True, False)
- Default: True

### Debug Logs

- Arguments: `--debug`, `-d`
- Description: Whether or not to log events to file
- Type: Boolean (True, False)
- Default: False
