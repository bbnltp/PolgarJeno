using System;

namespace SimCity.Model.Table.Field.PublicFacility
{
    public abstract class PublicFacility : PositionBasedTableField
    {
        #region Constructors

        /// <summary>
        /// PublicFacility constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        protected PublicFacility(int tableRowPosition, int tableColumnPosition)
            : base(tableRowPosition, tableColumnPosition)
        { }

        #endregion


        #region Public methods

        /// <summary>
        /// The size of the public facility.
        /// </summary>
        public override int Size() { return 2; }

        /// <summary>
        /// The radius of the public facilities positive effect.
        /// </summary>
        public virtual int RadiusOfPositiveEffect() { return 0; }

        /// <summary>
        /// The one time build expence of the public facility.
        /// </summary>
        public override int OneTimeBuildExpence()   { return 6000; }

        /// <summary>
        /// The annual reservation expence of the public facility.
        /// </summary>
        public override int AnnualReservationExpence() { return 100; }

        #endregion
    }
}
