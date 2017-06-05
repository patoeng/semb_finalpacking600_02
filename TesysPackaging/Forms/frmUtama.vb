Imports System.Text
Imports System.IO
'Imports Microsoft.VisualBasic
Imports System.Threading
Imports System.Data.OleDb
Imports Tkx.Lppa
Imports ControlBPM


Public Class frmUtama
#Region "Declaration "
    Dim NetApp As Tkx.Lppa.Application = Nothing
    Dim NetApp2 As Tkx.Lppa.Application = Nothing
    Private Modbus As ModbusTCP_Client
    Dim MSComm1 As New MSCommLib.MSComm
    Dim MSCommScanner1 As New MSCommLib.MSComm
    Dim MSCommScanner2 As New MSCommLib.MSComm
    'WithEvents MSCommScanner1 As SerialPort
    'WithEvents MSCommScanner2 As SerialPort
#End Region

    Public varEAC As String
    Public varCTP As String
    Public varUL As String
    Public varKC As String
    Public varKCNumber As String
    Public varCIndonesia As String
    Public varMadeinNew As String
    Public varMadeinCh As String
    Public varMadeinRu As String
    Public varMadeinOld As String
    Public varDescProRu As String
    Public firstchar As Integer
    Public pos As Integer
    Public findchar As String
    Public searchchar As String
    Public varPath As String
    Dim DBConnectionWithoutBPM As String
    Dim KoneksiTanpaBPM As OleDbConnection
    Dim RemainingGroupFromTable As Integer
    Dim ByCounterPrinted As Boolean
    Dim oldTxtBarcode1 As String
    Dim scanner1_lock As Boolean
    Dim WeighingStillSet As Boolean
    Dim TimerCounterStillInterval As Integer
    Dim WeighingIsStillInterval As Integer
    Dim TimerCounterStillIntervalRemaining As Integer
    Dim WeighingIsStillIntervalRemaining As Integer



#Region "Declaration Folder"
    Sub SettingsFolder()
        Settings.Folder.Main = AppDomain.CurrentDomain.BaseDirectory
        Settings.Folder.Config = Settings.Folder.Main & "Config"
        Settings.Folder.DBase = Settings.Folder.Main & "DBase"
        Settings.Folder.Datalog = Settings.Folder.Main & "Datalog"
        Settings.Folder.Picture = Settings.Folder.Main & "Picture"
        Settings.ConfigFile = Settings.Folder.Config & "\Config.ini"
    End Sub
    Sub FolderCheck()
        If Not My.Computer.FileSystem.DirectoryExists(Settings.Folder.Config) Then My.Computer.FileSystem.CreateDirectory(Settings.Folder.Config)
        If Not My.Computer.FileSystem.DirectoryExists(Settings.Folder.DBase) Then My.Computer.FileSystem.CreateDirectory(Settings.Folder.DBase)
        If Not My.Computer.FileSystem.DirectoryExists(Settings.Folder.Picture) Then My.Computer.FileSystem.CreateDirectory(Settings.Folder.Picture)
        If Not My.Computer.FileSystem.DirectoryExists(Settings.Folder.Datalog) Then My.Computer.FileSystem.CreateDirectory(Settings.Folder.Datalog)
        If Not File.Exists(Settings.ConfigFile) Then MsgBox("Konfigurasi Error" & vbCrLf & "File Konfigurasi Tidak Ditemukan di : " & Settings.ConfigFile)
    End Sub
#End Region
    Private Sub TimerDatetime_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerDatetime.Tick
        txtDateS2.Text = "Date  :  " & Microsoft.VisualBasic.Format(Now(), "dd-MM-yyyy")
        txtTimes2.Text = "Time  :  " & Microsoft.VisualBasic.Format(Now(), "hh:mm:ss")
    End Sub
    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        Try
            If NetApp IsNot Nothing Then
                If NetApp.Documents IsNot Nothing Then
                    NetApp.Documents.CloseAll(False)
                End If
                NetApp.Quit()
            End If

            If NetApp2 IsNot Nothing Then
                If NetApp2.Documents IsNot Nothing Then
                    NetApp2.Documents.CloseAll(False)
                End If
                NetApp2.Quit()
            End If

        Catch x As Exception
        End Try
    End Sub

    Private Sub frmUtama_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        KillApp("lppa")
    End Sub

    Public Sub frmUtama_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SettingsFolder()
        FolderCheck()
        firstload = True
        scanner1_lock = False
        lblGroupPrintPending.Text = "Group Print" & Chr(13) & Chr(10) & "Pending"

        If True Then
            Dim DBlocation As String = ""
            Try
                Dim NamaDB As String = ReadINI(Settings.ConfigFile, "DB", "Name")
                OpenDB(Settings.Folder.DBase & "\" & NamaDB, "")

                DBConnectionWithoutBPM = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Settings.Folder.DBase & "\TeSys.mdb" & ";User Id=admin;Password=;"
                KoneksiTanpaBPM = New OleDbConnection(DBConnectionWithoutBPM)
                KoneksiTanpaBPM.Open()
                KoneksiTanpaBPM.Close()

                'writePrintGroupLog("", "", "", 0, 0, 0, 0)

                IP_PLC = ReadINI(Settings.ConfigFile, "PLC", "PLCAdress")
                Modbus = New ModbusTCP_Client(IP_PLC, "502")
                txtIPAdress.Text = IP_PLC

                strPrinterIndividu = ReadINI(Settings.ConfigFile, "PRINTER", "PrinterIndividu")
                strPrinterGroup = ReadINI(Settings.ConfigFile, "PRINTER", "PrinterGroup")

                sPortScanner1 = ReadINI(Settings.ConfigFile, "BARCODESCANNER", "PortScanner1")
                sBaudrateScanner1 = ReadINI(Settings.ConfigFile, "BARCODESCANNER", "BaudrateScanner1")
                sPortScanner2 = ReadINI(Settings.ConfigFile, "BARCODESCANNER", "PortScanner2")
                sBaudrateScanner2 = ReadINI(Settings.ConfigFile, "BARCODESCANNER", "BaudrateScanner2")
                DelayPrint = ReadINI(Settings.ConfigFile, "DelayPrint", "P2")
                AN_LastRunning = ReadINI(Settings.ConfigFile, "Output", "AN")
                txtPrintPending.Text = ReadINI(Settings.ConfigFile, "PRINTER", "GroupPending")
                txtQtyPass.Text = ReadINI(Settings.ConfigFile, "Output", "Pass")
                txtQtyFail.Text = ReadINI(Settings.ConfigFile, "Output", "Fail")
                txtQtyOutput.Text = Val(txtQtyPass.Text) + Val(txtQtyFail.Text)
                Toleransi = ReadINI(Settings.ConfigFile, "WEIGHING", "Tolerance")
                If Val(Toleransi) <= 0 Then Toleransi = 0.03

                TimerCounterStillInterval = Val(ReadINI(Settings.ConfigFile, "TIMER", "Idle"))
                WeighingIsStillInterval = Val(ReadINI(Settings.ConfigFile, "TIMER", "WeighingIdle"))

                

                If (TimerCounterStillInterval <= 120000) Then TimerCounterStillInterval = 120000
                If WeighingIsStillInterval <= 5000 Then WeighingIsStillInterval = 5000

                If TimerCounterStillInterval < WeighingIsStillInterval Then TimerCounterStillInterval = WeighingIsStillInterval

                TimerCounterStillIntervalRemaining = TimerCounterStillInterval
                WeighingIsStillIntervalRemaining = WeighingIsStillInterval
                timerNoActivity.Interval = TimerCounterStillInterval
                timerWieghingIsStill.Interval = WeighingIsStillInterval
                WeighingStillSet = False
            Catch ex As Exception
                MsgBox("Database Connection Fail, Pllease check config file. Error : " & ex.Message, MsgBoxStyle.Critical, "Loading Parameters")
                End
            End Try
        End If


        'Load Weighing
        If MSComm1.PortOpen = True Then MSComm1.PortOpen = False
        Try
            With MSComm1
                .CommPort = ReadINI(Settings.ConfigFile, "WEIGHING", "WComPort")
                .Settings = ReadINI(Settings.ConfigFile, "WEIGHING", "WBaudrate")
                .PortOpen = True
            End With

        Catch ex As Exception
            MsgBox("Cannot Open Port Weighing.", MsgBoxStyle.Critical, "Packaging")
        End Try

        InitBarcodeScanner()
        tmrRead.Enabled = True
        tmrWeighing.Enabled = True
        Timer1.Enabled = True
        NetApp = New Tkx.Lppa.Application()
        NetApp2 = New Tkx.Lppa.Application()
        'AN_LastRunning = "338911035108"
        SearchData(AN_LastRunning)
        LogReff = txtReference.Text

        txtGroupRemain.Text = Val(txtgroup.Text) - (Val(txtQtyPass.Text) Mod Val(txtgroup.Text))
        TmrPrintGroup.Interval = Val(DelayPrint) * 1000
        ''FirstLd.Enabled = True
        txtStatusGroup.Text = ""
        TimerPulse1s.Enabled = True
        'firstload = False
    End Sub

#Region "BARCODE SCANNER Keyence"

    Sub InitBarcodeScanner()
        If MSCommScanner1.PortOpen = True Then MSCommScanner1.PortOpen = False
        Try
            With MSCommScanner1
                .CommPort = sPortScanner1
                .Settings = sBaudrateScanner1
                .PortOpen = True
            End With

        Catch ex As Exception
            MsgBox("Cannot Open Port Scanner 1.", MsgBoxStyle.Critical, "Packaging")
        End Try

        If MSCommScanner2.PortOpen = True Then MSCommScanner2.PortOpen = False
        Try
            With MSCommScanner2
                .CommPort = sPortScanner2
                .Settings = sBaudrateScanner2
                .PortOpen = True
            End With

        Catch ex As Exception
            MsgBox("Cannot Open Port Scanner 2.", MsgBoxStyle.Critical, "Packaging")
        End Try
    End Sub

    Public Sub ScannerCommand(ByVal ScannerID As Integer, ByVal strCommand As String)
        Try
            If ScannerID = 1 Then
                tmrBarcodeRespone1.Enabled = True
                MSCommScanner1.Output = strCommand & vbCr
            ElseIf ScannerID = 2 Then
                txtBarcode2.Text = ""
                tmrBarcodeRespone2.Enabled = True
                Dim dummy As String = MSCommScanner2.Input 'Bersihkan Input Buffer
                MSCommScanner2.Output = strCommand & vbCr
            End If
        Catch x As Exception
        End Try
    End Sub

    Sub ReadBarcode1()
        Try
            tmrBarcodeRespone1.Enabled = False
            WriteReg(13, 1) 'finis scan1
            If StsDebugMode = True Then
                DebugMode.ListBox1.Items.Add(MSCommScanner1.Input)
                DebugMode.ListBox1.SelectedIndex = (DebugMode.ListBox1.Items.Count) - 1
            Else
                bank1.Text = MSCommScanner1.Input
                If Int(InStr(1, bank1.Text, "33")) > 0 Then
                    bank10.Text = Int(InStr(1, bank1.Text, "33"))
                    txtBarcode1.Text = Mid(bank1.Text, Int(InStr(1, bank1.Text, "33")), 200)
                    bank1.Text = ""
                End If
                MSCommScanner1.Input = ""
                End If

        Catch x As Exception
        End Try
    End Sub

    Sub ReadBarcode2()
        Try
            tmrBarcodeRespone2.Enabled = False
            If StsDebugMode = True Then
                DebugMode.ListBox2.Items.Add(MSCommScanner2.Input & Len(MSCommScanner2.Input).ToString)
                DebugMode.ListBox2.SelectedIndex = (DebugMode.ListBox2.Items.Count) - 1
            Else
                Dim Barcode2Read As String = MSCommScanner2.Input

                If Barcode2Read = "" Then
                    txtBarcode2.Text = "Time Out"
                Else
                    txtBarcode2.Text = Barcode2Read
                End If
                MSCommScanner2.Output = "LOFF" & vbCr
                Barcode2Read = MSCommScanner2.Input 'Bersihkan Input Buffer

            End If
        Catch x As Exception
        End Try
    End Sub

#End Region



#Region "WEIGHING"
    Private Sub tmrWeighing_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrWeighing.Tick
        ReadWeighing()
    End Sub
    Sub ReadWeighing()
        Try
            Dim weighbuf As String = ""
            weighbuf = weighbuf & MSComm1.Input
            If InStr(1, weighbuf, vbCrLf) <> 0 Then
                weighbuf = Mid(weighbuf, 1, InStr(1, weighbuf, vbCr) - 1)
                txtWeighingReal.Text = Trim(weighbuf)
                weighbuf = ""
            End If
        Catch x As Exception
        End Try
    End Sub

#End Region

#Region "Label Printer"
    'Printer Individu
    Sub Printing_Label_Individu()
        LoadDoc_Individu()
        PrintIndividu()
        WriteReg(9, 1) 'finish printing
    End Sub

    Public Sub LoadDoc_Individu() '(ByVal sPartno As String, ByVal sReff As String, ByVal sDateTime As String)
        Dim sFile As String = ""

        If NetApp IsNot Nothing Then
            If NetApp.Documents IsNot Nothing Then
                NetApp.Documents.CloseAll(False)
            End If
        End If

        Dim doc As Document
        If txtLastPrint.Text = "1" Then
            sFile = Settings.Folder.Config & "\Label\Tesys_Ctt_Individu.lab" 'Blank.lab" remark by herman@tuguss.com 09092015
        Else
            sFile = Settings.Folder.Config & "\Label\Tesys_Ctt_Individu.lab"
        End If

        Try
            If True Then
                If File.Exists(sFile) Then
                    doc = NetApp.Documents.Open(sFile, False)
                Else
                    MessageBox.Show("Can't find Barcode file!")
                End If
            End If

        Catch Ex As Exception
            MessageBox.Show(String.Format("Setup report error: {0}", Ex.Message), "Report error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try

        doc = NetApp.ActiveDocument

        If doc IsNot Nothing Then
            Try
                If Len(Trim(txtArticleNumber.Text)) = 13 Then
                    txtArticleNumber.Text = Mid(txtArticleNumber.Text, 1, 12)
                End If
                doc.Variables.FormVariables![Art_number].Value = txtArticleNumber.Text
                doc.Variables.FormVariables![Bitmap].Value = TxtBitmap.Text
                doc.Variables.FormVariables![Current].Value = TxtCurrent.Text
                doc.Variables.FormVariables![Desc_Eng].Value = txtEnglish.Text
                doc.Variables.FormVariables![Desc_Fre].Value = txtFrance.Text
                doc.Variables.FormVariables![Desc_Ger].Value = txtGerman.Text
                doc.Variables.FormVariables![Desc_Spa].Value = TxtSpain.Text
                doc.Variables.FormVariables![Desc_Ita].Value = TxtIta.Text
                doc.Variables.FormVariables![Load_power].Value = TxtLoadpower.Text
                doc.Variables.FormVariables![Power].Value = TxtPower.Text
                doc.Variables.FormVariables![Reference].Value = txtReference.Text
                If Len(Trim(TxtBack.Text)) = 13 Then
                    TxtBack.Text = Mid(TxtBack.Text, 1, 12)
                End If
                doc.Variables.FormVariables![Back].Value = TxtBack.Text
                doc.Variables.FormVariables![voltage].Value = txtVoltage.Text


                '################ variable codesoft baru #####################
                'If txtLastPrint.Text <> "1" Then 'commented by anom 27 11 2016
                doc.Variables.FormVariables![imgEAC].Value = varEAC
                doc.Variables.FormVariables![imgCTP].Value = varCTP
                doc.Variables.FormVariables![imgUL].Value = varUL
                doc.Variables.FormVariables![imgKC].Value = varKC
                doc.Variables.FormVariables![KC_Number].Value = varKCNumber
                doc.Variables.FormVariables![C_Indonesia].Value = varCIndonesia
                doc.Variables.FormVariables![Madein_New].Value = varMadeinNew
                doc.Variables.FormVariables![Madein_Ru].Value = varMadeinRu
                doc.Variables.FormVariables![Madein_Ch].Value = varMadeinCh
                doc.Variables.FormVariables![Madein_Old].Value = varMadeinOld
                doc.Variables.FormVariables![DescPro_Ru].Value = varDescProRu
                doc.Variables.FormVariables![Path].Value = varPath
                ' End If
                '################ variable codesoft baru #####################

            Catch x As Exception
                MsgBox("Label Variable not Match")
                Exit Sub
            End Try
        End If


        If doc IsNot Nothing Then
            Dim split As String() = strPrinterIndividu.Split(New [Char]() {","c})
            If split.GetLength(0) = 2 Then
                Dim directAccess As Boolean = False
                Dim PrinterName As String = split(0)
                Dim PortName As String = split(1)
                If PortName.StartsWith("->") Then
                    directAccess = True
                    PortName = PortName.Substring(2)
                End If
                doc.Printer.SwitchTo(PrinterName, PortName, directAccess)
            End If
        End If
    End Sub
    Public Sub PrintIndividu()
        Dim doc As Document = NetApp.ActiveDocument
        If doc IsNot Nothing Then
            doc.PrintDocument(1)
        End If
    End Sub


    'printer Group
    Sub Printing_Label_Group()
        LoadDoc_Group()
        PrintGroup()
        If (Val(txtPrintPending.Text) > 0) Then
            txtPrintPending.Text = Val(txtPrintPending.Text) - 1
        End If


    End Sub

    Public Sub LoadDoc_Group() '(ByVal sPartno As String, ByVal sReff As String, ByVal sDateTime As String)
        listBoxPrinterGroupLogAdd("Preparing to Print Completed Group Label...")
        Dim sFile As String = ""

        If NetApp2 IsNot Nothing Then
            If NetApp2.Documents IsNot Nothing Then
                NetApp2.Documents.CloseAll(False)
            End If
        End If

        Dim doc As Document
        sFile = Settings.Folder.Config & "\Label\Tesys_Ctt_group.lab"

        Try
            If True Then
                If File.Exists(sFile) Then
                    doc = NetApp2.Documents.Open(sFile, False)
                Else
                    MessageBox.Show("Can't find Barcode file!")
                End If
            End If
        Catch Ex As Exception
            MessageBox.Show(String.Format("Setup report error: {0}", Ex.Message), "Report error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try

        doc = NetApp2.ActiveDocument
        If doc IsNot Nothing Then
            Try
                doc.Variables.FormVariables![Art_number].Value = txtArticleNumber.Text
                doc.Variables.FormVariables![Bitmap].Value = TxtBitmap.Text
                doc.Variables.FormVariables![Current].Value = TxtCurrent.Text
                doc.Variables.FormVariables![Desc_Eng].Value = txtEnglish.Text
                doc.Variables.FormVariables![Desc_Fre].Value = txtFrance.Text
                doc.Variables.FormVariables![Desc_Ger].Value = txtGerman.Text
                doc.Variables.FormVariables![Desc_Spa].Value = TxtSpain.Text
                doc.Variables.FormVariables![Desc_Ita].Value = TxtIta.Text
                doc.Variables.FormVariables![Load_power].Value = TxtLoadpower.Text
                doc.Variables.FormVariables![Power].Value = TxtPower.Text
                doc.Variables.FormVariables![Reference].Value = txtReference.Text

                If Len(Trim(TxtBack.Text)) = 13 Then
                    TxtBack.Text = Mid(TxtBack.Text, 1, 12)
                End If

                doc.Variables.FormVariables![Back].Value = TxtBack.Text
                doc.Variables.FormVariables![voltage].Value = txtVoltage.Text
                doc.Variables.FormVariables![Qty_Group].Value = txtgroup.Text

                '################ variable codesoft baru (Grouping) #####################

                doc.Variables.FormVariables![imgEAC].Value = varEAC
                doc.Variables.FormVariables![imgCTP].Value = varCTP
                doc.Variables.FormVariables![imgUL].Value = varUL
                doc.Variables.FormVariables![imgKC].Value = varKC
                doc.Variables.FormVariables![KC_Number].Value = varKCNumber
                doc.Variables.FormVariables![C_Indonesia].Value = varCIndonesia
                doc.Variables.FormVariables![Madein_New].Value = varMadeinNew
                doc.Variables.FormVariables![Madein_Ch].Value = varMadeinCh
                doc.Variables.FormVariables![Madein_Ru].Value = varMadeinRu
                doc.Variables.FormVariables![Madein_Old].Value = varMadeinOld
                doc.Variables.FormVariables![DescPro_Ru].Value = varDescProRu
                doc.Variables.FormVariables![Path].Value = varPath

                '################ variable codesoft baru (Grouping) #####################

            Catch x As Exception
                MsgBox("Label Variable not Match")
                Exit Sub
            End Try
        End If


        If doc IsNot Nothing Then
            Dim split As String() = strPrinterGroup.Split(New [Char]() {","c})
            If split.GetLength(0) = 2 Then
                Dim directAccess As Boolean = False
                Dim PrinterName As String = split(0)
                Dim PortName As String = split(1)
                If PortName.StartsWith("->") Then
                    directAccess = True
                    PortName = PortName.Substring(2)
                End If
                doc.Printer.SwitchTo(PrinterName, PortName, directAccess)
            End If
        End If
        listBoxPrinterGroupLogAdd("Preparing to Print Completed Group Label... Done!")
    End Sub
    Public Sub PrintGroup()
        listBoxPrinterGroupLogAdd("Printing Group Label...")
        Dim doc As Document = NetApp2.ActiveDocument
        If doc IsNot Nothing Then
            doc.PrintDocument(1)

        End If
        listBoxPrinterGroupLogAdd("Printing Group Label... Done!")
    End Sub
    Public Sub LoadDoc_Group_Incomplete(ByVal ArticleNumber As String, ByVal Reference As String, ByVal back As String, ByVal QtyGroup As Integer, ByVal QtyRemaining As Integer) '(ByVal sPartno As String, ByVal sReff As String, ByVal sDateTime As String)
        listBoxPrinterGroupLogAdd("Preparing To Print Incomplete Label...")
        Dim sFile As String = ""

        If NetApp2 IsNot Nothing Then
            If NetApp2.Documents IsNot Nothing Then
                NetApp2.Documents.CloseAll(False)
            End If
        End If

        Dim doc As Document
        sFile = Settings.Folder.Config & "\Label\Tesys_Ctt_group_Incomplete2.lab"

        Try
            If True Then
                If File.Exists(sFile) Then
                    doc = NetApp2.Documents.Open(sFile, False)
                Else
                    MessageBox.Show("Can't find Barcode file!")
                End If
            End If
        Catch Ex As Exception
            MessageBox.Show(String.Format("Setup report error: {0}", Ex.Message), "Report error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        End Try

        doc = NetApp2.ActiveDocument
        If doc IsNot Nothing Then
            Try
                doc.Variables.FormVariables![Art_number].Value = ArticleNumber

                doc.Variables.FormVariables![Reference].Value = Reference

                If Len(Trim(TxtBack.Text)) = 13 Then
                    TxtBack.Text = Mid(TxtBack.Text, 1, 12)
                End If

                doc.Variables.FormVariables![Back].Value = back
                doc.Variables.FormVariables![Qty_Group].Value = "GROUP : " & txtgroup.Text
                doc.Variables.FormVariables![var36].Value = "QTY : " & (Val(txtgroup.Text) - Val(txtGroupRemain.Text)).ToString
                doc.Variables.FormVariables![var37].Value = txtGroupRemain.Text

                '################ variable codesoft baru (Grouping) #####################



                '################ variable codesoft baru (Grouping) #####################

            Catch x As Exception
                MsgBox("Label Variable not Match")
                Exit Sub
            End Try

        End If


        If doc IsNot Nothing Then
            Dim split As String() = strPrinterGroup.Split(New [Char]() {","c})
            If split.GetLength(0) = 2 Then
                Dim directAccess As Boolean = False
                Dim PrinterName As String = split(0)
                Dim PortName As String = split(1)
                If PortName.StartsWith("->") Then
                    directAccess = True
                    PortName = PortName.Substring(2)
                End If
                doc.Printer.SwitchTo(PrinterName, PortName, directAccess)
            End If
        End If
        listBoxPrinterGroupLogAdd("Preparing To Print Incomplete Label... Done")
    End Sub
#End Region

#Region "DATABASE data search"
    Public Function SearchData(ByVal ArtNumber As String) As Boolean
        Try
            KoneksiTanpaBPM.Open()
            cmd = New OleDbCommand("SELECT * FROM Tesys_Contactors_FL WHERE Art_number='" & ArtNumber & "'", KoneksiTanpaBPM)
            dr = cmd.ExecuteReader
            dr.Read()

            If dr.HasRows Then
                txtArticleNumber.Text = dr(2).ToString
                TxtBitmap.Text = dr(20).ToString
                TxtBack.Text = dr(6).ToString
                TxtCurrent.Text = dr(18).ToString
                txtEnglish.Text = dr(12).ToString
                txtFrance.Text = dr(13).ToString
                txtGerman.Text = dr(14).ToString
                TxtSpain.Text = dr(15).ToString
                TxtIta.Text = dr(16).ToString
                TxtLoadpower.Text = dr(11).ToString
                TxtPower.Text = dr(19).ToString
                txtReference.Text = dr(5).ToString
                txtRunningReff.Text = dr(5).ToString
                If InStr(txtReference.Text, "AA") Then
                    txtformula25.Text = txtReference.Text
                ElseIf InStr(txtReference.Text, "AB") Then
                    txtformula25.Text = txtReference.Text
                Else
                    txtformula25.Text = ""
                End If

                txtVoltage.Text = dr(17).ToString
                txtgroup.Text = dr(22).ToString
                txtupper.Text = Val(dr(29).ToString) * Val(dr(22).ToString) ' 16
                txtlower.Text = Val(dr(30).ToString) * Val(dr(22).ToString) ' 16
                If CheckIfToleransiOK(Val(dr(30).ToString), Val(dr(29).ToString), Toleransi) = False Then
                    listBoxPrinterGroupLogAdd("Toleransi Weighing di luar spec, please re-calibrate!!")
                End If
                varEAC = dr(31).ToString
                varCTP = dr(32).ToString
                varUL = dr(33).ToString
                varKC = dr(34).ToString
                varKCNumber = dr(35).ToString
                varCIndonesia = dr(36).ToString
                varMadeinCh = dr(24).ToString
                varMadeinRu = dr(37).ToString
                varMadeinNew = dr(38).ToString
                varMadeinOld = dr(39).ToString

                varDescProRu = dr(40).ToString

                varPath = "D:\Modified TesysPackaging\TesysPackaging\bin\Debug\image\"

                RemainingGroupFromTable = Val(dr(41).ToString)
                If RemainingGroupFromTable <= 0 Then RemainingGroupFromTable = Val(txtgroup.Text)

                SearchData = True
            Else
                SearchData = False
            End If
            dr.Close()
        Catch x As Exception
            MsgBox(x.ToString)
        End Try


        KoneksiTanpaBPM.Close()
    End Function

    Sub UpdateArticleLastRemaining(ByVal ArticleNumber As String, ByVal LastRemainingQty As Integer)
        Try
            'Koneksi.Close()
            KoneksiTanpaBPM.Open()
            Dim cmd2 As OleDbCommand = New OleDbCommand("UPDATE Tesys_Contactors_FL SET LastRunningGroupRemaining='" & LastRemainingQty.ToString & "' WHERE Art_number='" & ArticleNumber & "'", KoneksiTanpaBPM)
            Dim dr3 = cmd2.ExecuteNonQuery()

            cmd2 = New OleDbCommand("SELECT * FROM Tesys_Contactors_FL WHERE Art_number='" & ArticleNumber & "'", KoneksiTanpaBPM)
            Dim dr2 As OleDbDataReader = cmd2.ExecuteReader
            dr2.Read()
            If dr2.HasRows Then
                listBoxPrinterGroupLogAdd(dr2.GetString(5) & " " & dr2.GetString(41))
            End If
            KoneksiTanpaBPM.Close()
            
        Catch x As Exception
            MsgBox(x.Message)
        End Try

    End Sub
    Sub listBoxPrinterGroupLogAdd(ByVal log As String)
        listBoxPrinterGroupLog.Items.Add(log)
        listBoxPrinterGroupLog.SelectedIndex = listBoxPrinterGroupLog.Items.Count - 1
    End Sub
    Sub listBoxPrinterGroupLogClear()
        listBoxPrinterGroupLog.Items.Clear()
    End Sub
    Sub ShowLogReff(ByVal ArtNumber As String)
        Try
            KoneksiTanpaBPM.Open()
            cmd = New OleDbCommand("SELECT * FROM Tesys_Contactors_FL WHERE Art_number='" & ArtNumber & "'", KoneksiTanpaBPM)
            dr = cmd.ExecuteReader
            dr.Read()
            If dr.HasRows Then LogReff = dr.GetString(5)
            KoneksiTanpaBPM.Open()
        Catch x As Exception
            KoneksiTanpaBPM.Close()
        End Try
    End Sub
    Function CheckIfToleransiOK(ByVal Minimum As Double, ByVal Maximum As Double, byval Toleransi as Double) As Boolean
        Dim ToleransiKaliDua As Double = Toleransi * 2
        Dim SelisihMaxMin As Double = Maximum - Minimum
        Dim a As Boolean = (ToleransiKaliDua <= SelisihMaxMin + 0.01) And (ToleransiKaliDua >= SelisihMaxMin - 0.01)
        Return a
    End Function

    Public Sub txtBarcode1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBarcode1.TextChanged
        timerNoActivity.Enabled = False
        Dim s = timerWieghingIsStill.Enabled

        If txtBarcode1.Text <> "" And scanner1_lock = False Then
            Dim ArtNumber As String = Mid(txtBarcode1.Text, Int(InStr(1, txtBarcode1.Text, "3")), 12)

            '338911035108LC1D32M7123456789001962
            '1 =run 2=set reff (Mw11)
            If txtStartScan1.Text = "2" Then
                If ((txtArticleNumber.Text <> ArtNumber And txtBarcode1.Text <> oldTxtBarcode1) Or txtArticleNumber.Text = "") Then
                    Dim OldReffNumber = LogReff
                    Dim OldArticleNumber = txtArticleNumber.Text
                    LogReff = ""
                    ShowLogReff(ArtNumber)
                    Dim Konfirmasi As MsgBoxResult = MsgBoxResult.No

                    If Val(txtGroupRemain.Text) <> Val(txtgroup.Text) And Val(txtGroupRemain.Text) <> 0 Then
                        scanner1_lock = True
                        Konfirmasi = DialogBox("Mengakhiri Reference Proses", "Anda akan mengakhiri reference process," & Chr(13) & Chr(10) & " Reference lama belum complete", "Print InComplete Label", "Print Complete Label", "Batalkan")
                        If Konfirmasi = MsgBoxResult.Yes Then
                            'Print Incomplete Lable
                            GroupLablePrintIncomplete(OldArticleNumber, OldReffNumber, TxtBack.Text, Val(txtgroup.Text), Val(txtGroupRemain.Text))
                            writePrintGroupLog("Incomplete Print by Change Reference", OldArticleNumber, OldReffNumber, Val(txtGroupRemain.Text), Val(txtQtyPass.Text), Val(txtQtyFail.Text), Val(txtgroup.Text))
                            UpdateArticleLastRemaining(OldArticleNumber, Val(txtGroupRemain.Text))
                        End If
                        If Konfirmasi = MsgBoxResult.No Then
                            Printing_Label_Group()
                            writePrintGroupLog("Complete Print by Change Reference", OldArticleNumber, OldReffNumber, Val(txtGroupRemain.Text), Val(txtQtyPass.Text), Val(txtQtyFail.Text), Val(txtgroup.Text))
                            ResetCounterAndReference()
                            UpdateArticleLastRemaining(OldArticleNumber, Val(txtgroup.Text))
                            ShowLogReff(ArtNumber)
                        End If
                        scanner1_lock = False
                    Else
                        UpdateArticleLastRemaining(OldArticleNumber, Val(txtgroup.Text))
                    End If
                    If Konfirmasi <> MsgBoxResult.Cancel Then
                        If SearchData(ArtNumber) = True Then
                            SetRunningReference(txtBarcode1.Text.Trim, txtReference.Text) 'Send reff to PLC
                            listLogScanner1.Items.Clear()
                            listLogScanner2.Items.Clear()
                            listLogScanner1.Items.Add(LogReff & "  Set Reference Success")
                            WriteINI(Settings.ConfigFile, "Output", "AN", ArtNumber)
                            WriteINI(Settings.ConfigFile, "Output", "Pass", "0")
                            WriteINI(Settings.ConfigFile, "Output", "Fail", "0")
                            WriteINI(Settings.ConfigFile, "PRINTER", "GroupPending", "0")
                            txtQtyPass.Text = Val(txtgroup.Text) - (RemainingGroupFromTable)
                            txtQtyFail.Text = "0"
                            txtQtyOutput.Text = txtQtyPass.Text
                            txtGroupRemain.Text = RemainingGroupFromTable
                            If RemainingGroupFromTable <> Val(txtgroup.Text) Then
                                listBoxPrinterGroupLogAdd("Prosess terakhir reference '" & LogReff.ToString & " belum complete")
                            End If
                            txtPrintPending.Text = "0"
                            writePrintGroupLog("Change Referensce Successfull", txtArticleNumber.Text, txtReference.Text, Val(txtGroupRemain.Text), Val(txtQtyPass.Text), Val(txtQtyFail.Text), Val(txtgroup.Text))
                            SetRunningReference(txtBarcode1.Text.Trim, txtReference.Text)
                        Else
                            listLogScanner1.Items.Add("Reference Not Found")
                        End If
                    Else
                        listLogScanner1.Items.Add("Reference Change canceled")
                        LogReff = OldReffNumber
                    End If
                Else
                    SetRunningReference(txtBarcode1.Text.Trim, txtReference.Text)
                End If
            ElseIf txtStartScan1.Text = "1" Then
                Dim reff As String
                reff = ArtNumber
                Dim PanjangSpace As Integer = 25 - Len(ArtNumber)
                reff = reff & Microsoft.VisualBasic.Space(PanjangSpace)


                If ArtNumber = txtArticleNumber.Text.Trim Then
                    WriteReg(12, 1)
                    listLogScanner1.Items.Add(LogReff & "  - PASS   " & Now.ToString)
                Else
                    WriteReg(12, 2)
                    listLogScanner1.Items.Add(LogReff & "  - NG     " & Now.ToString)
                End If
            End If

            listLogScanner1.SelectedIndex = (listLogScanner1.Items.Count) - 1
            ' txtBarcode1.Text = ""
            WriteReg(11, 0) 'Scan1 Finish
        End If
        txtBarcode1.Text = ""
        timerNoActivity.Enabled = True
        TimerCounterStillIntervalRemaining = TimerCounterStillInterval
        timerWieghingIsStill.Enabled = s
        If s = False Then WeighingIsStillIntervalRemaining = WeighingIsStillInterval
    End Sub
    Sub GroupLablePrintIncomplete(ByVal ArticleNumber As String, ByVal Reference As String, ByVal back As String, ByVal GroupQty As Integer, ByVal RemainingQty As Integer)
        LoadDoc_Group_Incomplete(ArticleNumber, Reference, back, GroupQty, RemainingQty)
        PrintGroup()
    End Sub
    Sub SetRunningReference(ByVal DataMetrix As String, ByVal Reference As String)
        'DATA MATRIX :  3389 1103 5108 LC1D 32M7 1234 5678 9001 962" (9)
        'REFF        :  LC1D32M7

        ConvertDataSendPLC(DataMetrix, 40, "Terbalik") '40/2 'MW65 - MW84
        For i As Integer = 0 To 20
            WriteReg(65 + i, iConvertDataSendPLC(i))
        Next i
        CLS_Memory(20)

        ConvertDataSendPLC(Reference, 16, "Terbalik")  'MW85 - MW92
        For i As Integer = 0 To 8
            WriteReg(85 + i, iConvertDataSendPLC(i))
        Next i

        CLS_Memory(8)

    End Sub

    Sub CLS_Memory(ByVal nx As Integer)
        For cls As Integer = 0 To nx
            iConvertDataSendPLC(cls) = 0
        Next
    End Sub


#End Region

#Region "PLC"
    Public Sub WriteReg(ByVal adr As Integer, ByVal val As Long)
        Dim Mw() As Long = {val}
        Modbus.Write_Words(adr, 1, Mw)
    End Sub


    Public Function ReadRegAll(ByVal StartAdd As Integer) As Boolean
        Dim Buffer(30) As Long
        If Modbus.Read_Words(StartAdd, Buffer.Length, Buffer) Then
            txtStartScan1.Text = Buffer(11)
            txtScan1Ok.Text = Buffer(12)
            txtRejectProd.Text = Buffer(8)
            Barcode1Err.Text = Buffer(14) 'finish
            txtStartPrint1.Text = Buffer(10)
            txtLastPrint.Text = Buffer(15)
            Barcode2Err.Text = Buffer(20) 'finish
            txtStartScan2.Text = Buffer(21)
            txtScan2Ok.Text = Buffer(22)

            'txtScan2Finish.Text = Buffer(23) 'Finish
            'txtAricleN_1.Text = Buffer(15)
            'txtAricleN_2.Text = Buffer(16)
            'txtAricleN_3.Text = Buffer(17)
            'txtAricleN_4.Text = Buffer(18)

        End If
    End Function

    Private Sub tmrRead_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrRead.Tick
        txtStatusPLC.Text = StatusPLC
        ReadRegAll(0)
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        hit += 1
        If txtStatusPLC.Text = "CONNECTED" Then
            If hit = 2 Then
                lblSinyal.BackColor = Color.White
            ElseIf hit = 3 Then
                lblSinyal.BackColor = Color.Blue
                hit = 1
            End If
        Else
            lblSinyal.BackColor = Color.Red
        End If
    End Sub

#End Region


    Private Sub Barcode1Err_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Barcode1Err.TextChanged
        If Barcode1Err.Text = "1" Then
            ScannerCommand(1, "LOFF")
        End If
    End Sub
    Private Sub Barcode2Err_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Barcode2Err.TextChanged
        If Barcode2Err.Text = "1" Then
            tmrErrB2.Enabled = True
        End If
    End Sub
    Private Sub txtBarcode2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtBarcode2.TextChanged
        If StsDebugMode = False Then
            Dim reff As String
            reff = txtBarcode2.Text.Trim
            If txtBarcode2.Text <> "" Then
                Dim PanjangSpace As Integer = 25 - Len(txtBarcode2.Text.Trim)
                reff = reff & Microsoft.VisualBasic.Space(PanjangSpace)
                If Mid(txtBarcode2.Text.Trim, 1, 12) = txtArticleNumber.Text.Trim Then  'txtRunningReff.Text.Trim Then
                    WriteReg(22, 1) 'pass
                    listLogScanner2.Items.Add(reff & "PASS    " & Now.ToString)
                    txtQtyPass.Text = Val(txtQtyPass.Text) + 1
                    txtGroupRemain.Text = Val(txtgroup.Text) - (Val(txtQtyPass.Text) Mod Val(txtgroup.Text))
                    WriteINI(Settings.ConfigFile, "Output", "Pass", txtQtyPass.Text)
                Else
                    WriteReg(22, 2) 'fail
                    listLogScanner2.Items.Add(reff & "NG      " & Now.ToString)
                    txtQtyFail.Text = Val(txtQtyFail.Text) + 1
                    WriteINI(Settings.ConfigFile, "Output", "Fail", txtQtyFail.Text)
                End If
                txtQtyOutput.Text = Val(txtQtyPass.Text) + Val(txtQtyFail.Text)
                listLogScanner2.SelectedIndex = (listLogScanner2.Items.Count) - 1
                'txtBarcode2.Text = ""
                WriteReg(23, 1) 'Scan2 Finish
            Else
              
            End If

        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        txtBarcode2.Text = TextBox2.Text
    End Sub


    Private Sub txtStartScan1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtStartScan1.TextChanged
        '1 = run reff    2= Set reff
        If txtStartScan1.Text = "1" Or txtStartScan1.Text = "2" Then
            If scanner1_lock = False Then
                ScannerCommand(1, "LON")
            End If
        End If

    End Sub

    Private Sub txtStartScan2_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtStartScan2.TextChanged
        If txtStartScan2.Text = "1" Then
            txtBarcode2.Text = ""
            If TimerScan2TO.Enabled = False Then
                TimerScan2TO.Enabled = True 'Enable jika ingin ada time limit antar scan2 
                ScannerCommand(2, "LON")
            End If
        End If
    End Sub

    Private Sub txtStartPrint1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtStartPrint1.TextChanged
        If txtStartPrint1.Text = "1" Then
            Printing_Label_Individu()
            WriteReg(10, 0) 'Print1 Finish
        End If
    End Sub
    Private Sub txtGroupRemain_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGroupRemain.TextChanged
        timerNoActivity.Enabled = False
        'timerWieghingIsStill.Enabled = False
        If txtQtyOutput.Text <> 0 And firstload = False Then
            If QtyAdjust = False Then
                If Val(txtGroupRemain.Text) = Val(txtgroup.Text) Then
                    txtStatusGroup.Text = "Print Ready"
                    txtPrintPending.Text = Val(txtPrintPending.Text) + 1

                End If
            Else
                If Val(txtGroupRemain.Text) = Val(txtgroup.Text) Then
                    txtStatusGroup.Text = "Print Ready"
                    If Val(txtPrintPending.Text) > 0 Then
                        txtPrintPending.Text = Val(txtPrintPending.Text) - 1
                    End If
                End If
                End If
        End If
            If Val(txtPrintPending.Text) <= Val(txtgroup.Text) And (Val(txtWeighing.Text) > Val(txtlower.Text)) And (Val(txtWeighing.Text) < Val(txtupper.Text)) Then
                tmrWeighingStabilitazion.Enabled = True
            End If
            If LogReff <> "" Then
                timerNoActivity.Enabled = True
                'timerWieghingIsStill.Enabled = True
                TimerCounterStillIntervalRemaining = TimerCounterStillInterval
            End If
            firstload = False
            QtyAdjust = False
    End Sub

    Private Sub txtWeighing_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtWeighing.TextChanged
        timerWieghingIsStill.Enabled = False
        WeighingIsStillIntervalRemaining = WeighingIsStillInterval
        WeighingStillSet = False
        timerNoActivity.Enabled = False
        If (Val(txtWeighing.Text) > Val(txtlower.Text)) And (Val(txtWeighing.Text) < Val(txtupper.Text)) Then
            If Val(txtPrintPending.Text) > 0 Then
                writePrintGroupLog("Automatic Complete Group Print", txtArticleNumber.Text, txtReference.Text, Val(txtGroupRemain.Text), Val(txtQtyPass.Text), Val(txtQtyFail.Text), Val(txtgroup.Text))
                TmrPrintGroup.Enabled = True
                listBoxPrinterGroupLogAdd("Delayed Automatic Group Print Started...")
                tmrWeighingStabilitazion.Enabled = False
            Else
                TmrPrintGroup.Enabled = False
                tmrWeighingStabilitazion.Enabled = True
            End If
            txtWeighingShow.ForeColor = Color.Green
        Else
            tmrWeighingStabilitazion.Enabled = False
            txtWeighingShow.ForeColor = Color.Black
        End If
        timerNoActivity.Enabled = True
        TimerCounterStillIntervalRemaining = TimerCounterStillInterval
        timerWieghingIsStill.Enabled = True
        'WeighingIsStillIntervalRemaining = WeighingIsStillInterval
    End Sub


    Private Sub TmrPrintGroup_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TmrPrintGroup.Tick
        TmrPrintGroup.Enabled = False
        txtStatusGroup.Text = ""
        If Val(txtPrintPending.Text) >= 0 Then
            Printing_Label_Group()
            listBoxPrinterGroupLogAdd("Delayed Automatic Group Print... Done!!")
        Else
            listBoxPrinterGroupLogAdd("Delayed Automatic Group Print... Canceled!!")
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        txtBarcode1.Text = TextBox2.Text
    End Sub

    Private Sub btnDebug_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDebug.Click
        showfrm = "Debug"
        frmLogin.ShowDialog()
        'StsDebugMode = True
        'DebugMode.ShowDialog()
    End Sub

    Private Sub txtWeighingReal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtWeighingReal.TextChanged
        If Len(txtWeighingReal.Text) = 8 Then '8 and 10
            txtWeighing.Text = Val(txtWeighingReal.Text)
            txtWeighingShow.Text = Val(txtWeighingReal.Text) & " Kg"
        End If
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Toleransi = 0.03
        txtStartScan1.Text = "2"
        Me.txtBarcode1.Text = "3389110802214LC1D093B7123456789001962"
    End Sub

    Private Sub tmrBarcodeRespone2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrBarcodeRespone2.Tick
        ReadBarcode2()
    End Sub

    Private Sub tmrBarcodeRespone1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrBarcodeRespone1.Tick
        ReadBarcode1()

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        txtStatusGroup.Text = "ready"
    End Sub
    Private Sub tmrErrB2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrErrB2.Tick
        tmrErrB2.Enabled = False
        ScannerCommand(2, "LOFF")
    End Sub

    Private Sub btnCalibration_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalibration.Click
        showfrm = "W"
        frmLogin.ShowDialog()
    End Sub

    Private Sub FirstLd_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FirstLd.Tick
        FirstLd.Enabled = False
        firstload = False
    End Sub

    Private Sub btnDec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDec.Click
        showfrm = "li"
        If (Li = False) Then
            frmLogin.ShowDialog()
        End If
        If (Li = True) Then
            DecQTY()
            TimerPassword.Enabled = False
            TimerPassword.Enabled = True

        End If 'If MsgBox("Decrease Quantity ?", MsgBoxStyle.YesNo, "Packaging") = MsgBoxResult.Yes Then

        'End If
    End Sub
    Dim QtyAdjust As Boolean
    Sub DecQTY()
        If Val(txtGroupRemain.Text) > 0 Then
            txtQtyPass.Text = Val(txtQtyPass.Text) + 1
            txtQtyOutput.Text = Val(txtQtyPass.Text) + Val(txtQtyFail.Text)
            'QtyAdjust = True
            txtGroupRemain.Text = Val(txtgroup.Text) - (Val(txtQtyPass.Text) Mod Val(txtgroup.Text))

            WriteINI(Settings.ConfigFile, "Output", "Pass", txtQtyPass.Text)
            'txtGroupRemain.Text = (Val(txtGroupRemain.Text) - 1).ToString
        End If
    End Sub
    Sub IncQTY()
        If Val(txtGroupRemain.Text) < Val(txtgroup.Text) And Val(txtQtyPass.Text) > 0 Then
            txtQtyPass.Text = Val(txtQtyPass.Text) - 1
            txtQtyOutput.Text = Val(txtQtyPass.Text) + Val(txtQtyFail.Text)
            QtyAdjust = True
            txtGroupRemain.Text = Val(txtgroup.Text) - (Val(txtQtyPass.Text) Mod Val(txtgroup.Text))

            WriteINI(Settings.ConfigFile, "Output", "Pass", txtQtyPass.Text)
            'txtGroupRemain.Text = (Val(txtGroupRemain.Text) - 1).ToString
        End If
    End Sub

    Private Sub txtRejectProd_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRejectProd.TextChanged
        If Val(txtRejectProd.Text) = 1 Then
            IncQTY()
        End If
    End Sub

    Private Sub listLogScanner1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles listLogScanner1.SelectedIndexChanged

    End Sub

    Public Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button6_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
    End Sub

    Private Sub tmr_calib_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmr_calib.Tick

    End Sub

    Private Sub Button6_Click_2(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub txtRunningReff_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtRunningReff.Click

    End Sub

    Private Sub txtRunningReff_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtRunningReff.MouseDown



    End Sub

    Private Sub txtRunningReff_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRunningReff.TextChanged

    End Sub

    Private Sub txtGroupRemain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub txtQtyPass_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtQtyPass.Click

    End Sub

    Private Sub txtQtyPass_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtQtyPass.TextChanged
        If txtQtyPass.Text = "2" Then
            If txtupper.Text = "0" And txtlower.Text = "0" Then
                calib_warn.ShowDialog()
            End If
        End If
    End Sub

    Private Sub Button6_Click_3(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Sub ResetCounterAndReference()
        SetRunningReference("0000000000000000000000000000000000000000", "0000000000000000")
        txtArticleNumber.Text = ""
        LogReff = ""
        txtRunningReff.Text = ""
        WriteINI(Settings.ConfigFile, "Output", "AN", txtArticleNumber.Text)
        WriteINI(Settings.ConfigFile, "Output", "Pass", "0")
        WriteINI(Settings.ConfigFile, "Output", "Fail", "0")
        WriteINI(Settings.ConfigFile, "PRINTER", "GroupPending", "0")
        txtQtyPass.Text = "0"
        txtQtyFail.Text = "0"
        txtQtyOutput.Text = Val(txtQtyPass.Text) + Val(txtQtyFail.Text)
        txtGroupRemain.Text = Val(txtgroup.Text) - (Val(txtQtyPass.Text) Mod Val(txtgroup.Text))
        txtPrintPending.Text = "0"
        listBoxPrinterGroupLogAdd("Reset Counter Successfull!")
        txtMsg.Text = "-"

        scanner1_lock = False
    End Sub
    Private Sub Button6_Click_4(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetCounter.Click


        Li = False
        showfrm = "li"
        frmLogin.ShowDialog()
        If (Li = True) Then
            ResetCounterAndReference()
            Li = False
        End If
    End Sub

    Private Sub Button6_Click_5(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        UpdateArticleLastRemaining("338911035108", 15)
    End Sub

    Private Sub Button7_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Toleransi = 0.03
        txtStartScan1.Text = "2"
        Me.txtBarcode1.Text = "338911038667LC2D18Q7123456789001962"
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerNoActivity.Tick
        'Warning Counter is Still

        Dim s = timerWieghingIsStill.Enabled
        timerWieghingIsStill.Enabled = False

        timerNoActivity.Enabled = False
        If scanner1_lock = False Then
            If (Val(txtQtyOutput.Text) > 0) Then
                scanner1_lock = True
                Dim dialogResult = DialogBox("Counter Diam", "Counter sudah berhenti selama lebih dari : " & (TimerCounterStillInterval / 1000).ToString & " detik", "Akhiri Reference Proses", "Abaikan,dan Lanjutkan", "")
                timerWieghingIsStill.Enabled = False
                If dialogResult = MsgBoxResult.Yes Then
                    If Val(txtGroupRemain.Text) <> Val(txtgroup.Text) And Val(txtGroupRemain.Text) <> 0 Then

                        'Dim Konfirmasi = MsgBox("Anda akan mengakhiri reference process, Reference lama belum complete," & Chr(13) & Chr(10) & "Click [ Yes ] Jika anda ingin mencetak In-Complete Label" & Chr(13) & Chr(10) & "Click [ No ] Jika anda ingin mencetak Complete Label" & Chr(13) & Chr(10) & "Click [ Cancel ] Jika anda ingin membatalkan", MsgBoxStyle.YesNoCancel)
                        Dim Konfirmasi = DialogBox("Mengakhiri Reference Proses", "Anda akan mengakhiri reference process," & Chr(13) & Chr(10) & "Reference lama belum complete", "Print InComplete Label", "Print Complete Label", "Batalkan")
                        If Konfirmasi = MsgBoxResult.Yes Then
                            'Print Incomplete Lable
                            GroupLablePrintIncomplete(txtArticleNumber.Text, LogReff, TxtBack.Text, Val(txtgroup.Text), Val(txtGroupRemain.Text))
                            UpdateArticleLastRemaining(txtArticleNumber.Text, Val(txtGroupRemain.Text))
                            ResetCounterAndReference()
                        End If
                        If Konfirmasi = MsgBoxResult.No Then
                            Printing_Label_Group()
                            UpdateArticleLastRemaining(txtArticleNumber.Text, Val(txtgroup.Text))
                            ResetCounterAndReference()
                        End If

                    Else
                        Dim Konfirmasi = DialogBox("Mengakhiri Reference Proses", "Reference sudah complete", "Print Complete Label", "Akhiri Tanpa Print", "")
                        If Konfirmasi = MsgBoxResult.Yes Then
                            Printing_Label_Group()
                        End If
                        UpdateArticleLastRemaining(txtArticleNumber.Text, Val(txtgroup.Text))
                        LogReff = ""
                        ResetCounterAndReference()
                    End If

                End If
                scanner1_lock = False
            Else

            End If
        End If
        timerWieghingIsStill.Enabled = s
        If s = False Then WeighingIsStillIntervalRemaining = WeighingIsStillInterval
    End Sub

    Private Sub txtWeighingShow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtWeighingShow.Click

    End Sub

    Private Sub tmrCounterReady_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrCounterReady.Tick
        tmrCounterReady.Enabled = False
        If Val(txtPrintPending.Text) > 0 Then
            listBoxPrinterGroupLogAdd("Ada Print Pending,tetapi weighing di luar spesifikasi")
        End If
    End Sub

    Private Sub Label13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblGroupPrintPending.Click

    End Sub

    Private Sub timerWieghingIsStill_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerWieghingIsStill.Tick
        timerNoActivity.Enabled = False
        timerWieghingIsStill.Enabled = False
        WeighingIsStillIntervalRemaining = WeighingIsStillInterval
        If scanner1_lock = False Then
            If (Val(txtPrintPending.Text) > 0) And WeighingStillSet = False And txtGroupRemain.Text <> "NaN" Then
                scanner1_lock = True
                Dim oldArticleNumber As String = txtArticleNumber.Text
                Dim oldReffNumber As String = txtReference.Text
                'Dim Konf = MsgBox("Nilai weighing tetap, Tetapi ada " & txtPrintPending.Text & "  Group Print Pending." & Chr(13) & Chr(10) & "Pastikan weighing sudah terkalibrasi and Pastikan jumlah Individual Box sesuai!" & Chr(13) & Chr(10) & Chr(13) & Chr(10) & "Apakah ingin print Group Label? ", MsgBoxStyle.YesNo)
                Dim Konf = DialogBox("Weighing Idle", "Nilai weighing tidak berubah selama lebih dari :" & (WeighingIsStillInterval / 1000).ToString & " detik", "Print Group Label" & Chr(13) & Chr(10) & "Untuk Print Pending", "Abaikan", "")
                If Konf = MsgBoxResult.Yes And Val(txtPrintPending.Text) Then
                    listBoxPrinterGroupLogAdd("Nilai weighing tetap, Tetapi ada " & txtPrintPending.Text & "  Group Print Pending." & Chr(13) & Chr(10) & "Pastikan weighing sudah terkalibrasi and Pastikan jumlah Individual Box sesuai!")
                    WeighingStillSet = True
                    Printing_Label_Group()
                    writePrintGroupLog("Complete Group Print by Weighing Idle Time Out", oldArticleNumber, oldReffNumber, Val(txtGroupRemain.Text), Val(txtQtyPass.Text), Val(txtQtyFail.Text), Val(txtgroup.Text))
                End If
                scanner1_lock = False
            End If
        End If
        'TimerCounterStillIntervalRemaining = TimerCounterStillInterval
        timerNoActivity.Enabled = True
    End Sub

    Private Sub tmrWeighingStabilitazion_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrWeighingStabilitazion.Tick
        tmrWeighingStabilitazion.Enabled = False
        listBoxPrinterGroupLogAdd("Weighing Box sudah masuk Spesifikasi tetapi belum ada printing pending dari Counter." & Chr(13) & Chr(10) & "Pastikan Jumlah individual box sesuai  dan weighing reference ter-kalibrasi!!!")
    End Sub

    Private Sub txtPrintPending_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtPrintPending.TextChanged
        If Val(txtPrintPending.Text) >= 0 Then
            WriteINI(Settings.ConfigFile, "PRINTER", "GroupPending", txtPrintPending.Text)
        End If
        If Val(txtPrintPending.Text) > 0 Then
            timerWieghingIsStill.Enabled = False
            'WeighingIsStillIntervalRemaining = WeighingIsStillInterval
            tmrCounterReady.Enabled = True
            timerWieghingIsStill.Enabled = True
        Else
            txtMsg.Text = "-"
        End If
    End Sub

    Function DialogBox(ByVal Title As String, ByVal message As String, ByVal Button1 As String, ByVal Button2 As String, ByVal Button3 As String) As DialogResult
        Dim d As DialogForm
        d = New DialogForm
        d.DialogTittle = Title
        d.Message = message
        d.Button1Text = Button1
        d.Button2Text = Button2
        d.Button3Text = Button3

        Dim r = d.ShowDialog()
        d.Dispose()
        Return r

    End Function

    Private Sub TimerPulse1s_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerPulse1s.Tick
        If timerNoActivity.Enabled = True Then
            TimerCounterStillIntervalRemaining -= 1000
            If LabelCounterIdleOn.BackColor = Color.White Then
                LabelCounterIdleOn.BackColor = Color.Blue
            Else
                LabelCounterIdleOn.BackColor = Color.White
            End If
        Else
            LabelCounterIdleOn.BackColor = Color.White
            TimerCounterStillIntervalRemaining = TimerCounterStillInterval
        End If

        If timerWieghingIsStill.Enabled = True Then
            WeighingIsStillIntervalRemaining -= 1000

            If LabelWeighingTimerOn.BackColor = Color.White Then
                LabelWeighingTimerOn.BackColor = Color.Blue
            Else
                LabelWeighingTimerOn.BackColor = Color.White
            End If
        Else
            LabelWeighingTimerOn.BackColor = Color.White
            WeighingIsStillIntervalRemaining = WeighingIsStillInterval
        End If
        LabelWeighingTimerTick.Text = WeighingIsStillIntervalRemaining / 1000
        LabelIdleTimerTick.Text = TimerCounterStillIntervalRemaining / 1000
    End Sub

    Private Sub Button8_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        showfrm = "li"
        If (Li = False) Then
            frmLogin.ShowDialog()
        End If
        If (Li = True) Then
            IncQTY()
            TimerPassword.Enabled = False
            TimerPassword.Enabled = True
        End If

    End Sub

    Function writePrintGroupLog(ByVal triggeredBy As String, ByVal ArticleNumber As String, ByVal referenceNumber As String, ByVal RemainingQty As Integer, ByVal QtyPass As Integer, ByVal QtyFail As Integer, ByVal GroupSize As Integer) As Boolean
        Dim dateToday As Date = Date.Today
        Dim writeHead As Boolean = False
        Dim namaFile As String = dateToday.ToString("yyyyMMdd")
        Dim f As FileInfo = New FileInfo(Settings.Folder.Main & "\DataLog\" & namaFile & "_FrontLine3.log.csv")

        Try
            If Not f.Exists Then
                Dim y As FileStream = f.Create()
                y.Close()
                writeHead = True
            End If

            Dim s As StreamWriter = f.AppendText()
            If writeHead = True Then
                s.WriteLine("Date, Message, Article Number, Reference Number, Remaining Qty, Qty Pass, Qty Fail, Group Size")
            End If
            s.WriteLine(Date.Now.ToString("yyyy-MMM-dd H:mm:ss") & "," & triggeredBy & "," & ArticleNumber & "," & referenceNumber & "," & RemainingQty.ToString & "," & QtyPass.ToString & "," & QtyFail.ToString & "," & Val(txtgroup.Text))
            s.Flush()
            s.Close()

        Catch x As Exception
        End Try


    End Function

    Private Sub TimerPassword_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerPassword.Tick
        TimerPassword.Enabled = False
        Li = False
    End Sub

    Private Sub TimerScan2TO_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerScan2TO.Tick
        TimerScan2TO.Enabled = False
    End Sub
End Class




