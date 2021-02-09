using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;

namespace WPDSharp
{
    public class PortableDeviceContent
    {
        internal PortableDeviceContent(PortableDevice device, IPortableDeviceContent content)
        {
            Device = device;
            _content = content;
            _contentID = "DEVICE";

        }

        public PortableDeviceContent(PortableDeviceContent parent, string contentID)
        {
            Parent = parent;
            Device = parent.Device;
            _content = parent._content;
            _contentID = contentID;
        }

        public PortableDeviceContent Parent { get; protected set; }

        private readonly string _contentID;

        public PortableDevice Device
        {
            get; protected set;
        }


        public Stream OpenRead()
        {
            IPortableDeviceResources res;
            _content.Transfer(out res);

            uint size = 0;
            IStream stream;

            res.GetStream(ContentID, ref WPDConstants.PortableDevicePKeys.WPD_RESOURCE_DEFAULT, 0, ref size, out stream);
            System.Runtime.InteropServices.ComTypes.IStream istream = (System.Runtime.InteropServices.ComTypes.IStream)stream;
            return new ComIStreamWrapper(istream);
        }

        public PortableDeviceContent[] GetChildren()
        {
            List<PortableDeviceContent> children = new List<PortableDeviceContent>();

            PortableDeviceApiLib.IEnumPortableDeviceObjectIDs objectIDs;
            _content.EnumObjects(0, ContentID, null, out objectIDs);

            uint fetched = 0;
            do
            {
                string childContentID;
                objectIDs.Next(1, out childContentID, ref fetched);

                if (fetched > 0)
                {
                    children.Add(new PortableDeviceContent(this, childContentID));
                }
            } while (fetched > 0);

            return children.ToArray();
        }

        public string Name
        {
            get
            {
                string val;
                PropertyValues.GetStringValue(ref WPDConstants.PortableDevicePKeys.WPD_OBJECT_NAME, out val);
                return val;
            }
        }

        public string FullName
        {
            get
            {
                return Parent != null ? (Parent.Name + "\\" + Name) : Name;
            }
        }

        public string OriginalFileName
        {
            get
            {
                string val;
                PropertyValues.GetStringValue(ref WPDConstants.PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, out val);
                return val;
            }
        }

        public long Size
        {
            get
            {
                if (IsFolder)
                    return 0;

                try
                {
                    long val;
                    PropertyValues.GetSignedLargeIntegerValue(ref WPDConstants.PortableDevicePKeys.WPD_OBJECT_SIZE,
                                                              out val);
                    return (long) val;
                } catch (COMException e)
                {
                    if (e.ErrorCode == -2147023728)
                    {
                        return 0;
                    } else
                    {
                        throw;
                    }
                }
            }
        }

        public bool IsFolder
        {
            get
            {
                return ContentType == WPDConstants.PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER || ContentType == WPDConstants.PortableDeviceGuids.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT;
            }
        }

        public Guid ContentType
        {
            get
            {
                Guid val;
                PropertyValues.GetGuidValue(ref WPDConstants.PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, out val);

                return val;
            }
        }

        private IPortableDeviceProperties _properties;

        protected IPortableDeviceProperties Properties
        {
            get
            {
                if (_properties == null)
                {
                    _content.Properties(out _properties);
                }

                return _properties;
            }
        }

        private IPortableDeviceValues _propertyValues;
        protected IPortableDeviceValues PropertyValues
        {
            get
            {
                if (_propertyValues == null)
                {
                    Properties.GetValues(ContentID, PropertyKeys, out _propertyValues);
                }

                return _propertyValues;
            }
        }

        public string ContentID
        {
            get { return _contentID; }
        }

        private IPortableDeviceKeyCollection _propertyKeys;
        protected IPortableDeviceKeyCollection PropertyKeys
        {
            get
            {
                if (_propertyKeys == null)
                {
                    Properties.GetSupportedProperties(ContentID, out _propertyKeys);
                }

                return _propertyKeys;
            }
        }

        protected IPortableDeviceContent _content;
    }
}