using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace GenerateNurikabe
{
  class Program
  {

    static List<uint> legalMoveList = new List<uint>();

    static void Main(string[] args)
    {
      ulong totalLegalPuzzles;

      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Reset();
      stopwatch.Start();
      totalLegalPuzzles = NurikabeInstance.StartMultithreadedRun(8, 3);

      Console.WriteLine(totalLegalPuzzles);
      stopwatch.Stop();
      Console.WriteLine(stopwatch.Elapsed);

      Console.WriteLine("Done. Enter 'exit' to exit. Case sensitive.");
      string response;
      while((response = Console.ReadLine()) != "exit")
      {
        Console.WriteLine("Done. Enter 'exit' to exit. Case sensitive.");
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
      private string nurikabePattern;
      private int[] visited; // int array to take advantage of Array.Clear
                             //public List<string> REMOVE = new List<string>();

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
        //this.ListIndex = indexVal;
        this.LegalPuzzles = 1; // Including the trivial case.
        this.visited = new int[patternSizeVal];
      }

      public void GenerateLegalNurikabe()
      {
        for (ulong i = this.StartPoint; i <= this.StopPoint; i++)
        {
          // No ulong Convert.ToString exists. However, casting a ulong as a long does not change the binary.
          nurikabePattern = Convert.ToString((long)i, 2).PadLeft(this.BinaryPatternSize, '0');
          if (!HasPool() && IsContinuous()) //Checking for pools is faster. && is short circuit, so HasPool() is first.
            this.LegalPuzzles += 1;
          Array.Clear(this.visited, 0, this.visited.Length);
        }
      }


      private bool HasPool()
      {
        bool hasPool = false;
        int iterations = (this.Dimension - 1) * (this.Dimension - 1); // No Math.Pow exists for integers.
        int puzzleIndex = 0;

        for (int i = 1; i <= iterations; i++)
        {
          if (nurikabePattern[puzzleIndex] == '1' && nurikabePattern[puzzleIndex + 1] == '1' &&
              nurikabePattern[puzzleIndex + Dimension] == '1' && nurikabePattern[puzzleIndex + Dimension + 1] == '1')
          {
            hasPool = true;
            break;
          }
          // Jump 2 spots every n - 1 iterations.
          if (i % (Dimension - 1) == 0)
            puzzleIndex += 2;
          else
            puzzleIndex++;
        }

        return hasPool;
      }

      // Check speed performance of IndexOf/IndexOfAny
      // Figure out iterative version
      private int Area(int index)
      {
        int area = 0;

        if (nurikabePattern[index] == '1')
        {
          this.visited[index] = 1;
          area = 1;

          if (index - Dimension >= 0 && visited[index - Dimension] == 0)   // Above
            area += Area(index - Dimension);
          if (index + Dimension <= BinaryPatternSize - 1 && visited[index + Dimension] == 0) // Below
            area += Area(index + Dimension);
          if ((index + 1) % Dimension != 0 && visited[index + 1] == 0)   // Right
            area += Area(index + 1);
          if (index % Dimension != 0 && visited[index - 1] == 0) // Left
            area += Area(index - 1);
        }
        return area;
      }

      private bool IsContinuous()
      {
        bool isContinuous = true;
        int occurencesOfZero = nurikabePattern.Length - nurikabePattern.Replace("1", "").Length;

        // It is not continuous if we can't hit all water squares.
        if (occurencesOfZero != Area(nurikabePattern.IndexOf("1")))
          isContinuous = false;

        return isContinuous;
      }

      // Extend upper limit on puzzles by using BigInteger Class. May just be too hard to go past 8.
      /// <summary>
      /// Starts a multithreaded run of a nurikabe puzzle. It partitions the possible patterns into the desired number of threads.
      /// For thread counts and puzzle sizes that do not divide evenly, the last thread does the remaining puzzles.
      /// </summary>
      /// <param name="numOfThreads">The desired number of threads to use.</param>
      /// <param name="sizeOfPuzzle">The length or width of an n x n nurikabe puzzle.</param>
      /// <param name="parameters"></param>
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
  }
}