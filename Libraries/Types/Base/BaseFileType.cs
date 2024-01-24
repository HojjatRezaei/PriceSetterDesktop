namespace PriceSetterDesktop.Libraries.Types.Base
{
    using WPFCollection.Data.Interface;
    using System.Xml;
    public abstract class BaseFileType<T> : IGeneratable where T : IXmlItem, new()
    {
        public int ElementSeed { get; set; }

        public IXmlItem CreateObject()
        {
            return (IXmlItem)this;
        }

        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = new T();
            foreach (XmlNode node in nodeList)
            {
                var searchProperty = newObject.GetType().GetProperty(node.Name);
                if (searchProperty != null)
                {
                    var newVal = Convert.ChangeType(node.InnerText, searchProperty.PropertyType);
                    searchProperty.SetValue(newObject, newVal);
                }
            }
            ElementSeed = seed;
            return newObject;
        }
        public string GenerateIdentifier()
        {
            //propertyHash;
            return "";
        }
    }
}
