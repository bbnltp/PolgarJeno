using System;

namespace SimCity.Model.Table.Field
{
    abstract public class TableField
    {
        /// <summary>
        /// TableField constructor.
        /// </summary>
        protected TableField() { }

        /// <summary>
        /// The size of the table field.
        /// </summary>
        public virtual int Size() { return 1; }

        /// <summary>
        /// The one time build expence of the table field.
        /// </summary>
        public virtual int OneTimeBuildExpence() { return 0; }

        /// <summary>
        /// The annual reservation expence of the table field.
        /// </summary>
        public virtual int AnnualReservationExpence() { return 0; }

        /// <summary>
        /// The table field type of the table field.
        /// </summary>
        public virtual TableFieldType TableFieldType() { return Field.TableFieldType.EmptyField; }
    }
}
