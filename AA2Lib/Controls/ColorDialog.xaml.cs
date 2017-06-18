// --------------------------------------------------
// AA2Lib - ColorDialog.xaml.cs
// --------------------------------------------------

using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AA2Lib.Code;
using Point = System.Windows.Point;
using Color = ReiFX.Color;

namespace AA2Lib.Controls
{
    /// <summary>
    ///     Interaction logic for ColorDialog.xaml
    /// </summary>
    public partial class ColorDialog : Window, INotifyPropertyChanged
    {
        private byte _alpha;
        private byte _blue;
        private Color _color;
        private byte _green;
        private double _hue;
        private bool _mDown;
        private byte _red;
        private double _saturation;
        private double _value;

        public byte Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                UpdateAlpha();
                OnPropertyChanged("Alpha");
            }
        }

        public byte Blue
        {
            get { return _blue; }
            set
            {
                _blue = value;
                UpdateRgb();
                OnPropertyChanged("Blue");
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                UpdateColor();
                OnPropertyChanged("Color");
            }
        }

        public byte Green
        {
            get { return _green; }
            set
            {
                _green = value;
                UpdateRgb();
                OnPropertyChanged("Green");
            }
        }

        public double Hue
        {
            get { return _hue; }
            set
            {
                _hue = value;
                UpdateHsv();
                OnPropertyChanged("Hue");
            }
        }

        public byte Red
        {
            get { return _red; }
            set
            {
                _red = value;
                UpdateRgb();
                OnPropertyChanged("Red");
            }
        }

        public double Saturation
        {
            get { return _saturation; }
            set
            {
                _saturation = value;
                UpdateHsv();
                OnPropertyChanged("Saturation");
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                UpdateHsv();
                OnPropertyChanged("Value");
            }
        }

        public ColorDialog()
        {
            InitializeComponent();
        }

        private void ButtonPick_OnMouseDown(object sender, RoutedEventArgs e)
        {
            _mDown = true;
            Mouse.Capture(sender as Button);
            Stream cursorStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("AA2Lib.Resources.Picker.cur");
            Mouse.OverrideCursor = new Cursor(cursorStream);
            //throw new NotImplementedException();
        }

        private void ButtonPick_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mDown)
            {
                Point pos = PointToScreen(e.GetPosition(null));
                Bitmap cap = ScreenShot.Capture(new Rectangle((int) pos.X, (int) pos.Y, 1, 1));
                System.Drawing.Color capCol = cap.GetPixel(0, 0);
                Color = Color.FromRgb(capCol.R, capCol.G, capCol.B, capCol.A);
            }
            //throw new NotImplementedException();
        }

        private void ButtonPick_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mDown)
            {
                _mDown = false;
                Mouse.Capture(null);
                Mouse.OverrideCursor = null;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void RandomButton_OncClick(object sender, RoutedEventArgs e)
        {
            float _ph, _ps, _pv;
            ((ReiFX.Color) Color).ToHsv(out _ph, out _ps, out _pv);
            double hue = (Core.Random.Next(0, 180) + _ph) % 360;
            double sat = Core.Random.NextDouble();
            double val = Core.Random.NextDouble();
            Color = ReiFX.Color.FromHsv(hue, sat, val);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateAlpha()
        {
            Color rgb = Color.FromRgb(_red, _green, _blue, _alpha);
            Color = rgb;

            OnPropertyChanged("Color");
        }

        private void UpdateColor()
        {
            Color rgb = Color;

            _red = rgb.R;
            _green = rgb.G;
            _blue = rgb.B;
            _alpha = rgb.A;

            double h, s, v;
            ColorHelper.ColorToHSV(rgb, out h, out s, out v);
            _hue = h;
            _saturation = 100 * s;
            _value = 100 * v;

            OnPropertyChanged("Hue");
            OnPropertyChanged("Saturation");
            OnPropertyChanged("Value");
            OnPropertyChanged("Red");
            OnPropertyChanged("Green");
            OnPropertyChanged("Blue");
            OnPropertyChanged("Alpha");
        }

        private void UpdateHsv()
        {
            Color rgb = ColorHelper.ColorFromHSV(_hue, _saturation / 100.0, _value / 100.0, _alpha / 255.0);

            _red = rgb.R;
            _green = rgb.G;
            _blue = rgb.B;

            Color = rgb;

            OnPropertyChanged("Red");
            OnPropertyChanged("Green");
            OnPropertyChanged("Blue");
            OnPropertyChanged("Color");
        }

        private void UpdateRgb()
        {
            Color rgb = Color.FromRgb(_red, _green, _blue, _alpha);

            Color = rgb;

            double h, s, v;
            ColorHelper.ColorToHSV(rgb, out h, out s, out v);
            _hue = h;
            _saturation = 100 * s;
            _value = 100 * v;

            OnPropertyChanged("Hue");
            OnPropertyChanged("Saturation");
            OnPropertyChanged("Value");
            OnPropertyChanged("Color");
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}