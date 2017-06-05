Imports System.Windows.Forms



Public Class DialogForm
    Public DialogTittle As String
    Public Message As String
    Public Button1Text As String
    Public Button2Text As String
    Public Button3Text As String

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Yes
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.DialogResult = Windows.Forms.DialogResult.No
        Me.Close()
    End Sub

    Private Sub DialogForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Text = DialogTittle
        Label1.Text = Message
        OK_Button.Text = Button1Text
        Cancel_Button.Text = Button3Text
        Button1.Text = Button2Text

        If Button1Text <> "" Then OK_Button.Visible = True
        If Button2Text <> "" Then Button1.Visible = True
        If Button3Text <> "" Then Cancel_Button.Visible = True

    End Sub
End Class
