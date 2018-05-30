<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTaikoSR
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtOsuFolder = New System.Windows.Forms.TextBox()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.barLoad = New System.Windows.Forms.ProgressBar()
        Me.lblReport = New System.Windows.Forms.Label()
        Me.dgvSR = New System.Windows.Forms.DataGridView()
        Me.chkDT = New System.Windows.Forms.CheckBox()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        CType(Me.dgvSR, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtOsuFolder
        '
        Me.txtOsuFolder.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.txtOsuFolder.Location = New System.Drawing.Point(12, 12)
        Me.txtOsuFolder.Name = "txtOsuFolder"
        Me.txtOsuFolder.Size = New System.Drawing.Size(318, 20)
        Me.txtOsuFolder.TabIndex = 0
        '
        'btnLoad
        '
        Me.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.btnLoad.Location = New System.Drawing.Point(336, 10)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(75, 23)
        Me.btnLoad.TabIndex = 1
        Me.btnLoad.Text = "Load"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'barLoad
        '
        Me.barLoad.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.barLoad.Location = New System.Drawing.Point(417, 10)
        Me.barLoad.Name = "barLoad"
        Me.barLoad.Size = New System.Drawing.Size(371, 23)
        Me.barLoad.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.barLoad.TabIndex = 2
        '
        'lblReport
        '
        Me.lblReport.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.lblReport.AutoSize = True
        Me.lblReport.Location = New System.Drawing.Point(12, 35)
        Me.lblReport.Name = "lblReport"
        Me.lblReport.Size = New System.Drawing.Size(83, 13)
        Me.lblReport.TabIndex = 3
        Me.lblReport.Tag = ""
        Me.lblReport.Text = "100 maps found"
        '
        'dgvSR
        '
        Me.dgvSR.AllowUserToAddRows = False
        Me.dgvSR.AllowUserToDeleteRows = False
        Me.dgvSR.AllowUserToResizeRows = False
        Me.dgvSR.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvSR.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSR.Location = New System.Drawing.Point(12, 51)
        Me.dgvSR.Name = "dgvSR"
        Me.dgvSR.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgvSR.Size = New System.Drawing.Size(776, 368)
        Me.dgvSR.StandardTab = True
        Me.dgvSR.TabIndex = 4
        '
        'chkDT
        '
        Me.chkDT.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.chkDT.AutoSize = True
        Me.chkDT.Location = New System.Drawing.Point(332, 34)
        Me.chkDT.Name = "chkDT"
        Me.chkDT.Size = New System.Drawing.Size(88, 17)
        Me.chkDT.TabIndex = 5
        Me.chkDT.Text = "Calculate DT"
        Me.chkDT.UseVisualStyleBackColor = True
        '
        'lblSearch
        '
        Me.lblSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblSearch.AutoSize = True
        Me.lblSearch.Location = New System.Drawing.Point(9, 428)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Size = New System.Drawing.Size(44, 13)
        Me.lblSearch.TabIndex = 6
        Me.lblSearch.Text = "Search:"
        '
        'txtSearch
        '
        Me.txtSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSearch.Location = New System.Drawing.Point(59, 425)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(729, 20)
        Me.txtSearch.TabIndex = 7
        Me.txtSearch.WordWrap = False
        '
        'frmTaikoSR
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.txtSearch)
        Me.Controls.Add(Me.lblSearch)
        Me.Controls.Add(Me.chkDT)
        Me.Controls.Add(Me.dgvSR)
        Me.Controls.Add(Me.lblReport)
        Me.Controls.Add(Me.barLoad)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.txtOsuFolder)
        Me.Name = "frmTaikoSR"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Taiko Star Rating Comparison Tool"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.dgvSR, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtOsuFolder As TextBox
    Friend WithEvents btnLoad As Button
    Friend WithEvents barLoad As ProgressBar
    Friend WithEvents lblReport As Label
    Friend WithEvents dgvSR As DataGridView
    Friend WithEvents chkDT As CheckBox
    Friend WithEvents lblSearch As Label
    Friend WithEvents txtSearch As TextBox
End Class
