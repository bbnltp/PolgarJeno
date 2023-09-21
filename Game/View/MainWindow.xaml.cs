using System.Windows;
using Microsoft.Win32;

namespace SimCity.View
{
    public partial class MainWindow : Window
    {
        private bool isClosed;
        private bool isCancel;
        GameWindow gameWindow;
        public MainWindow()
        {
            this.WindowState = WindowState.Maximized;

            gameWindow = null;
            isClosed = false;
            isCancel = false;
            InitializeComponent();
            loadGameBtn.Click += LoadGame;
        }

        void NewGame(object sender, RoutedEventArgs e)
        {
            gameWindow = new GameWindow();
            gameWindow.Show();

            this.Close();
        }
        void cancelReset()
        {

        }
        void LoadGame(object sender, RoutedEventArgs e)
        {


            if (!isClosed && !isCancel)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Loading map";
                if (openFileDialog.ShowDialog() == true)
                {
                    //loadGameBtn.Content = openFileDialog.FileName;

                    gameWindow = new GameWindow(openFileDialog.FileName);
                    gameWindow.Show();

                    isClosed = true;

                    this.Close();
                }
                else isCancel = true;
            }
            else isCancel = false;
        }
        
    }
}
