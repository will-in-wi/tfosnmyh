

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
        ' Preset with saved setting later
        'Try
        ' DefaultId = Me.DefaultMidiDevice
        'Catch ex As Exception
        'MessageBox.Show("Error reading default MIDI output device ID: " + ex.Message, Nothing, MessageBoxButton.OK, MessageBoxImage.Error)
        'End Try

        Dim i As Integer = 0
        While i < DeviceCount
            Dim caps As WinMM.Midi.MIDIOUTCAPS = Nothing

            Dim err As WinMM.Midi.err = WinMM.Midi.midiOutGetDevCaps(i, caps, Runtime.InteropServices.Marshal.SizeOf(caps))

            If err = WinMM.Midi.err.MMSYSERR_NOERROR Then
                cboMidiDevice.Items.Add(caps.szPname)
            Else
                cboMidiDevice.Items.Add("Device #" & i.ToString & ": " & err.ToString())
            End If

            i += 1
        End While

        'for (int i = 0; i < DeviceCount; ++i) {
        '		WinMM.Midi.MIDIOUTCAPS Caps = default(WinMM.Midi.MIDIOUTCAPS);
        '		WinMM.Midi.Error Error = WinMM.Midi.midiOutGetDevCaps(i, ref Caps, Marshal.SizeOf(Caps));
        '		if (Error == WinMM.Midi.Error.MMSYSERR_NOERROR) {
        '			this.MidiOutDevicePicker.Items.Add(Caps.szPname);
        '		} else {
        '			this.MidiOutDevicePicker.Items.Add("Device #" + i.ToString() + ": " + Error.ToString());
        '		}
        '	}
        '	if (DefaultId < this.MidiOutDevicePicker.Items.Count && DefaultId > -2) this.MidiOutDevicePicker.SelectedIndex = DefaultId;
        '}

        'Dim myMidi As New Midi
        'myMidi.PlayMidi()
    End Sub
End Class
