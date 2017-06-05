Public Class calib_warn

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        showfrm = "warn"
        frmLogin.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If Me.BackColor = Color.Red Then
            Me.BackColor = Color.Yellow
        Else
            Me.BackColor = Color.Red
        End If
        '       Label3.Text = frmUtama.txtRunningReff.Text
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub calib_warn_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = True
    End Sub
End Class