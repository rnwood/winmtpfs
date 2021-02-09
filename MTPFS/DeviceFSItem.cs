using System;
using System.Collections.Generic;
using System.Text;
using WPDSharp;

namespace MTPFS
{
    public class DeviceFSItem : FSItem
    {
        public DeviceFSItem(PortableDevice device)
        {
            this._device = device;
        }

        private WPDSharp.PortableDevice _device;

        #region Overrides of FSItem

        public override string Name
        {
            get { return _device.FriendlyName; }
        }

        public override bool IsDirectory
        {
            get { return true; }
        }

        public override long Size
        {
            get { return 0; }
        }

        public override FSItem[] Children
        {
            get { return new FSItem[]{new ContentFSItem(_device.Content)}; }
        }

        #endregion
    }
}
