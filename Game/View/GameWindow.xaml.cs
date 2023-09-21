using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using SimCity.ViewModel;
using SimCity.Model;
using System.Timers;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using SimCity.Model.Table.Field;

namespace SimCity.View
{
    public partial class GameWindow : Window
    {
        private GameModel _gameModel;
        private gameViewModel _gameViewModel;

        private Button speedBtn;
        private Button dateBtn;
        private Button moneyBtn;

        private int showX = -1;
        private int showY = -1;

        private bool isPlaying = false;

        public Func<string, string, bool> OkCancelDialog { get; set; }

        #region Constructor
        public GameWindow(string path = "")
        {
            this.WindowState = WindowState.Maximized;

            FileGamePersistence pers = new FileGamePersistence();
            _gameModel = new GameModel(pers);
            //GameTimer.GameTimerElapsed += new EventHandler<TimerEventArgs>(OnTimedEvent);
            _gameModel.TimerElapsed+= new EventHandler<TimerEventArgs>(OnTimedEvent);
            GameEconomy.EconomyChanged += new EventHandler<EconomyEventArgs>(OnFundsEvent);
            gameViewModel.FieldInfoEvent += new EventHandler<FieldEventArgs>(OnFieldInfoEvent);
            isPlaying = true;
            InitializeComponent();
            _gameViewModel = new gameViewModel(_gameModel);
            gameTableGrid.DataContext = _gameViewModel;
            itemGrid.DataContext = _gameViewModel;
            _gameViewModel.OkCancelDialog = (msg, caption) => MessageBox.Show(msg, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            setMenuGrid();


            if (path != "")
            {
                LoadGameFromPath(path);
            }

            openInfos(null, null);
            
        }
        #endregion

        #region Public Methods - Window setup
        public void OnFieldInfoEvent(object source, FieldEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (title.Text == "Infos" && e.Timed == true)
                {
                    infoGridClose(null, null);
                    openInfos(null, null);
                    return;
                }

                if (title.Text == "Management" && e.Timed == true)
                {
                    infoGridClose(null, null);
                    openManagement(null, null);
                    return;
                }

                infoGridClose(null, null);
                if (e.Facility != null)  // facility
                {
                    showX = e.Facility.TableRowPosition;
                    showY = e.Facility.TableColumnPosition;

                    string szoveg = e.Facility.TableFieldType().ToString();
                    this.Dispatcher.Invoke(() =>
                    {
                        title.Text = "Facility Infos";

                        int fontSize = 20;

                        // First element
                        TextBlock text = new TextBlock();
                        text.FontSize = fontSize;
                        text.Text = "Field type: " + szoveg;

                        Grid.SetColumn(text, 0);
                        Grid.SetRow(text, 0);
                        infosGrid.Children.Add(text);
                    });
                }
                else if (e.Zone != null)  // zone
                {
                    showX = e.Zone.TableRowPosition;
                    showY = e.Zone.TableColumnPosition;

                    string szoveg = e.Zone.TableFieldType().ToString();

                    title.Text = "Zone Infos";

                    int fontSize = 20;

                    // First element
                    TextBlock text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Field type: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 0);
                    infosGrid.Children.Add(text);

                    // Second element
                    szoveg = e.Zone.ResidentCounter + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Resident: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 1);
                    infosGrid.Children.Add(text);

                    // Third element
                    szoveg = e.Zone.RetiredCounter + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Retired: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 2);
                    infosGrid.Children.Add(text);

                    // Fourth element
                    szoveg = e.Zone.TaxesCollected() + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Taxes collected: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 3);
                    infosGrid.Children.Add(text);

                    // Fifth element
                    szoveg = e.Zone.RetirementExpences() + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Retirement expenses: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 4);
                    infosGrid.Children.Add(text);

                    // Sixth element
                    szoveg = e.Zone.ZoneLevel.ZoneLevelType() + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Zone level: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 5);
                    infosGrid.Children.Add(text);

                    // Seventh element
                    szoveg = e.Zone.ZoneLevel.MaximumCapacity() + "";
                    text = new TextBlock();
                    text.FontSize = fontSize;
                    text.Text = "Capacity: " + szoveg;

                    Grid.SetColumn(text, 0);
                    Grid.SetRow(text, 6);
                    infosGrid.Children.Add(text);

                    // Eighth element
                    if(e.Zone.IsOnFire > 0) { szoveg = "This zone is on fire!"; }
                    else { szoveg = ""; }
					text = new TextBlock();
					text.FontSize = fontSize;
					text.Text = szoveg;

					Grid.SetColumn(text, 0);
					Grid.SetRow(text, 7);
					infosGrid.Children.Add(text);

                    // Nineth element
                    szoveg = "";
                    for (int i = 0; i < e.Zone.ResidentList.Count; i++)
                    {
                        szoveg += e.Zone.ResidentList[i].Name + ": " + e.Zone.ResidentList[i].HappinessLevel + " (" + e.Zone.ResidentList[i].Age + ")\n";
                    }

                    TextBox text2 = new TextBox();
                    text2.FontSize = fontSize;
                    text2.Text = szoveg;
                    text2.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                    Grid.SetColumn(text2, 0);
                    Grid.SetRow(text2, 8);
                    infosGrid.Children.Add(text2);
                }
                else
                {
                    showY = showX = -1;
                }
            });
        }
        public void OnFundsEvent(object source, EconomyEventArgs e)
        {
            string szoveg = e.GameFunds + "$";
            this.Dispatcher.Invoke(() =>
            {
                moneyBtn.Content = szoveg;
            });
        }
        public void OnTimedEvent(object source, TimerEventArgs e)
        {
            if (!isPlaying) return;

            string szoveg = e.GameYear.ToString() + ". " + e.GameMonth.ToString() + ". " + e.GameDay.ToString() + ".";
            this.Dispatcher.Invoke(() =>
            {
                if (isPlaying && _gameModel.HappinessLevel < Model.Population.HappinessLevel.Low)
                {
                    isPlaying = false;
                    _gameModel.PlaySpeed = PlaySpeedType.GamePaused;
                    MessageBox.Show("You are a bad president.\nPolgar Jeno will replace you");
                    goToMenu(null, null);
                    return;
                }
                dateBtn.Content = szoveg;
            });
            
            if(isPlaying && showX != -1 && showY != -1)
            {
                _gameViewModel.MezoInspect(showX, showY);
            }

        }
        public void setMenuGrid()
        {
            int fontSize = 20;

            // Infos Button
            Button button = new Button();
            button.Content = "Infos";
            button.Name = "infosBtn";
            button.FontSize = fontSize;
            button.Click += openInfos;

            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 0);
            menuGrid.Children.Add(button);

            // Management Button
            button = new Button();
            button.Content = "Management";
            button.Name = "managmentBtn";
            button.FontSize = fontSize;
            button.Click += openManagement;

            Grid.SetColumn(button, 1);
            Grid.SetRow(button, 0);
            menuGrid.Children.Add(button);

            // Menu Button
            button = new Button();
            button.Content = "Menu";
            button.Name = "menuBtn";
            button.FontSize = fontSize;
            button.Click += goToMenu;

            Grid.SetColumn(button, 2);
            Grid.SetRow(button, 0);
            menuGrid.Children.Add(button);

            // Date Button
            dateBtn = new Button();
            dateBtn.Content = "2000. January. 1.";
            dateBtn.Name = "dateBtn";
            dateBtn.FontSize = fontSize;

            Grid.SetColumn(dateBtn, 3);
            Grid.SetRow(dateBtn, 0);
            menuGrid.Children.Add(dateBtn);

            // Speed Button
            speedBtn = new Button();
            //TODO button content legyen az aminek lennie kell
            if (_gameModel.PlaySpeed == PlaySpeedType.GamePaused) speedBtn.Content = "Paused";
            else
            speedBtn.Content = _gameModel.PlaySpeed.ToString();
            //speedBtn.Name = "dateBtn";
            speedBtn.FontSize = fontSize;
            speedBtn.Click += changeSpeed;

            Grid.SetColumn(speedBtn, 4);
            Grid.SetRow(speedBtn, 0);
            menuGrid.Children.Add(speedBtn);

            // Money Button
            moneyBtn = new Button();
            moneyBtn.Content = _gameModel.GameFunds + "$";
            moneyBtn.Name = "moneyBtn";
            moneyBtn.FontSize = fontSize;

            Grid.SetColumn(moneyBtn, 5);
            Grid.SetRow(moneyBtn, 0);
            menuGrid.Children.Add(moneyBtn);
        }
        public void changeSpeed(object sender, RoutedEventArgs e)
        {
            // here we need to change the actual speed

            string content = (string)speedBtn.Content;

            switch (content)
            {
                case "Paused":
                    _gameModel.PlaySpeed = PlaySpeedType.Normal;
                    speedBtn.Content = "Normal";
                    break;
                case "Normal":
                    _gameModel.PlaySpeed = PlaySpeedType.Faster;
                    speedBtn.Content = "Fast";
                    break;
                case "Fast":
                    _gameModel.PlaySpeed = PlaySpeedType.Quickest;
                    speedBtn.Content = "Fastest";
                    break;
                case "Fastest":
                    _gameModel.PlaySpeed = PlaySpeedType.GamePaused;
                    speedBtn.Content = "Paused";
                    break;
                default:
                    break;
            }
        }
        public void openManagement(object sender, RoutedEventArgs e)
        {
            infoGridClose(null, null);

            title.Text = "Management";
            infoGrid.IsEnabled = true;
            infoGrid.Visibility = Visibility.Visible;

            int fontSize = 20;

            // First element
            TextBlock text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Tax Rate:";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 0);
            infosGrid.Children.Add(text);

            // First input
            TextBox text2 = new TextBox();
            text2.FontSize = fontSize;
            text2.Text = _gameModel.TaxRate + "";
            text2.Margin = new Thickness(10);
            text2.TextChanged += onchagsada;

            Grid.SetColumn(text2, 0);
            Grid.SetRow(text2, 1);
            infosGrid.Children.Add(text2);
        }
        public void openInfos(object sender, RoutedEventArgs e)
        {
            infoGridClose(null, null);

            title.Text = "Infos";

            int fontSize = 20;

            //================= Economy infos ====================

            // GameFunds collected
            TextBlock text = new TextBlock();
            text.FontSize = fontSize;

            text.Text = "Economy information:\n" +
                        "Game funds:      " + _gameModel.GameFunds + "$\n" +
                        "Tax rate:        " + _gameModel.TaxRate + "%\n" +
                        "Taxes collected: " + _gameModel.TaxesCollected + "$\n" +
                        "Pension expence: " + _gameModel.RetirementExpenses + "$\n";

            int totalCashFlow = _gameModel.TaxesCollected - _gameModel.RetirementExpenses;

            if (_gameModel.TableFieldExpenses.Count > 0)
            {
                text.Text += "\nAnnual reservation expences:\n";

                foreach (KeyValuePair<TableFieldType, int> expenceItem
                     in _gameModel.TableFieldExpenses)
                {
                    switch (expenceItem.Key)
                    {
                        case TableFieldType.PoliceDepartment:
                            text.Text += "Police expences:  ";
                            break;

                        case TableFieldType.FireDepartment:
                            text.Text += "Fire d. expences: ";
                            break;

                        case TableFieldType.Stadium:
                            text.Text += "Stadium expences: ";
                            break;

                        case TableFieldType.Road:
                            text.Text += "Road expences:    ";
                            break;

                        default:
                            text.Text += "Other expences:   ";
                            break;
                    }
                    
                    text.Text += expenceItem.Value.ToString() + "$\n";
                    totalCashFlow -= expenceItem.Value;
                }
            }

            text.Text += "\nTotal cash flow: " + totalCashFlow + "$\n";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 0);
            infosGrid.Children.Add(text);

            /*
            text.Text = "Game funds: " + _gameModel.GameFunds + "$";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 0);
            infosGrid.Children.Add(text);

            // Tax rate
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Tax rate: " + _gameModel.TaxRate + "%";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 1);
            infosGrid.Children.Add(text);

            // Taxes collected
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Taxes collected: " + _gameModel.TaxesCollected + "$";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 2);
            infosGrid.Children.Add(text);

            // Pension expences
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Pension expence: " + _gameModel.RetirementExpenses + "$";

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 3);
            infosGrid.Children.Add(text);

            int index = 4;
            foreach (KeyValuePair<TableFieldType, int> expenceItem 
                     in _gameModel.TableFieldExpenses)
            {
                text = new TextBlock();
                text.FontSize = fontSize;
                text.Text = expenceItem.Key.ToString() + 
                            " expence: " +
                            expenceItem.Value.ToString() + "$";

                Grid.SetColumn(text, 0);
                Grid.SetRow(text, index);
                infosGrid.Children.Add(text);

                index += 1;
            }
            */

            //================= Population infos ====================

            // Population counter
            text = new TextBlock();
            text.FontSize = fontSize;

            text.Text = "\nPopulation information:\n" + 
                        "Total population:   " + _gameModel.PopulationCounter + "\n" +
                        "Working population: " + _gameModel.WorkingCounter + "\n" +
                        "Retired population: " + _gameModel.RetiredCounter + "\n" +
                        "Overall Happiness:  " + _gameModel.HappinessLevel + "\n" +
                        "Happiness duration: " + _gameModel.HappinessStreak;


            Grid.SetColumn(text, 0);
            Grid.SetRow(text, 1);
            infosGrid.Children.Add(text);

            /*
            // Working population counter
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Working population: " + _gameModel.WorkingCounter;

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, index);
            infosGrid.Children.Add(text);
            index += 1;

            // Retired population counter
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Retired population: " + _gameModel.RetiredCounter;

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, index);
            infosGrid.Children.Add(text);
            index += 1;

            // Overall happiness
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Overall Happiness: " + _gameModel.HappinessLevel;

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, index);
            infosGrid.Children.Add(text);
            index += 1;

            // Overall happiness duration
            text = new TextBlock();
            text.FontSize = fontSize;
            text.Text = "Happiness duration: " + _gameModel.HappinessStreak;

            Grid.SetColumn(text, 0);
            Grid.SetRow(text, index);
            infosGrid.Children.Add(text);
            index += 1;
            */
        }
        public void infoGridClose(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                int intTotalChildren = infosGrid.Children.Count - 1;
                for (int intCounter = intTotalChildren; intCounter >= 0; intCounter--)
                {
                    if (infosGrid.Children[intCounter].GetType() == typeof(TextBox))
                    {
                        TextBox ucCurrentChild = (TextBox)infosGrid.Children[intCounter];
                        infosGrid.Children.Remove(ucCurrentChild);
                    }
                    else if (infosGrid.Children[intCounter].GetType() == typeof(TextBlock))
                    {
                        TextBlock ucCurrentChild = (TextBlock)infosGrid.Children[intCounter];
                        infosGrid.Children.Remove(ucCurrentChild);
                    }
                }
            });
        }
        public void MakeCatasrophy(object sender, RoutedEventArgs e)
        {
            _gameModel.GenerateCatastrophy();
        }
        #endregion

        #region Public Methods - extra
        public void goToMenu(object sender, RoutedEventArgs e)
        {
           
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            isPlaying = false;
            _gameModel.PlaySpeed = PlaySpeedType.GamePaused;
            _gameModel.gameTimer=null;
            _gameModel = null;
            _gameViewModel = null;
            
            this.Close();
        }
        public async void LoadGameFromPath(string path)
        {
            try
            {
                await _gameModel.LoadGameAsync(path);
                //_gameViewModel = new ViewModel.gameViewModel(_gameModel);

                _gameViewModel.RefreshTable();
                setMenuGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+" LoadGameFromPath");
                //throw new Exception();
            }
        }

        public void NewGame(object sender,RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
            gameWindow.openInfos(null, null);

            this.Close();
        }
        public async void LoadGame(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Loading game";

                if (openFileDialog.ShowDialog() == true)
                {
                    _gameModel.PlaySpeed = PlaySpeedType.GamePaused;
                    await _gameModel.LoadGameAsync(openFileDialog.FileName);
                    //_gameViewModel = new gameViewModel(_gameModel);
                    openInfos(null, null);
                }
                _gameViewModel.RefreshTable();
                setMenuGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+" LoadGame:GameWindow");
                //throw new Exception();
            }
        }

        public async void SaveGame(object sender, RoutedEventArgs e)
        {
            if (_gameModel.CanSave())
            {
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Game saving";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            await _gameModel.SaveGameAsync(saveFileDialog.FileName);
                            MessageBox.Show("Successful saving", "Saving", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + " SaveGame:GameWindow");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " SaveGame");
                }
            }
            else
            {
                MessageBox.Show("Cannot save game during a catastrophy");
            }
        }
        #endregion
        public void SizeChangedEvent(object sender, RoutedEventArgs e)
        {
            
            _gameViewModel.Width = (float)this.ActualWidth - 515;
            _gameViewModel.Height = (float)this.ActualHeight - 90;
            _gameViewModel.ResizeTable();

            infosGrid.Height = (float)this.ActualHeight - 300;

            
        }
        private void onchagsada(object sender, TextChangedEventArgs e)
        {
            TextBox s = (TextBox)sender;

           Regex regex = new Regex("[^0-9]+");
            e.ToString();
            if(s.Text.ToString().Length > 0 && !regex.IsMatch(s.Text.ToString()))
            {
                _gameModel.TaxRate = Convert.ToInt32(s.Text.ToString());
            }
        }
    }
}
