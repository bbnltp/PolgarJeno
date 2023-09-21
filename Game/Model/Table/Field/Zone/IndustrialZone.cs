using System;
using System.Collections.Generic;

namespace SimCity.Model.Table.Field.Zone
{
    public class IndustrialZone : Zone
    {
        #region Constructors

        /// <summary>
        /// IndustrialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        public IndustrialZone(int tableRowPosition, int tableColumnPosition)
             : base(tableRowPosition, tableColumnPosition) { }

        /// <summary>
        /// IndustrialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        /// <param name="isOnFire">True is the field is on fire.</param>
        public IndustrialZone(int tableRowPosition, int tableColumnPosition, bool isOnFire)
             : base(tableRowPosition, tableColumnPosition) 
		{
			if(isOnFire) { IsOnFire = 1; }
			else IsOnFire = 0;
		}

        #endregion


        #region Public Methods

		/// <summary>
		/// The income per resident in the industrial zone.
		/// </summary>
        public override int IncomePerResident() { return 2000; }

		/// <summary>
		/// The one time build expence of the industrial zone.
		/// </summary>
        public override int OneTimeBuildExpence() { return 7000; }

        /// <summary>
        /// The fire chance of the industrial zone.
        /// </summary>
        public override int FireChance() { return 20; }

        /// <summary>
        /// Sets the polic industrail zone on fire if conditions are met.
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
        /// The tableFieldType of the industrial zone.
        /// </summary>
		public override TableFieldType TableFieldType()
        {
            return Field.TableFieldType.IndustrialZone;
        }

        #endregion
    }
}
