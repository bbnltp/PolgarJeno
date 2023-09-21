using System;
using System.Collections.Generic;

namespace SimCity.Model.Table.Field.Zone
{
    public class ResidentialZone : Zone
    {
		#region Fields

        private Dictionary<Zone, (int distance, int workerCount)> workZones;

        #endregion

        #region Constructors

        /// <summary>
        /// ResidentialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        public ResidentialZone(int tableRowPosition, int tableColumnPosition)
             : base(tableRowPosition, tableColumnPosition)
        {
            workZones = new Dictionary<Zone, (int distance, int workerCount)>();
        }

        /// <summary>
        /// ResidentialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        /// <param name="isOnFire">True is the field is on fire.</param>
        public ResidentialZone(int tableRowPosition, int tableColumnPosition, bool isOnFire)
            : base(tableRowPosition, tableColumnPosition)
        {
            workZones = new Dictionary<Zone, (int distance, int workerCount)>();
            if (isOnFire) { IsOnFire = 1; }
            else IsOnFire = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property for the work zones.
        /// </summary>
        public Dictionary<Zone, (int distance, int workerCount)> WorkZones
        {
            get { return workZones; }
            set { workZones = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
		/// The income per resident in the residential zone.
		/// </summary>
        public override int IncomePerResident() { return 1000; }

        /// <summary>
		/// The one time build expence of the residential zone.
		/// </summary>
		public override int OneTimeBuildExpence() {return 5000; }

        /// <summary>
        /// The fire chance of the residential zone.
        /// </summary>
		public override int FireChance() { return 15; }

        /// <summary>
        /// Sets the polic residential zone on fire if conditions are met.
        /// </summary>
        /// <param name="effect">The effect seed.</param>
		public override void IsSetOnFire(int effect)
		{
            Random rng = new Random();

			if (this.FireChance() > rng.Next(1, 51))
			{
				isOnFire = 1;
			}
		}

        /// <summary>
        /// The tableFieldType of the residential zone.
        /// </summary>
		public override TableFieldType TableFieldType()
        {
            return Field.TableFieldType.ResidentialZone;
        }

        #endregion
    }
}
