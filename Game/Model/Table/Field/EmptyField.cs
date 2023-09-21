using System;

namespace SimCity.Model.Table.Field
{
    public class EmptyField : TableField
    {
        #region Static fields and methods

        private static EmptyField? instance;

        /// <summary>
        /// Factory method for the empty field.
        /// </summary>
        public static EmptyField Instance()
        {
            if (instance == null) instance = new EmptyField();
            return instance;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// EmptyField constructor.
        /// </summary>
        private EmptyField() : base() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The size of the empty field.
        /// </summary>
        public override int Size() { return 1; }

        /// <summary>
        /// The one time build expence of the empty field.
        /// </summary>
        public override int OneTimeBuildExpence()       { return 0; }

        /// <summary>
        /// The annual reservation expence of the empty field.
        /// </summary>
        public override int AnnualReservationExpence()  { return 0; }

        /// <summary>
        /// The table field type of the empty field.
        /// </summary>
        public override TableFieldType TableFieldType() { return Field.TableFieldType.EmptyField; }

        #endregion
    }
}
