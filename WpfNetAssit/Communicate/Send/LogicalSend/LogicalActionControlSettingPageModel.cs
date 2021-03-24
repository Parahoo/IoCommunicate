using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfNetAssit.LogicalAction.ControlAction;

namespace WpfNetAssit.Communicate.Send.LogicalSend
{
    public class LogicalActionControlSettingPageModel : ViewModelBase
    {
        private ObservableCollection<string> avaiableTemplates = new ObservableCollection<string>();
        public ObservableCollection<string> AvaiableTemplates
        {
            get { return avaiableTemplates; }
            set { Set("AvaiableTemplates", ref avaiableTemplates, value); }
        }

        string path = "./LogicalActions";

        public bool IsCoverSave { get; set; } = false;
        private string newFileName = "";
        public string NewFileName
        {
            get { return newFileName; }
            set { Set("NewFileName", ref newFileName, value); IsCoverSave = CheckCoverSave(newFileName); }
        }

        private bool CheckCoverSave(string fileName)
        {
            string target = (fileName.EndsWith(".xaml") ? fileName.Substring(0, fileName.Length - 5) : fileName);
            foreach (var item in avaiableTemplates)
            {
                if (item.Equals(target))
                    return true;
            }
            return false;
        }

        public bool IsSelected { get; set; } = false;

        private string selectFileName = null;
        public string SelectFileName
        {
            get { return selectFileName; }
            set { Set("SelectFileName", ref selectFileName, value);IsSelected = (SelectFileName != null); }
        }



        public ICommand ApplyTemplateCommand { get; }
        public ICommand SaveTemplateCommand { get; }
        public ICommand CoverSaveTemplateCommand { get; }

        public ICommand ImportTemplateFileCommand { get; }
        public ICommand DeleteTemplateFileCommand { get; }

        public ControlAction RootAction { get; set; }

        public LogicalActionControlSettingPageModel()
        {
            LoadAvaiableActionTemplate();
            ApplyTemplateCommand = new RelayCommand(ApplyTemplate);
            SaveTemplateCommand = new RelayCommand(SaveTemplate);
            CoverSaveTemplateCommand = new RelayCommand(CoverSaveTemplate);
            ImportTemplateFileCommand = new RelayCommand(ImportTemplateFile);
            DeleteTemplateFileCommand = new RelayCommand(DeleteTemplateFile);
        }

        private void ImportTemplateFile()
        {
            LoadAvaiableActionTemplate();
        }

        private void DeleteTemplateFile()
        {
            string filename = GetFullPath(SelectFileName);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            LoadAvaiableActionTemplate();
        }

        string GetFullPath(string filename)
        {
            if (filename == null)
                return "";
            return path + "/" + (filename.EndsWith(".xaml") ? filename : (filename + ".xaml"));
        }

        private Action<ControlAction> Apply;

        private void ApplyTemplate()
        {
            string filename = GetFullPath(SelectFileName);
            if(File.Exists(filename))
            {
                ControlActionBuilder builder = XamlSerializeHelper<ControlActionBuilder>.Load(filename);
                Apply?.Invoke(builder.Build() as ControlAction);
            }
        }

        private void SaveTemplate()
        {
            string filename = GetFullPath(NewFileName);
            bool bSave = true;
            if (File.Exists(filename))
                bSave = false;

            if(bSave)
            {
                CoverSaveTemplate();
            }
        }

        private void CoverSaveTemplate()
        {
            string filename = GetFullPath(NewFileName);

            ControlActionBuilder builder = RootAction.SerializeBuilder() as ControlActionBuilder;
            XamlSerializeHelper<ControlActionBuilder>.Save(filename, builder);
            LoadAvaiableActionTemplate();
        }

        internal void Open(Action<ControlAction> p)
        {
            LoadAvaiableActionTemplate();
            Apply = p;
        }

        private void LoadAvaiableActionTemplate()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var files = Directory.GetFiles(path, "*.xaml");
            List<string> names = new List<string>();
            foreach (var item in files)
            {
                names.Add(Path.GetFileNameWithoutExtension(item));
            }
            AvaiableTemplates = new ObservableCollection<string>(names);
        }
    }
}
