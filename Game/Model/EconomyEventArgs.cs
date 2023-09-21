using System;

namespace SimCity.Model
{
    public class EconomyEventArgs : EventArgs
    {
        #region Fields

        private int taxRate;
        private int taxesCollected;
        private int gameFunds;
        private int retirementExpences;

        #endregion

        #region Constructor

        /// <summary>
        /// EconomyEventArgs constructor.
        /// </summary>
        /// <param name="taxRate">The tax rate.</param>
        /// <param name="taxesCollected">The taxes collected.</param>
        /// <param name="gameFunds">The game funds.</param>
        /// <param name="retirementExpences">The retirement expences.</param>
        public EconomyEventArgs(int taxRate, int taxesCollected, int gameFunds, int retirementExpences)
        {
            this.taxRate = taxRate;
            this.taxesCollected = taxesCollected;
            this.gameFunds = gameFunds;
            this.retirementExpences = retirementExpences;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property for the taxes collected.
        /// </summary>
        public int TaxesCollected 
        { 
            get { return taxesCollected; } 
        }

        /// <summary>
        /// Property for the tax rate.
        /// </summary>
        public int TaxRate
        {
            get { return taxRate; }
        }

        /// <summary>
        /// Property for the game funds.
        /// </summary>
        public int GameFunds
        {
            get { return gameFunds; }
        }

        /// <summary>
        /// Property for the retirement expences.
        /// </summary>
        public int RetirementExpences
        {
            get { return retirementExpences; }
        }
        #endregion
    }
}
