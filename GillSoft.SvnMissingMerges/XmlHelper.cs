using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GillSoft.SvnMissingMerges
{
    internal static class XmlHelper
    {
        public static XmlElement AddElement(this XmlElement element, string name)
        {
            var res = element.OwnerDocument.CreateElement(name);
            element.AppendChild(res);
            return res;
        }

        public static XmlElement AddAttribute(this XmlElement element, string name, object value)
        {
            var attr = element.OwnerDocument.CreateAttribute(name);
            attr.Value = string.Empty + value;
            element.Attributes.Append(attr);
            return element;
        }

    }
}
