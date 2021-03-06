'Code Starts here ....
'Import Systems which we are gonna use in our code
Imports System
Imports System.ComponentModel
Imports System.Threading
Imports System.IO.Ports


'frmMain is the name of our form ....
'Here starts our main form code .....
Public Class frmMain

    Const TAB_PAGE0 = 0
    Const TAB_PAGE1 = 1
    Const REG_READ_MODE = 1
    Const REG_WRITE_MODE = 2
    Const FW_TEST_NO = 0
    Const FW_TEST_WAITING = 1
    Const FW_TEST_NG = 2
    Const FW_TEST_PASS = 3

    Dim myPort As Array

    Private Property fileSaveName As Object

    Delegate Sub SetTextCallback(ByVal [text] As String)

    Dim act As Integer
    Dim AUTO_SEND As Integer = False
    Dim Gbuff() As Byte
    Dim strPort As String
    Dim CMD_Action As Boolean = False
    Dim REGValueChenage As Boolean = False

    'Dump 設定
    Dim Dump_Start, Dump_End, Dump_Loop As Integer

    'WriteREG 設定
    Dim WriteREG_Start, WriteREG_End, WriteREG_Loop As Integer
    Dim RW_REG_Action As Integer = 0
    Dim strREGStopTime As String = ""
    Dim strREGValueTemp As String = "00"

    'FW TEST FLAF
    Dim FW_ACTION As Integer = 0

    Dim strWorKCOMPort As String = ""

    Dim btnINTFlag As Boolean = False

    'TP2824 ACCESS set
    Dim blnTP2824ACCESS As Boolean = True

    'Select REGISTER
    Dim strSelectRegister As String = ""

    'Close FormFlag

    'Page Load Code Starts Here....
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            myPort = IO.Ports.SerialPort.GetPortNames()
            ' cmbBaud.Items.Add(9600)
            ' cmbBaud.Items.Add(19200)
            'cmbBaud.Items.Add(38400)
            'cmbBaud.Items.Add(57600)
            cmbBaud.Items.Add(115200)

            For i = 0 To UBound(myPort)
                cmbPort.Items.Add(myPort(i))
            Next
        Catch ex As Exception
            MsgBox("無任何COM PORT可使用")
            cmbPort.Enabled = False
            btnConnect.Enabled = False
        End Try

        'Call GetSerialPortNames()

        cmbPort.Text = cmbPort.Items.Item(0)
        cmbBaud.Text = cmbBaud.Items.Item(0)
        btnDisconnect.Enabled = False

        GroupBox2.Enabled = False
        btnGroupBox.Enabled = True
        GroupBox3.Enabled = False
        GroupBox1.Enabled = False
        TabControl1.Enabled = False

        btnGroupBox.Enabled = False

        SerialPort1.BaudRate = "115200"
        Timer1.Enabled = False

        Button8.Enabled = False
        Label4.Visible = False

        Timer3.Enabled = True

        Timer2.Interval = 1000 '設Timer2的時間間隔為1000毫秒，也就是1秒
        Timer2.Enabled = True '啟動Timer2

        'TabControl1.SelectedIndex = 0
        ComboBox6.SelectedIndex = ComboBox2.SelectedIndex

        BootStartup()

        ToolStripStatusLabel1.Text = "工作目錄：" & Application.StartupPath

        'Label37.Text = TimeOfDay.ToString("tt h:mm:ss ")
        'Label37.Text = TimeOfDay.AddSeconds(12).ToString("mm:ss")

        ' Me.REG00.ContextMenuStrip = ContextMenuStrip1

        Install_REG_POPMENU()

        Form2.Close()
        Form3.Close()

        If TabControl1.SelectedIndex = 1 Then
            GroupBox4.Enabled = True
        Else
            GroupBox4.Enabled = False
        End If
      

    End Sub
    Sub BootStartup()

        Dim REG() As TextBox = { _
        REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
        REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
        REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
        REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
        REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
        REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
        REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
        REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
        REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
        REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
        REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
        REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
        REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
        REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
        REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
        REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        Try
            Dim FileNum As Integer
            Dim strTemp As String
            FileNum = FreeFile()
            FileOpen(FileNum, My.Computer.FileSystem.CurrentDirectory() & "\RS232.ini", OpenMode.Input)

            Do Until EOF(FileNum)
                strTemp = LineInput(FileNum)

                If InStr(1, strTemp, "COMPORTSAVE=True") Then
                    COMPORTSAVE.Checked = True
                ElseIf InStr(1, strTemp, "CheckBox1=True") Then
                    CheckBox1.Checked = True
                ElseIf InStr(1, strTemp, "REG") Then
                    If InStr(1, strTemp, "Color.Yellow") Then
                        REG(Integer.Parse(Val("&H" + Mid(strTemp, 4, 2) + "&"))).BackColor = Color.Yellow
                    ElseIf InStr(1, strTemp, "Color.Pink") Then
                        REG(Integer.Parse(Val("&H" + Mid(strTemp, 4, 2) + "&"))).BackColor = Color.Pink
                    ElseIf InStr(1, strTemp, "Color.LightBlue") Then
                        REG(Integer.Parse(Val("&H" + Mid(strTemp, 4, 2) + "&"))).BackColor = Color.LightBlue                    
                    End If
                ElseIf InStr(1, strTemp, "COM") And COMPORTSAVE.Checked = True Then
                    strWorKCOMPort = strTemp
                    UART_CONNECT_INIT()
                ElseIf InStr(1, strTemp, "TabControl1.SelectedIndex") Then
                    TabControl1.SelectedIndex = Conversion.Val(Mid(strTemp, 27, 1))
                End If
            Loop
            FileClose(FileNum)
        Catch ex As Exception

        End Try
    End Sub
    Sub Install_REG_POPMENU()
        Dim REG() As TextBox = { _
     REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
     REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
     REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
     REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
     REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
     REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
     REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
     REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
     REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
     REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
     REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
     REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
     REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
     REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
     REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
     REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        For index As Integer = 1 To 255
            REG(index).ContextMenuStrip = ContextMenuStrip1
        Next

    End Sub

    Function ReceiveSerialData() As String
        ' Receive strings from a serial port.
        Dim returnStr As String = ""

        Dim com1 As IO.Ports.SerialPort = Nothing
        Try
            ' com1 = My.Computer.Ports.OpenSerialPort("COM21")
            com1 = SerialPort1
            com1.ReadTimeout = 10000
            Do
                Dim Incoming As String = com1.ReadLine()
                If Incoming Is Nothing Then
                    Exit Do
                Else
                    returnStr &= Incoming & vbCrLf
                End If
            Loop
        Catch ex As TimeoutException
            returnStr = "Error: Serial Port read timed out."
        Finally
            If com1 IsNot Nothing Then com1.Close()
        End Try

        Return returnStr
    End Function
    Sub GetSerialPortNames()
        ' Show all available COM ports.
        For Each sp As String In My.Computer.Ports.SerialPortNames
            cmbPort.Items.Add(sp)
        Next
    End Sub
    Private Declare Function timeGetTime Lib "winmm.dll" () As Long

    'Sub Delay(ByVal DelayTime As Long) '单位是毫秒
    '    DelayTime = DelayTime + timeGetTime
    '    Do While timeGetTime < DelayTime
    '        'DoEvents()
    '    Loop
    'End Sub
    'Page Load Code Ends Here ....

    'Connect Button Code Starts Here ....
    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click

        SerialPort1.PortName = cmbPort.Text
        SerialPort1.BaudRate = cmbBaud.Text
        SerialPort1.Parity = IO.Ports.Parity.None
        SerialPort1.StopBits = IO.Ports.StopBits.One
        SerialPort1.DataBits = 8

        Me.SerialPort1.ReadBufferSize = 4096
        Me.SerialPort1.ReadTimeout = -1
        'me.serialport1.receivedbytesthreshold = 1
        Me.SerialPort1.RtsEnable = Enabled
        Me.SerialPort1.WriteBufferSize = 2048
        Me.SerialPort1.WriteTimeout = -1
        Me.SerialPort1.DtrEnable = Enabled

        ' uart_connect_init()

        Try
            SerialPort1.Open()

            btnConnect.Enabled = False
            btnDisconnect.Enabled = True

            GroupBox2.Enabled = True
            btnGroupBox.Enabled = True
            GroupBox3.Enabled = True
            GroupBox1.Enabled = True
            TabControl1.Enabled = True
            Button8.Enabled = True
            Label4.Visible = True

            cmbPort.Enabled = False

            Timer2.Interval = 1000 '設timer2的時間間隔為1000毫秒，也就是1秒
            Timer2.Enabled = True '啟動timer2

            Timer3.Interval = 500 '設timer5的時間間隔為500毫秒，也就是0.5秒
            Timer3.Enabled = True '啟動timer2

            cmbBaud.Enabled = False


            Button14.Enabled = True
            Button15.Enabled = True
            TabControl1.Enabled = True

            strWorKCOMPort = cmbPort.Text


            If TabControl1.SelectedIndex = 1 Then
                GroupBox4.Enabled = True
            Else
                GroupBox4.Enabled = False
            End If

        Catch ex As Exception
            MsgBox(cmbPort.Text & " 有問題無法使用")
        End Try


    End Sub
    Sub UART_CONNECT_INIT()

        If strWorKCOMPort <> "" Then
            SerialPort1.PortName = strWorKCOMPort
            cmbPort.Text = strWorKCOMPort
            'Else
            '    SerialPort1.PortName = cmbPort.Text
            '    strWorKCOMPort = cmbPort.Text
        End If

        SerialPort1.BaudRate = cmbBaud.Text
        SerialPort1.Parity = IO.Ports.Parity.None
        SerialPort1.StopBits = IO.Ports.StopBits.One
        SerialPort1.DataBits = 8

        Me.SerialPort1.ReadBufferSize = 4096
        Me.SerialPort1.ReadTimeout = -1
        'Me.SerialPort1.ReceivedBytesThreshold = 1
        Me.SerialPort1.RtsEnable = Enabled
        Me.SerialPort1.WriteBufferSize = 2048
        Me.SerialPort1.WriteTimeout = -1
        Me.SerialPort1.DtrEnable = Enabled

        Try
            SerialPort1.Open()

            btnConnect.Enabled = False
            btnDisconnect.Enabled = True

            GroupBox2.Enabled = True
            btnGroupBox.Enabled = True
            GroupBox3.Enabled = True
            GroupBox1.Enabled = True
            TabControl1.Enabled = True
            Button8.Enabled = True
            Label4.Visible = True

            cmbPort.Enabled = False

            Timer2.Interval = 1000 '設Timer2的時間間隔為1000毫秒，也就是1秒
            Timer2.Enabled = True '啟動Timer2

            Timer3.Interval = 500 '設Timer5的時間間隔為500毫秒，也就是0.5秒
            Timer3.Enabled = True '啟動Timer2

            cmbBaud.Enabled = False


            Button14.Enabled = True
            Button15.Enabled = True
            TabControl1.Enabled = True



        Catch ex As Exception
            If strWorKCOMPort <> "" Then
                MsgBox(strWorKCOMPort & " 有問題無法使用")
                strWorKCOMPort = ""

            Else
                MsgBox(cmbPort.Text & " 有問題無法使用")

            End If
            cmbPort.Text = cmbPort.Items(0)
        End Try
    End Sub
    'Connect Button Code Ends Here ....

    'Disconnect Button Code Starts Here ....
    Private Sub btnDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDisconnect.Click

        If RW_REG_Action Or Timer1.Enabled = True Then
            PRINT("COM PORT 使 用 中 無 法 中 斷 !!")
            ' Delay(1)  '暫停1秒
            btnINTFlag = True
            Dump_Loop = 255
            WriteREG_Loop = 255
            Timer1.Enabled = False
            Exit Sub
        End If

        Try

            SerialPort1.Close()

            btnConnect.Enabled = True
            btnDisconnect.Enabled = False

            GroupBox2.Enabled = False
            btnGroupBox.Enabled = False
            GroupBox3.Enabled = False
            GroupBox1.Enabled = False
            TabControl1.Enabled = False
            Button8.Enabled = False
            Label4.Visible = False

            Timer2.Enabled = False
            Timer1.Enabled = False
            Timer3.Enabled = False
            ComboBox1.SelectedIndex = 0
            Button3.Text = "開始"

            txtTransmit.Text = ""
            rtbReceived.Text = ""
            DEBUGTextBox1.Text = ""
            'TextBox1.Text = "MENU"

            cmbPort.Enabled = True
            cmbBaud.Enabled = True

            Button3.BackColor = Color.Empty

            Timer5.Enabled = False
            RW_REG_Action = 0
            Timer6.Stop()
            Timer6.Enabled = False
            'Timer6.Stop()
            'Timer6.Start()
            '  btnREGGroup.Enabled = True

            GroupBox4.Enabled = False
        Catch ex As Exception
            MsgBox(cmbPort.Text & " 有問題無法關閉")
        End Try


    End Sub
    'Disconnect Button Code Ends Here ....

    'Send Button Code Starts Here ....
    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click

        Dim strbuff As String = ""

        strbuff = txtTransmit.Text

        Me.DEBUGTextBox1.Text &= txtTransmit.Text

        'buff = HexStr2ByteArray(strbuff)
        ' buff = strbuff
        SerialPort1.Write(DEBUGTextBox1.Text)
        'Me.DEBUGTextBox1.Text &= ByteArrayToHex(buff)
        'DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        'DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        'DEBUGTextBox1.Focus()
        '        txtTransmit.Text = ByteArrayToHex(buff)
        txtTransmit.Text = DEBUGTextBox1.Text

        'DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        'DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        'DEBUGTextBox1.Focus()
    End Sub
    'Send Button Code Ends Here ....
    ' Private Function UnicodeBytesToString(
    ' ByVal bytes() As Byte) As String

    '    Return System.Text.Encoding.Unicode.GetString(bytes)
    ' End Function

    'Serial Port Receiving Code Starts Here ....
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        'ReceivedText(SerialPort1.ReadExisting())
        Dim buff() As Byte
        Dim s As String = ""
        Dim fw As String = ""
        Dim strbuff As String = ""

        '  Dim myArr() : myArr = Array(84, 104, 105, 115, 32, _
        ' 105, 115, 32, 97, 32, 116, 101, 115, 116, 33)

        If DirectCast(sender, SerialPort).BytesToRead > 0 Then
            ReDim buff(SerialPort1.BytesToRead - 1)

            SerialPort1.Read(buff, 0, buff.Length)

            s = ByteArrayToStr(buff)

            ReceivedText(s)

        End If
        'End If

    End Sub
    'Serial Port Receiving Code Ends Here ....
    'Serial Port Receiving Code(Invoke) Starts Here ....
    Private Sub ReceivedText(ByVal [text] As String)
        Dim Str As String = ""
        Dim Str2 As String = ""
        Dim Str_number As Integer
        Dim Data As Integer = 0
        Dim strCRC As String = ""
        Dim intValue2 As Integer = 0

        If Me.rtbReceived.InvokeRequired Then
            Dim x As New SetTextCallback(AddressOf ReceivedText)
            Me.Invoke(x, New Object() {(text)})
        Else
            'buff = HexStr2ByteArray([text])
            Str = [text]

            Me.rtbReceived.Text &= [text]

            Me.RichTextBox1.Text &= [text]
            Me.rtbReceived.SelectionStart = Me.rtbReceived.Text.Length   '文本的选取长度
            Me.rtbReceived.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
            ' Me.rtbReceived.Focus()
            Me.rtbReceived.SelectionAlignment = 0 ' 設定顯示罝中

            'Alignment
            'Me.rtbReceived.Select(Me.rtbReceived.Text.Length, 0)

            'Me.DEBUGTextBox1.Text &= vbNewLine
            Str_number = InStr(1, RichTextBox1.Text, "STX")

            If Str_number > 0 Then
                '    Me.DEBUGTextBox1.Text &= "STX ADDR=" & Str_number.ToString() & " "

                If (InStr(1, RichTextBox1.Text, "ETX") = (Str_number + 11)) Then
                    Me.DEBUGTextBox1.Text &= "R " & Mid(RichTextBox1.Text, Str_number + 3, 2) & " " & Mid(RichTextBox1.Text, Str_number + 3 + 2, 2) & vbNewLine

                    'CRC check
                    intValue2 = Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3, 2)) _
                        Xor Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3 + 2, 2)) _
                    Xor Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3 + 2 + 2, 2))

                    If intValue2 <> Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 9, 2)) Then
                        'PRINT("CMD CRC OK = " & Hex(intValue2))
                        'Else
                        PRINT("CMD CRC NG = " & Hex(intValue2))
                        RichTextBox1.Text = ""
                        Exit Sub
                    End If

                    'DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
                    'DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
                    ' Me.DEBUGTextBox1.SelectionAlignment = 0
                    'DEBUGTextBox1.Focus()

                    TextBox5.Text = Mid(RichTextBox1.Text, Str_number + 3 + 2, 2)

                    GETREGDATA(Mid(RichTextBox1.Text, Str_number + 3, 2), TextBox5.Text)

                    If RW_REG_Action = REG_READ_MODE Then
                        If Dump_Loop < Dump_End Then
                            Dump_Loop += 1
                            TextBox2.Text = Hex(Dump_Loop)
                            SendCMD(21)
                        Else

                            RW_REG_Action = 0
                            Button14.Enabled = True
                            Button15.Enabled = True
                            btnREGGroup.Enabled = True

                            PRINT("READ ALL OK!!")


                            RW_REG_Action = 0
                            strREGStopTime = ""
                            Label37.Text = "_"
                            btnREGGroup.Enabled = True

                            'Timer6.Interval = 15000
                            'Timer6.Stop()
                            'Timer6.Enabled = False
                            'Timer6.Stop()
                            'Timer6.Start()
                        End If
                    End If

                    If RW_REG_Action = REG_WRITE_MODE Then

                        If WriteREG_Loop < WriteREG_End Then
                            WriteREG_Loop += 1
                            WRITEREGDATA("REG" & Hex(WriteREG_Loop))

                        Else

                            RW_REG_Action = 0
                            Button14.Enabled = True
                            Button15.Enabled = True
                            btnREGGroup.Enabled = True
                            PRINT("WRITE ALL OK!!")

                            ' Timer6.Interval = 15000
                            'Timer6.Stop()
                            'Timer6.Enabled = False
                            'Timer6.Stop()
                            'Timer6.Start()

                            RW_REG_Action = 0
                            strREGStopTime = ""
                            Label37.Text = "_"
                            btnREGGroup.Enabled = True

                        End If

                    End If

                    TextBox4.Text = TextBox5.Text

                    RichTextBox1.Text = ""
                Else
                    'PRINT("GOT STX ,ETX =NG" & " ,LEN=" & Len(RichTextBox1.Text))
                    'RichTextBox1.Text = Mid(RichTextBox1.Text, Str_number + 3, Len(RichTextBox1.Text) - 3)
                    If Len(RichTextBox1.Text) > 50 Then
                        RichTextBox1.Text = ""
                    End If

                End If
            Else
                If Len(RichTextBox1.Text) > 50 Then
                    RichTextBox1.Text = ""
                End If
            End If

            If InStr(1, RichTextBox1.Text, "FW") Then
                FW_ACTION = FW_TEST_PASS
                RichTextBox1.Text = ""
            End If

            CMD_Action = False
        End If


    End Sub
    'Private Sub ReceivedText2(ByVal Str As String)
    '    If Me.DEBUGTextBox1.InvokeRequired Then
    '        Dim x As New SetTextCallback(AddressOf ReceivedText2)
    '        Me.Invoke(x, New Object() {(Text)})
    '    Else
    '        DEBUGTextBox1.Text &= Str
    '        DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
    '        DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
    '        'DEBUGTextBox1.Focus()
    '    End If
    'End Sub
    Private Sub PRINT(ByVal str As String)

        DEBUGTextBox1.Text &= str & vbNewLine
        'DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        'DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        ''DEBUGTextBox1.Focus()
        'DEBUGTextBox1.SelectionAlignment = 0

    End Sub
    Private Sub GETREGDATA(ByVal Addr As String, ByVal Data As String)

        Dim REG() As TextBox = { _
       REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
       REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
       REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
       REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
       REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
       REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
       REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
       REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
       REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
       REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
       REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
       REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
       REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
       REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
       REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
       REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        'Try
        If REG(Integer.Parse(Val("&H" + (Addr) + "&"))).Text <> Data Then

            REG(Integer.Parse(Val("&H" + (Addr) + "&"))).Text = Data
            REG(Integer.Parse(Val("&H" + (Addr) + "&"))).ForeColor = Color.Red
        Else

            If REGValueChenage = True Then
                REG(Integer.Parse(Val("&H" + (Addr) + "&"))).ForeColor = Color.Purple
                REGValueChenage = False
            Else
                REG(Integer.Parse(Val("&H" + (Addr) + "&"))).ForeColor = Color.Empty
            End If

        End If

        '你的程式碼
        'Catch ex As Exception
        '出了錯該怎麼辦？
        'End Try
        'Val("&H" + (Addr) + "&")
    End Sub
    Private Sub WRITEREGDATA(ByVal Str As String)
        '   Private Sub WRITEREGDATA(ByVal REG As Object)
        Dim REG() As TextBox = { _
       REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
       REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
       REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
       REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
       REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
       REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
       REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
       REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
       REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
       REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
       REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
       REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
       REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
       REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
       REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
       REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        TextBox3.Text = Mid(Str, 4, 2)
        TextBox2.Text = TextBox3.Text
        TextBox4.Text = REG(Integer.Parse(Val("&H" + (Mid(Str, 4, 2)) + "&"))).Text
        DEBUGTextBox1.Text &= "W " & TextBox3.Text & " " & TextBox4.Text & " "

        SendCMD(22)

    End Sub
    'Serial Port Receiving Code(Invoke) Ends Here ....
    'Com Port Change Warning Code Starts Here ....
    Private Sub cmbPort_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbPort.SelectedIndexChanged
        If SerialPort1.IsOpen = False Then
            SerialPort1.PortName = cmbPort.Text
        Else
            MsgBox("Valid only if port is Closed", vbCritical)
        End If
    End Sub
    'Com Port Change Warning Code Ends Here ....
    'Baud Rate Change Warning Code Starts Here ....
    Private Sub cmbBaud_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbBaud.SelectedIndexChanged
        If SerialPort1.IsOpen = False Then
            SerialPort1.BaudRate = cmbBaud.Text
        Else
            MsgBox("Valid only if port is Closed", vbCritical)
        End If
    End Sub
    'Baud Rate Change Warning Code Ends Here ....
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        'If AUTO_SEND== True Then
        'If CheckBox2.Checked = True Then
        SendCMD(16)
        'End If
        'End If



        ' If rtbReceived.Lines.Length > 100 Then
        'Dim temp As String = ""
        'temp = TextBox1.Text.Remove(0, TextBox1.Lines(0).Length + 2) '去除第一行和換行符號
        'rtbReceived.Text = "" '清空
        'TextBox1.AppendText(temp) '附加文字 ，其游標會自動再最後一行
        'TextBox1.Focus() '取得焦點
        'End If
    End Sub
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick

        Dim DayString, TimeString As String '用來顯示日期與時間字串變數
        'DayString = Format(Now, "Long Date") '指定DayString為時間格式為有西元年的日期
        DayString = Format(Now, "yyyy/MM/dd") '指定DayString為時間格式為有西元年的日期
        'TimeString = Format(Now, "AMPM(hh:mm:ss)") 'Format(Now, "AMPM hh:mm:ss") '指定TimeString為時間格式AMPM hh:mm:ss
        TimeString = TimeOfDay.ToString("tt h:mm:ss ")
        Label4.Text = DayString + Space(1) + TimeString '將結果Show在Label1(space(1)為空一格)

        If strREGStopTime = TimeOfDay.ToString("hh:mm:ss") And RW_REG_Action <> 0 Then
            If RW_REG_Action = REG_READ_MODE Then
                PRINT("TIME OUT REG_READ_MODE = NG")
            ElseIf RW_REG_Action = REG_WRITE_MODE Then
                PRINT("TIME OUT REG_WRITE_MODE = NG")
            End If

            RW_REG_Action = 0
            strREGStopTime = ""
            Label37.Text = "_"
            btnREGGroup.Enabled = True

            'MsgBox("暫存器讀取時間到了", vbCritical)

        End If


        If btnINTFlag = True And RW_REG_Action = 0 Then

            Try

                SerialPort1.Close()

                btnConnect.Enabled = True
                btnDisconnect.Enabled = False

                GroupBox2.Enabled = False
                btnGroupBox.Enabled = False
                GroupBox3.Enabled = False
                GroupBox1.Enabled = False
                TabControl1.Enabled = False
                Button8.Enabled = False
                Label4.Visible = False

                Timer2.Enabled = False
                Timer1.Enabled = False
                Timer3.Enabled = False
                ComboBox1.SelectedIndex = 0
                Button3.Text = "開始"

                txtTransmit.Text = ""
                rtbReceived.Text = ""
                DEBUGTextBox1.Text = ""
                'TextBox1.Text = "MENU"

                cmbPort.Enabled = True
                cmbBaud.Enabled = True

                Button3.BackColor = Color.Empty

                Timer5.Enabled = False
                RW_REG_Action = 0
                Timer6.Stop()
                Timer6.Enabled = False
                'Timer6.Stop()
                'Timer6.Start()
                '  btnREGGroup.Enabled = True

                btnINTFlag = False
            Catch ex As Exception
                MsgBox(cmbPort.Text & " 有問題無法關閉")
            End Try

        End If
    End Sub
    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        If Timer1.Enabled = True Then
            If Button3.BackColor = Color.Red Then
                Button3.BackColor = Color.Empty
            Else
                Button3.BackColor = Color.Red
            End If
        End If
    End Sub

    Private Sub rtbReceived_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtbReceived.TextChanged

        Me.rtbReceived.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        Me.rtbReceived.SelectionStart = Me.rtbReceived.Text.Length   '文本的选取长度
        'Me.rtbReceived.Focus()

        Me.rtbReceived.SelectionAlignment = 0 ' 設定顯示罝中

    End Sub
    Private Function UnicodeBytesToString(ByVal bytes() As Byte) As String
        Return System.Text.Encoding.Unicode.GetString(bytes)
    End Function
    Public Function HexStr2ByteArray(ByVal data As String) As Byte()
        Dim sendData As Byte() = New Byte((data.Length / 2) - 1) {}
        For i As Integer = 0 To (sendData.Length - 1)
            sendData(i) = CByte(Convert.ToInt32(data.Substring(i * 2, 2), 16))
        Next i
        Return sendData
    End Function

    Public Function ByteArrayToHex(ByVal comByte As Byte()) As String
        'create a new StringBuilder object
        Dim builder As New System.Text.StringBuilder(comByte.Length * 2)
        'loop through each byte in the array
        For Each data As Byte In comByte
            builder.Append(Convert.ToString(data, 16).PadLeft(2, "0"c).PadRight(2, " "c))
            'convert the byte to a string and add to the stringbuilder
        Next
        'return the converted value
        Return builder.ToString().ToUpper()
    End Function

    Private Sub btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SendCMD(0)

        '        Dim buff() As Byte '= {&H7E, &H1, &H1, &H0, &H0}
        '        Dim strbuff As String
        '        strbuff = Replace(strDVRreadyNotice, ",", "")
        '        buff = HexStr2ByteArray(strbuff)
        '        SerialPort1.Write(buff, 0, 5)
        '        Me.DEBUGTextBox1.Text &= ByteArrayToHex(buff)

        '        DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        '        DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        '        DEBUGTextBox1.Focus()

        '        txtTransmit.Text = ByteArrayToHex(buff)
    End Sub

    Public Function AsciiStringToHexString(ByVal asciiString As String) As String
        Dim ascii() As Byte = System.Text.Encoding.Default.GetBytes(asciiString)
        Dim count As Integer = ascii.Length
        Dim hexArray(count - 1) As String
        For idx As Integer = 0 To count - 1
            hexArray(idx) = ascii(idx).ToString("x2")
            'SerialPort1.Write(HexStringToByteArray(strTmp[i]), 0, 1) 
        Next
        Return String.Join(" ", hexArray)
    End Function


    Public Function HexStringToAsciiString(ByVal hexString As String) As String
        Dim array() As String = hexString.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        For idx As Integer = 0 To array.Length - 1
            array(idx) = Chr(CInt(String.Format("&h{0}", array(idx))))
        Next
        Return String.Join(String.Empty, array)
    End Function
    ' 將字串轉成byte陣列
    Shared Function StrToByteArray(ByVal str As String) As Byte()
        Dim encoding As New System.Text.ASCIIEncoding()
        Return encoding.GetBytes(str)
    End Function

    ' 將byte陣列轉成字串
    Shared Function ByteArrayToStr(ByVal bt As Byte()) As String
        Dim encoding As New System.Text.ASCIIEncoding()
        Return encoding.GetString(bt)
    End Function
    '暫停 ASecond 秒
    Private Sub Delay(ByVal ASecond As Integer)
        '讓CPU去做其它事情，畫面才不會感覺死當。
        Application.DoEvents()
        '暫停 ASecond *1000 毫秒
        System.Threading.Thread.Sleep(ASecond * 1000)

    End Sub
    'Private Sub tbxASCII_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbxASCII.TextChanged
    '  If Me.ActiveControl Is sender Then
    '  tbxHex.Text = AsciiStringToHexString(tbxASCII.Text)
    '   End If
    'End Sub

    'Private Sub tbxHex_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbxHex.TextChanged
    '     If Me.ActiveControl Is sender Then
    '          '  tbxASCII.Text = HexStringToAsciiString(tbxHex.Text)
    '       End If
    '    End Sub

    Private Sub btnCLRMessage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        ' DEBUGTextBox1.Text = ""
        ' txtTransmit.Text = ""
        ' rtbReceived.Text = ""
        Me.Close()


    End Sub
    Private Sub SendCMD(ByVal idx As String)
        '        Dim strDVRreadyNotice As String = "7E,01,01,00,00,"  'DVRreadyNotice
        '        Dim strbtnWDT As String = "7E,02,01,00,03,"  'DVRreadyNotice
        '        Dim strbtnWDT As String = "7E,02,01,00,03,"  'DVRreadyNotice

        Dim strbuff As String = ""
        If idx = 0 Then
            strbuff = Replace("7E,01,01,00,00", ",", "")   'DVRreadyNotice
        ElseIf idx = 1 Then
            strbuff = Replace("7E,02,01,00,03,", ",", "") 'DVR WDT KICK
        ElseIf idx = 2 Then
            strbuff = Replace("7E,03,01,00,02,", ",", "") 'MCU Entry sleep mode
        ElseIf idx = 3 Then
            strbuff = Replace("7E,04,01,00,05,", ",", "") 'DISTANCE_RESET
        ElseIf idx = 4 Then
            strbuff = Replace("7E,11,01,00,10,", ",", "") 'DVR reboot RESET
        ElseIf idx = 5 Then
            strbuff = Replace("7E,12,01,00,13,", ",", "") 'DVR shutdown RESET
        ElseIf idx = 6 Then
            strbuff = Replace("7E,05,08,01,05,02,06,03,07,04,08,05,", ",", "") 'Encoder deviation setting
        ElseIf idx = 7 Then
            'strbuff = Replace("7E,10,01,00,11,", ",", "") 'MCU FW VERSION
            Me.rtbReceived.Text = ""
            strbuff = "FW" + Chr(13)  'MCU FW VERSION
        ElseIf idx = 8 Then
            strbuff = Replace("7E,06,01,01,06,", ",", "") 'CAM INC 0.1V 
        ElseIf idx = 9 Then
            strbuff = Replace("7E,06,01,00,07,", ",", "") 'CAM DEC 0.1V
        ElseIf idx = 10 Then
            strbuff = Replace("7E,83,01,00,82,", ",", "") 'factory mode
        ElseIf idx = 11 Then
            strbuff = Replace("7E,30,00,30,", ",", "") 'isp mode
        ElseIf idx = 12 Then
            strbuff = Replace("7E,08,01,01,08,", ",", "") 'pwer start 1
        ElseIf idx = 13 Then
            strbuff = Replace("7E,08,01,00,09,", ",", "") 'pwer start 0
        ElseIf idx = 14 Then
            strbuff = Replace("7E,07,01,00,06,", ",", "") 'WDT Enable=0
        ElseIf idx = 15 Then
            strbuff = Replace("7E,07,01,01,07,", ",", "") 'WDT Enable=1
        ElseIf idx = 16 Then
            Me.rtbReceived.Text = ""
            'strbuff = TextBox1.Text + Chr(13)  '自動傳送資料
            strbuff = ComboBox3.Text + Chr(13)  '自動傳送資料

        ElseIf idx = 17 Then
            Me.rtbReceived.Text = ""
            strbuff = "UP" + Chr(13)  'UP KEY
        ElseIf idx = 18 Then
            Me.rtbReceived.Text = ""
            strbuff = "DOWN" + Chr(13)  'DOWN KEY
        ElseIf idx = 19 Then
            Me.rtbReceived.Text = ""
            strbuff = "SELECT" + Chr(13)  'SELECT KEY
        ElseIf idx = 20 Then
            Me.rtbReceived.Text = ""
            strbuff = "JUMP" + Chr(13)  'JUMP KEY
        ElseIf idx = 21 Then
            Me.rtbReceived.Text = ""
            TextBox5.Text = "?" : CMD_Action = True
            strbuff = "CMDR" + " " + TextBox1.Text + " " + TextBox2.Text + " " + Chr(13)  'CMD Read
            PRINT("CMDR" + " " + TextBox1.Text + " " + TextBox2.Text)
        ElseIf idx = 22 Then
            Me.rtbReceived.Text = ""
            TextBox5.Text = "?" : CMD_Action = True
            strbuff = "CMDW" + " " + TextBox1.Text + " " + _
                TextBox3.Text + " " + TextBox4.Text + Chr(13)  'CMD Write
            PRINT("CMDW" + " " + TextBox1.Text + " " + TextBox3.Text + " " + TextBox4.Text)
        ElseIf idx = 23 Then
            If blnTP2824ACCESS = False Then
                PRINT("ACCESS=1")
                strbuff = "ACCESS 1" + Chr(13) + "q" + Chr(13) '自動傳送資料
                blnTP2824ACCESS = True
                Button20.Text = "ACCESS=1"
            Else
                PRINT("ACCESS=0")
                strbuff = "ACCESS 0" + Chr(13) + "q" + Chr(13) '自動傳送資料
                blnTP2824ACCESS = False
                Button20.Text = "ACCESS=0"
            End If

        End If

        '  buff = HexStr2ByteArray(strbuff)
        'SerialPort1.Write(strbuff, 0, buff.Length)
        If SerialPort1.IsOpen = True Then
            SerialPort1.Write(strbuff)
        End If

        'Me.DEBUGTextBox1.Text &= ByteArrayToHex(buff)
        'txtTransmit.Text = ByteArrayToHex(buff)
        'txtTransmit.Text = strbuff 'ByteArrayToHex(buff)

        '   DEBUGTextBox1.Text = AsciiStringToHexString(strbuff)
        '   DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        '  DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        ' DEBUGTextBox1.Focus()

    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        'Dim buff() As Byte
        'Dim s As String = ""
        'Dim fw As String = ""
        'Dim strbuff As String = ""

        ' Try

        'If SerialPort1.BytesToRead > 0 Then
        '    ReDim buff(SerialPort1.BytesToRead - 1)

        '    SerialPort1.Read(buff, 0, buff.Length)

        '    's = ByteArrayToStr(buff)

        '    'ReceivedText(s)
        'End If
        If RW_REG_Action Then
            PRINT("COM PORT 使 用 中 延 遲 關 閉  !!")
            ' Delay(1)  '暫停1秒
            ' btnINTFlag = True
            Dump_Loop = 255
            WriteREG_Loop = 255
            'Exit Sub

            PRINT("Close Form..3")
            Delay(1)
            PRINT("Close Form..2")
            Delay(1)
            PRINT("Close Form..1")
            Delay(1)

        End If




        SerialPort1.Close()

        Timer1.Enabled = False
        Timer2.Enabled = False
        Timer3.Enabled = False
        Timer5.Enabled = False
        Timer6.Enabled = False

        ExitSaveConfiguration()

        Form2.Close()

        Form3.Close()

        Me.Close()
        'Catch ex As Exception
        '    'If RW_REG_Action Then
        '    '    PRINT("COM PORT 使 用 中 無 法 中 斷 !!")
        '    '    ' Delay(1)  '暫停1秒
        '    '    btnINTFlag = True
        '    '    Dump_Loop = 255
        '    '    WriteREG_Loop = 255
        '    '    Exit Sub
        '    'End If
        'End Try



    End Sub
    Sub ExitSaveConfiguration()

        Dim intSaveFileFlag As Boolean = False
        Dim REG() As TextBox = { _
        REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
        REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
        REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
        REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
        REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
        REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
        REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
        REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
        REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
        REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
        REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
        REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
        REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
        REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
        REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
        REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        Dim FileNum As Integer
        Dim strTemp As String
        Dim strTemp2 As String = ""
        Dim DayString, TimeString As String '用來顯示日期與時間字串變數
        FileNum = FreeFile()
        FileOpen(FileNum, My.Computer.FileSystem.CurrentDirectory() & "\RS232.ini", OpenMode.Output)
        DayString = Format(Now, "yyyy/MM/dd") '指定DayString為時間格式為有西元年的日期
        TimeString = TimeOfDay.ToString("tt h:mm:ss ")
        strTemp = "," + DayString + Space(1) + TimeString '將結果Show在Label1(space(1)為空一格)
        PrintLine(FileNum, strTemp)
        strTemp2 &= strTemp & vbNewLine

        If CheckBox1.Checked = True Then
            strTemp = CheckBox1.Name & "=True"
            PrintLine(FileNum, strTemp)
        End If

        If COMPORTSAVE.Checked = True Then
            strTemp = COMPORTSAVE.Name & "=True"
            PrintLine(FileNum, strTemp)
        End If

        If strWorKCOMPort <> "" Then
            strTemp = strWorKCOMPort
            PrintLine(FileNum, strTemp)
        End If

        Select Case (TabControl1.SelectedIndex)
            Case 0
                strTemp = "TabControl1.SelectedIndex=0"
                PrintLine(FileNum, strTemp)
            Case 1
                strTemp = "TabControl1.SelectedIndex=1"
                PrintLine(FileNum, strTemp)
            Case 2
                strTemp = "TabControl1.SelectedIndex=2"
                PrintLine(FileNum, strTemp)
        End Select

        For index As Integer = 0 To REG.Count - 1
            If REG(index).BackColor = Color.Yellow Then
                strTemp = REG(index).Name & "=" & "Color.Yellow"
                PrintLine(FileNum, strTemp)
            ElseIf REG(index).BackColor = Color.Pink Then
                strTemp = REG(index).Name & "=" & "Color.Pink"
                PrintLine(FileNum, strTemp)
            ElseIf REG(index).BackColor = Color.LightBlue Then
                strTemp = REG(index).Name & "=" & "Color.LightBlue"
                PrintLine(FileNum, strTemp)
                'ElseIf REG(index).BackColor = Color.Empty Then
                '    strTemp = REG(index).Name
                '    PrintLine(FileNum, strTemp)
            End If
            'strTemp = TextBox1.Text & "," & Mid(REG(index).Name, 4, 2) & "," & REG(index).Text
            'PrintLine(FileNum, strTemp)
            'strTemp2 &= strTemp & vbNewLine
        Next


        FileClose(FileNum)
    End Sub
    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Me.rtbReceived.Text = ""
        Label15.Text = ""
        'Me.DEBUGTextBox1.Text = ""
        'TextBox5.Text = "_"
    End Sub
    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Me.DEBUGTextBox1.Text = ""
        Label15.Text = ""
    End Sub


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        SendCMD(16)
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If Timer1.Enabled = False And ComboBox1.SelectedIndex <> 0 And ComboBox3.Text <> "" Then
            Timer1.Enabled = True
            Button3.Text = "停止"
            Button3.BackColor = Color.Red
            Button2.Text = ComboBox3.Text
        Else
            Timer1.Enabled = False
            Button3.Text = "開始"
            Button3.BackColor = Color.Empty

            If ComboBox1.SelectedIndex <> 0 Then
                Me.rtbReceived.Text = "停止自動傳送命令"
            End If
            ' ComboBox1.SelectedIndex = 0
        End If

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        'Me.rtbReceived.Text = "開始自動傳送命令"
        If ComboBox1.SelectedIndex = 0 Then
            Timer1.Enabled = False
            Button3.Text = "開始"
            Button3.BackColor = Color.Empty
        Else
            'Timer1.Enabled = True
            If ComboBox1.SelectedIndex = 1 Then
                Timer1.Interval = 1000
            ElseIf ComboBox1.SelectedIndex = 2 Then
                Timer1.Interval = 2000
            ElseIf ComboBox1.SelectedIndex = 3 Then
                Timer1.Interval = 3000
            ElseIf ComboBox1.SelectedIndex = 4 Then
                Timer1.Interval = 4000
            ElseIf ComboBox1.SelectedIndex = 5 Then
                Timer1.Interval = 5000
            ElseIf ComboBox1.SelectedIndex = 6 Then
                Timer1.Interval = 6000
            ElseIf ComboBox1.SelectedIndex = 7 Then
                Timer1.Interval = 7000
            ElseIf ComboBox1.SelectedIndex = 8 Then
                Timer1.Interval = 8000
            ElseIf ComboBox1.SelectedIndex = 9 Then
                Timer1.Interval = 9000
            ElseIf ComboBox1.SelectedIndex = 10 Then
                Timer1.Interval = 10000
            End If

        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        SendCMD(17)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        SendCMD(18)
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        SendCMD(19)
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        SendCMD(20)
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        'If CMD_Action = False Then
        TextBox3.Text = TextBox2.Text
        SendCMD(21)
        'End If
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        '  If CMD_Action = False Then
        TextBox2.Text = TextBox3.Text
        SendCMD(22)
        '   End If
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Dim intNumber As Integer = 0
        '   If CMD_Action = False Then
        intNumber = Val("&H" + TextBox4.Text + "&")
        If intNumber >= 255 Then
            intNumber = 255
            TextBox4.Text = Hex(intNumber)
        Else

            TextBox4.Text = Hex(intNumber + 1)
        End If

        SendCMD(22)
        '  End If
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Dim intNumber As Integer = 0
        ' If CMD_Action = False Then
        intNumber = Val("&H" + TextBox4.Text + "&")
        If intNumber <= 0 Then
            intNumber = 0
            TextBox4.Text = Hex(intNumber)
        Else
            TextBox4.Text = Hex(intNumber - 1)
        End If

        SendCMD(22)

        ' End If
    End Sub

    Private Sub TextBox4_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox4.KeyPress
        If Asc(e.KeyChar) = 13 Then
            'MsgBox("輸入的值是" & TextBox2.Text)
            Dim intNumber As Integer = 0

            ' If CMD_Action = False Then
            TextBox4.Text = UCase(TextBox4.Text)

            intNumber = Val("&H" + (TextBox4.Text) + "&")

            TextBox4.Text = Hex(intNumber)

            If intNumber >= 255 Then
                intNumber = 255
                TextBox4.Text = Hex(intNumber)
            ElseIf intNumber <= 0 Then
                intNumber = 0
                TextBox4.Text = Hex(intNumber)
            End If

            SendCMD(22)
            'End If

        End If
    End Sub

    Private Function Timer() As Object
        Throw New NotImplementedException
    End Function

    Private Sub DoEvents()
        Throw New NotImplementedException
    End Sub

    Private Sub btnMCUREV_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMCUREV.Click
        FW_ACTION = FW_TEST_WAITING
        Label15.Text = "WAITING"
        Label15.ForeColor = Color.Empty
        Timer5.Interval = 1000
        Timer5.Enabled = True
        SendCMD(7)
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        ' If e.KeyChar = Strings.ChrW(Keys.Return) Then
        Dim intNumber As Integer = 0
        TextBox1.Text = UCase(TextBox1.Text)

        intNumber = Val("&H" + (TextBox1.Text) + "&")

        TextBox1.Text = Hex(intNumber)

        If intNumber >= 255 Then
            intNumber = 255
            TextBox1.Text = Hex(intNumber)
        ElseIf intNumber <= 0 Then
            intNumber = 0
            TextBox1.Text = Hex(intNumber)
        End If
        '  End If
    End Sub


    Private Sub TextBox2_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress

        If Asc(e.KeyChar) = 13 Then
            Dim intNumber As Integer = 0
            TextBox2.Text = UCase(TextBox2.Text)

            intNumber = Val("&H" + (TextBox2.Text) + "&")

            TextBox2.Text = Hex(intNumber)

            If intNumber >= 255 Then
                intNumber = 255
                TextBox2.Text = Hex(intNumber)
            ElseIf intNumber <= 0 Then
                intNumber = 0
                TextBox2.Text = Hex(intNumber)
            End If

            TextBox3.Text = TextBox2.Text
            SendCMD(21)

        End If
    End Sub

    Private Sub TextBox3_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox3.KeyPress
        If Asc(e.KeyChar) = 13 Then
            Dim intNumber As Integer = 0
            TextBox3.Text = UCase(TextBox3.Text)

            intNumber = Val("&H" + (TextBox3.Text) + "&")

            TextBox3.Text = Hex(intNumber)

            If intNumber >= 255 Then
                intNumber = 255
                TextBox3.Text = Hex(intNumber)
            ElseIf intNumber <= 0 Then
                intNumber = 0
                TextBox3.Text = Hex(intNumber)
            End If
            TextBox2.Text = TextBox3.Text
            SendCMD(22)
        End If
    End Sub

    Private Sub ComboBox2_RegionChanged(sender As Object, e As EventArgs) Handles ComboBox2.RegionChanged

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged

        If ComboBox2.SelectedIndex = 0 Then
            TextBox1.Text = "40"
        ElseIf ComboBox2.SelectedIndex = 1 Then
            TextBox1.Text = "88"
        ElseIf ComboBox2.SelectedIndex = 2 Then
            TextBox1.Text = "A0"
        ElseIf ComboBox2.SelectedIndex = 3 Then
            TextBox1.Text = "12"
        End If

        ComboBox6.Text = ComboBox2.Text

    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox4.SelectedIndexChanged
        If ComboBox4.SelectedIndex = 0 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 1 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 2 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 3 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 4 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 5 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox4.SelectedIndex = 6 Then
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Else
            Me.rtbReceived.Font = New System.Drawing.Font("新細明體", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        End If
    End Sub
    Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        If ComboBox5.SelectedIndex = 0 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 1 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 2 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 3 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 4 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 5 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 16.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        ElseIf ComboBox5.SelectedIndex = 6 Then
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Else
            Me.DEBUGTextBox1.Font = New System.Drawing.Font("新細明體", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        End If
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        ' PRINT("REGStart.Text Integer=" & Convert.ToInt32(REGStart.Text, 16))
        DEBUGTextBox1.Text = ""
        Dump_Start = Convert.ToInt32(REGStart.Text, 16) 'Conversion.Val("&H" + REGStart.Text) '0
        Dump_End = Convert.ToInt32(REGEnd.Text, 16) 'Conversion.Val("&H" + REGEnd.Text) '&HFF '&  '0~255
        Dump_Loop = Dump_Start
        RW_REG_Action = REG_READ_MODE  'dump mode
        TextBox2.Text = Dump_Start 'Hex(Dump_Loop)
        SendCMD(21)
        'Button14.Enabled = False
        'Button15.Enabled = False
        btnREGGroup.Enabled = False
        'Timer6.Interval = 15000
        'Timer6.Enabled = True
        'Timer6.Start()

        Label37.Text = TimeOfDay.AddSeconds(10).ToString("hh:mm:ss")
        strREGStopTime = Label37.Text
    End Sub


    Private Sub frmMain_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        If TabControl1.Enabled = True And CheckBox1.Checked = True Then

            If UCase(e.KeyChar) = Strings.ChrW(Keys.F) Then
                SendCMD(7)
                FW_ACTION = FW_TEST_WAITING
                Label15.Text = "WAITING"
                Label15.ForeColor = Color.Empty
                Timer5.Interval = 1000
                Timer5.Enabled = True
            ElseIf UCase(e.KeyChar) = Strings.ChrW(Keys.M) Then
                SendCMD(16)
            ElseIf UCase(e.KeyChar) = Strings.ChrW(Keys.J) Then
                SendCMD(20)
            ElseIf UCase(e.KeyChar) = Strings.ChrW(Keys.S) Then
                SendCMD(19)
            ElseIf UCase(e.KeyChar) = Strings.ChrW(Keys.U) Then
                SendCMD(17)
            ElseIf UCase(e.KeyChar) = Strings.ChrW(Keys.D) Then
                SendCMD(18)
            ElseIf Asc(e.KeyChar) = 46 Then   '.
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber >= 255 Then
                    intNumber = 255
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber + 1)
                End If
                SendCMD(22)
            ElseIf Asc(e.KeyChar) = 44 Then     ',
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber <= 0 Then
                    intNumber = 0
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber - 1)
                End If
                SendCMD(22)
            ElseIf Asc(e.KeyChar) = 60 Then     '<
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber < 16 Then
                    intNumber = 0
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber - 16)
                End If
                SendCMD(22)
            ElseIf Asc(e.KeyChar) = 62 Then     '>
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber > 255 - 16 Then
                    intNumber = 255
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber + 16)
                End If
                SendCMD(22)
            End If
            If UCase(e.KeyChar) = Strings.ChrW(Keys.E) Then
                ExitAPP()
            End If
        End If

    End Sub
    Sub ExitAPP()

        ExitSaveConfiguration()

        SerialPort1.Close()
        Timer2.Enabled = False
        Timer1.Enabled = False
        Timer3.Enabled = False
        Me.Close()
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click

#Const FileModeAppend = 0
        Dim intSaveFileFlag As Boolean = False
        Dim REG() As TextBox = { _
        REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
        REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
        REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
        REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
        REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
        REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
        REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
        REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
        REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
        REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
        REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
        REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
        REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
        REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
        REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
        REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        Dim FileNum As Integer
        Dim strTemp As String
        Dim strTemp2 As String = ""
        Dim DayString, TimeString As String '用來顯示日期與時間字串變數

#If FileModeAppend = 0 Then  '新建資料存檔

        FileNum = FreeFile()
        FileOpen(FileNum, My.Computer.FileSystem.CurrentDirectory() & "\dump.txt", OpenMode.Output)
        DayString = Format(Now, "yyyy/MM/dd") '指定DayString為時間格式為有西元年的日期
        TimeString = TimeOfDay.ToString("tt h:mm:ss ")
        strTemp = "," + DayString + Space(1) + TimeString '將結果Show在Label1(space(1)為空一格)
        PrintLine(FileNum, strTemp)
        strTemp2 &= strTemp & vbNewLine
        For index As Integer = 0 To REG.Count - 1
            strTemp = TextBox1.Text & "," & Mid(REG(index).Name, 4, 2) & "," & REG(index).Text
            PrintLine(FileNum, strTemp)
            strTemp2 &= strTemp & vbNewLine
        Next
        FileClose(FileNum)

        'fileSaveName = Application.GetSaveAsFilename(fileFilter:="Text Files (*.txt), *.txt")
        'If fileSaveName <> False Then
        '    MsgBox("Save as " & fileSaveName)
        'End If

        'Dim saveFileDialog1 As New SaveFileDialog()
        SaveFileDialog1.Title = "另存新檔"
        'saveFileDialog1.Filter = "TXT Files (*.txt*)|*.txt" '"*.txt;*.rtf|*.txt;*.rtf"
        SaveFileDialog1.Filter = "文字檔案(*.txt)|*.txt|逗號分隔檔案(*.csv)|*.csv|任意檔案(*.*)|*.*"

        SaveFileDialog1.InitialDirectory = Application.StartupPath

        Dim result As DialogResult = SaveFileDialog1.ShowDialog()

        If result = Windows.Forms.DialogResult.OK Then

            Try
                'myFile = My.Computer.FileSystem.CurrentDirectory() & "\dump.txt"
                'My.Computer.FileSystem.WriteAllText(saveFileDialog1.FileName, "123 test", True)
                My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, strTemp2, False)
                'rtbReceived.SaveFile(myFile, RichTextBoxStreamType.RichNoOleObjs)

                'Me.Text = myFile
                intSaveFileFlag = True

                ToolStripStatusLabel1.Text = "已儲存檔案：" & SaveFileDialog1.FileName
            Catch ex As Exception
                ' MsgBox("資料檔案未存檔")
            End Try

        End If
#Else    '增加資料存檔
        DayString = Format(Now, "yyyy/MM/dd") '指定DayString為時間格式為有西元年的日期
        TimeString = TimeOfDay.ToString("tt h:mm:ss ")

        If Mid(TimeString, 1, 2) = "上午" Then
            TimeString = "AM" & Mid(TimeString, 3, 9)
        ElseIf Mid(TimeString, 1, 2) = "下午" Then
            TimeString = "FM" & Mid(TimeString, 3, 9)
        End If

        strTemp = "," + DayString + Space(1) + TimeString & vbNewLine '將結果Show在Label1(space(1)為空一格)
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.CurrentDirectory() & "\dump.txt", strTemp, True)

        For index As Integer = 0 To REG.Count - 1
            strTemp = TextBox1.Text & "," & Mid(REG(index).Name, 4, 2) & "," & REG(index).Text & vbNewLine
            My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.CurrentDirectory() & "\dump.txt", strTemp, True)
        Next
#End If
        'DEBUGTextBox1.Text &= "存檔完畢!" & vbNewLine
#If FileModeAppend = False Then
        Try
            If intSaveFileFlag = False Then
                MsgBox("暫存器資料未存檔！", vbCritical)
            Else

                MsgBox("暫存器資料存檔完畢！", vbInformation)
            End If
        Catch ex As Exception

        End Try

#Else
        MsgBox("增加資料存檔完畢！")
#End If


    End Sub
    Sub REGKeyCheck(ByVal Index As Integer)
        Dim REG() As TextBox = { _
               REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
               REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
               REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
               REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
               REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
               REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
               REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
               REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
               REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
               REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
               REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
               REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
               REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
               REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
               REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
               REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        REG(Index).Text = UCase(REG(Index).Text)
        REG(Index).ForeColor = Color.Blue
        TextBox3.Text = Hex(Index)
        TextBox2.Text = TextBox3.Text
        TextBox4.Text = REG(Index).Text
        REGValueChenage = True
        SendCMD(22)


        ' DEBUGTextBox1.Text &= REG(Index).Name & "W=" & REG(Index).Text & vbNewLine



    End Sub

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        Dim REG() As TextBox = { _
                      REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
                      REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
                      REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
                      REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
                      REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
                      REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
                      REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
                      REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
                      REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
                      REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
                      REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
                      REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
                      REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
                      REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
                      REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
                      REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        ' 例：從 Test.txt 檔案中讀取資料
        Dim FileNum As Integer
        Dim strTemp As String = ""
        Dim strTemp2 As String = ""
        Dim I2CAddr As String = ""
        'FileNum = FreeFile()
        'FileOpen(FileNum, My.Computer.FileSystem.CurrentDirectory() & "\dump.txt", OpenMode.Input)
        ''Do Until EOF(FileNum)
        'strTemp &= LineInput(FileNum) & vbNewLine
        ''Loop
        'DEBUGTextBox1.Text &= strTemp
        'FileClose(FileNum)

        ' Dim openFileDialog1 As New OpenFileDialog()
        Dim intStringNumber As Integer = 1

        OpenFileDialog1.Title = "開啟檔案"
        'OpenFileDialog1.Filter = "TXT Files (*.txt*)|*.txt" '"*.txt;*.rtf|*.txt;*.rtf"
        OpenFileDialog1.Filter = "文字檔案(*.txt)|*.txt|逗號分隔檔案(*.csv)|*.csv|任意檔案(*.*)|*.*"
        ' OpenFileDialog1.ShowDialog()
        OpenFileDialog1.InitialDirectory = Application.StartupPath

        Dim result As DialogResult = OpenFileDialog1.ShowDialog()

        If result = Windows.Forms.DialogResult.OK Then

            ToolStripStatusLabel1.Text = "開啟檔案：" & OpenFileDialog1.FileName

            '   PRINT("open result=" & result & "," & Windows.Forms.DialogResult.OK & "," & OpenFileDialog1.FileName)
            Try

                'myFile = My.Computer.FileSystem.CurrentDirectory() & "\dump.txt"
                'My.Computer.FileSystem.WriteAllText(saveFileDialog1.FileName, "123 test", True)
                strTemp2 = My.Computer.FileSystem.ReadAllText(OpenFileDialog1.FileName)
                'rtbReceived.SaveFile(myFile, RichTextBoxStreamType.RichNoOleObjs)

                FileNum = FreeFile()
                FileOpen(FileNum, OpenFileDialog1.FileName, OpenMode.Input)


                Do Until EOF(FileNum)


                    strTemp = LineInput(FileNum) '& vbNewLine
                    ' intStringNumber = InStr(1, strTemp, TextBox1.Text)
                    I2CAddr = Mid(strTemp, 1, 2)
                    intStringNumber = 1
                    Select Case I2CAddr
                        Case "40"
                            TextBox1.Text = I2CAddr
                            ComboBox2.SelectedIndex = 0
                            ComboBox6.SelectedIndex = ComboBox2.SelectedIndex
                        Case "88"
                            TextBox1.Text = I2CAddr
                            ComboBox2.SelectedIndex = 1
                            ComboBox6.SelectedIndex = ComboBox2.SelectedIndex
                        Case "A0"
                            TextBox1.Text = I2CAddr
                            ComboBox2.SelectedIndex = 2
                            ComboBox6.SelectedIndex = ComboBox2.SelectedIndex
                        Case "12"
                            TextBox1.Text = I2CAddr
                            ComboBox2.SelectedIndex = 3
                            ComboBox6.SelectedIndex = ComboBox2.SelectedIndex
                        Case Else
                            intStringNumber = 0
                    End Select



                    If intStringNumber Then

                        If REG(Integer.Parse(Val("&H" + Mid(strTemp, 1 + 3, 2) + "&"))).Text <> Mid(strTemp, 1 + 6, 2) Then
                            REG(Integer.Parse(Val("&H" + Mid(strTemp, 1 + 3, 2) + "&"))).ForeColor = Color.Red
                        Else
                            REG(Integer.Parse(Val("&H" + Mid(strTemp, 1 + 3, 2) + "&"))).ForeColor = Color.Empty
                        End If

                        REG(Integer.Parse(Val("&H" + Mid(strTemp, 1 + 3, 2) + "&"))).Text = Mid(strTemp, 1 + 6, 2)

                        'DEBUGTextBox1.Text &= Mid(strTemp, 1 + 3, 2) & " " & Mid(strTemp, 1 + 6, 2) & vbNewLine
                        DEBUGTextBox1.Text &= strTemp & vbNewLine
                    End If
                Loop



                'DEBUGTextBox1.Text &= strTemp
                FileClose(FileNum)

                'DEBUGTextBox1.Text &= strTemp2

                'Me.Text = myFile
                ' intSaveFileFlag = True
            Catch ex As Exception
                ' MsgBox("資料檔案未開啟")
            End Try
        Else
            MsgBox("資料檔案未開啟", vbCritical)
        End If


    End Sub

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click, Button19.Click

        Dim REG() As TextBox = { _
                      REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
                      REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
                      REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
                      REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
                      REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
                      REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
                      REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
                      REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
                      REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
                      REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
                      REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
                      REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
                      REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
                      REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
                      REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
                      REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}
        For index As Integer = 0 To 255
            If sender.Name = Button17.Name Then
                REG(index).Text = "00"
            Else
                REG(index).ForeColor = Color.Empty
                REG(index).BackColor = Color.Empty
            End If
        Next

        ToolStripStatusLabel1.Text = "工作目錄：" & Application.StartupPath

    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        DEBUGTextBox1.Text = ""
        WriteREG_Start = Convert.ToInt32(REGStart.Text, 16) '0
        WriteREG_End = Convert.ToInt32(REGEnd.Text, 16) '&HFF  '0~255
        WriteREG_Loop = WriteREG_Start
        RW_REG_Action = REG_WRITE_MODE   'Write mode
        TextBox3.Text = Hex(WriteREG_Loop)
        TextBox4.Text = REG00.Text
        btnREGGroup.Enabled = False
        DEBUGTextBox1.Text &= "W " & TextBox3.Text & " " & TextBox4.Text & " "
        SendCMD(22)
        'Button14.Enabled = False
        'Button15.Enabled = False
        'TabControl1.Enabled = False
        'Timer6.Interval = 15000
        'Timer6.Enabled = True
        'Timer6.Start()

        Label37.Text = TimeOfDay.AddSeconds(10).ToString("hh:mm:ss")
        strREGStopTime = Label37.Text

    End Sub

    Private Sub REG00_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles REG00.KeyPress, REGFF.KeyPress, REGFE.KeyPress, REGFD.KeyPress, REGFC.KeyPress, REGFB.KeyPress, REGFA.KeyPress, REGF9.KeyPress, REGF8.KeyPress, REGF7.KeyPress, REGF6.KeyPress, REGF5.KeyPress, REGF4.KeyPress, REGF3.KeyPress, REGF2.KeyPress, REGF1.KeyPress, REGF0.KeyPress, REGEF.KeyPress, REGEE.KeyPress, REGED.KeyPress, REGEC.KeyPress, REGEB.KeyPress, REGEA.KeyPress, REGE9.KeyPress, REGE8.KeyPress, REGE7.KeyPress, REGE6.KeyPress, REGE5.KeyPress, REGE4.KeyPress, REGE3.KeyPress, REGE2.KeyPress, REGE1.KeyPress, REGE0.KeyPress, REGDF.KeyPress, REGDE.KeyPress, REGDD.KeyPress, REGDC.KeyPress, REGDB.KeyPress, REGDA.KeyPress, REGD9.KeyPress, REGD8.KeyPress, REGD7.KeyPress, REGD6.KeyPress, REGD5.KeyPress, REGD4.KeyPress, REGD3.KeyPress, REGD2.KeyPress, REGD1.KeyPress, REGD0.KeyPress, REGCF.KeyPress, REGCE.KeyPress, REGCD.KeyPress, REGCC.KeyPress, REGCB.KeyPress, REGCA.KeyPress, REGC9.KeyPress, REGC8.KeyPress, REGC7.KeyPress, REGC6.KeyPress, REGC5.KeyPress, REGC4.KeyPress, REGC3.KeyPress, REGC2.KeyPress, REGC1.KeyPress, REGC0.KeyPress, REGBF.KeyPress, REGBE.KeyPress, REGBD.KeyPress, REGBC.KeyPress, REGBB.KeyPress, REGBA.KeyPress, REGB9.KeyPress, REGB8.KeyPress, REGB7.KeyPress, REGB6.KeyPress, REGB5.KeyPress, REGB4.KeyPress, REGB3.KeyPress, REGB2.KeyPress, REGB1.KeyPress, REGB0.KeyPress, REGAF.KeyPress, REGAE.KeyPress, REGAD.KeyPress, REGAC.KeyPress, REGAB.KeyPress, REGAA.KeyPress, REGA9.KeyPress, REGA8.KeyPress, REGA7.KeyPress, REGA6.KeyPress, REGA5.KeyPress, REGA4.KeyPress, REGA3.KeyPress, REGA2.KeyPress, REGA1.KeyPress, REGA0.KeyPress, REG9F.KeyPress, REG9E.KeyPress, REG9D.KeyPress, REG9C.KeyPress, REG9B.KeyPress, REG9A.KeyPress, REG99.KeyPress, REG98.KeyPress, REG97.KeyPress, REG96.KeyPress, REG95.KeyPress, REG94.KeyPress, REG93.KeyPress, REG92.KeyPress, REG91.KeyPress, REG90.KeyPress, REG8F.KeyPress, REG8E.KeyPress, REG8D.KeyPress, REG8C.KeyPress, REG8B.KeyPress, REG8A.KeyPress, REG89.KeyPress, REG88.KeyPress, REG87.KeyPress, REG86.KeyPress, REG85.KeyPress, REG84.KeyPress, REG83.KeyPress, REG82.KeyPress, REG81.KeyPress, REG80.KeyPress, REG7F.KeyPress, REG7E.KeyPress, REG7D.KeyPress, REG7C.KeyPress, REG7B.KeyPress, REG7A.KeyPress, REG79.KeyPress, REG78.KeyPress, REG77.KeyPress, REG76.KeyPress, REG75.KeyPress, REG74.KeyPress, REG73.KeyPress, REG72.KeyPress, REG71.KeyPress, REG70.KeyPress, REG6F.KeyPress, REG6E.KeyPress, REG6D.KeyPress, REG6C.KeyPress, REG6B.KeyPress, REG6A.KeyPress, REG69.KeyPress, REG68.KeyPress, REG67.KeyPress, REG66.KeyPress, REG65.KeyPress, REG64.KeyPress, REG63.KeyPress, REG62.KeyPress, REG61.KeyPress, REG60.KeyPress, REG5F.KeyPress, REG5E.KeyPress, REG5D.KeyPress, REG5C.KeyPress, REG5B.KeyPress, REG5A.KeyPress, REG59.KeyPress, REG58.KeyPress, REG57.KeyPress, REG56.KeyPress, REG55.KeyPress, REG54.KeyPress, REG53.KeyPress, REG52.KeyPress, REG51.KeyPress, REG50.KeyPress, REG4F.KeyPress, REG4E.KeyPress, REG4D.KeyPress, REG4C.KeyPress, REG4B.KeyPress, REG4A.KeyPress, REG49.KeyPress, REG48.KeyPress, REG47.KeyPress, REG46.KeyPress, REG45.KeyPress, REG44.KeyPress, REG43.KeyPress, REG42.KeyPress, REG41.KeyPress, REG40.KeyPress, REG3F.KeyPress, REG3E.KeyPress, REG3D.KeyPress, REG3C.KeyPress, REG3B.KeyPress, REG3A.KeyPress, REG39.KeyPress, REG38.KeyPress, REG37.KeyPress, REG36.KeyPress, REG35.KeyPress, REG34.KeyPress, REG33.KeyPress, REG32.KeyPress, REG31.KeyPress, REG30.KeyPress, REG2F.KeyPress, REG2E.KeyPress, REG2D.KeyPress, REG2C.KeyPress, REG2B.KeyPress, REG2A.KeyPress, REG29.KeyPress, REG28.KeyPress, REG27.KeyPress, REG26.KeyPress, REG25.KeyPress, REG24.KeyPress, REG23.KeyPress, REG22.KeyPress, REG21.KeyPress, REG20.KeyPress, REG1F.KeyPress, REG1E.KeyPress, REG1D.KeyPress, REG1C.KeyPress, REG1B.KeyPress, REG1A.KeyPress, REG19.KeyPress, REG18.KeyPress, REG17.KeyPress, REG16.KeyPress, REG15.KeyPress, REG14.KeyPress, REG13.KeyPress, REG12.KeyPress, REG11.KeyPress, REG10.KeyPress, REG0F.KeyPress, REG0E.KeyPress, REG0D.KeyPress, REG0C.KeyPress, REG0B.KeyPress, REG0A.KeyPress, REG09.KeyPress, REG08.KeyPress, REG07.KeyPress, REG06.KeyPress, REG05.KeyPress, REG04.KeyPress, REG03.KeyPress, REG02.KeyPress, REG01.KeyPress

        '接著就是把該物件的Id顯示出來
        'Dim txt As TextBox = CType(sender, TextBox)
        'PRINT(txt.Name)
        Dim charNumber As Integer = 0

        If Asc(e.KeyChar) = 13 Then

            sender.Text = UCase(sender.Text)

            If sender.Text = "" Then
                sender.Text = strREGValueTemp '"00"
            ElseIf Len(sender.Text) = 1 Then
                sender.Text = strREGValueTemp 'sender.Text = "0" & sender.Text
            End If

            For i = 1 To Len(sender.Text)
                charNumber = Asc(Mid(sender.Text, i, 1))

                If (charNumber >= 48 And charNumber <= 59) Or (charNumber >= 65 And charNumber <= 70) Then
                    PRINT("Key" & i & "=" & Asc(Mid(sender.Text, i, 1)))
                Else
                    If i = 1 Then
                        '  PRINT("Key1=Error" & "Change To=0")
                        sender.Text = Mid(strREGValueTemp, 1, 1) & Mid(sender.Text, 2, 1)
                    Else
                        'PRINT("Key2=Error" & "Change To=0")
                        sender.Text = Mid(sender.Text, 1, 1) & Mid(strREGValueTemp, 2, 1)
                    End If

                End If

            Next i


            WRITEREGDATA(sender.Name)
        End If
    End Sub


    Private Sub Timer5_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer5.Tick
        If FW_ACTION = FW_TEST_WAITING Then
            Label15.Text = "NG"
            Label15.ForeColor = Color.Red
            FW_ACTION = FW_TEST_NG
            Timer5.Interval = 5000
        ElseIf FW_ACTION = FW_TEST_PASS Then
            Label15.Text = "PASS"
            Label15.ForeColor = Color.Green
            FW_ACTION = FW_TEST_NO
            Timer5.Interval = 5000
        Else
            Label15.Text = ""
            Label15.ForeColor = Color.Empty
            Timer5.Interval = 1000
            Timer5.Enabled = False
        End If

    End Sub

    Private Sub TextBox6_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox6.KeyPress
        If Asc(e.KeyChar) = 13 Then
            If SerialPort1.IsOpen = True Then
                SerialPort1.Write(TextBox6.Text + Chr(13))
            End If
        End If
    End Sub

    Private Sub TextBox6_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TextBox6.MouseClick
        'TextBox6.Text = ""
    End Sub

    Private Sub rtbReceived_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rtbReceived.KeyPress

        If Asc(e.KeyChar) = 13 Then
            SerialPort1.Write(TextBox6.Text + Chr(13))
            TextBox6.Text = ""
        ElseIf Asc(e.KeyChar) = 8 And Len(TextBox6.Text) <> 0 Then
            TextBox6.Text = Mid(TextBox6.Text, 1, Len(TextBox6.Text) - 1)
        Else
            TextBox6.Text &= e.KeyChar
        End If

    End Sub

    Private Sub ComboBox6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox6.SelectedIndexChanged
        If ComboBox6.SelectedIndex = 0 Then
            TextBox1.Text = "40"
        ElseIf ComboBox6.SelectedIndex = 1 Then
            TextBox1.Text = "88"
        ElseIf ComboBox6.SelectedIndex = 2 Then
            TextBox1.Text = "A0"
        ElseIf ComboBox6.SelectedIndex = 3 Then
            TextBox1.Text = "12"
        End If

        ComboBox2.SelectedIndex = ComboBox6.SelectedIndex
    End Sub

    Private Sub frmMain_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown

        If TabControl1.Enabled = True And CheckBox1.Checked = False Then

            If (e.KeyCode = Asc("F") Or e.KeyCode = Asc("f")) AndAlso e.Control Then
                SendCMD(7)
                FW_ACTION = FW_TEST_WAITING
                Label15.Text = "WAITING"
                Label15.ForeColor = Color.Empty
                Timer5.Interval = 1000
                Timer5.Enabled = True
            ElseIf (e.KeyCode = Asc("M") Or e.KeyCode = Asc("m")) AndAlso e.Control Then
                SendCMD(16)
            ElseIf (e.KeyCode = Asc("J") Or e.KeyCode = Asc("j")) AndAlso e.Control Then
                SendCMD(20)
            ElseIf (e.KeyCode = Asc("S") Or e.KeyCode = Asc("s")) AndAlso e.Control Then
                SendCMD(19)
            ElseIf (e.KeyCode = Asc("U") Or e.KeyCode = Asc("u")) AndAlso e.Control Then
                SendCMD(17)
            ElseIf (e.KeyCode = Asc("D") Or e.KeyCode = Asc("d")) AndAlso e.Control Then
                SendCMD(18)
            ElseIf e.KeyCode = 190 Then   '.
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber >= 255 Then
                    intNumber = 255
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber + 1)
                End If
                SendCMD(22)
            ElseIf e.KeyCode = 188 Then     ',
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber <= 0 Then
                    intNumber = 0
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber - 1)
                End If
                SendCMD(22)
            ElseIf e.KeyCode = 186 Then     '<
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber < 16 Then
                    intNumber = 0
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber - 16)
                End If
                SendCMD(22)
            ElseIf e.KeyCode = 222 Then     '>
                Dim intNumber As Integer = 0
                intNumber = Val("&H" + TextBox4.Text + "&")
                If intNumber > 255 - 16 Then
                    intNumber = 255
                    TextBox4.Text = Hex(intNumber)
                Else
                    TextBox4.Text = Hex(intNumber + 16)
                End If
                SendCMD(22)
            End If
            If (e.KeyCode = Asc("E") Or e.KeyCode = Asc("e")) AndAlso e.Control Then
                ExitAPP()
            End If
            'PRINT("Key=" & e.KeyCode) 'Asc(",")
            ' PRINT("Key=." & Chr(".")) 'Asc(",")
            'PRINT("Key(,)=" & Asc(",")) 'Asc(",")
            'PRINT("Key(.)=" & Asc(".")) 'Asc(",")
        End If
    End Sub

    Private Sub REG00_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles REGFF.MouseDoubleClick, REGFE.MouseDoubleClick, REGFD.MouseDoubleClick, REGFC.MouseDoubleClick, REGFB.MouseDoubleClick, REGFA.MouseDoubleClick, REGF9.MouseDoubleClick, REGF8.MouseDoubleClick, REGF7.MouseDoubleClick, REGF6.MouseDoubleClick, REGF5.MouseDoubleClick, REGF4.MouseDoubleClick, REGF3.MouseDoubleClick, REGF2.MouseDoubleClick, REGF1.MouseDoubleClick, REGF0.MouseDoubleClick, REGEF.MouseDoubleClick, REGEE.MouseDoubleClick, REGED.MouseDoubleClick, REGEC.MouseDoubleClick, REGEB.MouseDoubleClick, REGEA.MouseDoubleClick, REGE9.MouseDoubleClick, REGE8.MouseDoubleClick, REGE7.MouseDoubleClick, REGE6.MouseDoubleClick, REGE5.MouseDoubleClick, REGE4.MouseDoubleClick, REGE3.MouseDoubleClick, REGE2.MouseDoubleClick, REGE1.MouseDoubleClick, REGE0.MouseDoubleClick, REGDF.MouseDoubleClick, REGDE.MouseDoubleClick, REGDD.MouseDoubleClick, REGDC.MouseDoubleClick, REGDB.MouseDoubleClick, REGDA.MouseDoubleClick, REGD9.MouseDoubleClick, REGD8.MouseDoubleClick, REGD7.MouseDoubleClick, REGD6.MouseDoubleClick, REGD5.MouseDoubleClick, REGD4.MouseDoubleClick, REGD3.MouseDoubleClick, REGD2.MouseDoubleClick, REGD1.MouseDoubleClick, REGD0.MouseDoubleClick, REGCF.MouseDoubleClick, REGCE.MouseDoubleClick, REGCD.MouseDoubleClick, REGCC.MouseDoubleClick, REGCB.MouseDoubleClick, REGCA.MouseDoubleClick, REGC9.MouseDoubleClick, REGC8.MouseDoubleClick, REGC7.MouseDoubleClick, REGC6.MouseDoubleClick, REGC5.MouseDoubleClick, REGC4.MouseDoubleClick, REGC3.MouseDoubleClick, REGC2.MouseDoubleClick, REGC1.MouseDoubleClick, REGC0.MouseDoubleClick, REGBF.MouseDoubleClick, REGBE.MouseDoubleClick, REGBD.MouseDoubleClick, REGBC.MouseDoubleClick, REGBB.MouseDoubleClick, REGBA.MouseDoubleClick, REGB9.MouseDoubleClick, REGB8.MouseDoubleClick, REGB7.MouseDoubleClick, REGB6.MouseDoubleClick, REGB5.MouseDoubleClick, REGB4.MouseDoubleClick, REGB3.MouseDoubleClick, REGB2.MouseDoubleClick, REGB1.MouseDoubleClick, REGB0.MouseDoubleClick, REGAF.MouseDoubleClick, REGAE.MouseDoubleClick, REGAD.MouseDoubleClick, REGAC.MouseDoubleClick, REGAB.MouseDoubleClick, REGAA.MouseDoubleClick, REGA9.MouseDoubleClick, REGA8.MouseDoubleClick, REGA7.MouseDoubleClick, REGA6.MouseDoubleClick, REGA5.MouseDoubleClick, REGA4.MouseDoubleClick, REGA3.MouseDoubleClick, REGA2.MouseDoubleClick, REGA1.MouseDoubleClick, REGA0.MouseDoubleClick, REG9F.MouseDoubleClick, REG9E.MouseDoubleClick, REG9D.MouseDoubleClick, REG9C.MouseDoubleClick, REG9B.MouseDoubleClick, REG9A.MouseDoubleClick, REG99.MouseDoubleClick, REG98.MouseDoubleClick, REG97.MouseDoubleClick, REG96.MouseDoubleClick, REG95.MouseDoubleClick, REG94.MouseDoubleClick, REG93.MouseDoubleClick, REG92.MouseDoubleClick, REG91.MouseDoubleClick, REG90.MouseDoubleClick, REG8F.MouseDoubleClick, REG8E.MouseDoubleClick, REG8D.MouseDoubleClick, REG8C.MouseDoubleClick, REG8B.MouseDoubleClick, REG8A.MouseDoubleClick, REG89.MouseDoubleClick, REG88.MouseDoubleClick, REG87.MouseDoubleClick, REG86.MouseDoubleClick, REG85.MouseDoubleClick, REG84.MouseDoubleClick, REG83.MouseDoubleClick, REG82.MouseDoubleClick, REG81.MouseDoubleClick, REG80.MouseDoubleClick, REG7F.MouseDoubleClick, REG7E.MouseDoubleClick, REG7D.MouseDoubleClick, REG7C.MouseDoubleClick, REG7B.MouseDoubleClick, REG7A.MouseDoubleClick, REG79.MouseDoubleClick, REG78.MouseDoubleClick, REG77.MouseDoubleClick, REG76.MouseDoubleClick, REG75.MouseDoubleClick, REG74.MouseDoubleClick, REG73.MouseDoubleClick, REG72.MouseDoubleClick, REG71.MouseDoubleClick, REG70.MouseDoubleClick, REG6F.MouseDoubleClick, REG6E.MouseDoubleClick, REG6D.MouseDoubleClick, REG6C.MouseDoubleClick, REG6B.MouseDoubleClick, REG6A.MouseDoubleClick, REG69.MouseDoubleClick, REG68.MouseDoubleClick, REG67.MouseDoubleClick, REG66.MouseDoubleClick, REG65.MouseDoubleClick, REG64.MouseDoubleClick, REG63.MouseDoubleClick, REG62.MouseDoubleClick, REG61.MouseDoubleClick, REG60.MouseDoubleClick, REG5F.MouseDoubleClick, REG5E.MouseDoubleClick, REG5D.MouseDoubleClick, REG5C.MouseDoubleClick, REG5B.MouseDoubleClick, REG5A.MouseDoubleClick, REG59.MouseDoubleClick, REG58.MouseDoubleClick, REG57.MouseDoubleClick, REG56.MouseDoubleClick, REG55.MouseDoubleClick, REG54.MouseDoubleClick, REG53.MouseDoubleClick, REG52.MouseDoubleClick, REG51.MouseDoubleClick, REG50.MouseDoubleClick, REG4F.MouseDoubleClick, REG4E.MouseDoubleClick, REG4D.MouseDoubleClick, REG4C.MouseDoubleClick, REG4B.MouseDoubleClick, REG4A.MouseDoubleClick, REG49.MouseDoubleClick, REG48.MouseDoubleClick, REG47.MouseDoubleClick, REG46.MouseDoubleClick, REG45.MouseDoubleClick, REG44.MouseDoubleClick, REG43.MouseDoubleClick, REG42.MouseDoubleClick, REG41.MouseDoubleClick, REG40.MouseDoubleClick, REG3F.MouseDoubleClick, REG3E.MouseDoubleClick, REG3D.MouseDoubleClick, REG3C.MouseDoubleClick, REG3B.MouseDoubleClick, REG3A.MouseDoubleClick, REG39.MouseDoubleClick, REG38.MouseDoubleClick, REG37.MouseDoubleClick, REG36.MouseDoubleClick, REG35.MouseDoubleClick, REG34.MouseDoubleClick, REG33.MouseDoubleClick, REG32.MouseDoubleClick, REG31.MouseDoubleClick, REG30.MouseDoubleClick, REG2F.MouseDoubleClick, REG2E.MouseDoubleClick, REG2D.MouseDoubleClick, REG2C.MouseDoubleClick, REG2B.MouseDoubleClick, REG2A.MouseDoubleClick, REG29.MouseDoubleClick, REG28.MouseDoubleClick, REG27.MouseDoubleClick, REG26.MouseDoubleClick, REG25.MouseDoubleClick, REG24.MouseDoubleClick, REG23.MouseDoubleClick, REG22.MouseDoubleClick, REG21.MouseDoubleClick, REG20.MouseDoubleClick, REG1F.MouseDoubleClick, REG1E.MouseDoubleClick, REG1D.MouseDoubleClick, REG1C.MouseDoubleClick, REG1B.MouseDoubleClick, REG1A.MouseDoubleClick, REG19.MouseDoubleClick, REG18.MouseDoubleClick, REG17.MouseDoubleClick, REG16.MouseDoubleClick, REG15.MouseDoubleClick, REG14.MouseDoubleClick, REG13.MouseDoubleClick, REG12.MouseDoubleClick, REG11.MouseDoubleClick, REG10.MouseDoubleClick, REG0F.MouseDoubleClick, REG0E.MouseDoubleClick, REG0D.MouseDoubleClick, REG0C.MouseDoubleClick, REG0B.MouseDoubleClick, REG0A.MouseDoubleClick, REG09.MouseDoubleClick, REG08.MouseDoubleClick, REG07.MouseDoubleClick, REG06.MouseDoubleClick, REG05.MouseDoubleClick, REG04.MouseDoubleClick, REG03.MouseDoubleClick, REG02.MouseDoubleClick, REG01.MouseDoubleClick, REG00.MouseDoubleClick
        SetBoardColor(sender)
    End Sub

    Sub SetBoardColor(ByVal REG As Object)

        If REG.BackColor = Color.Yellow Then
            REG.BackColor = Color.Pink
        ElseIf REG.BackColor = Color.Pink Then
            REG.BackColor = Color.LightBlue
        ElseIf REG.BackColor = Color.LightBlue Then
            REG.BackColor = Color.Empty
        Else
            REG.BackColor = Color.Yellow
        End If

        ' TabControl1.Focus()

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ExitSaveConfiguration()

        SerialPort1.Close()
        Timer2.Enabled = False
        Timer1.Enabled = False
        Timer3.Enabled = False
        'Me.Close()
    End Sub

    Private Sub Timer6_Tick(sender As Object, e As EventArgs) Handles Timer6.Tick
        If RW_REG_Action = REG_READ_MODE Then
            PRINT("TIME OUT REG_READ_MODE = NG")
        ElseIf RW_REG_Action = REG_WRITE_MODE Then
            PRINT("TIME OUT REG_WRITE_MODE = NG")
        End If

        RW_REG_Action = 0
        Timer6.Stop()
        Timer6.Enabled = False
        Timer6.Stop()
        Timer6.Start()
        btnREGGroup.Enabled = True
    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged

        Dim intValue2 As Integer = Conversion.Val("&H" & REG1A.Text) And &HF0

        'PRINT("1st=" & Hex(intValue2))

        If ComboBox7.SelectedIndex < 7 Then
            intValue2 = intValue2 + (ComboBox7.SelectedIndex) Or &H8
            'PRINT("1A=" & Hex(intValue2))
        End If

        If intValue2 < &H10 Then
            REG1A.Text = "0" & Hex(intValue2)
        Else
            REG1A.Text = Hex(intValue2)
        End If

        TextBox3.Text = "1A"
        TextBox2.Text = TextBox3.Text
        TextBox4.Text = REG1A.Text
        SendCMD(22)
    End Sub

    Private Sub ComboBox8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox8.SelectedIndexChanged
        Dim intValue2 As Integer = Conversion.Val("&H" & REGF0.Text) And &H70

        'PRINT("1st=" & Hex(intValue2))

        If ComboBox8.SelectedIndex < 7 Then
            intValue2 = intValue2 + (ComboBox8.SelectedIndex) Or &H80
            'PRINT("1A=" & Hex(intValue2))
        End If

        If intValue2 < &H10 Then
            REGF0.Text = "0" & Hex(intValue2)
        Else
            REGF0.Text = Hex(intValue2)
        End If

        TextBox3.Text = "F0"
        TextBox2.Text = TextBox3.Text
        TextBox4.Text = REGF0.Text
        SendCMD(22)
    End Sub

    Private Sub ComboBox9_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox9.SelectedIndexChanged

        Dim objTEMP As TextBox = CType(REG45, TextBox)
        Dim intValue2 As Integer = Conversion.Val("&H" & objTEMP.Text) And &H70

        'PRINT("1st=" & Hex(intValue2))

        If ComboBox9.SelectedIndex < 7 Then
            intValue2 = intValue2 + (ComboBox9.SelectedIndex) Or &H8
            'PRINT("1A=" & Hex(intValue2))
        End If

        If intValue2 < &H10 Then
            objTEMP.Text = "0" & Hex(intValue2)
        Else
            objTEMP.Text = Hex(intValue2)
        End If

        TextBox3.Text = "45"
        TextBox2.Text = TextBox3.Text
        TextBox4.Text = objTEMP.Text
        SendCMD(22)
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged

        If TabControl1.SelectedIndex = 2 Then
            TextBox1.Text = "40"
            ComboBox2.SelectedIndex = 0
            ComboBox6.SelectedIndex = 0
        End If

        If TabControl1.SelectedIndex = 1 Then
            GroupBox4.Enabled = True
        Else
            GroupBox4.Enabled = False
        End If

    End Sub

    Private Sub DEBUGTextBox1_TextChanged(sender As Object, e As EventArgs) Handles DEBUGTextBox1.TextChanged
        DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
        DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
        'DEBUGTextBox1.Focus()
        DEBUGTextBox1.SelectionAlignment = 0

    End Sub

    Private Sub REG00_MouseClick(sender As Object, e As MouseEventArgs) Handles REG00.MouseClick, REGFF.MouseClick, REGFE.MouseClick, REGFD.MouseClick, REGFC.MouseClick, REGFB.MouseClick, REGFA.MouseClick, REGF9.MouseClick, REGF8.MouseClick, REGF7.MouseClick, REGF6.MouseClick, REGF5.MouseClick, REGF4.MouseClick, REGF3.MouseClick, REGF2.MouseClick, REGF1.MouseClick, REGF0.MouseClick, REGEF.MouseClick, REGEE.MouseClick, REGED.MouseClick, REGEC.MouseClick, REGEB.MouseClick, REGEA.MouseClick, REGE9.MouseClick, REGE8.MouseClick, REGE7.MouseClick, REGE6.MouseClick, REGE5.MouseClick, REGE4.MouseClick, REGE3.MouseClick, REGE2.MouseClick, REGE1.MouseClick, REGE0.MouseClick, REGDF.MouseClick, REGDE.MouseClick, REGDD.MouseClick, REGDC.MouseClick, REGDB.MouseClick, REGDA.MouseClick, REGD9.MouseClick, REGD8.MouseClick, REGD7.MouseClick, REGD6.MouseClick, REGD5.MouseClick, REGD4.MouseClick, REGD3.MouseClick, REGD2.MouseClick, REGD1.MouseClick, REGD0.MouseClick, REGCF.MouseClick, REGCE.MouseClick, REGCD.MouseClick, REGCC.MouseClick, REGCB.MouseClick, REGCA.MouseClick, REGC9.MouseClick, REGC8.MouseClick, REGC7.MouseClick, REGC6.MouseClick, REGC5.MouseClick, REGC4.MouseClick, REGC3.MouseClick, REGC2.MouseClick, REGC1.MouseClick, REGC0.MouseClick, REGBF.MouseClick, REGBE.MouseClick, REGBD.MouseClick, REGBC.MouseClick, REGBB.MouseClick, REGBA.MouseClick, REGB9.MouseClick, REGB8.MouseClick, REGB7.MouseClick, REGB6.MouseClick, REGB5.MouseClick, REGB4.MouseClick, REGB3.MouseClick, REGB2.MouseClick, REGB1.MouseClick, REGB0.MouseClick, REGAF.MouseClick, REGAE.MouseClick, REGAD.MouseClick, REGAC.MouseClick, REGAB.MouseClick, REGAA.MouseClick, REGA9.MouseClick, REGA8.MouseClick, REGA7.MouseClick, REGA6.MouseClick, REGA5.MouseClick, REGA4.MouseClick, REGA3.MouseClick, REGA2.MouseClick, REGA1.MouseClick, REGA0.MouseClick, REG9F.MouseClick, REG9E.MouseClick, REG9D.MouseClick, REG9C.MouseClick, REG9B.MouseClick, REG9A.MouseClick, REG99.MouseClick, REG98.MouseClick, REG97.MouseClick, REG96.MouseClick, REG95.MouseClick, REG94.MouseClick, REG93.MouseClick, REG92.MouseClick, REG91.MouseClick, REG90.MouseClick, REG8F.MouseClick, REG8E.MouseClick, REG8D.MouseClick, REG8C.MouseClick, REG8B.MouseClick, REG8A.MouseClick, REG89.MouseClick, REG88.MouseClick, REG87.MouseClick, REG86.MouseClick, REG85.MouseClick, REG84.MouseClick, REG83.MouseClick, REG82.MouseClick, REG81.MouseClick, REG80.MouseClick, REG7F.MouseClick, REG7E.MouseClick, REG7D.MouseClick, REG7C.MouseClick, REG7B.MouseClick, REG7A.MouseClick, REG79.MouseClick, REG78.MouseClick, REG77.MouseClick, REG76.MouseClick, REG75.MouseClick, REG74.MouseClick, REG73.MouseClick, REG72.MouseClick, REG71.MouseClick, REG70.MouseClick, REG6F.MouseClick, REG6E.MouseClick, REG6D.MouseClick, REG6C.MouseClick, REG6B.MouseClick, REG6A.MouseClick, REG69.MouseClick, REG68.MouseClick, REG67.MouseClick, REG66.MouseClick, REG65.MouseClick, REG64.MouseClick, REG63.MouseClick, REG62.MouseClick, REG61.MouseClick, REG60.MouseClick, REG5F.MouseClick, REG5E.MouseClick, REG5D.MouseClick, REG5C.MouseClick, REG5B.MouseClick, REG5A.MouseClick, REG59.MouseClick, REG58.MouseClick, REG57.MouseClick, REG56.MouseClick, REG55.MouseClick, REG54.MouseClick, REG53.MouseClick, REG52.MouseClick, REG51.MouseClick, REG50.MouseClick, REG4F.MouseClick, REG4E.MouseClick, REG4D.MouseClick, REG4C.MouseClick, REG4B.MouseClick, REG4A.MouseClick, REG49.MouseClick, REG48.MouseClick, REG47.MouseClick, REG46.MouseClick, REG45.MouseClick, REG44.MouseClick, REG43.MouseClick, REG42.MouseClick, REG41.MouseClick, REG40.MouseClick, REG3F.MouseClick, REG3E.MouseClick, REG3D.MouseClick, REG3C.MouseClick, REG3B.MouseClick, REG3A.MouseClick, REG39.MouseClick, REG38.MouseClick, REG37.MouseClick, REG36.MouseClick, REG35.MouseClick, REG34.MouseClick, REG33.MouseClick, REG32.MouseClick, REG31.MouseClick, REG30.MouseClick, REG2F.MouseClick, REG2E.MouseClick, REG2D.MouseClick, REG2C.MouseClick, REG2B.MouseClick, REG2A.MouseClick, REG29.MouseClick, REG28.MouseClick, REG27.MouseClick, REG26.MouseClick, REG25.MouseClick, REG24.MouseClick, REG23.MouseClick, REG22.MouseClick, REG21.MouseClick, REG20.MouseClick, REG1F.MouseClick, REG1E.MouseClick, REG1D.MouseClick, REG1C.MouseClick, REG1B.MouseClick, REG1A.MouseClick, REG19.MouseClick, REG18.MouseClick, REG17.MouseClick, REG16.MouseClick, REG15.MouseClick, REG14.MouseClick, REG13.MouseClick, REG12.MouseClick, REG11.MouseClick, REG10.MouseClick, REG0F.MouseClick, REG0E.MouseClick, REG0D.MouseClick, REG0C.MouseClick, REG0B.MouseClick, REG0A.MouseClick, REG09.MouseClick, REG08.MouseClick, REG07.MouseClick, REG06.MouseClick, REG05.MouseClick, REG04.MouseClick, REG03.MouseClick, REG02.MouseClick, REG01.MouseClick
        strREGValueTemp = sender.text
        sender.SelectAll()
        WRITEREGDATA(sender.Name)
        PRINT("Select REG=" + Mid(sender.Name, 4, 2) + ", strREGValueTemp=" + sender.text)
        strSelectRegister = Mid(sender.Name, 4, 2)
        Me.REG00.ContextMenuStrip = ContextMenuStrip1

    End Sub


    Private Sub btnTEST_Click(sender As Object, e As EventArgs) Handles btnTEST.Click

        Dim Str As String = ""
        Dim Str2 As String = ""
        Dim Str_number As Integer
        Dim Data As Integer = 0
        Dim strCRC As String = ""
        Dim strTEST As String = TESTOutput.Text
        Dim intValue2 As Integer = 0

        Str_number = InStr(1, strTEST, "STX")

        If Str_number > 0 Then
            '    Me.DEBUGTextBox1.Text &= "STX ADDR=" & Str_number.ToString() & " "

            If (InStr(1, strTEST, "ETX") = (Str_number + 11)) Then
                '  Me.DEBUGTextBox1.Text &= "R " & Mid(strTEST, Str_number + 3, 2) & " " & Mid(strTEST, Str_number + 3 + 2, 2) & vbNewLine
                PRINT("GOT STX , AND ETX OK" & " ,LEN=" & Len(strTEST))
                '命令執行完畢刪除命令
                TESTOutput.Text = Mid(strTEST, Str_number + 14, strTEST.Length)
                'CRC check
                'intValue2 = Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3, 2)) _
                '    Xor Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3 + 2, 2)) _
                'Xor Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 3 + 2 + 2, 2))

                'If intValue2 <> Conversion.Val("&H" & Mid(RichTextBox1.Text, Str_number + 9, 2)) Then
                '    'PRINT("CMD CRC OK = " & Hex(intValue2))
                '    'Else
                '    PRINT("CMD CRC NG = " & Hex(intValue2))
                '    Exit Sub
                'End If

                'DEBUGTextBox1.SelectionStart = DEBUGTextBox1.Text.Length   '文本的选取长度
                'DEBUGTextBox1.ScrollToCaret()  '关键之语句：将焦点滚动到文本内容后
                ' Me.DEBUGTextBox1.SelectionAlignment = 0
                'DEBUGTextBox1.Focus()

                'TextBox5.Text = Mid(RichTextBox1.Text, Str_number + 3 + 2, 2)

                'GETREGDATA(Mid(RichTextBox1.Text, Str_number + 3, 2), TextBox5.Text)

                'If RW_REG_Action = REG_READ_MODE Then
                '    If Dump_Loop < Dump_End Then
                '        Dump_Loop += 1
                '        TextBox2.Text = Hex(Dump_Loop)
                '        SendCMD(21)
                '    Else

                '        RW_REG_Action = 0
                '        Button14.Enabled = True
                '        Button15.Enabled = True
                '        btnREGGroup.Enabled = True

                '        PRINT("READ ALL OK!!")

                '        Timer6.Stop()
                '        Timer6.Enabled = False
                '        Timer6.Stop()
                '        Timer6.Start()
                '    End If
                'End If

                'If RW_REG_Action = REG_WRITE_MODE Then

                '    If WriteREG_Loop < WriteREG_End Then
                '        WriteREG_Loop += 1
                '        WRITEREGDATA("REG" & Hex(WriteREG_Loop))

                '    Else

                '        RW_REG_Action = 0
                '        Button14.Enabled = True
                '        Button15.Enabled = True
                '        btnREGGroup.Enabled = True
                '        PRINT("WRITE ALL OK!!")

                '        Timer6.Stop()
                '        Timer6.Enabled = False
                '        Timer6.Stop()
                '        Timer6.Start()
                '    End If

                'End If

                'TextBox4.Text = TextBox5.Text

                'RichTextBox1.Text = ""
            Else
                PRINT("GOT STX ,ETX =NG" & " ,LEN=" & Len(strTEST))
                '比對ETX錯誤,刪除STX 
                TESTOutput.Text = Mid(strTEST, Str_number + 3, Len(strTEST) - 3)
            End If
        Else
            PRINT("NO STX = NG" & " ,LEN=" & Len(strTEST))
        End If
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        SendCMD(23)
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click

        Dim REG() As TextBox = { _
         REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
         REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
         REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
         REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
         REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
         REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
         REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
         REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
         REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
         REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
         REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
         REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
         REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
         REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
         REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
         REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        REG(Conversion.Val("&H" & strSelectRegister)).BackColor = Color.Yellow

    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        Dim REG() As TextBox = { _
            REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
            REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
            REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
            REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
            REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
            REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
            REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
            REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
            REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
            REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
            REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
            REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
            REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
            REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
            REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
            REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        REG(Conversion.Val("&H" & strSelectRegister)).BackColor = Color.Pink

    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click

        Dim REG() As TextBox = { _
      REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
      REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
      REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
      REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
      REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
      REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
      REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
      REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
      REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
      REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
      REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
      REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
      REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
      REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
      REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
      REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        REG(Conversion.Val("&H" & strSelectRegister)).BackColor = Color.LightBlue

    End Sub

    Private Sub ToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click

        Dim REG() As TextBox = { _
         REG00, REG01, REG02, REG03, REG04, REG05, REG06, REG07, REG08, REG09, REG0A, REG0B, REG0C, REG0D, REG0E, REG0F, _
         REG10, REG11, REG12, REG13, REG14, REG15, REG16, REG17, REG18, REG19, REG1A, REG1B, REG1C, REG1D, REG1E, REG1F, _
         REG20, REG21, REG22, REG23, REG24, REG25, REG26, REG27, REG28, REG29, REG2A, REG2B, REG2C, REG2D, REG2E, REG2F, _
         REG30, REG31, REG32, REG33, REG34, REG35, REG36, REG37, REG38, REG39, REG3A, REG3B, REG3C, REG3D, REG3E, REG3F, _
         REG40, REG41, REG42, REG43, REG44, REG45, REG46, REG47, REG48, REG49, REG4A, REG4B, REG4C, REG4D, REG4E, REG4F, _
         REG50, REG51, REG52, REG53, REG54, REG55, REG56, REG57, REG58, REG59, REG5A, REG5B, REG5C, REG5D, REG5E, REG5F, _
         REG60, REG61, REG62, REG63, REG64, REG65, REG66, REG67, REG68, REG69, REG6A, REG6B, REG6C, REG6D, REG6E, REG6F, _
         REG70, REG71, REG72, REG73, REG74, REG75, REG76, REG77, REG78, REG79, REG7A, REG7B, REG7C, REG7D, REG7E, REG7F, _
 _
         REG80, REG81, REG82, REG83, REG84, REG85, REG86, REG87, REG88, REG89, REG8A, REG8B, REG8C, REG8D, REG8E, REG8F, _
         REG90, REG91, REG92, REG93, REG94, REG95, REG96, REG97, REG98, REG99, REG9A, REG9B, REG9C, REG9D, REG9E, REG9F, _
         REGA0, REGA1, REGA2, REGA3, REGA4, REGA5, REGA6, REGA7, REGA8, REGA9, REGAA, REGAB, REGAC, REGAD, REGAE, REGAF, _
         REGB0, REGB1, REGB2, REGB3, REGB4, REGB5, REGB6, REGB7, REGB8, REGB9, REGBA, REGBB, REGBC, REGBD, REGBE, REGBF, _
         REGC0, REGC1, REGC2, REGC3, REGC4, REGC5, REGC6, REGC7, REGC8, REGC9, REGCA, REGCB, REGCC, REGCD, REGCE, REGCF, _
         REGD0, REGD1, REGD2, REGD3, REGD4, REGD5, REGD6, REGD7, REGD8, REGD9, REGDA, REGDB, REGDC, REGDD, REGDE, REGDF, _
         REGE0, REGE1, REGE2, REGE3, REGE4, REGE5, REGE6, REGE7, REGE8, REGE9, REGEA, REGEB, REGEC, REGED, REGEE, REGEF, _
         REGF0, REGF1, REGF2, REGF3, REGF4, REGF5, REGF6, REGF7, REGF8, REGF9, REGFA, REGFB, REGFC, REGFD, REGFE, REGFF}

        REG(Conversion.Val("&H" & strSelectRegister)).BackColor = Color.Empty

    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        SerialPort1.Write(TextBox6.Text + Chr(13))
    End Sub


    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        Form3.Show()
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)




        If m.Msg = 161 And m.WParam = 20 Then

            '            MessageBox.Show("你點到 X 了，視窗即將關閉!")

            Form2.Close()
            Form3.Close()
            Me.Close()

        Else

            MyBase.WndProc(m)

        End If

    End Sub


    Private Sub Button22_Click(sender As Object, e As EventArgs)
        Form2.Show()

    End Sub

    Private Sub REGStart_KeyPress(sender As Object, e As KeyPressEventArgs) Handles REGStart.KeyPress
        If Asc(e.KeyChar) = 13 Then

            Dim charNumber As Integer = 0
            REGStart.Text = UCase(REGStart.Text)

            If REGStart.Text = "" Then
                REGStart.Text = "00"
            ElseIf Len(REGStart.Text) = 1 Then
                REGStart.Text = "0" & REGStart.Text
            End If

            For i = 1 To Len(REGStart.Text)
                charNumber = Asc(Mid(REGStart.Text, i, 1))

                If (charNumber >= 48 And charNumber <= 59) Or (charNumber >= 65 And charNumber <= 70) Then
                    PRINT("Key" & i & "=" & Asc(Mid(REGStart.Text, i, 1)))
                Else
                    If i = 1 Then
                        'PRINT("Key1=Error" & "Change To=0")
                        'REGStart.Text = "0" & Mid(REGStart.Text, 2, 1)
                        REGStart.Text = Mid(strREGValueTemp, 1, 1) & Mid(REGStart.Text, 2, 1)
                    Else
                        ' PRINT("Key2=Error" & "Change To=0")
                        'REGStart.Text = Mid(REGStart.Text, 1, 1) & "0"
                        REGStart.Text = Mid(REGStart.Text, 1, 1) & Mid(strREGValueTemp, 2, 1)
                    End If

                End If

            Next i

            'Dim intNumber As Integer = 0
            'REGStart.Text = UCase(REGStart.Text)

            'intNumber = Val("&H" + (REGStart.Text))

            'REGStart.Text = Hex(intNumber)

            'If intNumber >= 255 Then
            '    intNumber = 255
            '    REGStart.Text = Hex(intNumber)
            'ElseIf intNumber <= 0 Then
            '    intNumber = 0
            '    REGStart.Text = Hex(intNumber)
            'End If

            'SendCMD(22)
            'REGStart.Text = Integer.Parse(Val("&H" + UCase(REGStart.Text)) + "&")
        End If
    End Sub


    Private Sub REGEnd_KeyPress(sender As Object, e As KeyPressEventArgs) Handles REGEnd.KeyPress

        If Asc(e.KeyChar) = 13 Then
            Dim charNumber As Integer = 0
            REGEnd.Text = UCase(REGEnd.Text)

            If REGEnd.Text = "" Then
                REGEnd.Text = "00"
            ElseIf Len(REGEnd.Text) = 1 Then
                REGEnd.Text = "0" & REGEnd.Text
            End If

            For i = 1 To Len(REGEnd.Text)
                charNumber = Asc(Mid(REGEnd.Text, i, 1))

                If (charNumber >= 48 And charNumber <= 59) Or (charNumber >= 65 And charNumber <= 70) Then
                    PRINT("Key" & i & "=" & Asc(Mid(REGEnd.Text, i, 1)))
                Else
                    If i = 1 Then
                        'PRINT("Key1=Error" & "Change To=0")
                        'REGStart.Text = "0" & Mid(REGStart.Text, 2, 1)
                        REGEnd.Text = Mid(strREGValueTemp, 1, 1) & Mid(REGEnd.Text, 2, 1)
                    Else
                        ' PRINT("Key2=Error" & "Change To=0")
                        'REGStart.Text = Mid(REGStart.Text, 1, 1) & "0"
                        REGEnd.Text = Mid(REGEnd.Text, 1, 1) & Mid(strREGValueTemp, 2, 1)
                    End If

                End If

            Next i

            'If REGEnd.Text = "" Then
            '    REGEnd.Text = "00"
            'ElseIf Len(REGEnd.Text) = 1 Then
            '    REGEnd.Text = "0" & REGEnd.Text
            'End If

            'For i = 1 To Len(REGEnd.Text)
            '    PRINT("Key" & i & "=" & Asc(Mid(REGEnd.Text, i, 1)))
            'Next i

            'Dim intNumber As Integer = 0
            'REGEnd.Text = UCase(REGEnd.Text)

            'intNumber = Val("&H" + (REGEnd.Text) + "&")

            'REGEnd.Text = Hex(intNumber)

            'If intNumber >= 255 Then
            '    intNumber = 255
            '    REGEnd.Text = Hex(intNumber)
            'ElseIf intNumber <= 0 Then
            '    intNumber = 0
            '    REGEnd.Text = Hex(intNumber)
            'End If
        End If
    End Sub


    Private Sub REGEnd_TextChanged(sender As Object, e As EventArgs) Handles REGEnd.TextChanged


        'REGEnd.Text = UCase(REGEnd.Text)

        'If REGEnd.Text = "" Then
        '    REGEnd.Text = "00"
        'End If

        'For i = 1 To Len(REGEnd.Text)
        '    PRINT("Key" & i & "=" & Asc(Mid(REGEnd.Text, i, 1)))
        'Next i


    End Sub

    
    Private Sub REGStart_MouseClick(sender As Object, e As MouseEventArgs) Handles REGStart.MouseClick
        strREGValueTemp = sender.Text
        sender.SelectAll()
        PRINT("strREGValueTemp=" + sender.Text)
    End Sub

    Private Sub REGEnd_MouseClick(sender As Object, e As MouseEventArgs) Handles REGEnd.MouseClick
        strREGValueTemp = sender.Text
        sender.SelectAll()
        PRINT("strREGValueTemp=" + sender.Text)
    End Sub

    Private Sub REG00_Leave(sender As Object, e As EventArgs) Handles REG00.Leave, REGFF.Leave, REGFE.Leave, REGFD.Leave, REGFC.Leave, REGFB.Leave, REGFA.Leave, REGF9.Leave, REGF8.Leave, REGF7.Leave, REGF6.Leave, REGF5.Leave, REGF4.Leave, REGF3.Leave, REGF2.Leave, REGF1.Leave, REGF0.Leave, REGEF.Leave, REGEE.Leave, REGED.Leave, REGEC.Leave, REGEB.Leave, REGEA.Leave, REGE9.Leave, REGE8.Leave, REGE7.Leave, REGE6.Leave, REGE5.Leave, REGE4.Leave, REGE3.Leave, REGE2.Leave, REGE1.Leave, REGE0.Leave, REGDF.Leave, REGDE.Leave, REGDD.Leave, REGDC.Leave, REGDB.Leave, REGDA.Leave, REGD9.Leave, REGD8.Leave, REGD7.Leave, REGD6.Leave, REGD5.Leave, REGD4.Leave, REGD3.Leave, REGD2.Leave, REGD1.Leave, REGD0.Leave, REGCF.Leave, REGCE.Leave, REGCD.Leave, REGCC.Leave, REGCB.Leave, REGCA.Leave, REGC9.Leave, REGC8.Leave, REGC7.Leave, REGC6.Leave, REGC5.Leave, REGC4.Leave, REGC3.Leave, REGC2.Leave, REGC1.Leave, REGC0.Leave, REGBF.Leave, REGBE.Leave, REGBD.Leave, REGBC.Leave, REGBB.Leave, REGBA.Leave, REGB9.Leave, REGB8.Leave, REGB7.Leave, REGB6.Leave, REGB5.Leave, REGB4.Leave, REGB3.Leave, REGB2.Leave, REGB1.Leave, REGB0.Leave, REGAF.Leave, REGAE.Leave, REGAD.Leave, REGAC.Leave, REGAB.Leave, REGAA.Leave, REGA9.Leave, REGA8.Leave, REGA7.Leave, REGA6.Leave, REGA5.Leave, REGA4.Leave, REGA3.Leave, REGA2.Leave, REGA1.Leave, REGA0.Leave, REG9F.Leave, REG9E.Leave, REG9D.Leave, REG9C.Leave, REG9B.Leave, REG9A.Leave, REG99.Leave, REG98.Leave, REG97.Leave, REG96.Leave, REG95.Leave, REG94.Leave, REG93.Leave, REG92.Leave, REG91.Leave, REG90.Leave, REG8F.Leave, REG8E.Leave, REG8D.Leave, REG8C.Leave, REG8B.Leave, REG8A.Leave, REG89.Leave, REG88.Leave, REG87.Leave, REG86.Leave, REG85.Leave, REG84.Leave, REG83.Leave, REG82.Leave, REG81.Leave, REG80.Leave, REG7F.Leave, REG7E.Leave, REG7D.Leave, REG7C.Leave, REG7B.Leave, REG7A.Leave, REG79.Leave, REG78.Leave, REG77.Leave, REG76.Leave, REG75.Leave, REG74.Leave, REG73.Leave, REG72.Leave, REG71.Leave, REG70.Leave, REG6F.Leave, REG6E.Leave, REG6D.Leave, REG6C.Leave, REG6B.Leave, REG6A.Leave, REG69.Leave, REG68.Leave, REG67.Leave, REG66.Leave, REG65.Leave, REG64.Leave, REG63.Leave, REG62.Leave, REG61.Leave, REG60.Leave, REG5F.Leave, REG5E.Leave, REG5D.Leave, REG5C.Leave, REG5B.Leave, REG5A.Leave, REG59.Leave, REG58.Leave, REG57.Leave, REG56.Leave, REG55.Leave, REG54.Leave, REG53.Leave, REG52.Leave, REG51.Leave, REG50.Leave, REG4F.Leave, REG4E.Leave, REG4D.Leave, REG4C.Leave, REG4B.Leave, REG4A.Leave, REG49.Leave, REG48.Leave, REG47.Leave, REG46.Leave, REG45.Leave, REG44.Leave, REG43.Leave, REG42.Leave, REG41.Leave, REG40.Leave, REG3F.Leave, REG3E.Leave, REG3D.Leave, REG3C.Leave, REG3B.Leave, REG3A.Leave, REG39.Leave, REG38.Leave, REG37.Leave, REG36.Leave, REG35.Leave, REG34.Leave, REG33.Leave, REG32.Leave, REG31.Leave, REG30.Leave, REG2F.Leave, REG2E.Leave, REG2D.Leave, REG2C.Leave, REG2B.Leave, REG2A.Leave, REG29.Leave, REG28.Leave, REG27.Leave, REG26.Leave, REG25.Leave, REG24.Leave, REG23.Leave, REG22.Leave, REG21.Leave, REG20.Leave, REG1F.Leave, REG1E.Leave, REG1D.Leave, REG1C.Leave, REG1B.Leave, REG1A.Leave, REG19.Leave, REG18.Leave, REG17.Leave, REG16.Leave, REG15.Leave, REG14.Leave, REG13.Leave, REG12.Leave, REG11.Leave, REG10.Leave, REG0F.Leave, REG0E.Leave, REG0D.Leave, REG0C.Leave, REG0B.Leave, REG0A.Leave, REG09.Leave, REG08.Leave, REG07.Leave, REG06.Leave, REG05.Leave, REG04.Leave, REG03.Leave, REG02.Leave, REG01.Leave
        PRINT(sender.Name + "_Leave")
        Dim charNumber As Integer = 0
        sender.Text = UCase(sender.Text)

        If sender.Text = "" Then
            sender.Text = strREGValueTemp '"00"
        ElseIf Len(sender.Text) = 1 Then
            sender.Text = strREGValueTemp 'sender.Text = "0" & sender.Text
        End If

        For i = 1 To Len(sender.Text)
            charNumber = Asc(Mid(sender.Text, i, 1))

            If (charNumber >= 48 And charNumber <= 59) Or (charNumber >= 65 And charNumber <= 70) Then
                PRINT("Key" & i & "=" & Asc(Mid(sender.Text, i, 1)))
            Else
                If i = 1 Then
                    '  PRINT("Key1=Error" & "Change To=0")
                    sender.Text = Mid(strREGValueTemp, 1, 1) & Mid(sender.Text, 2, 1)
                Else
                    'PRINT("Key2=Error" & "Change To=0")
                    sender.Text = Mid(sender.Text, 1, 1) & Mid(strREGValueTemp, 2, 1)
                End If

            End If

        Next i


        WRITEREGDATA(sender.Name)
    End Sub

  
End Class
'Whole Code Ends Here ....