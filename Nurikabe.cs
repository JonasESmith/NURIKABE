///////////////////////////////////////////////////////////////////////////////////
///                                                                             ///
///  Programmer       : Jonas Smith                                             /// 
///                                                                             ///
///  Purpose          : Test Nurikabe patterns, and create a sudoGame of it     ///
///                                                                             ///
///  Code Index       : 1. INITIALIZE ALL VARIABLES FOR THE APPLICATION         ///
///                     2. BUTTON EVENTS / METHODS                              ///
///                     3. LOAD EVENTS                                          ///
///                     4. PATTERN CHECKS/EVENTS/GENERATION                     ///
///                     5. AXILLARY METHODS                                     ///
///                     6. FUTURE IDEAS                                         ///
///                                                                             ///
///////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System;

namespace NurikabeApp
{
  public partial class Nurikabe : Form
  {
    public Nurikabe()
    {
      InitializeComponent();
      MatrixSizeComboBox.SelectedIndex = 0;
      LoadAssets();
    }

    // ************************************************************************************** //
    // 1. INITIALIZE ALL VARIABLES FOR THE APPLICATION                                        //
    // ************************************************************************************** //

    /// <summary>
    ///   INITIALIZE ALL VARIABLES FOR THE APPLICATION
    /// </summary>
    #region Initialize all variables for the application
    static string notContinuous = "The pattern is not continuous";
    static string containsPool  = "There is a pool in the matrix";
    static string goodPattern   = "This is a good pattern!";
    static string path          = "pattern.txt";
    static string boolList;

    static List<string> generatedRows = new List<string>();
    static List<int[,]> MatrixList = new List<int[,]>();
    static List<char>   pattern     = new List<char>();

    static  string[] PoolCheckArray;
    static  int[,]   matrixClone, matrix;
    static  int      matrixSize,  area, patternCount, recursiveCalls;

    static bool patternCheck;

    #endregion




    // ************************************************************************************** //
    // 2. BUTTON EVENTS / METHODS                                                             //
    // ************************************************************************************** //

    /// <summary>
    ///   This collection of code deal's with all button click events, that is refreshing UI 
    ///     elements or changing the layout of the form itself. This area of code calls to all
    ///     other collections of code. using Load Events and pattern checks to test the patterns
    ///     that are passed in either by recursive methods in Pattern Checks or by the user
    ///     playing the simple nurikabe game that I created. 
    /// </summary>
    #region BUTTON EVENTS / METHODS    

    private void LoadButtons(int length)
    {
      int panelSize     = 250;
      int sideLength    = 0;
      int buttonsMargin = 0;

      /// <summary>
      ///  Bellow is where it decides the size of the buttons and the distance between them,
      ///    based on the (int length) that is passed to it when it is called in LoadAssets();
      /// <summary>
      switch(length)
      {
        case 2:
          sideLength = 55; 
          break;
        case 3 :
          sideLength = 50;
          break;
        case 4:
          sideLength = 35;
          break;
        case 5:
          sideLength = 30;
          break;
        case 6:
          sideLength = 32;
          break;
        case 7:
          sideLength = 28;
          break;
        case 8:
          sideLength = 25;
          break;
      }

      for(int i = 0; i < matrixSize; i++)
      {
        buttonsMargin += sideLength;
        if (i < (matrixSize - 1))
          buttonsMargin += 2; 
      }

      int margin = ((panelSize - buttonsMargin) / 2);
      int buttonSize = 0;
      int topMargin = margin;
      int leftMargin = margin;
      int nameCount = 1;

      /// <summary>
      ///  Loads buttons into the array in the configuration I want to properly name/associate
      ///   them with their corresponding matrix value. 
      /// </summary> 
      for (int i = 0; i < length; i++)
      {
        leftMargin = margin;
        for (int j = 0; j < length; j++)
        {
          Button button = new Button();
          buttonPanel.Controls.Add(button);

          button.Size   = new Size(sideLength, sideLength);
          button.Click += this.button_Click;
          button.Name   = "button" + nameCount;
          button.Left   = leftMargin;
          button.Top    = topMargin;
          leftMargin   += button.Height;
          buttonSize    = button.Height;
          nameCount++;
        }
        topMargin += buttonSize;
      }
    }



    //<summary>
    ///  This event is used on all buttons to update the button colors and the array associated 
    ///   with it. 
    //</summary>
    private void button_Click(object sender, EventArgs e)
    {
      Button button = sender as Button;
      string name   = button.Name;
      int nameInt, row, col;

      nameInt = Convert.ToInt32(name.Replace("button", "")); nameInt--;

      row = nameInt / matrixSize;
      col = nameInt - (row * matrixSize);

      if(matrix[row, col] == 0)
      {
        matrix[row, col] = 1;
      }
      else
      {
        matrix[row, col] = 0;
      }

      LoadMatrix(matrixSize);

      if(button.BackColor == Color.Black)
      {
        button.BackColor = SystemColors.Control;
      }
      else
      {
        button.BackColor = Color.Black;
      }
      button.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255); //Transparent 

      button.Text = "";
    }

    private void ChangeButtons()
    {
      /// <summary>
      ///   Foreach control in the panel check if the control is a button then, check if the 
      ///    background is black if it is change it to the regular color before we recolor the
      ///    buttons based on the matrix. 
      /// </summary>
      foreach(Control control in buttonPanel.Controls)
      {
        if(control.GetType()==typeof(Button))
        {
          Button button = control as Button;
          if(button.BackColor == Color.Black)
          {
            button.BackColor = SystemColors.Control;
          }
        }
      }

      string[] pattern = PrintMatrix();
      int btnNum; 

      foreach (Control control in buttonPanel.Controls)
      {
        if (control.GetType() == typeof(Button))
        {
          Button button = control as Button;
          btnNum = Convert.ToInt32(button.Name.Replace("button" ,""));

          if(pattern[btnNum - 1] == "1")
          {
            button.BackColor = Color.Black;
          }
        }
      }
    }

    /// <summary>
    ///  This is where the patterns are tested. 
    /// </summary>
    private void testBtn_Click(object sender, EventArgs e)
    {
      messageLabel.Text = "";

      matrixClone = (int[,])matrix.Clone();

      /// <summary>
      ///  If their are no pools then check if the water is continuous.
      /// </summary> 
      if (PoolCheck())
      {
        if(ContinuityCheck())
        {
          patternCheck = true;
        }
      }

      if (patternCheck)
        PatternMessage(goodPattern);
    }

    //// <summary>
    ////    This is used when the ComboBox selection is changed, to update the UI
    //// </summary>
    private void CmbBoxMSize_SelectedIndexChanged(object sender, EventArgs e)
    {
      LoadAssets();
    }

    //// <summary>
    ////    This can be used to randomly generate a new pattern not in use. 
    //// </summary>
    private void genButton_Click(object sender, EventArgs e)
    {
      patternCheck = false;
      Random rnd = new Random();

      for (int i = 0; i < matrixSize; i++)
      {
        for (int j = 0; j < matrixSize; j++)
        {
          matrix[i, j] = rnd.Next(0, 2);
        }
      }
      LoadMatrix(matrixSize);
      ChangeButtons();
      messageLabel.Text = "";
    }

    private void clearBtn_Click(object sender, EventArgs e)
    {
      messageLabel.Text = "";
      LoadAssets();
    }

    //// <summary>
    ////    Generates all possible patterns for the currently selected matrix size. 
    //// </summary>
    private void generatePatternsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      /// <summary>
      ///   Need to also start a small timer here to properly adjust and understand how long the
      ///    events that take place are processing. This will allow me to know exacts as far as 
      ///    percentages of calculations and the time taken by each process. I can also go about
      ///    using subprocesses to try and finishthis process much faster. 
      /// </summary>

      generatedRows.Clear();
      pattern.Clear();
      for (int i = 0; i <= (matrixSize * matrixSize); i++)
        pattern.Add(' ');

      Stopwatch timer = new Stopwatch();
      timer.Start();

      patternCount = 0;
      recursiveCalls = 1; 

      //error
      GenPatterns(0);

      
      List<string> localPattern = new List<string>();

      for (int i = 0; i < matrixSize; i++)
        localPattern.Add("");

      GenMatrixPattern(0, localPattern);
      timer.Stop();

      localPattern.Clear();
      CountLabel.Text = "Patterns : " + patternCount.ToString();
      RecurLabel.Text = "Calls    : " + recursiveCalls.ToString();

      timeLabel.Text = timer.Elapsed.TotalSeconds.ToString();
      timeLabel.Update();
      // 3.306 Minutes to calculate 5x5 patterns when  checking for continuity.  
      // 2.9 minutes to calculate 5x5 pattern when not checking for continuity. 
    }

    /// <summary>
    ///   Allows the user to view the patterns created if there is a file that was generated to 
    ///     store the patterns. 
    /// </summary>
    private void viewPatternsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (File.Exists(path))
        System.Diagnostics.Process.Start("notepad", path);
      else
        MessageBox.Show(path + " does not exist", "Invalid File Name", MessageBoxButtons.OK, 
                                                                       MessageBoxIcon.Error);
    }

    #endregion




    // ************************************************************************************** //
    // 3. LOAD EVENTS                                                                         //
    // ************************************************************************************** //

    /// <summary>
    ///   This collection of code deal's with most Load Events for the UI that being loading a
    ///     new matrix or loading the assets for refreshing buttons. 
    /// </summary>
    #region LOAD EVENTS


    /// <summary>
    ///   Clear's matrix and resizes based on value passed to count. 
    /// </summary>
    private void LoadMatrix(int count)
    {
      matrixLabel.Text = "";

      for (int i = 0; i < count; i++)
      {
        for (int j = 0; j < count; j++)
        {
          matrixLabel.Text += matrix[i, j].ToString() + "   ";
        }
        matrixLabel.Text += "\n";
      }
    }

    /// <summary>
    ///   Loads all assets, generally when something is changed or reset in the UI. 
    /// </summary>
    private void LoadAssets()
    {
      this.buttonPanel.Controls.Clear();
      matrixSize = Convert.ToInt32(MatrixSizeComboBox.GetItemText(MatrixSizeComboBox.SelectedItem));

      matrix = new int[matrixSize, matrixSize];
      LoadMatrix(matrixSize);

      LoadButtons(matrixSize);
    }
    #endregion




    // ************************************************************************************** //
    // 4. PATTERN CHECKS/EVENTS/GENERATION                                                    //
    // ************************************************************************************** //

    /// <summary>
    ///   This collection of code deal's with the checks for continuity and all axillary methods 
    ///     to properly check, and test patterns in the matrixes. This is where the efficeinty 
    ///     of the code can improve the most. 
    /// </summary>
    #region PATTERN CHECKS/EVENTS/GENERATION 

    /// <summary>
    ///   Checks if there are any pools or 2x2 areas on the pattern that are water blocks. 
    ///     this is not efficient and can be fixed I have a feeling. 
    /// </summary>
    private bool PoolCheck()
    {
      patternCheck = true;
      int[,] testMatrix = new int[2, 2];
      for (int i = 0; i < matrixSize - 1; i++)
      {
        for (int j = 0; j < matrixSize - 1; j++)
        {
          testMatrix[0, 0] = matrix[i, j];
          testMatrix[0, 1] = matrix[i, j + 1];
          testMatrix[1, 0] = matrix[i + 1, j];
          testMatrix[1, 1] = matrix[i + 1, j + 1];

          if (testMatrix[0, 0] == 1 && testMatrix[0, 1] == 1 && testMatrix[1, 0] == 1 && testMatrix[1, 1] == 1)
          {
            PatternMessage(containsPool);
            patternCheck = false;
          }
        }
      }

      return patternCheck;
    }

    private bool PoolCheck(string[] matrix)
    {
      patternCheck      = true;
      int[,] testMatrix = new int[2, matrixSize];
      char[] arrayOne   = matrix[0].ToCharArray();
      char[] arrayTwo   = matrix[1].ToCharArray();

      for (int j = 0; j < matrixSize - 1; j++)
      {
        testMatrix[0, 0] = Int32.Parse(arrayOne[j].ToString());
        testMatrix[0, 1] = Int32.Parse(arrayOne[j + 1].ToString());
        testMatrix[1, 0] = Int32.Parse(arrayTwo[j].ToString());
        testMatrix[1, 1] = Int32.Parse(arrayTwo[j + 1].ToString());

        if (testMatrix[0, 0] == 1 && testMatrix[0, 1] == 1 && testMatrix[1, 0] == 1 && testMatrix[1, 1] == 1)
        {
          PatternMessage(containsPool);
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
        if(col + 1 <= matrixSize - 1)
          area += Area(row, col + 1);
        if(row + 1 <= matrixSize - 1)
         area += Area(row + 1, col);
        if(col - 1 >= 0)
          area += Area(row, col - 1);
        if(row - 1 >= 0)
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
      } done:;

      /// <summary>
      ///    Calls recursive method to count all countinuous blocks of water
      /// </summary>
      streamCount = Area(row, col);

      /// <summary>
      ///    If the method call above does not return the total count of water blocks then we can 
      ///     conclude the water is not continuous.
      /// </summary> 
      if (streamCount != waterCount)
      {
        PatternMessage(notContinuous);
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


    /// <summary>
    ///   This produces all possible patterns for a row of n sizes, by first putting a water block
    ///     in each section of the pattern, then switching them to zeros as the method continues. 
    /// </summary>
    private void GenPatterns(int index)
    {
      string finalPattern;

      if (index < (matrixSize))
      {
        pattern[index] = '1';
        GenPatterns(index + 1);

        pattern[index] = '0';
        GenPatterns(index + 1);
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
    private void GenMatrixPattern(int index, List<string> pattern)
    {
      char[] copyArray = new char[matrixSize];
      PoolCheckArray = new string[2];

      if (index < (matrixSize))
      {
        for (int i = 0; i < generatedRows.Count; i++)
        {
          pattern[index] = (generatedRows[i]);
          if (index != 0)
          {
            PoolCheckArray[0] = pattern[index - 1];
            PoolCheckArray[1] = pattern[index];

            if (PoolCheck(PoolCheckArray))
            {

              #region Verticle Line and Horizontal Line Checks


              string[,] check = new string[2, matrixSize];

              string horCheck = "";
              //string verCheck = "";

              int horCount = 0, verCount = 0;


              for (int k = 0; k < matrixSize; k++)
              {
                horCheck = "";
                //verCheck = "";
                for (int j = 0; j < matrixSize; j++)
                {
                  horCheck += matrix[k, j];
                  //verCheck += matrix[j, k];
                }

                if (horCheck.Contains("1"))
                  check[0, k] = "1";
                else
                  check[0, k] = "0";

                //  if (verCheck.Contains("1"))
                //    check[1, k] = "1";
                //  else
                //    check[1, k] = "0";
              }

              for (int j = 0; j < matrixSize; j++)
              {
                if (check[0, j] == "1")
                {
                  horCount++;
                }
                //  if (check[1, j] == "1")
                //  {
                //    verCount++;
                //  }
              }

              int actualHorCount = 0, actualVerCount = 0;

              for (int j = 0; j < matrixSize; j++)
              {
                if (check[0, j] == "1")
                {
                  actualHorCount++;
                  if (j + 1 < matrixSize && check[0, j + 1] == "1")
                  {
                    continue;
                  }
                  else
                  {
                    break;
                  }
                }


                //  if (check[1, j] == "1")
                //  {
                //    actualVerCount++;
                //  }
              }

              if (horCount != actualHorCount) //|| verCount != actualVerCount)
              {
                patternCheck = false;
              }

              #endregion
              // Pattern Check is true && index is at maximum size of matrix. 
              if (patternCheck && index == matrixSize - 1)
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

                ContinuityCheck();
              }
            }
          }
          if (patternCheck || index == 0)
          {
            GenMatrixPattern(index + 1, pattern);
            recursiveCalls++;
          }
        }
      }
      else if (patternCheck)
      {
        patternCount++;
        //MatrixList.Add(PrintPattern(pattern));
      }
    }



    #region Possible Pruning methods
    /// Back up 
    //   private void GenPatterns(int index)
    //{
    //  if (index<(matrixSize* matrixSize))  /// Extend pattern
    //  {
    //    recursiveCalls++;
    //    pattern[index] = '1';
    //    GenPatterns(index + 1);

    //    pattern[index] = '0';
    //    GenPatterns(index + 1);
    //  }
    //  else
    //  {
    //    if (TestPattern())
    //      patternCount++;
    //      //MatrixList.Add(PrintPattern());
    //  }
    //}



    //private void GenPatterns(int index)
    //{
    //  string finalPattern;

    //  if (index < (matrixSize))  /// Extend pattern
    //  {
    //    pattern[index] = '1';
    //    GenPatterns(index + 1);

    //    pattern[index] = '0';
    //    GenPatterns(index + 1);
    //  }
    //  else
    //  {
    //    finalPattern = "";
    //    foreach (char ch in pattern)
    //      finalPattern += ch;
    //    generatedRows.Add(finalPattern);
    //  }
    //}

    //private void GenMatrixPattern(int index, List<string> pattern)
    //{
    //  char[] copyArray = new char[matrixSize];
    //  PoolCheckArray = new string[2];

    //  if (index < (matrixSize))
    //  {
    //    for (int i = 0; i < generatedRows.Count; i++)
    //    {
    //      pattern[index] = (generatedRows[i]);
    //      if (index != 0)
    //      {
    //        PoolCheckArray[0] = pattern[index - 1];
    //        PoolCheckArray[1] = pattern[index];

    //        // Sets a value to true or false;
    //        if (PoolCheck(PoolCheckArray))
    //        {
    //          //for (int j = 0; j < matrixSize; j++)
    //          //{
    //          //  if (pattern[j].Contains("1"))
    //          //    boolList += "1";
    //          //  else
    //          //    boolList += "0";
    //          //}

    //          //// Check if a row has a 1, if a row between two rows that have a one is zero it 
    //          //// is not continuous. 
    //          //if (boolList.Contains("101"))
    //          //{
    //          //  patternCheck = false;
    //          //}
    //        }

    //      }
    //      if (patternCheck || index == 0)
    //      {
    //        GenMatrixPattern(index + 1, pattern);
    //        recursiveCalls++;
    //      }
    //    }
    //  }
    //  else if (patternCheck)
    //  {

    //    // Copying current pattern into matrix
    //    for (int k = 0; k < matrixSize; k++)
    //    {
    //      copyArray = pattern[k].ToCharArray();
    //      for (int j = 0; j < matrixSize; j++)
    //      {
    //        matrix[k, j] = Int32.Parse(copyArray[j].ToString());
    //      }
    //    }
    //    matrixClone = (int[,])matrix.Clone();

    //    if (ContinuityCheck())
    //    {
    //      patternCount++;
    //      //MatrixList.Add(PrintPattern(pattern));
    //    }
    //  }
    //}
    #endregion

    /// <summary>
    ///    First load's the pattern into the matrix, then makes a clone of the matrix for the 
    ///     continuity check then finally returns a value based on weather the poolCheck, and 
    ///     continuity check go through. 
    /// </summary>
    private bool TestPattern()
    {
      patternCheck = true;

      int index = 0; 

      for(int row = 0; row < matrixSize; row++)
      {
        for(int col = 0; col < matrixSize; col++)
        {
          matrix[row, col] = Int32.Parse(Convert.ToString(pattern[index]));
          index++;
        }
      }

      matrixClone = (int[,])matrix.Clone();

      if (PoolCheck())
      {
        if (ContinuityCheck())
        {
          patternCheck = true;
        }
      }
      return patternCheck;
    }

    private void CopyMatrix(List<char> pattern)
    {
      int index = 0;

      for(int row = 0; row < matrixSize; row++)
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
    private int[,] PrintPattern()
    {
      int[,] patternMatrix = new int[matrixSize, matrixSize];
      int index = 0; 

      for(int i = 0; i < matrixSize; i++)
      {
        for (int j = 0; j < matrixSize; j++)
        {
          //error
          patternMatrix[i, j] = Int32.Parse(Convert.ToString(pattern[index]));
          index++;
        }
      }
      return patternMatrix;
    }

    private void createReportToolStripMenuItem_Click(object sender, EventArgs e)
    {
      CreateReport();
    }

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


    private string[] PrintMatrix()
    {
      string[] patternString = new string[matrixSize * matrixSize];
      int i = 0;

      foreach (int ch in matrix)
      {
        patternString[i] = ch.ToString(); i++;
      }

      return patternString;
    }
    #endregion




    // ************************************************************************************** //
    // 5. AXILLARY METHODS                                                                    //
    // ************************************************************************************** //

    /// <summary>
    ///   This collection of code deal's with the Messages with the UI and creating reports of 
    ///     the patterns created from the above code to be viewed by the user. 
    /// </summary>
    #region AXILLARY METHODS 
    private void PatternMessage(string message)
    {
      messageLabel.Text = message;
    }

    private void CreateReport()
    {
      StreamWriter sw = File.CreateText(path);
      sw.Flush();
      int index = 1;
      string margin = "     ";

      foreach(int[,] matrix in MatrixList)
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
      } sw.Close();
    }
    #endregion
  }
}

// ************************************************************************************** //
// 6. FUTURE IDEAS                                                                        //
// ************************************************************************************** //

/// <summary>
///  Future Ideas     : 1. Add a more efficient version of Generate patters in the PATTERN CHECKS
///                      collection of code. Instead of using a recursive method to test every
///                      possible pattern for each grid I could instead fragment the process
///                      by first testing all possible soluciton of a row. then applying each 
///                      possible row against itself to better test each possible pattern while
///                      also pruning each branch to better increase efficienty. 
///                     2. Add timer and possilbe calls vs actuall calls of each recurisive loop 
///                       through the method to generate patterns. this method by far is the most
///                       resource entensive and can improve the most. 
///                           
///                           EX. currently (3/24/18) the process to test all possible solutions 
///                             of a 5x5 grid takes roughly 5 minutes in time for 33m patterns, 
///                             this is with continuity checking. For a 6x6 Grid it would roughly 
///                             be 68b patterns meaning the time would grow signifigantly to about
///                             170 hours of processing time. This is with Zero Pruning. The 
///                             possibility of adding in both the first and second "future idea"
///                             would mean that more possible patterns could be processed and 
///                             applied to the study. 
/// </summary>