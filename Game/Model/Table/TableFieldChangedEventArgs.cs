using System;
using SimCity.Model.Table.Field;

namespace SimCity.Model.Table
{
    public class TableFieldChangedEventArgs : EventArgs
    {
        #region Fields

        private int tableRow;
        private int tableColumn;
        private TableField tableField;
        private bool isRemoved;

        #endregion

        #region Constructors

        /// <summary>
        /// TableFieldChangedEventArgs constructor.
        /// </summary>
        /// <param name="tableRow">The field's table row.</param>
        /// <param name="tableColumn">The field's table column.</param>
        /// <param name="tableField">The field.</param>
        /// <param name="isRemoved">True if the field has been removed.</param>
        public TableFieldChangedEventArgs(int tableRow, int tableColumn, TableField tableField, bool isRemoved)
        {
            this.tableRow       = tableRow;
            this.tableColumn    = tableColumn;
            this.tableField     = tableField;
            this.isRemoved      = isRemoved;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property for the table row.
        /// </summary>
        public int TableRow
        { 
            get { return tableRow; } 
        }

        /// <summary>
        /// Property for the table column.
        /// </summary>
        public int TableColumn
        { 
            get { return tableColumn; } 
        }

        /// <summary>
        /// Property for the field.
        /// </summary>
        public TableField TableField
        { 
            get { return tableField; }
        }

        /// <summary>
        /// Property for the is removed.
        /// </summary>
        public bool IsRemoved
        {
            get { return isRemoved; }
        }

        #endregion
    }
}
