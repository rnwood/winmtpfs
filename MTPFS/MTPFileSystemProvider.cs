using System;
using System.Collections.Generic;
using System.IO;
using NeoGeo.Library.SMB.Provider;

namespace MTPFS
{
    class MTPFileSystemProvider : FileSystemProvider
    {
        public MTPFileSystemProvider()
        {
            _topItem = new TopLevelFSItem();
        }

        private readonly TopLevelFSItem _topItem;

        public override string Name
        {
            get
            {
                return "blah";
            }
        }

        #region Overrides of FileSystemProvider

        public override NT_STATUS Create(UserContext UserContext, string Name, SearchFlag SearchFlags, FileMode Mode, FileAccess Access, FileShare Share, FileAttributes Attributes, out FileContext fileContext)
        {
            lock (this)
            {
                FSItem item = _topItem.GetItem(Name);

                if (item != null)
                {
                    MTPFileContext mtpFileContext;
                    NT_STATUS status = item.Create(UserContext, Name, SearchFlags, Mode, Access, Share, Attributes, out mtpFileContext);
                    fileContext = mtpFileContext;
                    return status;
                }

                fileContext = null;
                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS Close(FileContext FileObject)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext) FileObject).FSItem;
                return item.Close();
            }
        }

        public override NT_STATUS Close(FileContext FileObject, DateTime LastWriteTime)
        {
            return Close(FileObject);
        }

        public override NT_STATUS Delete(UserContext UserContext, string FileName)
        {
            lock (this)
            {
                FSItem item = _topItem.GetItem(Name);

                if (item != null)
                {
                    return item.Delete(UserContext);
                }

                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS GetAttributes(UserContext UserContext, string Name, out DirectoryContext Attr)
        {
            lock (this)
            {
                FSItem item = _topItem.GetItem(Name);

                if (item != null)
                {
                    return item.GetAttributes(UserContext, out Attr);
                }

                Attr = null;
                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS FSInfo(UserContext UserContext, out FileSystemAttributes data)
        {
            return base.FSInfo(UserContext, out data);
        }

        public override NT_STATUS GetAttributes(UserContext UserContext, FileContext FileObject, out DirectoryContext Attr)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.GetAttributes(UserContext, out Attr);
            }
        }

        public override NT_STATUS SetAttributes(UserContext UserContext, FileContext FileObject, DirectoryContext Attr)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.SetAttributes(UserContext, Attr);
            }
        }

        public override NT_STATUS GetStreamInfo(UserContext UserContext, string Name, out List<DirectoryContext> StreamInfo)
        {
            lock (this)
            {
                FSItem item = GetItem(Name);

                if (item != null)
                {
                    StreamInfo = new List<DirectoryContext>();
                    return NT_STATUS.OK;
                }

                StreamInfo = null;
                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS ReadDirectory(UserContext UserContext, FileContext FileObject)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.ReadDirectory(UserContext, (MTPFileContext)FileObject);
            }
        }

        public override NT_STATUS CreateDirectory(UserContext UserContext, string DirName, FileAttributes Attributes)
        {
            lock (this)
            {
                string parent = Path.GetDirectoryName(DirName);
                string name = Path.GetFileName(parent);
                FSItem item = GetItem(DirName);

                if (item != null)
                {
                    return item.CreateChildDirectory(UserContext, name, Attributes);
                }

                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS DeleteDirectory(UserContext UserContext, string DirName)
        {
            lock (this)
            {
                FSItem item = GetItem(DirName);

                if (item != null)
                {
                    return item.DeleteDirectory(UserContext);
                }

                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        public override NT_STATUS Read(UserContext UserContext, FileContext FileObject, long Offset, ref int Count, ref byte[] Buffer, int BufferStart)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.Read(UserContext, Offset, ref Count, ref Buffer, BufferStart);
            }
        }

        public override NT_STATUS Write(UserContext UserContext, FileContext FileObject, long Offset, ref int Count, ref byte[] Buffer, int BufferStart)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.Write(UserContext, Offset, ref Count, ref Buffer, BufferStart);
            }
        }

        public override NT_STATUS Lock(UserContext UserContext, FileContext FileObject, long Offset, long Length)
        {
            lock (this)
            {
                return NT_STATUS.NOT_SUPPORTED;
            }
        }

        public override NT_STATUS Unlock(UserContext UserContext, FileContext FileObject, long Offset, long Length)
        {
            lock (this)
            {
                return NT_STATUS.NOT_SUPPORTED;
            }
        }

        public override NT_STATUS DeviceIO(UserContext UserContext, FileContext FileObject, int Command, bool IsFsctl, ref byte[] Input, ref byte[] Output, ref int ValidLength)
        {
            lock (this)
            {
                return NT_STATUS.NOT_SUPPORTED;
            }
        }

        public override NT_STATUS Flush(UserContext UserContext, FileContext FileObject)
        {
            lock (this)
            {
                FSItem item = ((MTPFileContext)FileObject).FSItem;
                return item.Flush(UserContext);
            }
        }


        public override NT_STATUS Rename(UserContext UserContext, string OldName, string NewName)
        {
            lock (this)
            {
                FSItem item = GetItem(OldName);

                if (item != null)
                {
                    return item.Rename(UserContext, NewName);
                }

                return NT_STATUS.NO_SUCH_FILE;
            }
        }

        private FSItem GetItem(string name)
        {
            return _topItem.GetItem(name);
        }

        public override string FileSystemProviderType
        {
            get { return "blah"; }
            set { }
        }

        #endregion
    }
}