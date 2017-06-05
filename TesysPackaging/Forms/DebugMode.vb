Public Class DebugMode

    Private Sub btnTgr1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTgr1.Click
        frmUtama.ScannerCommand(1, "LON")
    End Sub
    Private Sub btnTgr2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTgr2.Click
        frmUtama.ScannerCommand(2, "LON")
    End Sub
    Private Sub DebugMode_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        StsDebugMode = False
    End Sub

    Private Sub btnPrint1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint1.Click
        frmUtama.Printing_Label_Individu()
    End Sub

    Private Sub btnPrint2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrint2.Click
        frmUtama.Printing_Label_Group()
    End Sub
End Class