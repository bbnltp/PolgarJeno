using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using SimCity.Model.Population;
using SimCity.Model.Table;
//using SimCity.Model.Timer;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.Zone;
using SimCity.Model.Table.Field.PublicFacility;
using System.IO;

namespace SimCity.Model
{
    public class GameModel
    {
        #region Fields

        private GameTable        gameTable;
        public  GameTimer        gameTimer;
        private GameEconomy      gameEconomy;
        private GamePopulation   gamePopulation;
        private IGamePersistence _dataAccess;

        #endregion

        #region Events

        public event EventHandler GameOver;
        public event EventHandler GameCreated;
        public event EventHandler GameAdvanced;
        public event EventHandler<TimerEventArgs> TimerElapsed;

        #endregion

        #region Constructors

        /// <summary>
        /// GameModel constructor.
        /// </summary>
        /// <param name="gameTable">The game table.</param>
        /// <param name="gameEconomy">The game economy.</param>
        /// <param name="gamePopulation">The game population.</param>
        /// <param name="dataAccess">The data access.</param>
        public GameModel(
            GameTable gameTable,
            GameEconomy gameEconomy,
            GamePopulation gamePopulation,
            IGamePersistence dataAccess)
        {
            _dataAccess = dataAccess;
            this.gameTable = gameTable;
            this.gameEconomy = gameEconomy;
            this.gamePopulation = gamePopulation;
            this.gameTimer = new GameTimer(2000, Month.January, 1, PlaySpeedType.Normal);

            this.gameTimer.GameTimerWorking 
                += new EventHandler<TimerEventArgs>(GameTimer_TimerElapsed);
        }

        /// <summary>
        /// GameModel constructor.
        /// </summary>
        /// <param name="dataAccess">The data access.</param>
        public GameModel(IGamePersistence dataAccess)
        {
            _dataAccess = dataAccess;
            gameTable = new GameTable();
            gameEconomy = new GameEconomy();
            gamePopulation = new GamePopulation();
            gameTimer = new GameTimer(2000, Month.January, 1, PlaySpeedType.Normal);

            gameTimer.GameTimerWorking 
                += new EventHandler<TimerEventArgs>(GameTimer_TimerElapsed);
        }

        #endregion


        #region Properties

        /// <summary>
        /// Property for the table height.
        /// </summary>
        public int Height
        { 
            get { return gameTable.Height; }
        }

        /// <summary>
        /// Property for the table widht.
        /// </summary>
        public int Width
        {
            get { return gameTable.Widht; }
        }

        //===================== GameTable =======================

        /// <summary>
        /// Property for the game table.
        /// </summary>
        public TableField[,] Table
        {
            get { return gameTable.Table; }
        }

        //===================== GameTimer =======================

        /// <summary>
        /// Property for the play speed.
        /// </summary>
        public PlaySpeedType PlaySpeed
        {
            get { return gameTimer.PlaySpeed; }
            set { gameTimer.PlaySpeed = value; }
        }

        /// <summary>
        /// Property for the current year.
        /// </summary>
        public int CurrentYear
        {
            get { return gameTimer.CurrentYear; }
        }

        /// <summary>
        /// Property for the current month.
        /// </summary>
        public Month CurrentMonth
        {
            get { return gameTimer.CurrentMonth; }
        }

        /// <summary>
        /// Property for the current day.
        /// </summary>
        public int CurrentDay
        {
            get { return gameTimer.CurrentDay; }
        }

        //===================== GameEconomy =======================

        /// <summary>
        /// Property for the tax rate.
        /// </summary>
        public int TaxRate
        {
            get { return gameEconomy.TaxRate; }
            set { gameEconomy.TaxRate = value; }
        }

        /// <summary>
        /// Property for the taxes collected.
        /// </summary>
        public int TaxesCollected
        {
            get { return gameEconomy.TaxesCollected; }
        }

        /// <summary>
        /// Property for the game funds.
        /// </summary>
        public int GameFunds
        {
            get { return gameEconomy.GameFunds; }
        }

        /// <summary>
        /// Property for the retirement expences.
        /// </summary>
        public int RetirementExpenses
        {
            get { return gameEconomy.RetirementExpenses; }
        }

        /// <summary>
        /// Property for the table field expences.
        /// </summary>
        public Dictionary<TableFieldType, int> TableFieldExpenses
        {
            get { return gameEconomy.TableFieldExpenses; }
        }

        //================= GamePopulation ====================

        /// <summary>
        /// Property for the happiness level.
        /// </summary>
        public HappinessLevel HappinessLevel
        {
            get { return gamePopulation.HappinessLevel; }
        }

        /// <summary>
        /// Property for the happiness streak.
        /// </summary>
        public int HappinessStreak
        {
            get { return gamePopulation.HappinessStreak; }
        }

        /// <summary>
        /// Property for the population counter.
        /// </summary>
        public int PopulationCounter
        {
            get { return gamePopulation.PopulationCounter; }
        }

        /// <summary>
        /// Property for the retired counter.
        /// </summary>
        public int RetiredCounter
        {
            get { return gamePopulation.RetiredCounter; }
        }

        /// <summary>
        /// Property for the working counter.
        /// </summary>
        public int WorkingCounter
        {
            get { return gamePopulation.WorkingCounter; }
        }



        #endregion


        #region Public Methods

        /// <summary>
        /// Lehelyez a játéktáblára egy adott típusú elemet.
        /// </summary>
        /// <param name="row">Sor pozíció. (Bal felső pozícióhoz)</param>
        /// <param name="column">Oszlop pozíció. (Bal felső pocícióhoz)</param>
        /// <param name="fieldtype">Milyen típusú mezőt helyezzünk le.</param>
		public void PlaceTableField(int row, int column, TableFieldType fieldtype)
        {
            gameTable.PlaceField(row, column, fieldtype);
        }

        /// <summary>
        /// Töröli a játéktábláról a megadott pozícióból az ott lévő mezőelemet.
        /// </summary>
        /// <param name="row">Sor pozíció. (Bal felső pozícióhoz)</param>
        /// <param name="column">Oszlop pozíció. (Bal felső pocícióhoz)</param>
        /// <param name="forceRemove">Wheter to force the remove.</param>
        public void RemoveTableField(int row, int column, bool forceRemove) 
        { 
            gameTable.RemoveField(row, column, forceRemove, gamePopulation, gameEconomy);
        }

        /// <summary>
        /// Visszaadja a játéktábla adott pozíciójában lévő zónát.
        /// </summary>
        /// <param name="row">Sor pozíció. (Bal felső pozícióhoz)</param>
        /// <param name="column">Oszlop pozíció. (Bal felső pocícióhoz)</param>
        /// <returns>Az adott pozíción lévő zóna.</returns>
        public Zone GetZone(int row, int column)
        {
            return gameTable.GetZone(row, column);
        }

        /// <summary>
        /// Egy zónát felfejleszt egy magasabb színtre(3-as a max)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void UpgradeZone(int row, int column)
        {
            Zone? zone = gameTable.Table[row, column] as Zone;

            if (zone is not null)
            {
                int upgradeExpence = zone.ZoneLevel.UpgradeExpence();

                gameTable.UpgradeZone(row, column);
                gameEconomy.ChargeExtraExpence(upgradeExpence);
            }
        }

        /// <summary>
        /// Meghívja a GameTable-ben levő SimulatesFire() függvényt
        /// </summary>
        public void GenerateCatastrophy()
        {
            gameTable.SimulateFires(true, gamePopulation, gameEconomy);
        }

        /// <summary>
        /// Generates fires randomly.
        /// </summary>
        public void GenerateFire()
        {
            gameTable.SimulateFires(false, gamePopulation, gameEconomy);
        }

        /// <summary>
        /// Meghívja a GameTable GetFacility metódusát
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns>Visszatér egy PublicFacility objecttel amelynek a kordínátája megegyezik a paraméterrel</returns>
        public PublicFacility GetFacility(int row, int column)
        {
            return gameTable.GetFacility(row, column);
        }
        
        /// <summary>
        /// Decides if the save is allowed.
        /// </summary>
        /// <returns>True if the save is allowed.</returns>
        public bool CanSave()
        {
            for(int i=0; i<gameTable.PublicFacilities.Count; i++)
            {
                if (gameTable.PublicFacilities[i].IsOnFire > 0) return false;
            }
            for (int i = 0; i < gameTable.CommertialZones.Count; i++)
            {
                if (gameTable.CommertialZones[i].IsOnFire > 0) return false;
            }
            for (int i = 0; i < gameTable.ResidentialZones.Count; i++)
            {
                if (gameTable.ResidentialZones[i].IsOnFire > 0) return false;
            }
            for (int i = 0; i < gameTable.IndustrialZones.Count; i++)
            {
                if (gameTable.IndustrialZones[i].IsOnFire > 0) return false;
            }
            return true;
        }

        /// <summary>
        /// Loads the game.
        /// </summary>
        /// <param name="path">The path to the save file.</param>
        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            StreamReader reader = new StreamReader(path);
            gameTable = await _dataAccess.LoadAsync(reader);
            gamePopulation = await _dataAccess.LoadPopulation(reader, gameTable);
            gameEconomy = await _dataAccess.LoadEconomy(reader);
            gameTimer = await _dataAccess.LoadTimer(reader, gameTimer);

            gameEconomy.RaiseEconomyChanged();
            gameTimer.RaiseTimeChanged();
        }

        /// <summary>
        /// Saves the game.
        /// </summary>
        /// <param name="path">The path to the save file.</param>
        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");
            
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    await _dataAccess.SaveAsync(writer, gameTable);
                    await _dataAccess.SavePopulation(writer, gameTable, gamePopulation);
                    await _dataAccess.SaveEconomy(writer, gameEconomy);
                    await _dataAccess.SaveTimer(writer, gameTimer);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        
        #endregion

        #region Event handlers

        /// <summary>
        /// Event handler for the timer elapsed event.
        /// </summary>
        private void GameTimer_TimerElapsed(object sender, TimerEventArgs e)
        {
            if (e.GameDay == 1)
            {
                bool newYearStarted = e.GameMonth == Month.January;

                if (newYearStarted)
                {
                    gamePopulation.YearHavePassed(gameTable);
                }

                // Adding new residents to the game.
                gamePopulation.AddNewResidents(gameTable);

                // Updating the gameEconomy.
                gameEconomy.UpdateGameFunds(gamePopulation, newYearStarted);

                // Updating the happiness.
                gamePopulation.CalculateGameHappiness(gameTable, gameEconomy);
            }

			GenerateFire();

			// Fire and catastrophy simulation.
			/*Random random = new Random();

            if (random.Next(0, 500) < 1)
            {
                GenerateCatastrophy();
            }
            else
            {
                
            }*/

            // A day have passed.
            TimerElapsed.Invoke(this, new TimerEventArgs(e.GameYear, e.GameMonth, e.GameDay, e.GamePlaySpeed));
        }

        #endregion
    }
}
