# What is this?
This repository contains the findings of the research that was conducted by students at Missouri Southern State University.

**Students**: [Jonas Smith](https://github.com/JonasESmith), [Jaired Collins](https://github.com/jairedcollins), and [Andrew Robinson](https://github.com/SirArkimedes)

**Advisors**: Dr. Charles Curtis and Dr. Jacob Boswell

# NURIKABE
Applications to test all possible legal patterns for a [n,n] Nurikabe grid.

<h2> Rules </h2>
	<h4> 1. Water (black squares) must be continuous </h4>
		<img src="images/non-contiguous.PNG" alt="icon">
	<h4> 2. Cannot have pools of water </h4>
		<img src="images/pool.PNG" alt="icon">
	<h4> 3. Good pattern </h4>
		<img src="images/good.PNG" alt="icon">

# Solutions

	Brute force
		* A low level approach
		* considers all possible solutions when testing pattern
	Pruning
		* Attempts to prune bad patterns early
		* With large [n x n] prunes nearly 99% of patterns

### Solution times
  | N  | Brute (mm:ss.ms) | Prune (s) |
  | :---: | :---: | :---: |
  |3x3 | 00:00.0019308          | 0.0038    |
  |4x4 | 00:00.0049499          | 0.0867    |
  |5x5 | 00:01.1569876          | 13.2519   |
  |6x6 |  37:06.4171906         | 8118.2854 |
  |7x7 | N/A |  N/A       |

### Solution Results
  | N  | Good patterns |
  | :---: | :---: |
  |2x2 | 13 |
  |3x3 | 140 |
  |4x4 | 3,756 |
  |5x5 | 318,890 |
  |6x6 | 84,900,755 |
  |7x7 | N/A |
