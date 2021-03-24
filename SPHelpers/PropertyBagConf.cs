using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPHelpers
{
    public class PropertyBagConf<T>
    {
        public static T Get(Hashtable properties, string propertyName)
        {
            T objectFromProperty;
            Hashtable objectProperties = properties;
            if (!objectProperties.ContainsKey(propertyName))
            {
                Type genericType = typeof(T);
                objectFromProperty = (T)Activator.CreateInstance(genericType);
            }
            else
            {
                string propertyValue = properties[propertyName].ToString();
                objectFromProperty = JsonConvert.DeserializeObject<T>(propertyValue);
            }
            return objectFromProperty;
        }
        public static void Set(Hashtable properties, string propertyName, T propertyValue)
        {
            string propertyValueString = JsonConvert.SerializeObject(propertyValue, Formatting.Indented);
            properties[propertyName] = propertyValueString;
        }
    }
}
