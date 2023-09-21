using SimCity.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SimCity.Model.Table;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.Zone;

namespace SimCity.ViewModel
{
	internal class gameViewModel : ViewModelBase
    {
        public TableFieldType[] menuItems = { TableFieldType.Road, TableFieldType.ResidentialZone, TableFieldType.CommertialZone, TableFieldType.IndustrialZone, TableFieldType.PoliceDepartment, TableFieldType.FireDepartment, TableFieldType.Stadium };
		#region Properties

        private float width;
        private float height;
		public GameModel _model;

        
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitGameCommand { get; private set; }
        public DelegateCommand EditMapCommand { get; private set; }
        public DelegateCommand SelectItemCommand { get; private set; }
        public DelegateCommand SelectFieldCommand { get; private set; }
        public DelegateCommand SelectInfoCommand { get; private set; }
        public DelegateCommand SelectManagementCommand { get; private set; }
        public DelegateCommand SelectMenuCommand { get; private set; }

        public ObservableCollection<FieldModel> Fields { get; set; }
        public ObservableCollection<MenuField> MenuItems { get; set; }

        public Func<string, string, bool> OkCancelDialog { get; set; }

        public float Width { get => width; set { width = value; OnPropertyChanged(); } }
        public float Height { get => height; set { height = value; OnPropertyChanged(); } }

        #endregion

        #region Events

        public event EventHandler NewGame;
        public event EventHandler LoadGame;
        public event EventHandler SaveGame;
        public event EventHandler ExitGame;
        public event EventHandler EditMap;
        public event EventHandler SelectItem;
        public event EventHandler SelectField;
        public event EventHandler SelectInfo;
        public event EventHandler SelectManagement;
        public event EventHandler SelectMenu;

        public static event EventHandler<FieldEventArgs> FieldInfoEvent;
        public virtual void OnFielInfoEvent(FieldEventArgs e)
        {
            FieldInfoEvent.Invoke(this, e);
        }

        #endregion

        #region Constructor
        public gameViewModel(GameModel model)
        {
            _model = model;

            /*_model.FireDepartmentAdded += new EventHandler<GameEventArgs>(Model_FireDepartmentAdded);
            _model.PoliceDepartmentAdded += new EventHandler<GameEventArgs>(Model_PoliceDepartmentAdded);
            _model.StadiumAdded += new EventHandler<GameEventArgs>(Model_StadiumAdded);
            _model.ResidentialZoneAdded += new EventHandler<GameEventArgs>(Model_ResidentialZoneAdded);
            _model.CommercialZoneAdded += new EventHandler<GameEventArgs>(Model_CommercialZoneAdded);
            _model.IndustrialZoneAdded += new EventHandler<GameEventArgs>(Model_IndustrialZoneAdded);
            _model.RoadAdded += new EventHandler<GameEventArgs>(Model_RoadAdded);*/

            //GameTable.TableFieldChanged += new EventHandler<TableFieldChangedEventArgs>(FieldChanged);


            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());
            EditMapCommand = new DelegateCommand(param => OnEditMap());
            SelectItemCommand = new DelegateCommand(param => OnSelectItem());
            SelectFieldCommand = new DelegateCommand(param => OnSelectField());
            SelectInfoCommand = new DelegateCommand(param => OnSelectInfo());
            SelectManagementCommand = new DelegateCommand(param => OnSelectManagement());
            SelectMenuCommand = new DelegateCommand(param => OnSelectMenu());

            GameTable.TableFireChanged += new EventHandler<FireEventArgs>(Table_FireChangedEvent);
            GameTable.TableFieldChanged
                += new EventHandler<TableFieldChangedEventArgs>(FieldChanged);

            Fields = new ObservableCollection<FieldModel>();
            MenuItems = new ObservableCollection<MenuField>();

            for (int i = 0; i < _model.Height; i++)
            {
                for (int j = 0; j < _model.Width; j++)
                {
                    Fields.Add(new FieldModel
                    {
                        X = i,
                        Y = j,
                        Colour = "Green",
                        Choosen = false,
                        Index = i * model.Width + j,
                        Text = "",
                        Width = 20,
                        Height = 20,
                        FieldType = _model.Table[i, j].TableFieldType(),
                        StepCommand = new DelegateCommand(param => Step(Convert.ToInt32(param))),
                        //Inspect = new DelegateCommand(param => MezoInspect(Convert.ToInt32(param))),
                    }); ;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (i * 2 + j == 7)
                    {
                        MenuItems.Add(new MenuField
                        {
                            X = i,
                            Y = j,
                            Index = i * 2 + j,
                            Color = "White",
                            Text = "Upgrade",
                            Enabled = false,
                            PushCommand = new DelegateCommand(param => MenuPush(Convert.ToInt32(param)))
                        });
                    }
                    else if (i * 2 + j == 8)
                    {
                        MenuItems.Add(new MenuField
                        {
                            X = i,
                            Y = j,
                            Index = i * 2 + j,
                            Color = "White",
                            Text = "Destroy",
                            Enabled = false,
                            PushCommand = new DelegateCommand(param => MenuPush(Convert.ToInt32(param)))
                        });
                    }
                    else if (i * 2 + j == 9)
                    {
                        MenuItems.Add(new MenuField
                        {
                            X = i,
                            Y = j,
                            Index = i * 2 + j,
                            Color = "White",
                            Text = "Cancel",
                            Enabled = false,
                            PushCommand = new DelegateCommand(param => MenuPush(Convert.ToInt32(param)))
                        });
                    }
                    else
                    {
                        MenuItems.Add(new MenuField
                        {
                            X = i,
                            Y = j,
                            Index = i * 2 + j,
                            Color = "White",
                            FieldType = menuItems[i * 2 + j],
                            Text = menuItems[i * 2 + j].ToString(),
                            Enabled = false,
                            PushCommand = new DelegateCommand(param => MenuPush(Convert.ToInt32(param)))
                        });
                    }
                }
            }

            MenuItems.ElementAt(0).Image = "/View/road_icon.png";
            MenuItems.ElementAt(1).Image = "/View/home_icon.png";
            MenuItems.ElementAt(2).Image = "/View/commer_icon.png";
            MenuItems.ElementAt(3).Image = "/View/work_icon.png";
            MenuItems.ElementAt(4).Image = "/View/police_icon.png";
            MenuItems.ElementAt(5).Image = "/View/fire_icon.png";
            MenuItems.ElementAt(6).Image = "/View/stadium_icon.png";
            MenuItems.ElementAt(7).Image = "/View/upgrade_icon.png";
            MenuItems.ElementAt(8).Image = "/View/delete_icon.png";
            MenuItems.ElementAt(9).Image = "/View/cancel_icon.png";

            OnPropertyChanged("Fields");
            OnPropertyChanged("MenuItems");
            RefreshTable();
        }

        #endregion

        #region Public methods
        private void FieldChanged(object sender, TableFieldChangedEventArgs e)
        {
            // Updating the field's positions.
            redrawFieldImage(e.TableRow, e.TableColumn, e.TableField, false);

            // If the field is a road
            if (e.TableField is Road)
            {
                int row = e.TableRow;
                int col = e.TableColumn;

                // The above field is a road.
                if (row > 0 && _model.Table[row - 1, col] is Road road1)
                {
                    redrawFieldImage(row - 1, col, road1, false);
                }

                // The left field is a road.
                if (col > 0 && _model.Table[row, col - 1] is Road road2)
                {
                    redrawFieldImage(row, col - 1, road2, false);
                }

                // The bottom fild is a road
                if (row < _model.Table.GetLength(0) - 1 && _model.Table[row + 1, col] is Road road3)
                {
                    redrawFieldImage(row + 1, col, road3, false);
                }

                // The right field is a road.
                if (col < _model.Table.GetLength(1) - 1 && _model.Table[row, col + 1] is Road road4)
                {
                    redrawFieldImage(row, col + 1, road4, false);
                }
            }
        }
        private void Table_FireChangedEvent(object sender, FireEventArgs e)
        {
            // Refresh the currently burning buildings.
            foreach (PositionBasedTableField burning in e.CurrentlyBurning)
            {
                redrawFieldImage(
                    burning.TableRowPosition,
                    burning.TableColumnPosition, 
                    burning,
                    false
                );
            }

            // Refreshing the just extingushed buildings.
            foreach (PositionBasedTableField extingushed in e.JustExtinguished)
            {
                redrawFieldImage(
                    extingushed.TableRowPosition,
                    extingushed.TableColumnPosition,
                    extingushed,
                    false
                );
            }

            // Refreshing the previous truck positions.
            foreach ((int row, int column) position in e.PreviousTruckPositions)
            {
                redrawFieldImage(
                    position.row,
                    position.column,
                    Road.Instance(),
                    false
                );
            }

            // Refreshing the current truck positions.
            foreach ((int row, int column) position in e.CurrentTruckPositions)
            {
                redrawFieldImage(
                    position.row,
                    position.column,
                    Road.Instance(),
                    true
                );
            }
        }

        public void RefreshTable()
        {
			foreach (FieldModel field in Fields)
			{
                field.Image = getFilePath(field.X, field.Y, false);
            }

            RefreshMenu();
		}

        private void RefreshMenu()
        {
            foreach (MenuField menuitem in MenuItems) // inicializálni kell a mezőket is
            {
                if (menuitem.Enabled)
                {
                    menuitem.Color = "LightBlue";
                }
                else
                {
                    menuitem.Color = "White";
                }
            }
        }

        public void ResizeTable()
        {
            for (int i = 0; i < _model.Height; i++)
            {
                for (int j = 0; j < _model.Width; j++)
                {
                    Fields[i * _model.Width + j].Width = Width / _model.Width;
                    Fields[i * _model.Width + j].Height = Height / _model.Height;
                }
            }
        }
        #endregion

        #region Private methods

        private void redrawFieldImage(
            int startRow, 
            int startColumn, 
            TableField field, 
            bool truckOnTheRoad)
        {
            for (int row = startRow;
                 row < startRow + field.Size();
                 row += 1)
            {
                for (int column = startColumn;
                     column < startColumn + field.Size();
                     column += 1)
                {
                    Fields[row * _model.Width + column].Image = getFilePath(row, column, truckOnTheRoad);
                }
            }
        }

        private string getFilePath(int row, int column, bool truckOnTheRoad)
        {
            string pathWithFileName;
            string fileExtension   = ".png";
            int numberOfThePicture = -1;

            // The field from the table.
            TableField tableField = _model.Table[row, column];

            // If the field is positions based.
            if (tableField is PositionBasedTableField positionBasedField)
            {
                // The number of the picture.
                numberOfThePicture =
                    column - positionBasedField.TableColumnPosition +
                    (row - positionBasedField.TableRowPosition) * positionBasedField.Size();

				// If the building is on fire.
				if (positionBasedField.IsOnFire > 0)
				{
					pathWithFileName = "/View/items/burning";
					fileExtension = ".jpg";
				}
                else
                {
					// Getting the picture's path with the name of the picture.
					switch (positionBasedField.TableFieldType())
					{
						case TableFieldType.ResidentialZone:
							pathWithFileName = "/View/items/res/residental/residental";
							break;

						case TableFieldType.CommertialZone:
							pathWithFileName = "/View/items/res/commercial/commertial";
							break;

						case TableFieldType.IndustrialZone:
							pathWithFileName = "/View/items/res/industrial/industrial";
							break;

						case TableFieldType.PoliceDepartment:
							pathWithFileName = "/View/items/res/police/police";
							break;

						case TableFieldType.FireDepartment:
							pathWithFileName = "/View/items/res/firedepartment/firedepartment";
							break;

						case TableFieldType.Stadium:
							pathWithFileName = "/View/items/res/stadium/stadium";
							break;

						default:
							pathWithFileName = "/View/items/res/building_process/building_process";
							break;
					}

					// If the building is a zone.
					if (positionBasedField is Zone zone)
					{
						ZoneLevelOne levelOne = ZoneLevelOne.Instance();
						ZoneLevelTwo levelTwo = ZoneLevelTwo.Instance();

						int residentCounter = zone.ResidentCounter;
						int maxLevelAndResidentCounterDifference
							= zone.ZoneLevel.MaximumCapacity() - residentCounter;
						int zoneLevelCapacityDifference
							= zone.ZoneLevel.MaximumCapacity();

						// Getting the zoneLevel type.
						if (zone.ResidentCounter == 0)
						{
							pathWithFileName = "/View/items/res/building_process/building_process";
						}
						else if (zone.ResidentCounter > levelTwo.MaximumCapacity())
						{
							pathWithFileName += "_levelThree";
						}
						else if (zone.ResidentCounter > levelOne.MaximumCapacity())
						{
							pathWithFileName += "_levelTwo";
						}
						else if (zone.ResidentCounter > 0)
						{
							pathWithFileName += "_levelOne";
							//pathWithFileName = "/View/items/res/house";
						}

					}
				}				
                                
            }
            else if (tableField is Road)
            {
                bool top = row > 0 &&
                            (_model.Table[row - 1, column] is Road || _model.Table[row - 1, column] is PositionBasedTableField);

                bool left = column > 0 &&
                            (_model.Table[row, column - 1] is Road || _model.Table[row, column-1] is PositionBasedTableField);

				bool bottom = row < _model.Table.GetLength(0) - 1 &&
                              (_model.Table[row + 1, column] is Road || _model.Table[row + 1, column] is PositionBasedTableField);

                bool right = column < _model.Table.GetLength(1) - 1 &&
                              (_model.Table[row, column + 1] is Road || _model.Table[row, column+1] is PositionBasedTableField);

				if (truckOnTheRoad)
					pathWithFileName = "/View/items/_withFireTruck";
				else if (top && bottom && left && right)
                    pathWithFileName = "/View/items/crossroad";
                else if (bottom && left && top)
                    pathWithFileName = "/View/items/road2";
                else if (left && top && right)
                    pathWithFileName = "/View/items/road3";
                else if (top && right && bottom)
                    pathWithFileName = "/View/items/road4";
                else if (right && bottom && left)
                    pathWithFileName = "/View/items/road5";
                else if (right && bottom)
                    pathWithFileName = "/View/items/turn1";
                else if (bottom && left)
                    pathWithFileName = "/View/items/turn4";
                else if (left && top)
                    pathWithFileName = "/View/items/turn2";
                else if (top && right)
                    pathWithFileName = "/View/items/turn3";
                else if (left || right)
                    pathWithFileName = "/View/items/road0";
                else if (top || bottom)
                    pathWithFileName = "/View/items/road1";
                else
                    pathWithFileName = "/View/items/road0";
               
            }
            else
            {
                pathWithFileName = "/View/items/grass";
                fileExtension    = ".jpg";
            }

            return pathWithFileName +
                   (numberOfThePicture != -1 ? numberOfThePicture.ToString() : String.Empty) +
                   fileExtension;
        }

        public void MezoInspect(int x, int y)
        {
            FieldModel field = Fields[x * 40 + y];
            if (x == -1 || y == -1) { return; }
            try
            {
                FieldInfoEvent.Invoke(this, new FieldEventArgs(null, _model.GetFacility(field.X, field.Y), true));
            }
            catch (Exception)
            {
                try
                {
                    FieldInfoEvent.Invoke(this, new FieldEventArgs(_model.GetZone(field.X, field.Y), null, true));
                }
                catch (Exception)
                {
                    FieldInfoEvent.Invoke(this, new FieldEventArgs(null, null));
                }
            }
        }
        private void Step(int index)
        {
            try
            {
                FieldModel field = Fields[index];
                var elem = MenuItems.FirstOrDefault(x => x.Enabled);
                if (elem != null)
                {
                    if (elem.Text == "Destroy")
                    {
                        try
                        {
                            _model.RemoveTableField(field.X, field.Y, false);
                        }
                        catch (InvalidOperationException ex)
                        {
                            if (OkCancelDialog("Connections are going to be destroyed.\nDo you want to force the remove?", "Destroy conflict"))
                            {
                                _model.RemoveTableField(field.X, field.Y, true);
                            }
                        }
                    }
                    else if (elem.Text == "Upgrade")
                    {
                        _model.UpgradeZone(field.X, field.Y);
                    }
                    else
                    {
                        _model.PlaceTableField(field.X, field.Y, elem.FieldType);
                        if (elem.FieldType == TableFieldType.Road)
                        {
                            return;
                        }
                    }
                }

                try
                {
                    FieldInfoEvent.Invoke(this, new FieldEventArgs(null, _model.GetFacility(field.X, field.Y)));
                }
                catch (Exception)
                {
                    FieldInfoEvent.Invoke(this, new FieldEventArgs(_model.GetZone(field.X, field.Y), null));
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show("You can not place this here!");
                if (e.Message != "NOT A ZONE")
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void MenuPush(int index)
        {
            MenuField menu = MenuItems[index];
            if (menu.Text == "Cancel")
            {
                //_model.buildPlace();
                for (int i = 0; i < MenuItems.Count; i++)
                {
                    var elem = MenuItems[i].Enabled = false;
                }
            }
            else
            {
                //_model.pushedButton(menu.Index);
                var elem = MenuItems.FirstOrDefault(x => x.Enabled);
                if (elem != null)
                {
                    elem.Enabled = false;
                }
                menu.Enabled = true;
            }
            RefreshMenu();
		}

        #endregion

        #region Event methods

        private void Model_FireDepartmentAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("FireDepartment");
        }

        private void Model_PoliceDepartmentAdded(object sender, GameEventArgs e) 
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("PoliceDepartment");
        }

        private void Model_StadiumAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("Stadium");
        }

        private void Model_ResidentialZoneAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("ResidentialZone");
        }

        private void Model_CommercialZoneAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("CommercialZone");
        }

        private void Model_IndustrialZoneAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("IndustrialZone");
        }

        private void Model_RoadAdded(object sender, GameEventArgs e)
        {
            OnPropertyChanged("Fields");
            OnPropertyChanged("Road");
        }


        private void OnNewGame()
        {
            if (NewGame != null)
            {
                NewGame(this, EventArgs.Empty);
            }

        }
        
        private void OnLoadGame()
        {
            if (LoadGame != null)
            {
                LoadGame(this, EventArgs.Empty);
            }
            
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
            {
                SaveGame(this, EventArgs.Empty);
            }

        }

        private void OnExitGame()
        {
            if (ExitGame != null)
            {
                ExitGame(this, EventArgs.Empty);
            }

        }

        private void OnEditMap()
        {
            if (EditMap != null)
            {
                EditMap(this, EventArgs.Empty);
            }
        }

        private void OnSelectItem()
        {
            if (SelectItem != null)
            {
                SelectItem(this, EventArgs.Empty);
            }
        }

        private void OnSelectField()
        {
            if (SelectField != null)
            {
                SelectField(this, EventArgs.Empty);
            }
        }

        private void OnSelectInfo()
        {
            if (SelectInfo != null)
            {
                SelectInfo(this, EventArgs.Empty);
            }

        }

        private void OnSelectManagement()
        {
            if (SelectManagement != null)
            {
                SelectManagement(this, EventArgs.Empty);
            }
        }

        private void OnSelectMenu()
        {
            if (SelectMenu != null)
            {
                SelectMenu(this, EventArgs.Empty);
            }
        }

        #endregion


    }
}
