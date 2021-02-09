using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeoGeo.Library.SMB.Provider;

namespace MTPFS
{
    public abstract class FSItem
    {
        public abstract string Name
        {
            get;
        }

        public abstract bool IsDirectory
        {
            get;
        }

        public abstract long Size
        {
            get;
        }

        public abstract FSItem[] Children
        {
            get;
        }

        public FSItem GetItem(string name)
        {
            if (name == "\\" || name == "")
            {
                return this;
            }

            if (name.StartsWith("\\"))
            {
                name = name.Remove(0, 1);
            }

            string[] nameparts = name.Split(new char[] {'\\'}, 2);

            FSItem child = Children.FirstOrDefault(c => nameparts[0] == c.Name);

            if (child != null)
            {
                if (nameparts.Length > 1)
                {
                    return child.GetItem("\\" + nameparts[1]);
                } else
                {
                    return child;
                }
            }

            return null;
        }

        public virtual NT_STATUS Rename(UserContext context, string name)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS Flush(UserContext context)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS Write(UserContext userContext, long offset, ref int count, ref byte[] buffer, int bufferStart)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS Read(UserContext context, long offset, ref int count, ref byte[] buffer, int bufferStart)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS DeleteDirectory(UserContext userContext)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS CreateChildDirectory(UserContext userContext, string name, FileAttributes attributes)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public virtual NT_STATUS SetAttributes(UserContext context, DirectoryContext attr)
        {
            return NT_STATUS.NOT_IMPLEMENTED;
        }

        public NT_STATUS GetAttributes(UserContext context, out DirectoryContext attr)
        {
            attr = GetDirectoryContext();

            return NT_STATUS.OK;
        }

        protected DirectoryContext GetDirectoryContext()
        {
            lock (this)
            {
                DirectoryContext attr = new DirectoryContext(Name,
                                                             GetFileAttributes())
                {
                    FileSize = Size
                };
                return attr;
            }
        }

        protected FileAttributes GetFileAttributes()
        {
            lock (this)
            {
                return IsDirectory
                           ? FileAttributes.Directory
                           : FileAttributes.Normal;
            }
        }

        public virtual NT_STATUS ReadDirectory(UserContext userContext, MTPFileContext fileContext)
        {
            foreach (FSItem child in Children)
            {
                fileContext.Items.Add(child.GetDirectoryContext());
            }

            return NT_STATUS.OK;
        }

        public virtual NT_STATUS Delete(UserContext context)
        {
            return NT_STATUS.NO_SUCH_FILE;
        }

        public virtual NT_STATUS Close()
        {
            return NT_STATUS.OK;
        }

        public virtual NT_STATUS Create(UserContext userContext, string name, SearchFlag searchFlag, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, FileAttributes attributes, out MTPFileContext fileContext)
        {
            if (IsDirectory)
            {
                fileContext = new MTPFileContext(this);

                return NT_STATUS.OK;
            }

            fileContext = null;
            return NT_STATUS.NOT_IMPLEMENTED;
        }
    }
}
