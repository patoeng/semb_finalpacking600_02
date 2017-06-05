Imports System
Imports System.IO
Imports System.Text
Imports ControlBPM

Module Logging
    Public Sub Export2CSV(ByVal FileNama As String, ByVal sHeader As String, ByVal sData As String)

        If Not My.Computer.FileSystem.DirectoryExists(Settings.Folder.Datalog) Then My.Computer.FileSystem.CreateDirectory(Settings.Folder.Datalog)
        Dim LogFileName As String = Settings.Folder.Datalog & "\" & FileNama & Format(Now(), "_hh_mm_ss") & ".CSV"
        If Not File.Exists(LogFileName) Then
            Dim BuatJudul As String = sHeader + Environment.NewLine
            File.WriteAllText(LogFileName, BuatJudul, Encoding.UTF8)
        End If
        If sData <> "Head" Then
            Dim Newtext As String = sData + Environment.NewLine
            File.AppendAllText(LogFileName, Newtext, Encoding.UTF8)
        End If

    End Sub

End Module

