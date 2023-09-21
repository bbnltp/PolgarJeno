using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using SimCity.Model.Table.Field;

namespace SimCity.Model.Table
{
    public class FireEventArgs : EventArgs
    {
        #region Fields

        private List<PositionBasedTableField> currentlyBurning;
        private List<PositionBasedTableField> justExtinguished;
        private List<(int row, int column)> previousTruckPositions;
        private List<(int row, int column)> currentTruckPositions;

        #endregion

        #region Constructor

        /// <summary>
        /// FireEventArgs constructor.
        /// </summary>
        /// <param name="currentlyBurning">The list of currently burning fields.</param>
        /// <param name="justExtinguished">The list of just extinguished fields.</param>
        /// <param name="previousTruckPositions">The list of previously truck positions.</param>
        /// <param name="currentTruckPositions">The list of current truck positions.</param>
        public FireEventArgs(
            List<PositionBasedTableField> currentlyBurning,
            List<PositionBasedTableField> justExtinguished,
            List<(int row, int column)> previousTruckPositions,
            List<(int row, int column)> currentTruckPositions)
        {
            this.currentlyBurning = new List<PositionBasedTableField>(currentlyBurning);
            this.justExtinguished = new List<PositionBasedTableField>(justExtinguished);
            this.previousTruckPositions = new List<(int, int)>(previousTruckPositions);
            this.currentTruckPositions  = new List<(int, int)>(currentTruckPositions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property for the currenty burning fields.
        /// </summary>
        public List<PositionBasedTableField> CurrentlyBurning
        {
            get { return currentlyBurning; }
        }

        /// <summary>
        /// Property for the just Just extinguished fields.
        /// </summary>
        public List<PositionBasedTableField> JustExtinguished
        {
            get { return justExtinguished; }
        }

        /// <summary>
        /// Property for the previous truck positions.
        /// </summary>
        public List<(int row, int column)> PreviousTruckPositions
        {
            get { return previousTruckPositions; }
        }

        /// <summary>
        /// Property for the current truck positions.
        /// </summary>
        public List<(int row, int column)> CurrentTruckPositions
        {
            get { return currentTruckPositions; }
        }

        #endregion
    }
}
