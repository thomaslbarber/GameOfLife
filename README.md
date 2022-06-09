# GameOfLife
A repository implementing the zero-player [Game of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) (by John Conway) in the Unity game engine (v2020.1.6f1), using C#.


### How to Use
The project allows the user to move using the WASD keys and zoom in/out using the mouse scrollwheel. The GUI contains five simple buttons:
1. Start - Starts the simulation using the current world state; this can also be achieved by pressing the space bar when the simulation is stopped.
2. Stop - Stops the simulation; this can also be achieved by pressing the space bar when the simulation is playing.
3. Alive - Sets all cells in the world to alive.
4. Dead - Sets all cells in the world to dead.
5. Randomise - Randomises the state (alive/dead) of each cell, with a 1/8 chance of a cell being alive.

Whilst the simulation is paused, the user can change the state of a cell by clicking on it, with the mouse buttons corresponding to:
- Left - Makes the clicked cell alive.
- Right - Makes the clicked cell dead.


Once happy with the initial configuration of the world, pressing either the Start button or the space bar begins the simulation:

![Example Simulation](GameOfLife.gif)
