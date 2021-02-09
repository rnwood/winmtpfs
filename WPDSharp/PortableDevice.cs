using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;
using WPDConstants;

namespace WPDSharp
{
    public class PortableDevice
    {
        public static PortableDevice[] GetDevices()
        {
            uint numberOfDevices = 1;
            
            PortableDeviceManager.GetDevices(null, ref numberOfDevices);
            string[] deviceIDs = new string[numberOfDevices];

            PortableDeviceManager.GetDevices(deviceIDs, ref numberOfDevices);

            return deviceIDs.Select(id => new PortableDevice(id)).ToArray();
        }

        public PortableDevice(string deviceID)
        {
            DeviceID = deviceID;
        }

        public PortableDeviceContent Content
        {
            get
            {
                IPortableDeviceContent content;
                PortableDeviceClass.Content(out content);
                return new PortableDeviceContent(this, content);
            }
        }

        public PortableDeviceContent GetContent(string path)
        {
            if (path == "\\")
            {
                return Content;
            }

            string[] names = path.Split('\\');

            return Content;
        }

        public string FriendlyName
        {
            get
            {
                uint len=0;
                PortableDeviceManager.GetDeviceFriendlyName(DeviceID, null, ref len);
                ushort[] name = new ushort[len];
                PortableDeviceManager.GetDeviceFriendlyName(DeviceID, name, ref len);
                return ToString(name, len);
            }
        }


        public string Description
        {
            get
            {
                uint len = 0;
                PortableDeviceManager.GetDeviceDescription(DeviceID, null, ref len);
                ushort[] name = new ushort[len];
                PortableDeviceManager.GetDeviceDescription(DeviceID, name, ref len);
                return ToString(name, len);
            }
        }


        public static string ToString(ushort[] name, uint len)
        {
            StringBuilder str = new StringBuilder((int) len);
            for (int i=0; i<len-1; i++)
            {
                str.Append((char) name[i]);
            }
            return str.ToString();
        }

        private static PortableDeviceManager _portableDeviceManager;

        protected static PortableDeviceManager PortableDeviceManager
        {
            get
            {
                if (_portableDeviceManager == null)
                {
                    _portableDeviceManager = new PortableDeviceManager();
                }

                return _portableDeviceManager;
            }
        }


        private PortableDeviceApiLib.PortableDeviceClass _portableDeviceClass;

        protected PortableDeviceClass PortableDeviceClass
        {
            get
            {
                if (_portableDeviceClass == null)
                {
                    _portableDeviceClass = new PortableDeviceClass();

                    PortableDeviceApiLib.IPortableDeviceValues deviceValues =
(PortableDeviceApiLib.IPortableDeviceValues)
new PortableDeviceTypesLib.PortableDeviceValuesClass();

                    // We have to provide at the least our name, version, revision
                    deviceValues.SetStringValue(
                            ref PortableDevicePKeys.WPD_CLIENT_NAME, "WPDSharp");
                    deviceValues.SetUnsignedIntegerValue(
                            ref PortableDevicePKeys.WPD_CLIENT_MAJOR_VERSION, 1);
                    deviceValues.SetUnsignedIntegerValue(
                            ref PortableDevicePKeys.WPD_CLIENT_MINOR_VERSION, 0);
                    deviceValues.SetUnsignedIntegerValue(
                            ref PortableDevicePKeys.WPD_CLIENT_REVISION, 0);

 
                    _portableDeviceClass.Open(DeviceID, deviceValues);

                }

                return _portableDeviceClass;
            }
        }

        public string DeviceID{ get; protected set; }

        public string Manufacturer
        {
            get
            {
                uint len = 0;

                PortableDeviceManager.GetDeviceManufacturer(DeviceID, null, ref len);
                ushort[] name = new ushort[len];
                PortableDeviceManager.GetDeviceManufacturer(DeviceID, name, ref len);

                return ToString(name, len);
            }
        }
    }
}
