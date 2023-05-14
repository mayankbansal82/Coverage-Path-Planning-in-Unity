# Coverage-Path-Planning-in-Unity

This project implements two different algorithms for coverage path planning in a 2D environment. The goal is to generate a path that will cover the entire area, while minimizing the total distance traveled and time taken to do so.

## Algorithms
The two algorithms implemented in this project are:
1) Complete Coverage Path Planning
2) Spiral spanning tree coverage path planning

## Comparison
Both algorithms are capable of generating paths that cover the entire area, but they have different strengths and weaknesses. Complete coverage path planning covers each and every cell but gives overlapping cells in the path, while spiral spanning tree coverage does not cover each cell, it generates a path which does not repeat already visited cells.

In our tests, complete coverage path planning generates the path in a shorter time compared to the spiral spanning coverage algorithm. 

## Conclusion
In conclusion, both algorithms are viable options for coverage path planning in a 2D environment. The choice between them will depend on the specific requirements of the project, such as computational resources and robustness to changes in the environment.

## Results
The output of complete coverage path planning algorithm is shown below:

![Alt Text](https://github.com/mayankbansal82/Coverage-Path-Planning-in-Unity/blob/main/images/ccp.gif)


The output of spiral spanning tree coverage path planning algorithm is shown below:

![Alt Text](https://github.com/mayankbansal82/Coverage-Path-Planning-in-Unity/blob/main/images/stc.gif)

