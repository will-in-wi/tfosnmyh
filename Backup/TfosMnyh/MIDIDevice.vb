' Ported to VB.Net from C#. Original code here: http://www.benryves.com/products/vistamidi

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices

Namespace WinMM

    Public Class Midi

#Region "Types"

        <StructLayout(LayoutKind.Sequential)> _
        Public Structure MIDIOUTCAPS
            Public wMid As UShort
            Public wPid As UShort
            Public vDriverVersion As Integer
            '<MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)> _
            Public szPname As String
            Public wTechnology As UShort
            Public wVoices As UShort
            Public wNotes As UShort
            Public wChannelMask As UShort
            Public dwSupport As Integer
        End Structure

#End Region

#Region "Enums"

        Public Enum err
            MMSYSERR_NOERROR = 0
            MIDIERR_NODEVICE = (MIDIERR_BASE + 4)
            MMSYSERR_ALLOCATED = (MMSYSERR_BASE + 4)
            MMSYSERR_BADDEVICEID = (MMSYSERR_BASE + 2)
            MMSYSERR_INVALPARAM = (MMSYSERR_BASE + 11)
            MMSYSERR_NOMEM = (MMSYSERR_BASE + 7)
            MIDIERR_STILLPLAYING = (MIDIERR_BASE + 1)
            MMSYSERR_INVALHANDLE = (MMSYSERR_BASE + 5)
            MMSYSERR_BADERRNUM = (MMSYSERR_BASE + 9)
        End Enum

        Public Enum Message
            KeyRelease = &H80
            KeyPress = &H90
            KeyAfterTouch = &HA0
            ControlChange = &HB0
            PatchChange = &HC0
            ChannelAfterTouch = &HD0
            PitchWheelChange = &HE0
        End Enum

#End Region

#Region "Constants"

        Private Const MMSYSERR_BASE As Integer = 0
        Private Const MIDIERR_BASE As Integer = 64
        Private Const MAXPNAMELEN As Integer = 32

#End Region

#Region "Methods"

        '[DllImport("winmm.dll")]
        'public static extern Error midiOutOpen(ref int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);

        Public Declare Function midiOutOpen Lib "winmm.dll" (ByRef lphMidiOut As Integer, ByVal uDeviceID As Integer, ByVal dwInstance As Integer, ByVal dwFlags As Integer) As err

        '[DllImport("winmm.dll")]
        'public static extern Error midiOutClose(int hMidiOut);

        Public Declare Function midiOutClose Lib "winmm.dll" (ByVal hMidiOut As Integer) As err

        '[DllImport("winmm.dll")]
        'public static extern Error midiOutShortMsg(int hMidiOut, int dwMsg);

        Public Declare Function midiOutShortMsg Lib "winmm.dll" (ByVal hMidiOut As Integer, ByVal dwMsg As Integer) As err

        '[DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
        'public static extern Error midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS lpCaps, int uSize);

        Public Declare Function midiOutGetDevCaps Lib "winmm.dll" Alias "midiOutGetDevCapsA" (ByVal uDeviceID As Integer, ByRef lpCaps As MIDIOUTCAPS, ByVal uSize As Integer) As err

        '[DllImport("winmm.dll")]
        'public static extern ushort midiOutGetNumDevs();

        Public Declare Function midiOutGetNumDevs Lib "winmm.dll" () As UShort

#End Region
	End Class 
End Namespace 
