using System;
using System.Collections.Generic;
using System.Linq;
using SimCity.Model.Table.Field;
using SimCity.Model.Table;

namespace SimCity.Model
{
	public class GameEconomy
	{
        #region Fields

        private int taxRate;
		private int taxesCollected;

		private int gameFunds;
		private int retirementExpenses;

		private Dictionary<TableFieldType, int> tableFieldExpenses;

        #endregion

        #region Constructors

        /// <summary>
        /// GameEconomy constructor.
        /// </summary>
        /// <param name="taxRate">The tax rate.</param>
        /// <param name="taxesCollected">The taxes collected.</param>
        /// <param name="gameFunds">The game funds.</param>
        /// <param name="retirementExpenses">The retirement expences.</param>
        public GameEconomy(int taxRate, int taxesCollected, int gameFunds, int retirementExpenses)
		{
			this.taxRate = taxRate;
			this.taxesCollected = taxesCollected;
			this.gameFunds = gameFunds;
			this.retirementExpenses = retirementExpenses;
			this.tableFieldExpenses = new Dictionary<TableFieldType, int>();

			GameTable.TableFieldChanged +=
				new EventHandler<TableFieldChangedEventArgs>(GameTable_GameFieldChanged);
        }

        /// <summary>
        /// GameEconomy constructor.
        /// </summary>
        /// <param name="taxRate">The tax rate.</param>
        /// <param name="gameFunds">The game funds.</param>
        public GameEconomy(int taxRate, int gameFunds) : this(taxRate, 0, gameFunds, 0) { }

        /// <summary>
        /// GameEconomy constructor with default parameters. 
        /// </summary>
        public GameEconomy() : this(20, 250000) { }

        #endregion

        #region Properties

		/// <summary>
		/// The tax rate in the game.
		/// </summary>
		public int TaxRate
		{
			get { return taxRate; }
			set 
			{
				if (0 <= value && value <= 100 &&
					taxRate != value)
				{
                    taxRate = value;
                    RaiseEconomyChanged();
                }
			}
		}

		/// <summary>
		/// The taxes collected in the actual month.
		/// </summary>
		public int TaxesCollected
		{
			get { return taxesCollected; }
			set { taxesCollected = value; }
		}

		/// <summary>
		/// The total amount of game funds.
		/// </summary>
		public int GameFunds
		{
			get { return gameFunds; }
			set { gameFunds = value; }
		}

		/// <summary>
		/// The total amound of retirement expences.
		/// </summary>
		public int RetirementExpenses
		{
			get { return retirementExpenses;}
			set { retirementExpenses = value; }
		}

		/// <summary>
		/// The dictionary of table field expences.
		/// </summary>
		public Dictionary<TableFieldType, int> TableFieldExpenses
		{
			get { return tableFieldExpenses; }
		}

		#endregion

		#region Events

		public static event EventHandler<EconomyEventArgs>? EconomyChanged;

        #endregion

        #region Event handlers

		/// <summary>
		/// Event handler for the GameTable.GameFieldChanged event.
		/// </summary>
        public void GameTable_GameFieldChanged(Object? sender, TableFieldChangedEventArgs e) 
		{
			// Handle the event.
			if (!e.IsRemoved)
			{
				TableFieldAdded(e.TableField);
			}
			else
			{
				TableFieldRemoved(e.TableField);
			}

			// The economy is changed.
			RaiseEconomyChanged();
		}

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the gameFunds.
        /// </summary>
        /// <param name="gamePopulation">The gamePopulation.</param>
		/// <param name="newYearStarted">Tells if a new year have started.</param>
        public void UpdateGameFunds(GamePopulation gamePopulation, bool newYearStarted)
		{
			if (gamePopulation is null) return;

			int tableFieldExpences = 0;
			foreach (var tableFieldItem in tableFieldExpenses)
			{
                tableFieldExpences += tableFieldItem.Value;
			}
			
			// Calculate pension expences.
			CalculatePensionExpences(gamePopulation);

			// Calculate taxesCollected
            CalculateTaxesCollected(gamePopulation, newYearStarted);

			// Update gameFunds
			GameFunds +=
				TaxesCollected - (tableFieldExpences + RetirementExpenses);

			// Invoke EconomyChanged event
			RaiseEconomyChanged();
        }

		/// <summary>
		/// Used for charging extra expences.
		/// </summary>
		/// <param name="extraExpence">The amount to charge.</param>
		public void ChargeExtraExpence(int extraExpence)
		{
			gameFunds -= extraExpence;
			RaiseEconomyChanged();
		}

        #endregion

        #region Private methods

		/// <summary>
		/// Adds the annual reservation expence to of the placed field.
		/// </summary>
		/// <param name="tField">The placed table field.</param>
        private void TableFieldAdded(TableField tField)
        {
			TableFieldType fieldType     = tField.TableFieldType();
			int annualReservationExpence = tField.AnnualReservationExpence();

			// Updating the annual reservations if neccessary.
			if (annualReservationExpence != 0)
			{
				if (!tableFieldExpenses.ContainsKey(fieldType))
				{
                    tableFieldExpenses.Add(fieldType, annualReservationExpence);
                }
				else
				{
					tableFieldExpenses[fieldType] += annualReservationExpence;
                }
			}

			// Charge the ontime build expece of the field.
            gameFunds -= tField.OneTimeBuildExpence();
        }

        /// <summary>
        /// Subtracts the annual reservation expence to of the removed field.
        /// </summary>
        /// <param name="tField">The removed table field.</param>
        private void TableFieldRemoved(TableField tField)
        {
            TableFieldType fieldType = tField.TableFieldType();

			// Removing the reservation expence of the removed field.
			if (tableFieldExpenses.ContainsKey(fieldType))
			{
                tableFieldExpenses[fieldType] -= tField.AnnualReservationExpence();
            }

			// Returning 80% of the ontime build expence.
			gameFunds += Convert.ToInt32(0.8 * tField.OneTimeBuildExpence());
        }

        /// <summary>
        /// Calculates the taxes collected.
        /// </summary>
        /// <param name="gamePopulation">The gamePopulation class.</param>
        /// <returns>The calculated taxes collected.</returns>
        private void CalculateTaxesCollected(GamePopulation gamePopulation, bool newYearStarted)
		{
			int collectedTaxes = 0;
			double taxPercent  = taxRate / 100.0;

            foreach (Person person in gamePopulation.WorkingResidents)
			{
				int workIncome     = 0;
				int residentIncome = 0;

				if (person.WorkZone is not null) 
					workIncome = person.WorkZone.IncomePerResident();

				if (person.ResidentialZone is not null)
					residentIncome = person.ResidentialZone.IncomePerResident();

				// Calculating the tax payed.
                int taxPayed = 
					Convert.ToInt32(taxPercent * (workIncome + residentIncome));

				// Refreshing the pension.
				if (newYearStarted || person.Pension.Count == 0)
				{
                    if (person.Pension.Count == 20)
                    {
                        person.Pension.RemoveAt(0);
                    }
                    person.Pension.Add(taxPayed);
                }
				else
				{
					person.Pension[person.Pension.Count - 1] += taxPayed;
				}

				// Adds the collected tax to the player.
                collectedTaxes += taxPayed;
			}

			TaxesCollected = collectedTaxes;
		}

		/// <summary>
		/// Calculates the pension expences.
		/// </summary>
		/// <param name="gamePopulation">The gamePopulation class.</param>
		/// <returns>The calculated pensionExpences.</returns>
        private void CalculatePensionExpences(GamePopulation gamePopulation)
        {
            int pensionExpences = 0;

            foreach (Person retired in gamePopulation.RetiredResidents)
            {
				if (retired.Pension.Count != 1)
				{
                    int averagePension = Convert.ToInt32(retired.Pension.Average());
                    retired.Pension.Clear();
                    retired.Pension.Add(averagePension);
                    pensionExpences += retired.Pension[0];
                }
				else if (retired.Pension.Count == 1)
				{
                    pensionExpences += retired.Pension[0];
                }
            }

            RetirementExpenses = pensionExpences;
        }

        #endregion

        #region Event invokers

		/// <summary>
		/// Invokes the EconomyChanged event.
		/// </summary>
		public void RaiseEconomyChanged()
		{
			EconomyChanged?.Invoke(
				this,
				new EconomyEventArgs(TaxRate, TaxesCollected, GameFunds, RetirementExpenses)
			);
        }

        #endregion
    }
}
