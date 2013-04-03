using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace NotBooksCore
{
    /// <summary>
    /// Summary description for xmlutil
    /// </summary>
    public static class XmlUtil
    {
        public static bool XENull(XElement xe, string node)
        {
            return XENull(xe, node, false);
        }

        public static bool XENull(XElement xe, string x, bool attribute)
        {
            if (attribute)
            {
                if (xe.Attribute(x) == null) return true;
                return xe.Attribute(x).Value == null;
            }
            else
            {
                if (xe.Element(x) == null) return true;
                return xe.Element(x).Value == null;
            }
        }
    }
}
