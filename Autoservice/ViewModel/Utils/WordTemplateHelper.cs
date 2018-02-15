using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.ViewModel.Utils
{
    public static class WordTemplateHelper
    {
        public static void ReplaceLarge(Microsoft.Office.Interop.Word.Application wordApp, string text, string replacement)
        {
            if (replacement == null)
                replacement = "";

            List<string> subs = new List<string>();
            int counter = 0;
            while (counter <= replacement.Length)
            {
                if (replacement.Length < counter + 250)
                {
                    subs.Add(replacement.ToString().Substring(counter, replacement.Length - counter));
                }
                else
                {
                    subs.Add(replacement.Substring(counter, 250) + "#r#");
                }
                counter += 250;
            }

            Microsoft.Office.Interop.Word.Find fnd = wordApp.ActiveWindow.Selection.Find;

            fnd.ClearFormatting();
            fnd.Replacement.ClearFormatting();
            fnd.Forward = true;

            fnd.Text = text;
            fnd.Wrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindStop;
            fnd.Replacement.Text = subs[0];
            fnd.Execute(Forward: true, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            fnd.Text = "#r#";
            for (int i = 1; i < subs.Count; i++)
            {
                fnd.Replacement.Text = subs[i];
                fnd.Execute(Forward: true, Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);
            }
        }
    }
}
