using System;

namespace SimCity.Model.Table.Field.PublicFacility
{
    public class Stadium : PublicFacility
    {
        #region Constructors

        /// <summary>
        /// Stadium constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        public Stadium(int tableRowPosition, int tableColumnPosition)
             : base(tableRowPosition, tableColumnPosition) { }

        #endregion

        #region Public methods

        /// <summary>
        /// The annual reservation expence of the stadium.
        /// </summary>
        public override int AnnualReservationExpence() { return 10000; }

        /// <summary>
        /// The fire chance of the stadium.
        /// </summary>
		public override int FireChance() { return 15; }

        /// <summary>
        /// Sets the polic Stadium on fire if conditions are met.
        /// </summary>
        /// <param name="effect">The effect seed.</param>
		public override void IsSetOnFire(int effect)
		{
            Random rng = new Random();

			if (this.FireChance()-effect > rng.Next(1, 51))
			{
				isOnFire = 1;
			}
		}

        /// <summary>
        /// The radius of the stadium's positive effect.
        /// </summary>
        public override int RadiusOfPositiveEffect()
        {
            return 10;
        }

        /// <summary>
        /// The TableFieldType of the stadium.
        /// </summary>
        public override TableFieldType TableFieldType()
        {
            return Field.TableFieldType.Stadium;
        }

        #endregion
    }
}
