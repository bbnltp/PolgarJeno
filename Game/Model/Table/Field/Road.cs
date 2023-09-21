using System;

namespace SimCity.Model.Table.Field
{
    public class Road : TableField
    {
        #region Static fields and methods

        private static Road? instance;

        /// <summary>
        /// Factory method for the road.
        /// </summary>
        public static Road Instance()
        {
            if (instance == null) instance = new Road();
            return instance;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Road constructor.
        /// </summary>
        private Road() : base() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The size of the road.
        /// </summary>
        public override int Size() { return 1; }

        /// <summary>
        /// The one time build expence of the road.
        /// </summary>
        public override int OneTimeBuildExpence()       { return 1000; }

        /// <summary>
        /// The annual reservation expence of the road.
        /// </summary>
        public override int AnnualReservationExpence()  { return 100; }

        /// <summary>
        /// The table field type of road.
        /// </summary>
        public override TableFieldType TableFieldType() { return Field.TableFieldType.Road; }

        #endregion
    }
}
