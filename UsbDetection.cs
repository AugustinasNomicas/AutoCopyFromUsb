using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AutoCopyFromUsb
{
    class UsbDetection
    {
        const int WM_DEVICECHANGE = 0x0219; //see msdn site
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEREMOVALCOMPLETE = 0x8004;
        const int DBT_DEVTYPVOLUME = 0x00000002;

        [StructLayout(LayoutKind.Sequential)] //Same layout in mem
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }

        private static char DriveMaskToLetter(int mask)
        {
            const string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; //1 = A, 2 = B, 3 = C
            var cnt = 0;
            var pom = mask / 2;
            while (pom != 0)    // while there is any bit set in the mask shift it right        
            {
                pom = pom / 2;
                cnt++;
            }
            char letter = cnt < drives.Length ? drives[cnt] : '?';
            return letter;
        }

        public string GetNewDeviceLetter(Message m)
        {
            var result = string.Empty;
            if (m.Msg != WM_DEVICECHANGE) return result;
            var strct = Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));

            if (strct == null || strct.GetType() != typeof(DEV_BROADCAST_VOLUME))
                return result;

            var vol = (DEV_BROADCAST_VOLUME)strct;
            if ((m.WParam.ToInt32() == DBT_DEVICEARRIVAL) && (vol.dbcv_devicetype == DBT_DEVTYPVOLUME))
            {
                result = DriveMaskToLetter(vol.dbcv_unitmask).ToString(CultureInfo.InvariantCulture);
            }
            return result;
        }
    }
}
