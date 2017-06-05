Imports System.Data.OleDb
Imports ControlBPM

Public Class frmCalibration
    Private Sub btnGetData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetData.Click
        txtDataWeighing.Text = Val(frmUtama.txtWeighingReal.Text)
    End Sub
    Sub UpdateData(ByVal ArtNumber As String)
        Try
            Dim cmd2 As OleDbCommand = New OleDbCommand("SELECT * FROM Tesys_Contactors_FL WHERE Art_number='" & ArtNumber & "'", Koneksi)
            Dim drs As OleDbDataReader = cmd2.ExecuteReader
            drs.Read()
            If drs.HasRows Then
                Dim cmd3 As OleDbCommand = New OleDbCommand("Update Tesys_Contactors_FL SET lowerweight = '" & Val(txtLower.Text) & "', upperweight = '" & Val(txtUpper.Text) & "' Where Art_number = '" & ArtNumber & "'", Koneksi)
                cmd3.ExecuteNonQuery()
                cmd3.Dispose()
            Else
                MsgBox("Reference Not Found...", MsgBoxStyle.Exclamation)
            End If
            drs.Close()
            cmd2.Dispose()
        Catch x As Exception
            MsgBox(x.Message)
        End Try
    End Sub

    Private Sub btnCalibrate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalibrate.Click
        Toleransi = ReadINI(Settings.ConfigFile, "WEIGHING", "Tolerance")

        txtLower.Text = Val(txtDataWeighing.Text) - Val(Toleransi)
        txtUpper.Text = Val(txtDataWeighing.Text) + Val(Toleransi)
        UpdateData(frmUtama.txtArticleNumber.Text)
        TimerdbUPDATE.Enabled = True
        txtDataWeighing.Enabled = False
        txtLower.Enabled = False
        txtUpper.Enabled = False
    End Sub

    Private Sub txtDataWeighing_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtDataWeighing.TextChanged
        If txtDataWeighing.Text = "" Or Val(txtDataWeighing.Text) = 0 Then
            btnCalibrate.Enabled = False
        Else
            btnCalibrate.Enabled = True
        End If
    End Sub

    Private Sub frmCalibration_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dispose()
    End Sub

    Private Sub frmCalibration_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        btnCalibrate.Enabled = False
    End Sub

    Private Sub TimerdbUPDATE_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerdbUPDATE.Tick
        TimerdbUPDATE.Enabled = False
        frmUtama.SearchData(frmUtama.txtArticleNumber.Text)
        txtDataWeighing.Enabled = True
        txtLower.Enabled = True
        txtUpper.Enabled = True
    End Sub
End Class
