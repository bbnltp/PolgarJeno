using System;

namespace SimCity.Model.Table.Field
{
    public abstract class PositionBasedTableField : TableField
    {
        #region Fields

        protected readonly int tableRowPosition;
        protected readonly int tableColumnPosition;

        protected int isOnFire;

        #endregion

        #region Constructors

        /// <summary>
        /// PositionBasedTableField constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        /// <param name="isOnFire">True is the field is on fire.</param>
        public PositionBasedTableField(int tableRowPosition, int tableColumnPosition, int isOnFire)
        {
            this.tableRowPosition    = tableRowPosition;
            this.tableColumnPosition = tableColumnPosition;
            this.isOnFire            = isOnFire;
        }

        /// <summary>
        /// PositionBasedTableField constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        /// <param name="isOnFire">True is the field is on fire.</param>
        public PositionBasedTableField(int tableRowPosition, int tableColumnPosition)
            : this(tableRowPosition, tableColumnPosition, 0) { }

        #endregion

        #region Properties

        /// <summary>
        /// The table row position of the field.
        /// </summary>
        public int TableRowPosition
        {
            get { return tableRowPosition; }    
        }

        /// <summary>
        /// The table column position of the field.
        /// </summary>
        public int TableColumnPosition
        {
            get { return tableColumnPosition; }
        }

        /// <summary>
        /// Property for the isOnFire field.
        /// </summary>
        public int IsOnFire
        {
            get { return isOnFire; }
            set { isOnFire = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// The fire chanse of the PositionBasedTableField.
        /// </summary>
		public virtual int FireChance() { return 0; }

        /// <summary>
        /// Sets the polic PositionBasedTableField on fire if conditions are met.
        /// </summary>
        /// <param name="effect">The effect seed.</param>
        public virtual void IsSetOnFire(int effect) { }

        /// <summary>
        /// Test equality by the table row and column position.
        /// </summary>
        /// <param name="obj">The TrackableTableField object to compare with.</param>
        public override bool Equals(object? obj)
        {
            PositionBasedTableField? field = obj as PositionBasedTableField;

            bool objectEqual =  field != null &&
                               (field == this || 
                               (tableRowPosition == field.TableRowPosition &&
                                tableColumnPosition == field.TableColumnPosition)) ;

            return objectEqual;
        }

        #endregion
    }
}
