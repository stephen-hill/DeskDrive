// Copyright (c) 2008 Blue Onion Software
// All rights reserved

namespace BlueOnion
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Resources;

    public sealed class XmlResourceManager : ResourceManager, IDisposable
    {
        Stream stream;

        public XmlResourceManager(Stream resourceStream)
        {
            if (resourceStream == null)
                throw new ArgumentNullException("resourceStream");

            if (resourceStream.CanRead == false)
                throw new ArgumentException("Not readable", "resourceStream");

            if (resourceStream.CanSeek == false)
                throw new ArgumentException("Not seekable", "resourceStream");

            stream = resourceStream;
            ResourceSets = new Hashtable();
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");

            if (ResourceSets.Contains(culture.Name) == false && createIfNotExists)
            {
                stream.Position = 0;
                ResourceSets.Add(culture.Name, new XmlResourceSet(stream, culture));
            }

            return (XmlResourceSet)ResourceSets[culture.Name];
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }

            ReleaseAllResources();
            GC.SuppressFinalize(this);
        }
    }
}
