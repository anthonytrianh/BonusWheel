# BonusWheel

Notes:
- I created a quick function that reads data from a text file and allocates the specified number of prizes, prize types, counts, and drop chances which is used by the Wheel class.
  The file uses the following format:
  The first line indicates the number of prizes/items, n
  The following n lines: sprite-name prize-count drop-chance
- The prizes are instantiated, positioned, and rotated procedurally and distributed evenly around the wheel for however many prizes listed.
- The wheel and the result screen are set to separate layer masks on the main camera, which are then turned on or off according depending on which state is shown.
- The wheel's animation settings can be adjusted easily within the inspector.
- I sourced/modified a 2D perlin noise shader to create the glowing effect for the results screen.

CONTROLS (within Unity):
- Enter: Simulate 1000 spins and outputs to Assets/Resources/Text/Unit Test.txt
