# NURIKABE
Applications to test all possible legal patterns for a [n,n] Nurikabe grid.

<h2> Rules </h2>
	* Water (black squares) must be continuous
	<img src="images/non-contiguous.PNG" alt="icon">
	* Cannot have pools of water
	<img src="images/pool.PNG" alt="icon">
	* Good pattern
		<img src="images/good.PNG" alt="icon">
	
# Solutions
	1. Brute force 
		1. A low level approach
		2. considers all possible solutions when testing pattern
		
	2. Pruning 
		1. Attempts to prune bad patterns early
		2. With large [n x n] prunes nearly 99% of patterns

### Solution times
  | N  | Brute mm:ss.ms) | Prune (s) |
  | :---: | :---: | :---: |
  |3x3 | 00:00.0019308          | 0.0038    |
  |4x4 | 00:00.0049499          | 0.0867    |
  |5x5 | 00:01.1569876          | 13.2519   |
  |6x6 |  37:06.4171906         | 8118.2854 |
  |7x7 | N/A |  N/A       |