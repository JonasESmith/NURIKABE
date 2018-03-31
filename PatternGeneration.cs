using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System;

namespace NurikabeApp
{
  class PatternGeneration
  {
    string path = "pattern.txt";

    List<string> generatedRows = new List<string>();
    List<int[,]> MatrixList = new List<int[,]>();

    int matrixSize;

    public int recursiveCalls
    { get; private set; }

    public int patternCount
    { get; private set; }

    private int totalNumberOfThreads
    {
      get { return 5; }
    }

    // ************************************************************************************** //
    // 1. INITIALIZERS                                                                        //
    // ************************************************************************************** //

    #region Init
    public PatternGeneration(int size)
    {
      matrixSize = size;
    }
    #endregion

    // ************************************************************************************** //
    // 1. PUBLIC METHODS                                                                      //
    // ************************************************************************************** //

    #region Public methods
    public void RunSolutionGenerator(Action callback, Action ActionFinished)
    {
      GenerateRows(0, null);

      bool again;
      List<Thread> managedThreads = new List<Thread>();

      for (int i = 1; i <= totalNumberOfThreads; i++)
      {
        Thread thread = new Thread(StartPattenChecking);
        thread.IsBackground = true;
        thread.Start(i);
        managedThreads.Add(thread);
      }

      do
      {
        Application.DoEvents();
        again = false;
        for (int index = 0; index < managedThreads.Count; index++)
          again = again || managedThreads[index].IsAlive;
        callback();
      }
      while (again);

      // Callback here to try and update the values
      callback();

      // Below here will execute once the threads have completed.
      ActionFinished();
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
    #endregion

    // ************************************************************************************** //
    // 1. PRIVATE METHODS                                                                     //
    // ************************************************************************************** //

    #region Private methods
    /// <summary>
    ///   This produces all possible patterns for a row of n sizes, by first putting a water block
    ///     in each section of the pattern, then switching them to zeros as the method continues. 
    /// </summary>
    private void GenerateRows(int index, List<char> charList)
    {
      string finalPattern;

      if (charList == null)
      {
        charList = new List<char>();
        for (int i = 0; i < matrixSize; i++)
          charList.Add(' ');
      }

      if (index < (matrixSize))
      {
        charList[index] = '1';
        GenerateRows(index + 1, charList);

        charList[index] = '0';
        GenerateRows(index + 1, charList);
      }
      else
      {
        finalPattern = "";
        foreach (char ch in charList)
          finalPattern += ch;
        generatedRows.Add(finalPattern);
      }
    }

    /// <summary>
    /// This method is used for when a new thread spawns. This method with split the recursive
    /// work by how many threads get spawned.
    /// </summary>
    /// <param name="obj">
    /// The parameter is a Object type, but the method is expecting an integer of the thread order.
    /// </param>
    private void StartPattenChecking(Object obj)
    {
      int threadOrder;
      try
      {
        threadOrder = (int)obj;
      }
      catch (InvalidCastException)
      {
        threadOrder = 0;
      }

      int startPosition = generatedRows.Count - (generatedRows.Count / threadOrder);
      int endPostion = startPosition + (generatedRows.Count / totalNumberOfThreads);

      List<string> pattern = new List<string>();
      for (int i = 0; i < matrixSize; i++)
          pattern.Add("");

      for (int rows = startPosition; rows < endPostion; rows++)
      {
        pattern[0] = generatedRows[rows];
        GeneratePattern(1, pattern, rows, endPostion, true);
      }
    }

    /// <summary>
    ///   
    /// </summary>

    private void GeneratePattern(int index, List<string> pattern, int rowIndexToStartAt, int endPostion, bool continuous)
    {
      recursiveCalls++;

      string[] PoolCheckArray = new string[2];

      if (index < matrixSize)
      {
        for (int i = 0; i < generatedRows.Count; i++)
        {
          pattern[index] = generatedRows[i];
          PoolCheckArray[0] = pattern[index - 1];
          PoolCheckArray[1] = pattern[index];

          if (PoolCheck(PoolCheckArray))
          {
            continuous = true;
            if (ContinuityCheckMatrix(CopyMatrix(pattern)))
            { continuous = true; }
            else
            {
              if (index < matrixSize - 1)
              {
                for (int rowIndex = 0; rowIndex < generatedRows.Count; rowIndex++)
                {
                  pattern[index + 1] = generatedRows[rowIndex];

                  if (ContinuityCheckMatrix(CopyMatrix(pattern)))
                  {
                    continuous = true;
                    goto done;
                  }
                  else
                    continuous = false;
                }
              }
              else
              { continuous = false; }
              done:;
            }
          }
          else
          { continuous = false; }

          if (continuous)
          {
            GeneratePattern(index + 1, pattern, rowIndexToStartAt, endPostion, continuous);
          }
        }
      }
      else if (continuous)
      {
        patternCount++;
        //MatrixList.Add(PrintPattern(pattern));
      }
    }

    private bool PoolCheck(string[] matrix)
    {
      bool patternCheck = true;
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

    private int AreaForMatrix(int[,] matrix, int row, int col)
    {
      int area;
      /// <summary> 
      ///    Although this conditional statement is not needed for the first iteration it will be
      ///     utilized across all recuring iterations.
      /// </summary> 
      if (matrix[row, col] == 1)
      {
        matrix[row, col] = 3;
        area = 1;

        /// <summary>
        ///    These conditional statements make sure that the test does not go out of bounds of 
        ///     the testing Matrix.
        //</summary>
        if (col + 1 <= matrixSize - 1)
          area += AreaForMatrix(matrix, row, col + 1);
        if (row + 1 <= matrixSize - 1)
          area += AreaForMatrix(matrix, row + 1, col);
        if (col - 1 >= 0)
          area += AreaForMatrix(matrix, row, col - 1);
        if (row - 1 >= 0)
          area += AreaForMatrix(matrix, row - 1, col);
      }
      else
      {
        area = 0;
      }
      return area;
    }

    private bool ContinuityCheckMatrix(int[,] matrix)
    {
      bool patternCheck = true;
      int waterCount = WaterCountForMatrix(matrix);
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
        streamCount = AreaForMatrix(matrix, row, col);
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
    private int WaterCountForMatrix(int[,] matrix)
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

    private int[,] CopyMatrix(List<string> pattern)
    {
      int[,] newMatrix = new int[matrixSize, matrixSize];

      for (int k = 0; k < matrixSize; k++)
      {
        if (!String.IsNullOrEmpty(pattern[k]))
        {
          char[] copyArray = pattern[k].ToCharArray();
          for (int j = 0; j < matrixSize; j++)
          {
            newMatrix[k, j] = Int32.Parse(copyArray[j].ToString());
          }
        }
      }

      return newMatrix;
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
    #endregion
  }
}
