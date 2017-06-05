<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalibration
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.GroupPanel12 = New DevComponents.DotNetBar.Controls.GroupPanel
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtDataWeighing = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtLower = New System.Windows.Forms.TextBox
        Me.txtUpper = New System.Windows.Forms.TextBox
        Me.btnGetData = New DevComponents.DotNetBar.ButtonX
        Me.btnCalibrate = New DevComponents.DotNetBar.ButtonX
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.TimerdbUPDATE = New System.Windows.Forms.Timer(Me.components)
        Me.GroupPanel12.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupPanel12
        '
        Me.GroupPanel12.CanvasColor = System.Drawing.SystemColors.Control
        Me.GroupPanel12.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.VS2005
        Me.GroupPanel12.Controls.Add(Me.Label2)
        Me.GroupPanel12.Controls.Add(Me.txtDataWeighing)
        Me.GroupPanel12.Controls.Add(Me.Label1)
        Me.GroupPanel12.Controls.Add(Me.Label5)
        Me.GroupPanel12.Controls.Add(Me.txtLower)
        Me.GroupPanel12.Controls.Add(Me.txtUpper)
        Me.GroupPanel12.Controls.Add(Me.btnGetData)
        Me.GroupPanel12.Controls.Add(Me.btnCalibrate)
        Me.GroupPanel12.Location = New System.Drawing.Point(1, 8)
        Me.GroupPanel12.Name = "GroupPanel12"
        Me.GroupPanel12.Size = New System.Drawing.Size(339, 207)
        '
        '
        '
        Me.GroupPanel12.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2
        Me.GroupPanel12.Style.BackColorGradientAngle = 90
        Me.GroupPanel12.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground
        Me.GroupPanel12.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel12.Style.BorderBottomWidth = 1
        Me.GroupPanel12.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.GroupPanel12.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel12.Style.BorderLeftWidth = 1
        Me.GroupPanel12.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel12.Style.BorderRightWidth = 1
        Me.GroupPanel12.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanel12.Style.BorderTopWidth = 1
        Me.GroupPanel12.Style.CornerDiameter = 4
        Me.GroupPanel12.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanel12.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanel12.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.GroupPanel12.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanel12.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanel12.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GroupPanel12.TabIndex = 89
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(19, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(127, 18)
        Me.Label2.TabIndex = 151
        Me.Label2.Text = "DATA WEIGHING"
        '
        'txtDataWeighing
        '
        Me.txtDataWeighing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtDataWeighing.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDataWeighing.Location = New System.Drawing.Point(152, 14)
        Me.txtDataWeighing.Name = "txtDataWeighing"
        Me.txtDataWeighing.Size = New System.Drawing.Size(144, 26)
        Me.txtDataWeighing.TabIndex = 152
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(19, 95)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 18)
        Me.Label1.TabIndex = 150
        Me.Label1.Text = "UPPER"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(19, 56)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 18)
        Me.Label5.TabIndex = 147
        Me.Label5.Text = "LOWER"
        '
        'txtLower
        '
        Me.txtLower.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtLower.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLower.Location = New System.Drawing.Point(152, 51)
        Me.txtLower.Name = "txtLower"
        Me.txtLower.Size = New System.Drawing.Size(144, 26)
        Me.txtLower.TabIndex = 148
        '
        'txtUpper
        '
        Me.txtUpper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtUpper.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtUpper.Location = New System.Drawing.Point(154, 92)
        Me.txtUpper.Name = "txtUpper"
        Me.txtUpper.Size = New System.Drawing.Size(144, 26)
        Me.txtUpper.TabIndex = 149
        '
        'btnGetData
        '
        Me.btnGetData.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnGetData.ColorTable = DevComponents.DotNetBar.eButtonColor.BlueOrb
        Me.btnGetData.FocusCuesEnabled = False
        Me.btnGetData.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGetData.Location = New System.Drawing.Point(23, 137)
        Me.btnGetData.Name = "btnGetData"
        Me.btnGetData.Size = New System.Drawing.Size(112, 46)
        Me.btnGetData.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003
        Me.btnGetData.TabIndex = 126
        Me.btnGetData.Text = "Get Data"
        '
        'btnCalibrate
        '
        Me.btnCalibrate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnCalibrate.ColorTable = DevComponents.DotNetBar.eButtonColor.BlueOrb
        Me.btnCalibrate.FocusCuesEnabled = False
        Me.btnCalibrate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCalibrate.Location = New System.Drawing.Point(186, 137)
        Me.btnCalibrate.Name = "btnCalibrate"
        Me.btnCalibrate.Size = New System.Drawing.Size(112, 46)
        Me.btnCalibrate.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003
        Me.btnCalibrate.TabIndex = 125
        Me.btnCalibrate.Text = "Calibrate"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.GroupPanel12)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(3, -3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(346, 221)
        Me.GroupBox1.TabIndex = 90
        Me.GroupBox1.TabStop = False
        '
        'TimerdbUPDATE
        '
        Me.TimerdbUPDATE.Interval = 800
        '
        'frmCalibration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Teal
        Me.ClientSize = New System.Drawing.Size(352, 221)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCalibration"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Calibration"
        Me.GroupPanel12.ResumeLayout(False)
        Me.GroupPanel12.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupPanel12 As DevComponents.DotNetBar.Controls.GroupPanel
    Friend WithEvents btnGetData As DevComponents.DotNetBar.ButtonX
    Friend WithEvents btnCalibrate As DevComponents.DotNetBar.ButtonX
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtLower As System.Windows.Forms.TextBox
    Friend WithEvents txtUpper As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtDataWeighing As System.Windows.Forms.TextBox
    Friend WithEvents TimerdbUPDATE As System.Windows.Forms.Timer
End Class
