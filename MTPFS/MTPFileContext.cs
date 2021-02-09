using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeoGeo.Library.SMB.Provider;

namespace MTPFS
{
    public class MTPFileContext : FileContext
    {
        public MTPFileContext(FSItem fsItem) : base(fsItem.Name, fsItem.IsDirectory)
        {
            FSItem = fsItem;
        }

        public FSItem FSItem { get; protected set; }
    }
}
