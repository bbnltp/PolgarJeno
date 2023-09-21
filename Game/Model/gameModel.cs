using SimCity.Model.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimCity.Model
{
    //public enum Player {One,Two}

   
    public class MenuItem
    {
        public int id;
        public string title;
        public bool choose;
        public int size;
        

        public MenuItem(int id, string title, int size, bool choose)
        {
            this.id = id;
            this.title = title;
            this.size = size;
            this.choose = choose;
        }

    }

    public class Territory
    {
        public string title;
        public bool choosen;

        public Territory(string title, bool choosen)
        {
            this.title = title;
            this.choosen = choosen;
        }
    }

    internal class gameModel
    {
        #region Properties
        private IGamePersistence _dataAccess;
        private int _gameFunds;
        private String _currentDate;
        private bool _gameOver;

        public string[] menuItems = { "C.Z.", "I.Z.", "R.Z.", "Street", "Police", "Stadion", "FireDepartment","Catastrophy", "Cancel", "Build" };
        public int[] menuItemsSize = { 4, 4, 4, 1, 3, 3, 3,4 ,0, 0 };
        public int SIZE_X = 30;
        public int SIZE_Y = 40;
        public Territory[,] table;
        public List<MenuItem> menu = new List<MenuItem>();
        public List<(int, int, string)> buildplace = new List<(int, int, string)>();

        #endregion

        #region Events

        public event EventHandler<GameEventArgs> GameCreated;
        public event EventHandler<GameEventArgs> GameAdvanced;
        public event EventHandler<GameEventArgs> GameOver;
        //TODO: különböző játékelemek addolása
        
        public event EventHandler<GameEventArgs> FireDepartmentAdded;
        public event EventHandler<GameEventArgs> PoliceDepartmentAdded;
        public event EventHandler<GameEventArgs> StadiumAdded;
        public event EventHandler<GameEventArgs> ResidentialZoneAdded;
        public event EventHandler<GameEventArgs> CommercialZoneAdded;
        public event EventHandler<GameEventArgs> IndustrialZoneAdded;
        public event EventHandler<GameEventArgs> RoadAdded;


        #endregion


        #region Constructor

        public gameModel()
        {
            //_dataAccess = dataAccess;
            table = new Territory[SIZE_X, SIZE_Y];

            for (int i = 0; i < SIZE_X; i++)
            {
                for (int j = 0; j < SIZE_Y; j++)
                {
                    table[i, j] = new Territory("", false);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                menu.Add(new MenuItem(i, menuItems[i], menuItemsSize[i], false));
            }



        }

        #endregion

        public void pushedButton(int x)
        {
            var item = menu.Find(m => m.choose == true);
            if (item != null)
            {
                menu[menu.IndexOf(item)].choose = false;
            }

            menu[menu.IndexOf(menu.Find(m => m.id == x))].choose = true;

        }

        public void buildItem(int x, int y)
        {
            var item = menu.Find(m => m.choose == true);
            if (item != null)
            {
                //table[x,y].choosen = true;
                for (int i = x; i < x + item.size; i++)
                {
                    for (int j = y; j < y + item.size; j++)
                    {
                        buildplace.Add((i, j, item.title));
                    }
                }

            }


        }

        public void buildPlace()
        {
            var elem = menu.Find(m => m.choose == true);
            if (elem != null)
            {
                foreach (var item in buildplace)
                {
                    table[item.Item1, item.Item2].title = item.Item3;
                    switches(item.Item3);
                }
                buildplace.Clear();
            }



        }
        
        public void switches(string s)
        {
            switch (s)
            {
                case "Street":
                    OnRoadAdded();
                    break;
                case "Police":
                    OnPoliceDepartmentAdded(); 
                    break;
                case "Stadion":
                    OnStadiumAdded();
                    break;
                case "FireDepartment":
                    OnFireDepartmentAdded();
                    break;
                case "C.Z.":
                    OnCommercialZoneAdded();
                    break;
                case "I.Z.":
                    OnIndustrialZoneAdded();
                    break;
                case "R.Z.":
                    OnResidentialZoneAdded();
                    break;
            }
        }

        public void cancelAction()
        {
            buildplace.Clear();
			var item = menu.Find(m => m.choose == true);
			if (item != null)
			{
				menu[menu.IndexOf(item)].choose = false;
			}
		}

        /*
        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
        }
        */

        #region Public Editor Methods

        //TODO: kell beléjük cucc de ahhoz kell map, meg külön classok

        public void AddFireDepartment(int x, int y)
        {
            
            OnFireDepartmentAdded();
        }

        public void AddPoliceDepartment(int x, int y)
        {

            OnPoliceDepartmentAdded();
        }

        public void AddStadium(int x, int y)
        {

            OnStadiumAdded();
        }

        public void AddResidentialZone(int x, int y)
        {

            OnResidentialZoneAdded();
        }

        public void AddCommercialZone(int x, int y)
        {

            OnCommercialZoneAdded();
        }

        public void AddIndustrialZone(int x, int y)
        {

            OnIndustrialZoneAdded();
        }

        public void AddRoad(int x, int y)
        {

            OnRoadAdded();
        }

        #endregion

        #region Private Event Methods

        private void OnFireDepartmentAdded()
        {
            if (FireDepartmentAdded != null)
                FireDepartmentAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnPoliceDepartmentAdded()
        {
            if (PoliceDepartmentAdded != null)
                PoliceDepartmentAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnStadiumAdded()
        {
            if (StadiumAdded != null)
                StadiumAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnResidentialZoneAdded()
        {
            if (ResidentialZoneAdded != null)
                ResidentialZoneAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnCommercialZoneAdded()
        {
            if (CommercialZoneAdded != null)
                CommercialZoneAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnIndustrialZoneAdded()
        {
            if (IndustrialZoneAdded != null)
                IndustrialZoneAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }

        private void OnRoadAdded()
        {
            if (RoadAdded != null)
                RoadAdded(this, new GameEventArgs(_gameFunds, _currentDate, _gameOver));
        }
        #endregion



    }
}
