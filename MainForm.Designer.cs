namespace XIFocus;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.ListView _slotList;
    private System.Windows.Forms.ColumnHeader _columnFfxiWindows;
    private System.Windows.Forms.Label _statusLabel;
    private System.Windows.Forms.Button _btnRefresh;
    private System.Windows.Forms.Button _btnFocusNext;
    private System.Windows.Forms.Button _btnOptions;
    private System.Windows.Forms.Button _btnMoveUp;
    private System.Windows.Forms.Button _btnMoveDown;
    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this._slotList = new System.Windows.Forms.ListView();
        this._columnFfxiWindows = new System.Windows.Forms.ColumnHeader();
        this._statusLabel = new System.Windows.Forms.Label();
        this._btnRefresh = new System.Windows.Forms.Button();
        this._btnFocusNext = new System.Windows.Forms.Button();
        this._btnOptions = new System.Windows.Forms.Button();
        this._btnMoveUp = new System.Windows.Forms.Button();
        this._btnMoveDown = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // _slotList
        //
        this._slotList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this._columnFfxiWindows });
        this._slotList.Location = new System.Drawing.Point(12, 12);
        this._slotList.Size = new System.Drawing.Size(544, 200);
        this._slotList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this._slotList.FullRowSelect = true;
        this._slotList.MultiSelect = false;
        this._slotList.Name = "_slotList";
        this._slotList.View = System.Windows.Forms.View.Details;
        //
        // _columnFfxiWindows
        //
        this._columnFfxiWindows.Text = "FFXI Windows";
        this._columnFfxiWindows.Width = 600;
        //
        // _statusLabel
        //
        this._statusLabel.Location = new System.Drawing.Point(0, 215);
        this._statusLabel.Size = new System.Drawing.Size(600, 30);
        this._statusLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this._statusLabel.Name = "_statusLabel";
        this._statusLabel.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
        this._statusLabel.Text = "No FFXI windows found";
        this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        //
        // _btnRefresh
        //
        this._btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this._btnRefresh.Location = new System.Drawing.Point(12, 290);
        this._btnRefresh.Name = "_btnRefresh";
        this._btnRefresh.Size = new System.Drawing.Size(120, 30);
        this._btnRefresh.Text = "Refresh Windows";
        //
        // _btnOptions
        //
        this._btnOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        this._btnOptions.Location = new System.Drawing.Point(138, 290);
        this._btnOptions.Name = "_btnOptions";
        this._btnOptions.Size = new System.Drawing.Size(120, 30);
        this._btnOptions.Text = "Options";
        //
        // _btnFocusNext
        //
        this._btnFocusNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        this._btnFocusNext.Location = new System.Drawing.Point(468, 290);
        this._btnFocusNext.Name = "_btnFocusNext";
        this._btnFocusNext.Size = new System.Drawing.Size(120, 30);
        this._btnFocusNext.Text = "Focus Next";
        //
        // _btnMoveUp
        //
        this._btnMoveUp.Location = new System.Drawing.Point(564, 78);
        this._btnMoveUp.Size = new System.Drawing.Size(32, 32);
        this._btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this._btnMoveUp.Name = "_btnMoveUp";
        this._btnMoveUp.Text = "▲";
        this._btnMoveUp.Enabled = false;
        //
        // _btnMoveDown
        //
        this._btnMoveDown.Location = new System.Drawing.Point(564, 114);
        this._btnMoveDown.Size = new System.Drawing.Size(32, 32);
        this._btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this._btnMoveDown.Name = "_btnMoveDown";
        this._btnMoveDown.Text = "▼";
        this._btnMoveDown.Enabled = false;
        //
        // MainForm
        //
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(600, 332);
        this.MinimumSize = new System.Drawing.Size(600, 332);
        this.Controls.Add(this._btnFocusNext);
        this.Controls.Add(this._btnOptions);
        this.Controls.Add(this._btnRefresh);
        this.Controls.Add(this._btnMoveUp);
        this.Controls.Add(this._btnMoveDown);
        this.Controls.Add(this._statusLabel);
        this.Controls.Add(this._slotList);
        this.Text = "XI Focus";
        this.ResumeLayout(false);
    }

    #endregion
}
