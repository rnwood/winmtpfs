using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTPFS
{
    public class TopLevelFSItem : FSItem
    {
        #region Overrides of FSItem

        public override string Name
        {
            get { return ""; }
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
            get
            {
                return WPDSharp.PortableDevice.GetDevices().Select(dev => new DeviceFSItem(dev)).ToArray();
            }
        }

        #endregion
    }
}
