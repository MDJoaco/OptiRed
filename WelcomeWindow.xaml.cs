using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace OptiRed
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void btnNuevoProyecto_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Title = "Select a folder for your project",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true)
            {
                string projectName = Path.GetFileName(dialog.FolderName);
                string[] lines = { "PROJECT NAME", projectName, "ALGORITHM", "PASSENGERS" };

                string projectFilePath = Path.Join(dialog.FolderName, "project.orp");
                File.WriteAllLines(projectFilePath, lines);

                MainWindow mainWindow = new MainWindow(projectFilePath);
                mainWindow.Show();

                this.Close();
            }
        }

        private void btnEditarProyecto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select the project file",
                Filter = "Project files (*.orp)|*.orp|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(folder: Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                MainWindow mainWindow = new MainWindow(openFileDialog.FileName);
                mainWindow.Show();

                this.Close();
            }
        }
    }
}