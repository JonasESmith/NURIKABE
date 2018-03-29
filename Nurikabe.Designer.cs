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
      this.label2 = new System.Windows.Forms.Label();
      this.testBtn = new System.Windows.Forms.Button();
      this.messageLabel = new System.Windows.Forms.Label();
      this.MatrixSizeComboBox = new System.Windows.Forms.ComboBox();
      this.buttonPanel = new System.Windows.Forms.Panel();
      this.genButton = new System.Windows.Forms.Button();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
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
      this.menuStrip1.SuspendLayout();
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
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(292, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 10;
      this.label2.Text = "Matrix";
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
      // messageLabel
      // 
      this.messageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.messageLabel.Location = new System.Drawing.Point(253, 171);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(109, 64);
      this.messageLabel.TabIndex = 13;
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
      this.MatrixSizeComboBox.Location = new System.Drawing.Point(89, 37);
      this.MatrixSizeComboBox.Name = "MatrixSizeComboBox";
      this.MatrixSizeComboBox.Size = new System.Drawing.Size(40, 21);
      this.MatrixSizeComboBox.TabIndex = 14;
      this.MatrixSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.CmbBoxMSize_SelectedIndexChanged);
      // 
      // buttonPanel
      // 
      this.buttonPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.buttonPanel.Location = new System.Drawing.Point(0, 62);
      this.buttonPanel.Name = "buttonPanel";
      this.buttonPanel.Size = new System.Drawing.Size(250, 250);
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
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(365, 24);
      this.menuStrip1.TabIndex = 17;
      this.menuStrip1.Text = "menuStrip1";
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
      this.matrixSizeLabel.Location = new System.Drawing.Point(13, 40);
      this.matrixSizeLabel.Name = "matrixSizeLabel";
      this.matrixSizeLabel.Size = new System.Drawing.Size(76, 13);
      this.matrixSizeLabel.TabIndex = 18;
      this.matrixSizeLabel.Text = "length of side :";
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
      // timeLabel
      // 
      this.timeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.timeLabel.Location = new System.Drawing.Point(161, 40);
      this.timeLabel.Name = "timeLabel";
      this.timeLabel.Size = new System.Drawing.Size(89, 18);
      this.timeLabel.TabIndex = 21;
      this.timeLabel.Text = "Time :";
      // 
      // CountLabel
      // 
      this.CountLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.CountLabel.Location = new System.Drawing.Point(253, 143);
      this.CountLabel.Name = "CountLabel";
      this.CountLabel.Size = new System.Drawing.Size(109, 23);
      this.CountLabel.TabIndex = 22;
      this.CountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // RecurLabel
      // 
      this.RecurLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.RecurLabel.Location = new System.Drawing.Point(253, 120);
      this.RecurLabel.Name = "RecurLabel";
      this.RecurLabel.Size = new System.Drawing.Size(109, 23);
      this.RecurLabel.TabIndex = 23;
      this.RecurLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // Nurikabe
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(365, 319);
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
      this.Controls.Add(this.label2);
      this.Controls.Add(this.matrixLabel);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Nurikabe";
      this.Text = "NURIKABE";
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label matrixLabel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button testBtn;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.ComboBox MatrixSizeComboBox;
    private System.Windows.Forms.Panel buttonPanel;
    private System.Windows.Forms.Button genButton;
    private System.Windows.Forms.MenuStrip menuStrip1;
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
  }
}

