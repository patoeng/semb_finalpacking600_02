Imports System.Data.OleDb
Imports ControlBPM

Public Class frmLogin
    Dim sender As Object
    Dim e As System.EventArgs


    Sub GetOperator()
        Try

            cmd = New OleDbCommand("SELECT * from tblpassword WHERE Password='" & Trim(txtPass.Text) & "'", Koneksi)
            dr = cmd.ExecuteReader
            dr.Read()

            If dr.HasRows Then
                If showfrm = "W" Then
                    Dispose()
                    frmCalibration.ShowDialog()
                ElseIf showfrm = "Debug" Then
                    Dispose()
                    StsDebugMode = True
                    DebugMode.ShowDialog()
                ElseIf showfrm = "warn" Then
                    calib_warn.Timer1.Enabled = False
                    calib_warn.Close()
                    Dispose()
                ElseIf showfrm = "li" And dr(0).ToString = "admin" Then
                    Li = True
                    Dispose()
                End If

            Else
                MsgBox("Password Invalid .....", MsgBoxStyle.Information, "Warning ID")
                txtPass.Text = ""
            End If
        Catch x As Exception
            MsgBox("ERROR : " & x.Message)
        End Try
        dr.Close()

    End Sub

    Private Sub lblExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblExit.Click
        Dispose()
    End Sub

    Private Sub txtPass_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPass.KeyPress
        If e.KeyChar = Chr(13) Then
            UserValidation()
        End If
    End Sub
    Sub UserValidation()
        If txtPass.Text <> "" Then
            GetOperator()
        Else
            MsgBox("Please fill Password", MsgBoxStyle.Information, "Confirm")
            txtPass.Focus()
        End If
    End Sub

    Private Sub txtPass_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPass.TextChanged

    End Sub
End Class
