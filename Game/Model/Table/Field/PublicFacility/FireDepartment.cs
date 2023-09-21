using SimCity.Model.Table.Field.Zone;
using System;
using System.Collections.Generic;

namespace SimCity.Model.Table.Field.PublicFacility
{
    public class FireDepartment : PublicFacility
    {
        #region Constructors

        /// <summary>
        /// FireDepartment constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        public FireDepartment(int tableRowPosition, int tableColumnPosition)
             : base(tableRowPosition, tableColumnPosition) { }

        #endregion

        #region Public methods

        /// <summary>
        /// The annual reservation expence of the firedepartment.
        /// </summary>
        public override int AnnualReservationExpence() { return 5000; }

        /// <summary>
        /// The fire chance of the firedepartemt.
        /// </summary>
		public override int FireChance() { return 0; }
		
        /// <summary>
        /// The radius of the firedepartement's positive effect.
        /// </summary>
		public override int RadiusOfPositiveEffect()
        {
            return 10;
        }

        /// <summary>
        /// The TableFieldType of the firedepartment.
        /// </summary>
		public override TableFieldType TableFieldType()
        {
            return Field.TableFieldType.FireDepartment;
        }

        #endregion
    }
}
