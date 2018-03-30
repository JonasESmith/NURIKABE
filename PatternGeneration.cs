using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;

namespace NurikabeApp
{
  class PatternGeneration
  {
    string path = "pattern.txt";

    List<string> generatedRows = new List<string>();
    List<int[,]> MatrixList = new List<int[,]>();
    List<char> pattern;

    string[] PoolCheckArray;
    int[,] matrixClone, matrix;
    int matrixSize, area;

    bool patternCheck;

    private BackgroundWorker worker;
                                                   
    public PatternGeneration(int size, BackgroundWorker backgroundWorker)             
    {                                              
      matrixSize = size;                           
      matrix = new int[matrixSize, matrixSize];    
      matrixClone = new int[matrixSize, matrixSize];

      worker = backgroundWorker;
                                                   
      pattern = new List<char>();                  
      for (int i = 0; i < matrixSize; i++)
        pattern.Add(' ');
    }

    private bool PoolCheck(string[] matrix)
    {
      patternCheck = true;
      int[,] testMatrix = new int[2, matrixSize];
      char[] arrayOne = matrix[0].ToCharArray();
      char[] arrayTwo = matrix[1].ToCharArray();

      for (int j = 0; j < matrixSize - 1; j++)
      {
        testMatrix[0, 0] = Int32.Parse(arrayOne[j].ToString());
        testMatrix[0, 1] = Int32.Parse(arrayOne[j + 1].ToString());
        testMatrix[1, 0] = Int32.Parse(arrayTwo[j].ToString());
        testMatrix[1, 1] = Int32.Parse(arrayTwo[j + 1].ToString());

        if (testMatrix[0, 0] == 1 && testMatrix[0, 1] == 1 && testMatrix[1, 0] == 1 && testMatrix[1, 1] == 1)
        {
          patternCheck = false;
        }
      }

      return patternCheck;
    }

    private int Area(int row, int col)
    {
      /// <summary> 
      ///    Although this conditional statement is not needed for the first iteration it will be
      ///     utilized across all recuring iterations.
      /// </summary> 
      if (matrixClone[row, col] == 1)
      {
        matrixClone[row, col] = 3;
        area = 1;

        /// <summary>
        ///    These conditional statements make sure that the test does not go out of bounds of 
        ///     the testing Matrix.
        //</summary>
        if (col + 1 <= matrixSize - 1)
          area += Area(row, col + 1);
        if (row + 1 <= matrixSize - 1)
          area += Area(row + 1, col);
        if (col - 1 >= 0)
          area += Area(row, col - 1);
        if (row - 1 >= 0)
          area += Area(row - 1, col);
      }
      else
      {
        area = 0;
      }
      return area;
    }


    private bool ContinuityCheck()
    {
      int waterCount = this.waterCount();
      int streamCount;

      int row = 0, col = 0;

      /// <summary> 
      ///    Bellow finds the nearest water block and breaks out of all for loops with goto done; 
      /// </summary>
      if (waterCount > 1)
      {
        for (int i = 0; i <= matrixSize - 1; i++)
        {
          for (int j = 0; j <= matrixSize - 1; j++)
          {
            if (matrix[i, j] == 1)
            {
              row = i;
              col = j;
              goto done;
            }
          }
        }
        done:;

        /// <summary>
        ///    Calls recursive method to count all countinuous blocks of water
        /// </summary>
        streamCount = Area(row, col);
      }
      else
      {
        if (waterCount == 1)
          streamCount = 1;
        else
          streamCount = 0; 
      }

      /// <summary>
      ///    If the method call above does not return the total count of water blocks then we can 
      ///     conclude the water is not continuous.
      /// </summary> 
      if (streamCount == waterCount)
      {
        patternCheck = true;
      }
      else
      {
        patternCheck = false;
      }


      return patternCheck;
    }

    //// <summary>
    ////    Finds the total number of "water" blocks to test against the continuous blocks
    //// </summary>
    private int waterCount()
    {
      int count = 0;

      for (int i = 0; i <= matrixSize - 1; i++)
      {
        for (int j = 0; j <= matrixSize - 1; j++)
        {
          if (matrix[i, j] == 1)
            count++;
        }
      }
      return count;
    }

    public void OrganizeRows()
    {

    }

    /// <summary>
    ///   This produces all possible patterns for a row of n sizes, by first putting a water block
    ///     in each section of the pattern, then switching them to zeros as the method continues. 
    /// </summary>
    public void GenerateRows(int index)
    {
      string finalPattern;

      if (index < (matrixSize))
      {
        pattern[index] = '1';
        GenerateRows(index + 1);

        pattern[index] = '0';
        GenerateRows(index + 1);
      }
      else
      {
        finalPattern = "";
        foreach (char ch in pattern)
          finalPattern += ch;
        generatedRows.Add(finalPattern);
      }
    }

    /// <summary>
    ///   CHANGES CAN BE MADE HERE!
    ///     Here more than anywhere major changes can be made to improve the efficiency of producing good patterns. 
    /// 
    ///   This recursive method starts by entering the first patern into each row, then switching it 
    ///     with each iteration. It first checks for a pool when a new row is added. By doing this it
    ///     makes sure that all remaining patterns are not containing a pool. 
    /// </summary>
    
    public void GeneratePattern(int index, List<string> pattern, ref int patternCount, ref int recursiveCalls)
    {
      // Reasonable check for now, this blocks Main if updated every time this method gets called.
      if (recursiveCalls % 1000 == 0)
        worker.ReportProgress(0); // Report 0 progress, as progress may be unknown.

      char[] copyArray = new char[matrixSize];
      PoolCheckArray = new string[2];

      if (index < matrixSize)
      {
        for (int i = 0; i < generatedRows.Count; i++)
        {
          pattern[index] = generatedRows[i];
          if (index != 0)
          {
            PoolCheckArray[0] = pattern[index - 1];
            PoolCheckArray[1] = pattern[index];

            if (PoolCheck(PoolCheckArray))
            {
              for (int k = 0; k < matrixSize; k++)
              {
                
                if (!String.IsNullOrEmpty(pattern[k]))
                {
                  copyArray = pattern[k].ToCharArray();
                  for (int j = 0; j < matrixSize; j++)
                  {
                    matrix[k, j] = Int32.Parse(copyArray[j].ToString());
                  }
                }
              }

              matrixClone = (int[,])matrix.Clone();

              if(ContinuityCheck())
              {
                // Just needs to be empty for now. 
              }
              else
              {
                bool test = patternCheck;
                // add the row bellow if it is also continuous continue
                // However this would also have to do a pool check when the new row is added in!
                if (index < matrixSize - 1) // && !pattern[index].Contains("1"))
                {
                  for (int nun = 0; nun < generatedRows.Count; nun++)
                  {

                    pattern[index + 1] = generatedRows[nun];

                    for (int k = 0; k < matrixSize; k++)
                    {
                      if (!String.IsNullOrEmpty(pattern[k]))
                      {
                        copyArray = pattern[k].ToCharArray();
                        for (int j = 0; j < matrixSize; j++)
                        {
                          matrix[k, j] = Int32.Parse(copyArray[j].ToString());
                        }
                      }
                    }

                    matrixClone = (int[,])matrix.Clone();

                    if (ContinuityCheck())
                      goto done;
                  }
                }
                done : ;
              }
              //// Pattern Check is true && index is at maximum size of matrix. 
              //if (patternCheck && index == matrixSize - 1)
              //{
              //  ContinuityCheck();
              //}
            }
          }
          if (patternCheck || index == 0)
          {
            GeneratePattern(index + 1, pattern, ref patternCount, ref recursiveCalls);
            recursiveCalls++;
          }
        }
      }
      else if (patternCheck)
      {
        patternCount++;
        // MatrixList.Add(PrintPattern(pattern));
      }
    }

    private void CopyMatrix(List<char> pattern)
    {
      int index = 0;

      for (int row = 0; row < matrixSize; row++)
      {
        for (int col = 0; col < matrixSize; col++)
        {
          if (!(pattern[index] == ' '))
          {

            matrix[row, col] = Int32.Parse(Convert.ToString(pattern[index]));
            index++;
          }
        }
      }
    }

    //// <summary>
    ////   Converts the good patterns to a string of characters to more easily store the values. 
    //// </summary>
    private int[,] PrintPattern(List<string> pattern)
    {
      int[,] patternMatrix = new int[matrixSize, matrixSize];
      char[] tmpArray = new char[matrixSize];
      int index;

      for (int i = 0; i < matrixSize; i++)
      {
        tmpArray = pattern[i].ToCharArray();
        index = 0;
        for (int j = 0; j < matrixSize; j++)
        {
          //error
          patternMatrix[i, j] = Int32.Parse(Convert.ToString(tmpArray[index]));
          index++;
        }
      }
      return patternMatrix;
    }

    public void CreateReport()
    {
      StreamWriter sw = File.CreateText(path);
      sw.Flush();
      int index = 1;
      string margin = "     ";

      foreach (int[,] matrix in MatrixList)
      {
        sw.WriteLine("{0,-5}  ", (index + "."));

        for (int i = 0; i < matrixSize; i++)
        {
          sw.Write(margin);
          for (int j = 0; j < matrixSize; j++)
          {
            sw.Write(matrix[i, j] + " ");
          }
          sw.WriteLine();
        }
        sw.WriteLine();
        index++;
      }
      sw.Close();
    }
  }
}
