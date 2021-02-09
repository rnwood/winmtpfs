using System;
using System.IO;
using System.Linq;
using NeoGeo.Library.SMB.Provider;
using WPDSharp;

namespace MTPFS
{
    public class ContentFSItem : FSItem
    {
        public ContentFSItem(PortableDeviceContent content)
        {
            _content = content;
        }

        private PortableDeviceContent _content;

        #region Overrides of FSItem

        public override string Name
        {
            get { return _content.Name; }
        }

        public override bool IsDirectory
        {
            get { return _content.IsFolder; }
        }

        public override long Size
        {
            get { return _content.Size; }
        }

        public override FSItem[] Children
        {
            get { return _content.GetChildren().Select(c => new ContentFSItem(c)).ToArray(); }
        }

        #endregion

        #region Overrides of FSItem

        public override NT_STATUS Create(UserContext userContext, string name, SearchFlag searchFlag, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, FileAttributes attributes, out MTPFileContext fileContext)
        {
            if (!IsDirectory)
            {
                if (fileMode == FileMode.Open)
                {
                    fileContext = new MTPFileContext(this);
                    
                    using (BinaryReader reader =  new BinaryReader(_content.OpenRead()))
                    {
                        _data = reader.ReadBytes((int) _content.Size);
                    }

                    return NT_STATUS.OK;
                } else
                {
                    fileContext = null;
                    return NT_STATUS.NOT_IMPLEMENTED;
                }
            }

            return base.Create(userContext, name, searchFlag, fileMode, fileAccess, fileShare, attributes, out fileContext);
        }


        public override NT_STATUS Close()
        {
            _data = null;

            return NT_STATUS.OK;
        }

        private byte[] _data;

        public override NT_STATUS Read(UserContext context, long offset, ref int count, ref byte[] buffer, int bufferStart)
        {
            int readLen = (int) Math.Min(count, _data.Length - offset);

            for (long i = 0; i < readLen; i++)
            {
                buffer[i + bufferStart] = _data[i+offset];
            }

            return NT_STATUS.OK;
        }

        #endregion
    }
}