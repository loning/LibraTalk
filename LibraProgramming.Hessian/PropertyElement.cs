using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace LibraProgramming.Hessian
{
    public class PropertyElement : ISerializationElement
    {
        private string propertyname;
        private int? propertyOrder;

        public Type ObjectType => Property.PropertyType;

        public PropertyInfo Property
        {
            get;
        }

        public ISerializationElement Element
        {
            get;
        }

        public int PropertyOrder
        {
            get
            {
                if (!propertyOrder.HasValue)
                {
                    var attribute = Property.GetCustomAttribute<DataMemberAttribute>();
                    propertyOrder = attribute?.Order ?? 0;
                }

                return propertyOrder.Value;
            }
        }

        public string PropertyName
        {
            get
            {
                if (String.IsNullOrEmpty(propertyname))
                {
                    var attribute = Property.GetCustomAttribute<DataMemberAttribute>();
                    propertyname = null == attribute ? Property.Name : attribute.Name;
                }

                return propertyname;
            }
        }

        public PropertyElement(PropertyInfo property, ISerializationElement element)
        {
            Property = property;
            Element = element;
        }

        public void Serialize(HessianOutputWriter writer, object graph, HessianSerializationContext context)
        {
            Element.Serialize(writer, graph, context);
        }

        public object Deserialize(HessianInputReader reader, HessianSerializationContext context)
        {
            return Element.Deserialize(reader, context);
        }
    }
}