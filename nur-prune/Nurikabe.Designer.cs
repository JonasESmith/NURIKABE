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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.GeneratePatternTab = new System.Windows.Forms.TabPage();
            this.processingTimeLabel = new System.Windows.Forms.Label();
            this.cancelGenBtn = new System.Windows.Forms.Button();
            this.genPatternBtn = new System.Windows.Forms.Button();
            this.GameTab = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.GeneratePatternTab.SuspendLayout();
            this.GameTab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // matrixLabel
            // 
            this.matrixLabel.AutoSize = true;
            this.matrixLabel.Location = new System.Drawing.Point(34, 80);
            this.matrixLabel.Name = "matrixLabel";
            this.matrixLabel.Size = new System.Drawing.Size(0, 13);
            this.matrixLabel.TabIndex = 9;
            // 
            // testBtn
            // 
            this.testBtn.Location = new System.Drawing.Point(262, 217);
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
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.MatrixSizeComboBox.Location = new System.Drawing.Point(65, 2);
            this.MatrixSizeComboBox.Name = "MatrixSizeComboBox";
            this.MatrixSizeComboBox.Size = new System.Drawing.Size(37, 21);
            this.MatrixSizeComboBox.TabIndex = 14;
            this.MatrixSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.CmbBoxMSize_SelectedIndexChanged);
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonPanel.Location = new System.Drawing.Point(6, 6);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(250, 233);
            this.buttonPanel.TabIndex = 15;
            // 
            // genButton
            // 
            this.genButton.Location = new System.Drawing.Point(262, 192);
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
            this.menuStrip.Size = new System.Drawing.Size(388, 24);
            this.menuStrip.TabIndex = 17;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createReportToolStripMenuItem,
            this.viewPatternsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // createReportToolStripMenuItem
            // 
            this.createReportToolStripMenuItem.Name = "createReportToolStripMenuItem";
            this.createReportToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.createReportToolStripMenuItem.Text = "create report...";
            this.createReportToolStripMenuItem.Click += new System.EventHandler(this.createReportToolStripMenuItem_Click);
            // 
            // viewPatternsToolStripMenuItem
            // 
            this.viewPatternsToolStripMenuItem.Name = "viewPatternsToolStripMenuItem";
            this.viewPatternsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.viewPatternsToolStripMenuItem.Text = "view patterns...";
            this.viewPatternsToolStripMenuItem.Click += new System.EventHandler(this.viewPatternsToolStripMenuItem_Click);
            // 
            // matrixSizeLabel
            // 
            this.matrixSizeLabel.AutoSize = true;
            this.matrixSizeLabel.Location = new System.Drawing.Point(3, 4);
            this.matrixSizeLabel.Name = "matrixSizeLabel";
            this.matrixSizeLabel.Size = new System.Drawing.Size(67, 13);
            this.matrixSizeLabel.TabIndex = 18;
            this.matrixSizeLabel.Text = "side length : ";
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(262, 166);
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
            this.timeLabel.Location = new System.Drawing.Point(147, 45);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(109, 23);
            this.timeLabel.TabIndex = 21;
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CountLabel
            // 
            this.CountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CountLabel.Location = new System.Drawing.Point(147, 98);
            this.CountLabel.Name = "CountLabel";
            this.CountLabel.Size = new System.Drawing.Size(109, 23);
            this.CountLabel.TabIndex = 22;
            this.CountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RecurLabel
            // 
            this.RecurLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RecurLabel.Location = new System.Drawing.Point(31, 98);
            this.RecurLabel.Name = "RecurLabel";
            this.RecurLabel.Size = new System.Drawing.Size(109, 23);
            this.RecurLabel.TabIndex = 23;
            this.RecurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // messageLabel
            // 
            this.messageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.messageLabel.Location = new System.Drawing.Point(262, 102);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(109, 36);
            this.messageLabel.TabIndex = 13;
            this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(147, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 23);
            this.label1.TabIndex = 24;
            this.label1.Text = "Good Patterns";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RecurisveDisplay
            // 
            this.RecurisveDisplay.Location = new System.Drawing.Point(31, 75);
            this.RecurisveDisplay.Name = "RecurisveDisplay";
            this.RecurisveDisplay.Size = new System.Drawing.Size(109, 23);
            this.RecurisveDisplay.TabIndex = 25;
            this.RecurisveDisplay.Text = "Actual Calls";
            this.RecurisveDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // possibleCallsDisplay
            // 
            this.possibleCallsDisplay.Location = new System.Drawing.Point(31, 24);
            this.possibleCallsDisplay.Name = "possibleCallsDisplay";
            this.possibleCallsDisplay.Size = new System.Drawing.Size(109, 23);
            this.possibleCallsDisplay.TabIndex = 27;
            this.possibleCallsDisplay.Text = "Possible Calls";
            this.possibleCallsDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // possibleCallsLabel
            // 
            this.possibleCallsLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.possibleCallsLabel.Location = new System.Drawing.Point(31, 47);
            this.possibleCallsLabel.Name = "possibleCallsLabel";
            this.possibleCallsLabel.Size = new System.Drawing.Size(109, 23);
            this.possibleCallsLabel.TabIndex = 26;
            this.possibleCallsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.GeneratePatternTab);
            this.tabControl1.Controls.Add(this.GameTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(388, 269);
            this.tabControl1.TabIndex = 29;
            // 
            // GeneratePatternTab
            // 
            this.GeneratePatternTab.BackColor = System.Drawing.SystemColors.Control;
            this.GeneratePatternTab.Controls.Add(this.processingTimeLabel);
            this.GeneratePatternTab.Controls.Add(this.cancelGenBtn);
            this.GeneratePatternTab.Controls.Add(this.genPatternBtn);
            this.GeneratePatternTab.Controls.Add(this.matrixLabel);
            this.GeneratePatternTab.Controls.Add(this.possibleCallsDisplay);
            this.GeneratePatternTab.Controls.Add(this.possibleCallsLabel);
            this.GeneratePatternTab.Controls.Add(this.timeLabel);
            this.GeneratePatternTab.Controls.Add(this.RecurisveDisplay);
            this.GeneratePatternTab.Controls.Add(this.CountLabel);
            this.GeneratePatternTab.Controls.Add(this.label1);
            this.GeneratePatternTab.Controls.Add(this.RecurLabel);
            this.GeneratePatternTab.Location = new System.Drawing.Point(4, 22);
            this.GeneratePatternTab.Name = "GeneratePatternTab";
            this.GeneratePatternTab.Padding = new System.Windows.Forms.Padding(3);
            this.GeneratePatternTab.Size = new System.Drawing.Size(380, 243);
            this.GeneratePatternTab.TabIndex = 1;
            this.GeneratePatternTab.Text = "Generate Patterns";
            // 
            // processingTimeLabel
            // 
            this.processingTimeLabel.Location = new System.Drawing.Point(146, 22);
            this.processingTimeLabel.Name = "processingTimeLabel";
            this.processingTimeLabel.Size = new System.Drawing.Size(109, 23);
            this.processingTimeLabel.TabIndex = 31;
            this.processingTimeLabel.Text = "Processing Time (s)";
            this.processingTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cancelGenBtn
            // 
            this.cancelGenBtn.Location = new System.Drawing.Point(0, 220);
            this.cancelGenBtn.Name = "cancelGenBtn";
            this.cancelGenBtn.Size = new System.Drawing.Size(109, 23);
            this.cancelGenBtn.TabIndex = 30;
            this.cancelGenBtn.Text = "Cancel";
            this.cancelGenBtn.UseVisualStyleBackColor = true;
            this.cancelGenBtn.Visible = false;
            // 
            // genPatternBtn
            // 
            this.genPatternBtn.Location = new System.Drawing.Point(262, 45);
            this.genPatternBtn.Name = "genPatternBtn";
            this.genPatternBtn.Size = new System.Drawing.Size(110, 23);
            this.genPatternBtn.TabIndex = 29;
            this.genPatternBtn.Text = "Generate";
            this.genPatternBtn.UseVisualStyleBackColor = true;
            this.genPatternBtn.Click += new System.EventHandler(this.GoodPatternsBtn_Click);
            // 
            // GameTab
            // 
            this.GameTab.BackColor = System.Drawing.SystemColors.Control;
            this.GameTab.Controls.Add(this.buttonPanel);
            this.GameTab.Controls.Add(this.clearBtn);
            this.GameTab.Controls.Add(this.testBtn);
            this.GameTab.Controls.Add(this.messageLabel);
            this.GameTab.Controls.Add(this.genButton);
            this.GameTab.Location = new System.Drawing.Point(4, 22);
            this.GameTab.Name = "GameTab";
            this.GameTab.Padding = new System.Windows.Forms.Padding(3);
            this.GameTab.Size = new System.Drawing.Size(380, 243);
            this.GameTab.TabIndex = 0;
            this.GameTab.Text = "Game";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.MatrixSizeComboBox);
            this.panel1.Controls.Add(this.matrixSizeLabel);
            this.panel1.Location = new System.Drawing.Point(150, 22);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(115, 20);
            this.panel1.TabIndex = 30;
            // 
            // Nurikabe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 291);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Nurikabe";
            this.Text = "NURIKABE";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.GeneratePatternTab.ResumeLayout(false);
            this.GeneratePatternTab.PerformLayout();
            this.GameTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage GameTab;
    private System.Windows.Forms.TabPage GeneratePatternTab;
    private System.Windows.Forms.Button genPatternBtn;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button cancelGenBtn;
    private System.Windows.Forms.Label processingTimeLabel;
  }
}

