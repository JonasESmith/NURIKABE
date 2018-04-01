/// ///////////////////////////////////////////////////////////////////////////////
///                                                                             ///
///  Programmers      : Jonas Smith, Andrew Robinson                            /// 
///                                                                             ///
///  Purpose          : Test Nurikabe patterns, and create a sudoGame of it     ///
///                                                                             ///
///  Code Index       : 1. INIT VARIABLES                                       ///
///                     2. BUTTON EVENTS                                        ///
///                     3. PATTERN GENERATION                                   ///
///                     4. LOAD EVENTS                                          ///
///                     5. PATTERN EVENTS                                       ///
///                     6. AXILLARY METHODS                                     ///
///                                                                             ///
/// ///////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
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

    /* 1. INIT VARIABLES      */
    #region ---------- init -----------

    /// <summary>
    ///   INITIALIZE ALL VARIABLES FOR THE APPLICATION
    /// </summary>


    string notContinuous = "The pattern is not continuous";
    string containsPool  = "There is a pool in the matrix";
    string goodPattern   = "This is a good pattern!";
    string path          = "pattern.txt";
    string Time;

    PatternGeneration generator;
    private Stopwatch timer;

    List<string> generatedRows = new List<string>();
    List<int[,]> MatrixList    = new List<int[,]>();
    List<char>   pattern       = new List<char>();

    Thread[] ThreadArray;
    int[,]   matrixClone, matrix;
    int      matrixSize,  area, patternCount, recursiveCalls;

    static bool patternCheck;

    #endregion
    /* 1. ****** END **********/

    /* 2. BUTTON EVENTS       */
    #region ------ button events ------ 

    /// <summary>
    ///   This collection of code deal's with all button click events, that is refreshing UI 
    ///     elements or changing the layout of the form itself. This area of code calls to all
    ///     other collections of code. using Load Events and pattern checks to test the patterns
    ///     that are passed in either by recursive methods in Pattern Checks or by the user
    ///     playing the simple nurikabe game that I created. 
    /// </summary>

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

    /// <summary>
    ///  This event is used on all buttons to update the button colors and the array associated 
    ///   with it. 
    /// </summary>
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

      // LoadMatrix(matrixSize);

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
      // LoadMatrix(matrixSize);
      ChangeButtons();
      messageLabel.Text = "";
    }

    private void clearBtn_Click(object sender, EventArgs e)
    {
      messageLabel.Text = "";
      LoadAssets();
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
    /* 2. ****** END **********/

    /* 3. BACKGROUNDWORKER    */
    #region -- generation of patterns -
    /// <summary>
    ///   
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      UpdateGenerationLabels();
    }

    private void myWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      timer = new Stopwatch();
      timer.Start();

      BackgroundWorker worker = sender as BackgroundWorker;

      generator = new PatternGeneration(matrixSize);

      //generator.RunSolutionGenerator();
      recursiveCalls = generator.recursiveCalls;
      patternCount = generator.patternCount;

      timer.Stop();

      Time = timer.Elapsed.TotalSeconds.ToString();
    }

    private void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      Time = timer.Elapsed.TotalSeconds.ToString();
      UpdateGenerationLabels();
    }

    /// <summary>
    ///    Generates all possible patterns for the currently selected matrix size. 
    /// </summary>
    private void generatePatternsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      timer = new Stopwatch();
      timer.Start();

      generator = new PatternGeneration(matrixSize);

      /// <summary>
      ///   Bellow will have the ability to organize the rows based on number of 1's. This is
      ///     Not working as of now. 
      /// </summary>
      // generator.OrganizeRows();

      /// <summary>
      ///   This could potentially allow for multiThreading of the GeneratePatterns() method
      ///     This code is modeled after the CH16Prb2B given to us froim Dr. Oaks. It seems
      ///     kind of weird honestly. 
      /// </summary>

      generator.RunSolutionGenerator(SolutionGeneratorFinished, ActionFinished);
      //myWorker.RunWorkerAsync();
    }

    private void SolutionGeneratorFinished()
    {
      recursiveCalls = generator.recursiveCalls;
      patternCount = generator.patternCount;

      Time = timer.Elapsed.TotalSeconds.ToString();
      UpdateGenerationLabels();
    }

    private void ActionFinished()
    {
      recursiveCalls = generator.recursiveCalls;
      patternCount = generator.patternCount;

      timer.Stop();

      Time = timer.Elapsed.TotalSeconds.ToString();
      UpdateGenerationLabels();
    }
    #endregion
    /* 3. ****** END **********/

    /* 4. LOAD EVENTS         */
    #region - application load events -

    /// <summary>
    ///   This collection of code deal's with most Load Events for the UI that being loading a
    ///     new matrix or loading the assets for refreshing buttons. 
    /// </summary>

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
      // LoadMatrix(matrixSize);

      LoadButtons(matrixSize);
    }
    #endregion
    /* 4. ****** END **********/

    /* 5. PATTERN EVENTS      */
    #region -- nurikabe game checks ---


    /// <summary>
    ///   This collection of code deal's with the checks for continuity and all axillary methods 
    ///     to properly check, and test patterns in the matrixes. This is where the efficeinty 
    ///     of the code can improve the most. 
    /// </summary>



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

    private void createReportToolStripMenuItem_Click(object sender, EventArgs e)
    {
      generator.CreateReport();
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
    #endregion              //              // /* */
    /* 5. ****** END **********/

    /* 6. AXILLARY METHODS    */
    #region ------ aux methods --------

    /// <summary>
    ///   This collection of code deal's with the Messages with the UI and creating reports of 
    ///     the patterns created from the above code to be viewed by the user. 
    /// </summary>

    private void PatternMessage(string message)
    {
      messageLabel.Text = message;
    }

    private void UpdateGenerationLabels()
    {
      CountLabel.Text = "Patterns : " + patternCount.ToString();
      RecurLabel.Text = "Calls    : " + recursiveCalls.ToString();

      timeLabel.Text = "Time : " + Time;
      timeLabel.Update();
    }
    #endregion
    /* 6. ****** END **********/
  }
}