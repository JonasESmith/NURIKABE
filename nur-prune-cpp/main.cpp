#include <iostream>
#include <vector>
#include <bitset>

using namespace std; // allows me to not use std::

const int matrixDim = 3;
int area;
bool check;

// this will be the list of bitsets or the list of all possible rows for a pattern size.
std::vector<std::bitset<matrixDim>> generatedRows;
std::bitset<matrixDim> globalPattern;

// Declerations for the methods and functions used bellow
bool PoolCheck(vector<bitset<matrixDim>> poolVector);
bool ContinuityCheck(vector<bitset<matrixDim>> poolVector);

int ConnectedWater(vector<bitset<matrixDim>> pattern, int row, int col);
int WaterCount(vector<bitset<matrixDim>> pattern);

void GeneratePattern(int index, vector<bitset<matrixDim>> pattern, int &patternCount, int &recursiveCalls);
void GenerateRows(int index);

int main()
{
    // int matrixDim;
    // std::cout << "What dimension is the matrix? : ";
    // std::cin >> matrixDim;

    // start timer here is necessary

    int patternCount = 0, recursiveCalls = 0;

    bitset<matrixDim * matrixDim> patVect;

    std::vector<std::bitset<matrixDim>> genereatedPattern;

    bitset<matrixDim> blankBitSet;

    for (int i = 0; i < matrixDim; i++)
        genereatedPattern.push_back(blankBitSet);

    generatedRows.clear();
    GenerateRows(0);

    // for (int i = 0; i < generatedRows.size(); i++)
    // {
    //     for (int j = 0; j < generatedRows[i].size(); j++)
    //     {
    //         cout << generatedRows[i][j];
    //     }
    //     cout << "\n";
    // }

    // // this is an extremely easy way to test the PoolCheck method
    // vector<bitset<2>> poolTest;
    // poolTest.push_back(11);
    // poolTest.push_back(00);
    // cout << PoolCheck(poolTest);

    // easy way to test continuityCheck
    // vector<bitset<2>> contTest;
    // contTest.push_back(01);
    // contTest.push_back(00);
    // ContinuityCheck(contTest);

    GeneratePattern(0, genereatedPattern, patternCount, recursiveCalls);
    std::cout << "Pattern count   : " << patternCount << "\n";
    std::cout << "Recursive Calls : " << recursiveCalls << "\n";

    std::getchar();

    return 0;
}

bool PoolCheck(vector<bitset<matrixDim>> poolVector)
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

bool ContinuityCheck(vector<bitset<matrixDim>> pattern)
{
    int waterCount = WaterCount(pattern);
    int cnctWaterCount;

    int row, col;

    if (waterCount > 1)
    {
        for (int i = 0; i < matrixDim; i++)
        {
            for (int j = 0; j < matrixDim; j++)
            {
                if (pattern[i][j] == 1)
                {
                    row = i;
                    col = j;
                    goto done; // >:)
                }
            }
        }

        done:; // >:)

        vector<bitset<matrixDim>> copyPattern = pattern;

        cnctWaterCount = ConnectedWater(copyPattern, row, col);
    }
    else if (waterCount == 1)
        cnctWaterCount = 1;
    else
        cnctWaterCount = 0;

    if (cnctWaterCount == waterCount)
        check = true;
    else
        check = false;

    return check;
}

int ConnectedWater(vector<bitset<matrixDim>> pattern, int i, int j)
{
    if (pattern[i][j] == 1)
    {
        pattern[i][j] = NULL;
        area = 1;

        if (j + 1 <= matrixDim - 1)
            area += ConnectedWater(pattern, i, j + 1);
        if (i + 1 <= matrixDim - 1)
            area += ConnectedWater(pattern, i + 1, j);
        if (j - 1 >= 0)
            area += ConnectedWater(pattern, i, j - 1);
        if (i - i >= 0)
            area += ConnectedWater(pattern, i - 1, j);
    }
    else
        area = 0;

    return area;
}

int WaterCount(vector<bitset<matrixDim>> pattern)
{
    int count;

    for (int i = 0; i < matrixDim; i++)
    {
        for (int j = 0; j < matrixDim; j++)
        {
            if (pattern[i, j] == 1)
            {
                count++;
            }
        }
    }

    return count;
}

// recursive operation to check possible patterns
void GeneratePattern(int index, vector<bitset<matrixDim>> pattern, int &patternCount, int &recursiveCalls)
{
    if (index < matrixDim)
    {
        for (int i = 0; i < generatedRows.size(); i++)
        {
            pattern[index] = generatedRows[i];

            if (index != 0)
            {
                vector<bitset<matrixDim>> poolVector;

                poolVector.push_back(pattern[index - 1]);
                poolVector.push_back(pattern[index]);
                if (PoolCheck(poolVector))
                {
                    if (ContinuityCheck(pattern))
                    {
                        check = true;
                    }
                    else
                    {
                        if (index < matrixDim - 1)
                        {
                            for (int rowIndex = 0; rowIndex < generatedRows.size(); rowIndex++)
                            {
                                pattern[index + 1] = generatedRows[rowIndex];

                                if (ContinuityCheck(pattern))
                                    goto done; // >:)
                            }
                        }

                    }
                done:;
                }
            }
            if (check || index == 0)
            {
                GeneratePattern(index + 1, pattern, patternCount, recursiveCalls);
                recursiveCalls++;
            }
        }
    }
    else if (check)
        patternCount++;
}

void GenerateRows(int index)
{
    if (index < matrixDim)
    {
        globalPattern[index] = true;
        GenerateRows(index + 1);

        globalPattern[index] = false;
        GenerateRows(index + 1);
    }
    else
    {
        generatedRows.push_back(globalPattern);
    }
}