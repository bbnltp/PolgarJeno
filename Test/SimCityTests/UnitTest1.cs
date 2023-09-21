using System;
using System.Collections.Generic;
using SimCity.Model;
using SimCity.Model.Table;
using SimCity.Model.Population;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.Zone;
using System.Data.Common;
using System.Xml;
using SimCity.Model.Table.Field.PublicFacility;
using System.Security.RightsManagement;

namespace SimCityTest
{
    [TestClass]
    public class SimCityTest
    {
        private GameModel gameModel = null!;

        //[TestInitialize]
        /*
        public void Initialize()
        {
            gameModel = new GameModel(null);

            GameTable.TableFieldChanged
                += new EventHandler<TableFieldChangedEventArgs>(GameTable_TableFieldChanged);
            GameTimer.GameTimerElapsed
                += new EventHandler<TimerEventArgs>(GameTimer_TimerElapsed);
        }
        */

        [TestMethod]
        public void PlaceField_TableTest()
        {
            gameModel = new GameModel(null);

            gameModel.PlaySpeed = PlaySpeedType.GamePaused;

            //================== Place residentialZone test =================

            gameModel.PlaceTableField(0, 0, TableFieldType.ResidentialZone);
            TableField[,] gameTable = gameModel.Table;
            ResidentialZone? placedZone = gameTable[0, 0] as ResidentialZone;

            // The placed field is a residential zone.
            Assert.IsNotNull(placedZone);
            Assert.AreEqual(0, placedZone.TableRowPosition);
            Assert.AreEqual(0, placedZone.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 0; row < placedZone.Size(); ++row)
            {
                for (int column = 0; column < placedZone.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedZone, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove residentialzone test =================

            gameModel.RemoveTableField(0, 0, true);

            // The residentialZone is removed from the table
            for (int row = 0; row < placedZone.Size(); ++row)
            {
                for (int column = 0; column < placedZone.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            //================== Place industrialzone test =================
            gameModel.PlaceTableField(4, 4, TableFieldType.IndustrialZone);
            gameTable = gameModel.Table;
            IndustrialZone? placedIndustrial = gameTable[4, 4] as IndustrialZone;

            // The placed field is an industrial zone.
            Assert.IsNotNull(placedIndustrial);
            Assert.AreEqual(4, placedIndustrial.TableRowPosition);
            Assert.AreEqual(4, placedIndustrial.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 4; row < placedIndustrial.Size(); ++row)
            {
                for (int column = 4; column < placedIndustrial.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedIndustrial, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove industrialzone test =================

            gameModel.RemoveTableField(4, 4, true);

            // The industrialZone is removed from the table
            for (int row = 4; row < placedIndustrial.Size(); ++row)
            {
                for (int column = 4; column < placedIndustrial.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            //================== Place commertialzone test =================
            gameModel.PlaceTableField(8, 8, TableFieldType.CommertialZone);
            gameTable = gameModel.Table;
            CommertialZone? placedCommertial = gameTable[8, 8] as CommertialZone;

            // The placed field is an commertial zone.
            Assert.IsNotNull(placedCommertial);
            Assert.AreEqual(8, placedCommertial.TableRowPosition);
            Assert.AreEqual(8, placedCommertial.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 8; row < placedCommertial.Size(); ++row)
            {
                for (int column = 8; column < placedCommertial.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedCommertial, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove commertialzone test =================

            gameModel.RemoveTableField(8, 8, true);

            // The commertialzone is removed from the table
            for (int row = 8; row < placedCommertial.Size(); ++row)
            {
                for (int column = 8; column < placedCommertial.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            //================== Place firedepartment test =================
            gameModel.PlaceTableField(8, 8, TableFieldType.FireDepartment);
            gameTable = gameModel.Table;
            FireDepartment? placedFired = gameTable[8, 8] as FireDepartment;

            // The placed field is a firedepartment.
            Assert.IsNotNull(placedFired);
            Assert.AreEqual(8, placedFired.TableRowPosition);
            Assert.AreEqual(8, placedFired.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 8; row < placedFired.Size(); ++row)
            {
                for (int column = 8; column < placedFired.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedFired, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove firedepartment test =================

            gameModel.RemoveTableField(8, 8, true);

            // The firedepartment is removed from the table
            for (int row = 8; row < placedFired.Size(); ++row)
            {
                for (int column = 8; column < placedFired.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            //================== Place policedepartment test =================
            gameModel.PlaceTableField(8, 8, TableFieldType.PoliceDepartment);
            gameTable = gameModel.Table;
            PoliceDepartment? placedPoliced = gameTable[8, 8] as PoliceDepartment;

            // The placed field is a policedepartment.
            Assert.IsNotNull(placedPoliced);
            Assert.AreEqual(8, placedPoliced.TableRowPosition);
            Assert.AreEqual(8, placedPoliced.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 8; row < placedPoliced.Size(); ++row)
            {
                for (int column = 8; column < placedPoliced.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedPoliced, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove policedepartment test =================

            gameModel.RemoveTableField(8, 8, true);

            // The policeDepartment is removed from the table
            for (int row = 8; row < placedPoliced.Size(); ++row)
            {
                for (int column = 8; column < placedPoliced.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            //================== Place stadium test =================
            gameModel.PlaceTableField(8, 8, TableFieldType.Stadium);
            gameTable = gameModel.Table;
            Stadium? placedStadium = gameTable[8, 8] as Stadium;

            // The placed field is a stadium.
            Assert.IsNotNull(placedStadium);
            Assert.AreEqual(8, placedStadium.TableRowPosition);
            Assert.AreEqual(8, placedStadium.TableColumnPosition);

            // The placed field is int the table;
            for (int row = 8; row < placedStadium.Size(); ++row)
            {
                for (int column = 8; column < placedStadium.Size(); ++column)
                {
                    // The placed zone takes up a 4x4 spance
                    Assert.AreEqual(placedStadium, gameTable[row, column]);

                    // Cannot place a road onto the zone.
                    Assert.ThrowsException<ArgumentException>(
                        () => gameModel.PlaceTableField(row, column, TableFieldType.Road)
                    );
                }
            }

            //================== Remove stadium test =================

            gameModel.RemoveTableField(8, 8, true);

            // The stadium is removed from the table
            for (int row = 8; row < placedStadium.Size(); ++row)
            {
                for (int column = 8; column < placedStadium.Size(); ++column)
                {
                    // The placed is removed from the table.
                    Assert.AreEqual(EmptyField.Instance(), gameTable[row, column]);

                    // Now the road can be placed on the table.
                    gameModel.PlaceTableField(row, column, TableFieldType.Road);
                    Assert.AreEqual(Road.Instance(), gameTable[row, column]);
                }
            }

            

        }

        [TestMethod]
        public void PlaceField_EconomyTest()
        {
            gameModel = new GameModel(null);

            // Stopping the timer.
            gameModel.PlaySpeed = PlaySpeedType.GamePaused;

            //==================== Before the placed field ==================

            int gameFunds = gameModel.GameFunds;

            // Nincsen tablefieldExpences
            Assert.AreEqual(0, gameModel.TableFieldExpenses.Count);

            //======================= Place residential zone =====================

            gameModel.PlaceTableField(0, 0, TableFieldType.ResidentialZone);
            gameFunds -= gameModel.Table[0, 0].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(0, gameModel.TableFieldExpenses.Count);

            //======================= Place commertial zone =====================

            gameModel.PlaceTableField(0, 4, TableFieldType.CommertialZone);
            gameFunds -= gameModel.Table[0, 4].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(0, gameModel.TableFieldExpenses.Count);

            //======================= Place industrial zone =====================

            gameModel.PlaceTableField(0, 8, TableFieldType.IndustrialZone);
            gameFunds -= gameModel.Table[0, 8].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(0, gameModel.TableFieldExpenses.Count);
          
            //======================= Place policedepartment =====================

            gameModel.PlaceTableField(4, 0, TableFieldType.PoliceDepartment);
            gameFunds -= gameModel.Table[4, 0].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(1, gameModel.TableFieldExpenses.Count);
  
            //======================= Place firedepartment =====================

            gameModel.PlaceTableField(4, 4, TableFieldType.FireDepartment);
            gameFunds -= gameModel.Table[4, 4].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(2, gameModel.TableFieldExpenses.Count);

            
            //======================= Place two roads =====================

            // Place the first road.
            gameModel.PlaceTableField(0, 12, TableFieldType.Road);
            gameFunds -= gameModel.Table[0, 12].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(3, gameModel.TableFieldExpenses.Count);
            Assert.IsTrue(gameModel.TableFieldExpenses.ContainsKey(TableFieldType.Road));
            Assert.AreEqual(
                gameModel.Table[0, 12].AnnualReservationExpence(),
                gameModel.TableFieldExpenses[TableFieldType.Road]
            );

            // Place the second road.
            gameModel.PlaceTableField(0, 13, TableFieldType.Road);
            gameFunds -= gameModel.Table[0, 13].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(3, gameModel.TableFieldExpenses.Count);
            Assert.IsTrue(gameModel.TableFieldExpenses.ContainsKey(TableFieldType.Road));
            Assert.AreEqual(
                gameModel.Table[0, 13].AnnualReservationExpence() * 2,
                gameModel.TableFieldExpenses[TableFieldType.Road]
            );

            //=========================== Place a stadium =======================

            gameModel.PlaceTableField(0, 17, TableFieldType.Stadium);
            gameFunds -= gameModel.Table[0, 17].OneTimeBuildExpence();

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);
            Assert.IsTrue(gameModel.TableFieldExpenses.ContainsKey(TableFieldType.Stadium));
            Assert.AreEqual(
                gameModel.Table[0, 17].AnnualReservationExpence(),
                gameModel.TableFieldExpenses[TableFieldType.Stadium]
            );


            //====================== Remove the residential zone =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 0].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 0, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);

            //====================== Remove the commertial zone =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 4].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 4, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);

            //====================== Remove the industrial zone =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 8].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 8, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);
            
            //====================== Remove the policedepartment =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[4, 0].OneTimeBuildExpence());
            gameModel.RemoveTableField(4, 0, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);

            //====================== Remove the firedepartment =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[4, 4].OneTimeBuildExpence());
            gameModel.RemoveTableField(4, 4, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(4, gameModel.TableFieldExpenses.Count);

            
            //====================== Remove the two roads =================

            // Remove the first road.
            int roadResExpence = gameModel.Table[0, 12].AnnualReservationExpence();
            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 12].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 12, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(roadResExpence, gameModel.TableFieldExpenses[TableFieldType.Road]);

            // Remove the second road.
            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 13].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 13, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(0, gameModel.TableFieldExpenses[TableFieldType.Road]);

            //====================== Remove the stadium =================

            gameFunds += Convert.ToInt32(0.8 * gameModel.Table[0, 17].OneTimeBuildExpence());
            gameModel.RemoveTableField(0, 17, true);

            Assert.AreEqual(gameFunds, gameModel.GameFunds);
            Assert.AreEqual(0, gameModel.TableFieldExpenses[TableFieldType.Stadium]);
        }

        [TestMethod]
        public void HappinessTest()
        {
            GameTable gameTable = new GameTable();
            GameEconomy gameEconomy = new GameEconomy();
            GamePopulation gamePopulation = new GamePopulation();
            gameModel = new GameModel(gameTable, gameEconomy, gamePopulation, null);
            gameModel.PlaySpeed = PlaySpeedType.GamePaused;

            gameModel.PlaceTableField(10, 10, TableFieldType.ResidentialZone);
            gameModel.PlaceTableField(10, 14, TableFieldType.Road);

            gameModel.PlaceTableField(10, 16, TableFieldType.Road);
            gameModel.PlaceTableField(10, 17, TableFieldType.CommertialZone);

            //============================= Adding new residents. =================================

            Assert.AreEqual(0, gameModel.PopulationCounter);
            Assert.AreEqual(HappinessLevel.Normal, gameModel.HappinessLevel);
            Assert.AreEqual(0, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 17).ResidentCounter);

            // Cannot add new resident, because there is no connection between the zones.
            gamePopulation.AddNewResidents(gameTable);

            Assert.AreEqual(0, gameModel.PopulationCounter);
            Assert.AreEqual(HappinessLevel.Normal, gameModel.HappinessLevel);
            Assert.AreEqual(0, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 17).ResidentCounter);

            // Connecting the zones.
            gameModel.PlaceTableField(10, 15, TableFieldType.Road);
            gamePopulation.AddNewResidents(gameTable);

            Assert.AreEqual(10, gameModel.PopulationCounter);
            Assert.AreEqual(HappinessLevel.Normal, gameModel.HappinessLevel);
            Assert.AreEqual(10, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(10, gameModel.GetZone(10, 17).ResidentCounter);

            foreach (Person resident in gameModel.GetZone(10, 10).ResidentList)
            {
                Assert.AreEqual(HappinessLevel.Normal, resident.HappinessLevel);
            }

            // Placing a stadium which is not connected to the road.
            gameModel.PlaceTableField(5, 14, TableFieldType.Stadium);
            gameModel.PlaceTableField(9, 14, TableFieldType.Road);
            gamePopulation.CalculateGameHappiness(gameTable, gameEconomy);

            Assert.IsTrue(gameModel.HappinessLevel >= HappinessLevel.Normal);
            foreach (Person resident in gameModel.GetZone(10, 10).ResidentList)
            {
                Assert.IsTrue(resident.HappinessLevel >= HappinessLevel.Normal);
            }
        }

        [TestMethod]
        public void PensionTest()
        {
            GameTable gameTable = new GameTable();
            GameEconomy gameEconomy = new GameEconomy();
            GamePopulation gamePopulation = new GamePopulation();
            gameModel = new GameModel(gameTable, gameEconomy, gamePopulation, null);
            gameModel.PlaySpeed = PlaySpeedType.GamePaused;

            //================== Place residential and commertial zone ==============

            gameModel.PlaceTableField(10, 10, TableFieldType.ResidentialZone);
            gameModel.PlaceTableField(10, 14, TableFieldType.Road);
            gameModel.PlaceTableField(10, 15, TableFieldType.Road);
            gameModel.PlaceTableField(10, 16, TableFieldType.CommertialZone);

            // There are no wokers/residnets in the zones.
            Assert.AreEqual(0, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 10).RetiredCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 16).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 16).RetiredCounter);

            // At first there is no population.
            Assert.AreEqual(0, gameModel.PopulationCounter);
            Assert.AreEqual(0, gameModel.WorkingCounter);
            Assert.AreEqual(0, gameModel.RetiredCounter);

            //================== Create two new residents ==============

            Person first  = new Person(63, "Bob");
            Person second = new Person(37, "Polgár Jenõ");

            // They are workers.
            Assert.IsFalse(first.IsRetired);
            Assert.IsFalse(second.IsRetired);

            List<Person> residents = new List<Person>()
            {
                first, second
            };

            // Accommodate the two resindets.
            gamePopulation.AllResidents.Add(first);
            gamePopulation.AllResidents.Add(second);
            gamePopulation.WorkingResidents.Add(first);
            gamePopulation.WorkingResidents.Add(second);
            gamePopulation.AccommodateOrRemoveResidents(residents, gameTable, false);

            // There is population and no pension expence.
            Assert.AreEqual(2, gameModel.PopulationCounter);
            Assert.AreEqual(2, gameModel.WorkingCounter);
            Assert.AreEqual(0, gameModel.RetiredCounter);
            Assert.AreEqual(0, gameModel.RetirementExpenses);

            // There are two wokers/residnets in the zones.
            Assert.AreEqual(2, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 10).RetiredCounter);
            Assert.AreEqual(2, gameModel.GetZone(10, 16).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 16).RetiredCounter);

            //================== The first year goes by. =================

            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gamePopulation.YearHavePassed(gameTable);
            gameEconomy.UpdateGameFunds(gamePopulation, true);

            // They are still workers but a year older.
            Assert.IsFalse(first.IsRetired);
            Assert.IsFalse(second.IsRetired);
            Assert.AreEqual(64, first.Age);
            Assert.AreEqual(38, second.Age);

            // There is population, but no pension expence.
            Assert.AreEqual(2, gameModel.PopulationCounter);
            Assert.AreEqual(2, gameModel.WorkingCounter);
            Assert.AreEqual(0, gameModel.RetiredCounter);
            Assert.AreEqual(0, gameModel.RetirementExpenses);

            // There are two wokers/residnets in the zones.
            Assert.AreEqual(2, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 10).RetiredCounter);
            Assert.AreEqual(2, gameModel.GetZone(10, 16).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 16).RetiredCounter);


            //================== A second year goes by  ==============

            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gameEconomy.UpdateGameFunds(gamePopulation, false);
            gamePopulation.YearHavePassed(gameTable);
            gameEconomy.UpdateGameFunds(gamePopulation, true);

            // They first resident retires.
            Assert.IsTrue(first.IsRetired);
            Assert.IsFalse(second.IsRetired);
            Assert.AreEqual(65, first.Age);
            Assert.AreEqual(39, second.Age);

            // There is population and pension expence.
            Assert.AreEqual(2, gameModel.PopulationCounter);
            Assert.AreEqual(1, gameModel.WorkingCounter);
            Assert.AreEqual(1, gameModel.RetiredCounter);
            Assert.AreEqual(first.Pension[0], gameModel.RetirementExpenses);

            // There is one workers and one retired.
            Assert.AreEqual(2, gameModel.GetZone(10, 10).ResidentCounter);
            Assert.AreEqual(1, gameModel.GetZone(10, 10).RetiredCounter);
            Assert.AreEqual(1, gameModel.GetZone(10, 16).ResidentCounter);
            Assert.AreEqual(0, gameModel.GetZone(10, 16).RetiredCounter);
        }



        /*
        private void GameEconomy_EconomyChanged(object? sender, EconomyEventArgs e)
        {
            Assert.AreEqual(gameModel.GameFunds, e.GameFunds);
            Assert.AreEqual(gameModel.RetirementExpenses, e.RetirementExpences);
            Assert.AreEqual(gameModel.TaxesCollected, e.TaxesCollected);
            Assert.AreEqual(gameModel.TaxRate, e.TaxRate);
        }

        // Testing the tablefield removed event.
        private void GameTable_TableFieldChanged(object? sender, TableFieldChangedEventArgs e)
        {
            if (e.IsRemoved)
            {
                Assert.IsTrue(gameModel.Table[e.TableRow, e.TableColumn] is EmptyField);
            }
            else
            {
                Assert.AreEqual(gameModel.Table[e.TableRow, e.TableColumn], e.TableField);
            }
        }

        // Testing the timer elapsed event.
        private void GameTimer_TimerElapsed(object? sender, TimerEventArgs e)
        {
            Assert.AreEqual(gameModel.CurrentDay, e.GameDay);
            Assert.AreEqual(gameModel.CurrentMonth, e.GameMonth);
            Assert.AreEqual(gameModel.CurrentYear, e.GameYear);
            Assert.AreEqual(gameModel.PlaySpeed, e.GamePlaySpeed);
        }
        */
    }
}