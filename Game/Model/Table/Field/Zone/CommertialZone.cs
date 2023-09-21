using System;

namespace SimCity.Model.Table.Field.Zone
{
    public class CommertialZone : Zone
    {
        #region Constructors

        /// <summary>
        /// CommertialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        public CommertialZone(int tableRowPosition, int tableColumnPosition)
             : base(tableRowPosition, tableColumnPosition) { }

        /// <summary>
        /// CommertialZone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        /// <param name="isOnFire">True is the field is on fire.</param>
		public CommertialZone (int tableRowPosition, int tableColumnPosition, bool isOnFire)
			: base(tableRowPosition, tableColumnPosition)
		{
			if (isOnFire) { IsOnFire = 1; }
			else IsOnFire = 0;
		}

		#endregion

        #region Public Methods

        /// <summary>
        /// Income per resident in the commertial zone.
        /// </summary>
        public override int IncomePerResident() { return 1500; }

        /// <summary>
        /// The one time build expence of the commertial zone.
        /// </summary>
        public override int OneTimeBuildExpence() { return 7000; }

        /// <summary>
        /// The fire chance of the commertial zone.
        /// </summary>
		public override int FireChance() { return 15; }

        /// <summary>
        /// Sets the polic commertial zone on fire if conditions are met.
        /// </summary>
        /// <param name="effect">The effect seed.</param>
		public override void IsSetOnFire(int effect)
		{
            Random rng = new Random();
			if(this.FireChance() > rng.Next(1, 51))
			{
				isOnFire = 1;
			}
		}

        /// <summary>
        /// The tableFieldType of the commertial zone.
        /// </summary>
		public override TableFieldType TableFieldType()
        {
            return Field.TableFieldType.CommertialZone;
        }

        #endregion
    }
}
