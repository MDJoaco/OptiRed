using System.Windows;

namespace OptiRed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _projectFilePath;
        private GeneradorRutas _generatorRutas;

        public MainWindow(string projectFilePath)
        {
            InitializeComponent();
            _projectFilePath = projectFilePath;
            _generatorRutas = new GeneradorRutas("Executables", "optired-rutas.jar");
        }

        private void Menu_ShowConsole_Toggle(object sender, RoutedEventArgs e)
        {
            if (_generatorRutas.Running())
            {
                if (Menu_ShowConsole.IsChecked)
                {
                    _generatorRutas.ShowConsole();
                    return;
                }

                _generatorRutas.HideConsole();
            }

            Menu_ShowConsole.IsChecked = false;
        }
    }
}
