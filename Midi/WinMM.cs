using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WinMM
{

    public class Midi
    {

        #region Types

        [StructLayout(LayoutKind.Sequential)]
        public struct MIDIOUTCAPS
        {
            internal ushort wMid;
            internal ushort wPid;
            internal int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            internal string szPname;
            internal ushort wTechnology;
            internal ushort wVoices;
            internal ushort wNotes;
            internal ushort wChannelMask;
            internal int dwSupport;
        }

        #endregion

        #region Enums

        public enum Error
        {
            MMSYSERR_NOERROR = 0,
            MIDIERR_NODEVICE = (MIDIERR_BASE + 4),
            MMSYSERR_ALLOCATED = (MMSYSERR_BASE + 4),
            MMSYSERR_BADDEVICEID = (MMSYSERR_BASE + 2),
            MMSYSERR_INVALPARAM = (MMSYSERR_BASE + 11),
            MMSYSERR_NOMEM = (MMSYSERR_BASE + 7),
            MIDIERR_STILLPLAYING = (MIDIERR_BASE + 1),
            MMSYSERR_INVALHANDLE = (MMSYSERR_BASE + 5),
            MMSYSERR_BADERRNUM = (MMSYSERR_BASE + 9)
        };

        public enum Message
        {
            KeyRelease = 0x80,
            KeyPress = 0x90,
            KeyAfterTouch = 0xA0,
            ControlChange = 0xB0,
            PatchChange = 0xC0,
            ChannelAfterTouch = 0xD0,
            PitchWheelChange = 0xE0
        };

        #endregion

        #region Constants

        private const int MMSYSERR_BASE = 0;
        private const int MIDIERR_BASE = 64;
        private const int MAXPNAMELEN = 32;

        #endregion

        #region Methods


        [DllImport("winmm.dll")]
        public static extern Error midiOutOpen(ref int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);

        [DllImport("winmm.dll")]
        public static extern Error midiOutClose(int hMidiOut);

        [DllImport("winmm.dll")]
        public static extern Error midiOutShortMsg(int hMidiOut, int dwMsg);

        [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsA")]
        public static extern Error midiOutGetDevCaps(int uDeviceID, ref MIDIOUTCAPS lpCaps, int uSize);

        [DllImport("winmm.dll")]
        public static extern ushort midiOutGetNumDevs();

        public List<string> get_midi_devices()
        {
            // Populate the list of MIDI devices:
            ushort DeviceCount = WinMM.Midi.midiOutGetNumDevs();

            List<string> ret = null;

            // Check there's a sane number:
            //if (DeviceCount < 1)
            //{
            //    ret[0] = "";
            //    return ret; // No midi devices
            //}


            // Populate the box:

            for (int i = 0; i < DeviceCount; ++i)
            {
                WinMM.Midi.MIDIOUTCAPS Caps = default(WinMM.Midi.MIDIOUTCAPS);
                WinMM.Midi.Error Error = WinMM.Midi.midiOutGetDevCaps(i, ref Caps, Marshal.SizeOf(Caps));
                if (Error == WinMM.Midi.Error.MMSYSERR_NOERROR)
                {
                    ret[i]  = Caps.szPname;
                }
                else
                {
                    ret[i] = "Device #" + i.ToString() + ": " + Error.ToString();
                }
            }

            return ret;
        }

        #endregion
    }

    public class ManagedMidi
    {
        public string get_num_dev_name(int DeviceID)
        {
            Midi.MIDIOUTCAPS Caps = default(Midi.MIDIOUTCAPS);

            Midi.Error Error = Midi.midiOutGetDevCaps(DeviceID, ref Caps, Marshal.SizeOf(Caps));

            if (Error == Midi.Error.MMSYSERR_NOERROR)
            {
                return Caps.szPname;
            }
            else
            {
                return "Device #" + DeviceID + ": " + Error.ToString();
            }
        }


    }
}
