using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.ViewModel.Utils
{
public static class EnumExtender
    {
        public static string ToDescriptionString(this Enum enumerate)
        {
            var type = enumerate.GetType();
            var fieldInfo = type.GetField(enumerate.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : enumerate.ToString();
        }

        public static string[] GetAllDescriptions (Type type)
        {
            var values = Enum.GetValues(type);
            List<string> strings = new List<string>();
            foreach (var value in values)
            {
                strings.Add((value as Enum).ToDescriptionString());
            }
            return strings.ToArray();
        }
    }
}
