using Autoservice.DAL.Entities;
using Autoservice.DAL.Services;
using Autoservice.ViewModel.Utils;
using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoservice.Dialogs.Managers
{
    public class PurchaseManager : PanelViewModelBase
    {
        public Action OnExit { get; set; }

        public string Title { get; set; }

        public RelayCommand Save { get; private set; }

        public RelayCommand Cancel { get; private set; }

        private bool _isEdit { get; set; }

        public ObservableCollection<SparePart> SpareParts { get; set; }

        public void Initialize()
        {
            Panel = new PanelManager
            {
                RightButtons = new ObservableCollection<PanelButtonManager>
                {
                    new PanelButtonManager
                    {
                        OnButtonAction = (obj) => Export(),
                        ButtonIcon = "appbar_printer_text",
                        ButtonText = "Экспорт"
                    }
                }
            };
            Title = "Закупка запчастей";
        }

        private void Export()
        {
            string pdfsFolderPath = "PDFs";
            string templatePath = @"Templates\ListSP.docx";
            if (!Directory.Exists(pdfsFolderPath))
                Directory.CreateDirectory(pdfsFolderPath);

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Microsoft.Office.Interop.Word.Document aDoc = null;

            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
                aDoc =
                    wordApp.Documents.Open(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templatePath),
                        ReadOnly: false, Visible: false);
                aDoc.Activate();
                WriteSpareParts(wordApp);
                var ext = "pdf";
                var outputFilePath = string.Format("{0}{2}List-{1}.{3}", pdfsFolderPath, DateTime.Now.ToShortDateString(),
                    System.IO.Path.DirectorySeparatorChar, ext);
                var subIndex = 0;
                while (File.Exists(outputFilePath))
                {
                    outputFilePath = string.Format("{0}{2}List-{1}-{4:00}.{3}", pdfsFolderPath, DateTime.Now.ToShortDateString(),
                        System.IO.Path.DirectorySeparatorChar, ext, ++subIndex);
                }
                //aDoc.SaveAs(outputFilePath, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                aDoc.ExportAsFixedFormat(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputFilePath), WdExportFormat.wdExportFormatPDF);
                Process.Start(outputFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while generate pdf!");
            }
            finally
            {
                aDoc?.Close(false);

                wordApp?.Quit();
            }
        }
        private void WriteSpareParts(Application wordApp)
        {
            int COUNT_OF_ROWS = 30;            
            for (int i = SpareParts.Count-1; i >= 0; i--)
            {
                if (SpareParts[i] == null)
                    continue;
                WordTemplateHelper.ReplaceLarge(wordApp, $"#name{i}#", SpareParts[i].Name);            
                WordTemplateHelper.ReplaceLarge(wordApp, $"#count{i}#", SpareParts[i].Deficit.ToString());
            }
            for (int i = SpareParts.Count; i < COUNT_OF_ROWS; i++)
            {
                WordTemplateHelper.ReplaceLarge(wordApp, $"#name{i}#", "");
                WordTemplateHelper.ReplaceLarge(wordApp, $"#count{i}#", "");
            }
        }

        private void CancelHandler()
        {
            OnExit();
        }

        public override async void Refresh()
        {
            SetIsBusy(true);
            var service = Get<IGeneralService>();
            SpareParts = new ObservableCollection<SparePart>(await System.Threading.Tasks.Task.Run(() => service.GetAllSpareParts().Where(s => s.Deficit > 0)));
            RaisePropertyChanged("SpareParts");
            SetIsBusy(false);
        }
    }
}
