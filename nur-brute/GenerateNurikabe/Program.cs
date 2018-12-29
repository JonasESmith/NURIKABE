using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace GenerateNurikabe
{
  class Program
  {
    static void Main(string[] args)
    {
      ulong totalLegalPuzzles;

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      totalLegalPuzzles = NurikabeInstance.StartMultithreadedRun(8, 4);
      stopwatch.Stop();
      Console.WriteLine(totalLegalPuzzles);
      Console.WriteLine(stopwatch.Elapsed);
      Console.WriteLine();

      Console.WriteLine("Done. Enter 'exit' to exit. Case sensitive.");
      string response;
      while ((response = Console.ReadLine()) != "exit")
      {
        Console.WriteLine("Done. Enter 'exit' to exit. Case sensitive.");
      }
    }
  }

  public class NurikabeInstance
  {
    public ulong StartPoint { get; set; }
    public ulong StopPoint { get; set; }
    public int BinaryPatternSize { get; set; }
    public int Dimension { get; set; }
    //public int ListIndex { get; set; }
    public uint LegalPuzzles { get; protected set; }
    private BooleanArray nurikabePatternGenerator;
    private bool[] nurikabePattern;
    private bool[] visited;
    private int poolIterations; // Micro-optimizations can be crucial in large applications.
                                // This variable is for the HasPool() method, so it no longer
                                // needs to calculate this value each time it is called.

    /// <summary>
    /// An initializing constructor for an object to make multithreading easier. DO NOT start at 0.
    /// </summary>
    /// <param name="startPointVal">The starting puzzle, represented by a uint. DO NOT start at 0. The program will break and the 0th pattern is already accounted for.</param>
    /// <param name="stopPointVal">The stopping puzzle, represented by a uint. This value is inclusive.</param>
    /// <param name="patternSizeVal">The pattern size is the dimension squared. This is how long the binary strings will be.</param>
    /// <param name="dimVal">The dimension, which in an n x n puzzle is n.</param>
    /// <param name="indexVal">The index value that this Nurikabe instance may edit.</param>
    public NurikabeInstance(ulong startPointVal, ulong stopPointVal, int patternSizeVal, int dimVal)
    {
      this.StartPoint = startPointVal;
      this.StopPoint = stopPointVal;
      this.BinaryPatternSize = patternSizeVal;
      this.Dimension = dimVal;
      this.nurikabePatternGenerator = new BooleanArray(patternSizeVal, startPointVal);
      this.LegalPuzzles = 1; // Including the trivial case.
      this.visited = new bool[patternSizeVal];
      this.poolIterations = (this.Dimension - 1) * (this.Dimension - 1);
    }

    public void GenerateLegalNurikabe()
    {
      int j;

      for (ulong i = this.StartPoint; i <= this.StopPoint; i++)
      {
        nurikabePattern = nurikabePatternGenerator.Next;
        if (!HasPool() && IsContinuous()) //Checking for pools is faster. && is short circuit, so HasPool() is first.
          this.LegalPuzzles += 1;
        // for loops to reset small arrays are faster than Array.Clear()
        for (j = 0; j < this.BinaryPatternSize; j++)
          this.visited[j] = false;
        //Array.Clear(this.visited, 0, this.visited.Length);
      }
    }


    private bool HasPool()
    {
      bool hasPool = false;
      // Made poolIterations an instance variable.
      int puzzleIndex = 0;

      for (int i = 1; i <= this.poolIterations; i++)
      {
        if (nurikabePattern[puzzleIndex] && nurikabePattern[puzzleIndex + 1] &&
            nurikabePattern[puzzleIndex + Dimension] && nurikabePattern[puzzleIndex + Dimension + 1])
        {
          hasPool = true;
          break;
        }
        // Jump 2 spots, which corresponds to the beginning of the next row, every n - 1 iterations.
        if (i % (Dimension - 1) == 0)
          puzzleIndex += 2;
        else
          puzzleIndex++;
      }

      return hasPool;
    }

    private int Area(int index)
    {
      int area = 0;

      if (nurikabePattern[index])
      {
        this.visited[index] = true;
        area = 1;

        if (index - Dimension >= 0 && !visited[index - Dimension])   // Above
          area += Area(index - Dimension);
        if (index + Dimension <= BinaryPatternSize - 1 && !visited[index + Dimension]) // Below
          area += Area(index + Dimension);
        if ((index + 1) % Dimension != 0 && !visited[index + 1])   // Right
          area += Area(index + 1);
        if (index % Dimension != 0 && !visited[index - 1]) // Left
          area += Area(index - 1);
      }
      return area;
    }

    private bool IsContinuous()
    {
      bool isContinuous = true;
      ushort indexOfAOne = 0;
      ushort i;

      int sum = 0;
      for (i = 0; i < this.BinaryPatternSize; i++)
      {
        if (this.nurikabePattern[i])
          sum += 1;
      }

      // This seems to be slightly faster than Array.IndexOf
      for (i = 0; i < this.BinaryPatternSize; i++)
      {
        if (this.nurikabePattern[i])
        {
          indexOfAOne = i;
          break;
        }
      }

      // It is not continuous if we can't hit all water squares.
      //if (sum != Area(Array.IndexOf(nurikabePattern, true)))
      if (sum != Area(indexOfAOne))
        isContinuous = false;

      return isContinuous;
    }

    // This ToString() is mainly for the debugger.
    public override string ToString()
    {
      return $"{StartPoint} {StopPoint}";
    }

    // Extend upper limit on puzzles by using BigInteger Class. May just be too hard to go past 8.
    /// <summary>
    /// Starts a multithreaded run of a nurikabe puzzle. It partitions the possible patterns into the desired number of threads.
    /// For thread counts and puzzle sizes that do not divide evenly, the last thread does the remaining puzzles.
    /// </summary>
    /// <param name="numOfThreads">The desired number of threads to use.</param>
    /// <param name="sizeOfPuzzle">The length or width of an n x n nurikabe puzzle.</param>
    public static ulong StartMultithreadedRun(int numOfThreads, int sizeOfPuzzle)
    {
      NurikabeInstance[] nurikabes = new NurikabeInstance[numOfThreads];
      ThreadStart[] threadStarts = new ThreadStart[numOfThreads];
      List<Thread> threads = new List<Thread>();

      // There is no Math.Pow for integers.
      int lengthOfPattern = sizeOfPuzzle * sizeOfPuzzle;

      // Just for the purpose of Math.Pow. Does not allow implicit conversion of int to double.
      double lengthOfPatternDouble = sizeOfPuzzle * sizeOfPuzzle;

      // -1 because we already account for the trivial case.
      ulong numberOfPatterns = (ulong)Math.Pow(2, lengthOfPatternDouble) - 1;

      ulong shareSize = numberOfPatterns / (ulong)numOfThreads;
      ulong extra = numberOfPatterns % (ulong)numOfThreads;
      ulong sum = 0;

      for (int i = 0; i < numOfThreads; i++)
      {
        nurikabes[i] = new NurikabeInstance(sum + 1, 0, lengthOfPattern, sizeOfPuzzle);
        sum += shareSize;
        if (i == numOfThreads - 1)
          sum += extra;
        nurikabes[i].StopPoint = sum;
        threadStarts[i] = new ThreadStart(nurikabes[i].GenerateLegalNurikabe);
      }

      // Set up all threads
      for (int i = 0; i < numOfThreads; i++)
        threads.Add(new Thread(threadStarts[i]));

      // Start all threads.
      for (int i = 0; i < numOfThreads; i++)
        threads[i].Start();

      // Find better way to wait for all threads to finish. Join ties up threads.
      bool stillBusy = true;
      while (stillBusy)
      {
        stillBusy = false;
        for (int i = 0; i < numOfThreads; i++)
        {
          if (threads[i].IsAlive)
          {
            stillBusy = true;
            Thread.Sleep(1);
            break;
          }
        }
      }


      // Add up all puzzles.
      ulong totalLegalPuzzles = 0;
      for (int i = 0; i < numOfThreads; i++)
        totalLegalPuzzles += nurikabes[i].LegalPuzzles;
      //Each instance of a nurikabe puzzle includes the trivial case. This subtracts each extra trivial case and adds one back in.
      return totalLegalPuzzles - (ulong)numOfThreads + 1;
    }


    /// <summary>
    /// Starts a multithreaded run of a nurikabe puzzle. It partitions the possible patterns into the desired number of threads.
    /// For thread counts and puzzle sizes that do not divide evenly, the last thread does the remaining puzzles.
    /// This is an overloaded method that uses a start point and end point.
    /// </summary>
    /// <param name="numOfThreads">The desired number of threads to use.</param>
    /// <param name="sizeOfPuzzle">The length or width of an n x n nurikabe puzzle.</param>
    /// <param name="startPoint">The starting integer of a puzzle to work on. <strong><em>DO NOT</em></strong> start at 0.
    /// It is already included if this method is started at 1.</param>
    /// <param name="endPoint">The last integer of a puzzle to work on.</param>
    /// <returns></returns>
    public static ulong StartMultithreadedRun(int numOfThreads, int sizeOfPuzzle, ulong startPoint, ulong endPoint)
    {
      NurikabeInstance[] nurikabes = new NurikabeInstance[numOfThreads];
      ThreadStart[] threadStarts = new ThreadStart[numOfThreads];
      List<Thread> threads = new List<Thread>();

      // There is no Math.Pow for integers.
      int lengthOfPattern = sizeOfPuzzle * sizeOfPuzzle;

      // -1 because we already account for the trivial case.
      ulong numberOfPatterns = endPoint - startPoint + 1;

      ulong shareSize = numberOfPatterns / (ulong)numOfThreads;
      ulong extra = numberOfPatterns % (ulong)numOfThreads;
      ulong sum = startPoint - 1;

      for (int i = 0; i < numOfThreads; i++)
      {
        nurikabes[i] = new NurikabeInstance(sum + 1, 0, lengthOfPattern, sizeOfPuzzle);
        sum += shareSize;
        if (i == numOfThreads - 1)
          sum += extra;
        nurikabes[i].StopPoint = sum;
        threadStarts[i] = new ThreadStart(nurikabes[i].GenerateLegalNurikabe);
      }

      // Set up all threads
      for (int i = 0; i < numOfThreads; i++)
        threads.Add(new Thread(threadStarts[i]));

      // Start all threads.
      for (int i = 0; i < numOfThreads; i++)
        threads[i].Start();

      // Find better way to wait for all threads to finish. Join ties up threads.

      bool stillBusy = true;
      while (stillBusy)
      {
        stillBusy = false;
        for (int i = 0; i < numOfThreads; i++)
        {
          if (threads[i].IsAlive)
          {
            stillBusy = true;
            Thread.Sleep(1);
            break;
          }
        }
      }


      // Add up all puzzles.
      ulong totalLegalPuzzles = 0;
      for (int i = 0; i < numOfThreads; i++)
        totalLegalPuzzles += nurikabes[i].LegalPuzzles;
      //Each instance of a nurikabe puzzle includes the trivial case. 
      if (startPoint == 1)
        return totalLegalPuzzles - (ulong)numOfThreads + 1;
      else
        return totalLegalPuzzles - (ulong)numOfThreads;
    }
  }

  struct BooleanArray
  {
    private bool[] boolArray;
    private int position;

    public BooleanArray(int size)
    {
      this.boolArray = new bool[size];
      this.position = 0;
    }

    public BooleanArray(int size, ulong startPoint)
    {
      string startString = Convert.ToString(((long)startPoint - 1), 2).PadLeft(size, '0');
      this.boolArray = new bool[size];
      this.position = 0;

      for (int i = 0; i < size; i++)
        if (startString[i] == '1')
          this.boolArray[i] = true;
        else
          this.boolArray[i] = false;
    }

    public bool[] Next
    {
      get
      {
        if (this.boolArray[this.boolArray.Length - 1])
        {
          this.position = this.boolArray.Length - 2;
          while (this.boolArray[this.position])
          {
            this.boolArray[this.position] = false;
            this.position -= 1;
          }
          this.boolArray[this.position] = true;
        }
        this.boolArray[this.boolArray.Length - 1] = !this.boolArray[this.boolArray.Length - 1];

        return boolArray;
      }
    }
  }
}