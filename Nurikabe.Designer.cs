namespace NurikabeApp
{
  partial class Nurikabe
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.matrixLabel = new System.Windows.Forms.Label();
      this.testBtn = new System.Windows.Forms.Button();
      this.MatrixSizeComboBox = new System.Windows.Forms.ComboBox();
      this.buttonPanel = new System.Windows.Forms.Panel();
      this.genButton = new System.Windows.Forms.Button();
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.generatePatternsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.createReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewPatternsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.matrixSizeLabel = new System.Windows.Forms.Label();
      this.clearBtn = new System.Windows.Forms.Button();
      this.myWorker = new System.ComponentModel.BackgroundWorker();
      this.timeLabel = new System.Windows.Forms.Label();
      this.CountLabel = new System.Windows.Forms.Label();
      this.RecurLabel = new System.Windows.Forms.Label();
      this.messageLabel = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.RecurisveDisplay = new System.Windows.Forms.Label();
      this.possibleCallsDisplay = new System.Windows.Forms.Label();
      this.possibleCallsLabel = new System.Windows.Forms.Label();
      this.timeDisplay = new System.Windows.Forms.Label();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // matrixLabel
      // 
      this.matrixLabel.AutoSize = true;
      this.matrixLabel.Location = new System.Drawing.Point(257, 45);
      this.matrixLabel.Name = "matrixLabel";
      this.matrixLabel.Size = new System.Drawing.Size(0, 13);
      this.matrixLabel.TabIndex = 9;
      // 
      // testBtn
      // 
      this.testBtn.Location = new System.Drawing.Point(253, 289);
      this.testBtn.Name = "testBtn";
      this.testBtn.Size = new System.Drawing.Size(109, 23);
      this.testBtn.TabIndex = 11;
      this.testBtn.Text = "Test";
      this.testBtn.UseVisualStyleBackColor = true;
      this.testBtn.Click += new System.EventHandler(this.testBtn_Click);
      // 
      // MatrixSizeComboBox
      // 
      this.MatrixSizeComboBox.FormattingEnabled = true;
      this.MatrixSizeComboBox.Items.AddRange(new object[] {
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
      this.MatrixSizeComboBox.Location = new System.Drawing.Point(16, 52);
      this.MatrixSizeComboBox.Name = "MatrixSizeComboBox";
      this.MatrixSizeComboBox.Size = new System.Drawing.Size(67, 21);
      this.MatrixSizeComboBox.TabIndex = 14;
      this.MatrixSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.CmbBoxMSize_SelectedIndexChanged);
      // 
      // buttonPanel
      // 
      this.buttonPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.buttonPanel.Location = new System.Drawing.Point(0, 79);
      this.buttonPanel.Name = "buttonPanel";
      this.buttonPanel.Size = new System.Drawing.Size(250, 233);
      this.buttonPanel.TabIndex = 15;
      // 
      // genButton
      // 
      this.genButton.Location = new System.Drawing.Point(253, 264);
      this.genButton.Name = "genButton";
      this.genButton.Size = new System.Drawing.Size(109, 23);
      this.genButton.TabIndex = 16;
      this.genButton.Text = "Generate";
      this.genButton.UseVisualStyleBackColor = true;
      this.genButton.Click += new System.EventHandler(this.genButton_Click);
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(365, 24);
      this.menuStrip.TabIndex = 17;
      this.menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generatePatternsToolStripMenuItem,
            this.createReportToolStripMenuItem,
            this.viewPatternsToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // generatePatternsToolStripMenuItem
      // 
      this.generatePatternsToolStripMenuItem.Name = "generatePatternsToolStripMenuItem";
      this.generatePatternsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
      this.generatePatternsToolStripMenuItem.Text = "generate patterns...";
      this.generatePatternsToolStripMenuItem.Click += new System.EventHandler(this.generatePatternsToolStripMenuItem_Click);
      // 
      // createReportToolStripMenuItem
      // 
      this.createReportToolStripMenuItem.Name = "createReportToolStripMenuItem";
      this.createReportToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
      this.createReportToolStripMenuItem.Text = "create report...";
      this.createReportToolStripMenuItem.Click += new System.EventHandler(this.createReportToolStripMenuItem_Click);
      // 
      // viewPatternsToolStripMenuItem
      // 
      this.viewPatternsToolStripMenuItem.Name = "viewPatternsToolStripMenuItem";
      this.viewPatternsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
      this.viewPatternsToolStripMenuItem.Text = "view patterns...";
      this.viewPatternsToolStripMenuItem.Click += new System.EventHandler(this.viewPatternsToolStripMenuItem_Click);
      // 
      // matrixSizeLabel
      // 
      this.matrixSizeLabel.AutoSize = true;
      this.matrixSizeLabel.Location = new System.Drawing.Point(13, 36);
      this.matrixSizeLabel.Name = "matrixSizeLabel";
      this.matrixSizeLabel.Size = new System.Drawing.Size(81, 13);
      this.matrixSizeLabel.TabIndex = 18;
      this.matrixSizeLabel.Text = "length of puzzle";
      // 
      // clearBtn
      // 
      this.clearBtn.Location = new System.Drawing.Point(253, 238);
      this.clearBtn.Name = "clearBtn";
      this.clearBtn.Size = new System.Drawing.Size(109, 23);
      this.clearBtn.TabIndex = 19;
      this.clearBtn.Text = "Clear Pattern";
      this.clearBtn.UseVisualStyleBackColor = true;
      this.clearBtn.Click += new System.EventHandler(this.clearBtn_Click);
      // 
      // myWorker
      // 
      this.myWorker.WorkerReportsProgress = true;
      this.myWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.myWorker_DoWork);
      this.myWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.myWorker_ProgressChanged);
      this.myWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.myWorker_RunWorkerCompleted);
      // 
      // timeLabel
      // 
      this.timeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.timeLabel.Location = new System.Drawing.Point(145, 51);
      this.timeLabel.Name = "timeLabel";
      this.timeLabel.Size = new System.Drawing.Size(86, 21);
      this.timeLabel.TabIndex = 21;
      this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.timeLabel.Visible = false;
      // 
      // CountLabel
      // 
      this.CountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.CountLabel.Location = new System.Drawing.Point(253, 148);
      this.CountLabel.Name = "CountLabel";
      this.CountLabel.Size = new System.Drawing.Size(109, 23);
      this.CountLabel.TabIndex = 22;
      this.CountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // RecurLabel
      // 
      this.RecurLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.RecurLabel.Location = new System.Drawing.Point(253, 102);
      this.RecurLabel.Name = "RecurLabel";
      this.RecurLabel.Size = new System.Drawing.Size(109, 23);
      this.RecurLabel.TabIndex = 23;
      this.RecurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // messageLabel
      // 
      this.messageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.messageLabel.Location = new System.Drawing.Point(253, 200);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(109, 36);
      this.messageLabel.TabIndex = 13;
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(253, 125);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(109, 23);
      this.label1.TabIndex = 24;
      this.label1.Text = "Good Patterns";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // RecurisveDisplay
      // 
      this.RecurisveDisplay.Location = new System.Drawing.Point(253, 79);
      this.RecurisveDisplay.Name = "RecurisveDisplay";
      this.RecurisveDisplay.Size = new System.Drawing.Size(109, 23);
      this.RecurisveDisplay.TabIndex = 25;
      this.RecurisveDisplay.Text = "Actuall Calls";
      this.RecurisveDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // possibleCallsDisplay
      // 
      this.possibleCallsDisplay.Location = new System.Drawing.Point(253, 34);
      this.possibleCallsDisplay.Name = "possibleCallsDisplay";
      this.possibleCallsDisplay.Size = new System.Drawing.Size(109, 23);
      this.possibleCallsDisplay.TabIndex = 27;
      this.possibleCallsDisplay.Text = "Possible Calls";
      this.possibleCallsDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // possibleCallsLabel
      // 
      this.possibleCallsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.possibleCallsLabel.Location = new System.Drawing.Point(253, 57);
      this.possibleCallsLabel.Name = "possibleCallsLabel";
      this.possibleCallsLabel.Size = new System.Drawing.Size(109, 23);
      this.possibleCallsLabel.TabIndex = 26;
      this.possibleCallsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // timeDisplay
      // 
      this.timeDisplay.AutoSize = true;
      this.timeDisplay.Location = new System.Drawing.Point(146, 34);
      this.timeDisplay.Name = "timeDisplay";
      this.timeDisplay.Size = new System.Drawing.Size(85, 13);
      this.timeDisplay.TabIndex = 28;
      this.timeDisplay.Text = "Processing Time";
      this.timeDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.timeDisplay.Visible = false;
      // 
      // Nurikabe
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(365, 319);
      this.Controls.Add(this.timeDisplay);
      this.Controls.Add(this.possibleCallsDisplay);
      this.Controls.Add(this.possibleCallsLabel);
      this.Controls.Add(this.RecurisveDisplay);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.RecurLabel);
      this.Controls.Add(this.CountLabel);
      this.Controls.Add(this.timeLabel);
      this.Controls.Add(this.clearBtn);
      this.Controls.Add(this.MatrixSizeComboBox);
      this.Controls.Add(this.matrixSizeLabel);
      this.Controls.Add(this.genButton);
      this.Controls.Add(this.buttonPanel);
      this.Controls.Add(this.messageLabel);
      this.Controls.Add(this.testBtn);
      this.Controls.Add(this.matrixLabel);
      this.Controls.Add(this.menuStrip);
      this.MainMenuStrip = this.menuStrip;
      this.Name = "Nurikabe";
      this.Text = "NURIKABE";
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label matrixLabel;
    private System.Windows.Forms.Button testBtn;
    private System.Windows.Forms.ComboBox MatrixSizeComboBox;
    private System.Windows.Forms.Panel buttonPanel;
    private System.Windows.Forms.Button genButton;
    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem generatePatternsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewPatternsToolStripMenuItem;
    private System.Windows.Forms.Label matrixSizeLabel;
    private System.Windows.Forms.Button clearBtn;
    private System.ComponentModel.BackgroundWorker myWorker;
    private System.Windows.Forms.Label timeLabel;
    private System.Windows.Forms.Label CountLabel;
    private System.Windows.Forms.Label RecurLabel;
    private System.Windows.Forms.ToolStripMenuItem createReportToolStripMenuItem;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label RecurisveDisplay;
    private System.Windows.Forms.Label possibleCallsDisplay;
    private System.Windows.Forms.Label possibleCallsLabel;
    private System.Windows.Forms.Label timeDisplay;
  }
}

