using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UsAcRe {
    /// <summary>
    /// Interaction logic for WpfElementBounding.xaml
    /// </summary>
    public partial class WpfElementBounding : Window {
        private int boundingThickness = 1;
        private Rect location;

        public WpfElementBounding(int boundingThickness, double opacity) {
            InitializeComponent();
            this.boundingThickness = boundingThickness;
            outerBounding.BorderThickness = new Thickness(boundingThickness);
            innerBounding.BorderThickness = new Thickness(boundingThickness);
            outerBounding.BorderBrush = new SolidColorBrush(Colors.Yellow);
            innerBounding.BorderBrush = new SolidColorBrush(Colors.Red);
            this.Opacity = opacity;
        }

        public Rect Location {
            get { return location; }
            set {
                location = value;
                this.Left = location.Left - (2 * boundingThickness);
                this.Width = location.Width + (4 * boundingThickness);
                this.Top = location.Top - (2 * boundingThickness);
                this.Height = location.Height + (4 * boundingThickness);
            }
        }

        public void SetToolTip(string toolTipMessage) {

        }

        public void SetVisibility(bool show) {
            if (show) {
                this.Show();
            } else {
                this.Hide();
            }
        }
        
        public void OnDispose() {
            this.Close();
        }
    }
}
