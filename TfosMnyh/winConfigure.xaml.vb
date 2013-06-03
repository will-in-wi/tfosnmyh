Imports WinMM

Partial Public Class winConfigure

    Protected Sub checkHSDir()
        If Utilities.CheckHSDir(txtDataLocation.Text) = True Then
            Dim imgSrc As Windows.Media.ImageSource
            Dim imgConvert As New Windows.Media.ImageSourceConverter
            imgSrc = imgConvert.ConvertFromString("C:\Users\William Johnston\Documents\Visual Studio 2008\Projects\TfosMnyh\TfosMnyh\Images\accept.png")
            imgDataValid.Source = imgSrc
        Else
            Dim imgSrc As Windows.Media.ImageSource
            Dim imgConvert As New Windows.Media.ImageSourceConverter
            imgSrc = imgConvert.ConvertFromString("C:\Users\William Johnston\Documents\Visual Studio 2008\Projects\TfosMnyh\TfosMnyh\Images\cancel.png")

            imgDataValid.Source = imgSrc
        End If
    End Sub

    Private Sub btnAutoDetect_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAutoDetect.Click
        Dim path As String
        path = Utilities.AutoDetectHS()
        If path = "" Then
            MsgBox("Unable to find the HymnSoft installation. Make sure that it is installed or enter the location manually.")
        Else
            txtDataLocation.Text = path
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        If Utilities.CheckHSDir(txtDataLocation.Text) = True Then
            My.Settings.HSDataDir = txtDataLocation.Text
            My.Settings.Save()

            Me.Close()
        Else
            MsgBox("The HymnSoft data directory has not been configured correctly. Please try again.")
        End If

        My.Settings.MidiDevice = cboMidiDevice.SelectedIndex

        ' This is cheating. I am setting a registry key to define the global Windows MIDI output device.
        ' This effects other programs. The reason I am doing this is to avoid parsing the midi file and sending messages
        ' at a low level. Should be fixed later.
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\Microsoft\ActiveMovie\devenum\{4EFE2452-168A-11D1-BC76-00C04FB9453B}\Default MidiOut Device", "MidiOutId", cboMidiDevice.SelectedIndex)
    End Sub

    Private Sub txtDataLocation_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles txtDataLocation.TextChanged
        checkHSDir()
    End Sub

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Window1.Loaded
        txtDataLocation.Text = My.Settings.HSDataDir
        If Utilities.CheckHSDir(txtDataLocation.Text) = False Then
            Dim path As String
            path = Utilities.AutoDetectHS
            If path <> "" Then
                txtDataLocation.Text = path
            End If
        End If
        checkHSDir()

        ' Populate the list of MIDI devices:
        Dim DeviceCount As UShort = WinMM.Midi.midiOutGetNumDevs()

        ' Check there's a sane number:
        If DeviceCount < 1 Then
            MessageBox.Show("No MIDI devices found!", Nothing, MessageBoxButton.OK, MessageBoxImage.Error)
            Exit Sub
        End If

        ' Populate the box:

        Dim DefaultId As Integer = -1
        Dim MyMidiStuff As New WinMM.ManagedMidi

        Dim i As Integer = 0
        While i < DeviceCount
            cboMidiDevice.Items.Add(MyMidiStuff.get_num_dev_name(i))
            i = i + 1
        End While
        If cboMidiDevice.Items.Count > My.Settings.MidiDevice Then
            cboMidiDevice.SelectedIndex = My.Settings.MidiDevice
        End If

    End Sub

    Private Sub btnTestMidi_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnTestMidi.Click
        Dim err As New Midi.Error
        Dim hMidiOut As Long
        Dim T As Long
        err = Midi.midiOutOpen(hMidiOut, cboMidiDevice.SelectedIndex, 0, 0, 0)
        err = Midi.midiOutShortMsg(hMidiOut, 6567325)
        System.Threading.Thread.Sleep(1000)
        err = Midi.midiOutClose(hMidiOut)

    End Sub
End Class
