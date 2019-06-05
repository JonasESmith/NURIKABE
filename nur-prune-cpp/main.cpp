// nurikabe_prune_cpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
// Programmer			  : JONAS SMITH

#include <iostream>
#include <vector>
#include <bitset>
#include <cstdio>
#include <ctime>
#include <string>
#include <math.h>

const int matrixDim = 2;

void GenRows(int index);
void GenPattern(int index, std::vector< std::bitset<matrixDim> > pattern);
bool PoolCheck(std::vector< std::bitset<matrixDim>> poolVector);
bool ContCheck(std::vector< std::bitset<matrixDim>> Vector);
int ConnectedWater(std::vector< std::string > wtrCntPattern, int i, int j);

int patCount;
int recCount;
bool check = true;

std::vector< std::bitset<matrixDim> > generatedRows;
std::bitset<matrixDim> globalPattern;

int main()
{
	std::bitset<matrixDim> blank_bitSet;
	std::vector< std::bitset<matrixDim>> matrix;

	// this creates the blank pattern that we will use to genereate the patterns
	for (int i = 0; i < matrixDim; i++)
		matrix.push_back(blank_bitSet);

	//for (int i = 0; i < matrixDim; i++)
	//{
	//	for (int j = 0; j < matrixDim; j++)
	//		std::cout << matrix[i][j];
	//	std::cout << "\n";
	//}

	generatedRows.clear();
	GenRows(0);

	/*
	std::clock_t start = std::clock();
	double duration = 0;
	duration = (std::clock() - start) / (double)CLOCKS_PER_SEC;
	*/

	patCount = 0;
	recCount = 0;

	std::vector<std::bitset<matrixDim>> genPattern;
	for (int i = 0; i < matrixDim; i++)
		genPattern.push_back(blank_bitSet);

	GenPattern(0, genPattern);

	std::cout << "Possibl ptrns : " << std::pow(2, matrixDim * matrixDim) << "\n";
	std::cout << "Pattern count : " << patCount << "\n";
	std::cout << "Recursv count : " << recCount << "\n";


}

void GenRows(int index)
{
	if (index < matrixDim)
	{
		globalPattern[index] = true;
		GenRows(index + 1);

		globalPattern[index] = false;
		GenRows(index + 1);
	}
	else
	{
		generatedRows.push_back(globalPattern);
	}
}

void GenPattern(int index, std::vector< std::bitset<matrixDim> > pattern)
{
	if (index < matrixDim)
	{
		for (int i = 0; i < generatedRows.size(); i++)
		{
			pattern[index] = generatedRows[i];

			if (index != 0)
			{
				std::vector<std::bitset<matrixDim>> poolVector;

				poolVector.push_back(pattern[index - 1]);
				poolVector.push_back(pattern[index]);
				if (PoolCheck(poolVector))
				{
					if (ContCheck(pattern))
					{

					}
					else
					{
						if (index < matrixDim - 1)
						{
							for (int rowIndex = 0; rowIndex < generatedRows.size(); rowIndex++)
							{
								pattern[index + 1] = generatedRows[rowIndex];

								if (ContCheck(pattern))
									goto done; // >:)
							}
						}

					}
					done:;
				}
			}
			if (check || index == 0)
			{
				GenPattern(index + 1, pattern);
				recCount++;

				/*for (int i = 0; i < matrixDim; i++)
					for (int j = 0; j < matrixDim; j++)
						std::cout << pattern[i][j];*/
			}
		}
	}
	else if (check)
	{
		std::cout << patCount << " : Pattern : ";

		patCount++;
		for (int i = 0; i < matrixDim; i++)
		{
			for (int j = 0; j < matrixDim; j++)
				std::cout << pattern[i][j];
		}

		std::cout << "\n";
	}
}

bool PoolCheck(std::vector< std::bitset<matrixDim>> poolVector)
{
	check = true;
	for (int i = 0; i < poolVector[0].size() - 1; i++)
	{
		if (poolVector[0][i] == true && poolVector[0][i + 1] == true && poolVector[1][i] == true && poolVector[1][i + 1] == true)
		{
			check = false;
		}
	}

	return check;
}

bool ContCheck(std::vector< std::bitset< matrixDim >> contVector)
{
	int waterCount = 0;
 	int contStream = 0;
	int wtrRow;
	int wtrCol;

	for (int i = 0; i < matrixDim; i++)
		for (int j = 0; j < matrixDim; j++)
			if (contVector[i][j] == true) {
				waterCount++;
				if (waterCount == 1)
				{ wtrRow = i; wtrCol = j; } }
	if (waterCount > 1)
	{
		std::vector<std::string> copyPattern;
		for (int i = 0; i < matrixDim; i++)
		{
			copyPattern.push_back(contVector[i].to_string());
		}

		contStream = ConnectedWater(copyPattern, wtrRow, wtrCol);
	}
	else
		if (waterCount != 0)
			contStream = 1;

	if (contStream != waterCount)
		check = false;

	return check;
}

int ConnectedWater(std::vector< std::string > wtrCntPattern, int row, int col)
{
	int area;

	if (wtrCntPattern[row][col] == '1')
	{
		wtrCntPattern[row][col] = '3';
		area = 1;

		if (col + 1 <= matrixDim - 1)
			area += ConnectedWater(wtrCntPattern, row, col + 1);
		if (row + 1 <= matrixDim - 1)
			area += ConnectedWater(wtrCntPattern, row + 1, col);
		if (col - 1 >= 0)
			area += ConnectedWater(wtrCntPattern, row, col - 1);
		if (row - 1 >= 0)
			area += ConnectedWater(wtrCntPattern, row - 1, col);
	}
	else
	{
   		area = 0;
	}

	return area;
}