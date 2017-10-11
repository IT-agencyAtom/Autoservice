using ConstaSoft.Core.Controls.Managers;
using GalaSoft.MvvmLight;
using MahApps.Metro.IconPacks;

namespace Autoservice.Controls.Managers
{
    /// <summary>
    /// Common model for screens in application.</summary>
    /// <remarks>
    /// Model contains common objects, using for display screen 
    /// (icon, label, panel model, screen model, tooltip)</remarks>
    public class ScreenManager : ViewModelBase
    {
        public ScreenManager()
        {
            IsEnabled = true;
        }

        private PanelViewModelBase _tag;

        /// <summary>
        /// Screen model property</summary>
        public PanelViewModelBase Tag
        {
            get { return _tag; }
            set
            {
                if (_tag == value)
                    return;
                _tag = value;
                RaisePropertyChanged("Tag");
            }
        }

        public bool IsEnabled { get; set; }

        /// <summary>
        /// Panel model property</summary>
        public ViewModelBase Panel => _tag?.Panel;

        private PackIconMaterial _icon;
        /// <summary>
        /// Screen icon property</summary>
        public PackIconMaterial Icon
        {
            get { return _icon; }
            set
            {
                if (Equals(_icon, value))
                    return;
                _icon = value;
                RaisePropertyChanged("Icon");
            }
        }
        
        private string _label;
        /// <summary>
        /// Screen label property</summary>
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label == value)
                    return;
                _label = value;
                RaisePropertyChanged("Label");
            }
        }

        private string _toolTip;
        /// <summary>
        /// Screen tooltip property</summary>
        public string ToolTip
        {
            get { return _toolTip; }
            set
            {
                if (_toolTip == value)
                    return;
                _toolTip = value;
                RaisePropertyChanged("ToolTip");
            }
        }
    }
}