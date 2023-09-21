using System;

namespace SimCity.Model.Table.Field.Zone
{
    public class ZoneLevelOne : ZoneLevel
    {
        #region Static fields and methods


        private static ZoneLevelOne? instance;

        /// <summary>
        /// Factory method for the level one zone.
        /// </summary>
        public static ZoneLevelOne Instance()
        {
            if (instance == null) instance = new ZoneLevelOne();
            return instance;
        }

        #endregion

        #region Consturctors

        /// <summary>
        /// The ZoneLevelOne constructor.
        /// </summary>
        private ZoneLevelOne() : base() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The upgrade expence of the level one zone.
        /// </summary>
        public override int UpgradeExpence() { return 10000; }

        /// <summary>
        /// The maximum capacity of the level one zone.
        /// </summary>
        public override int MaximumCapacity() { return 10; }

        /// <summary>
        /// The zone level type of the level one zone.
        /// </summary>
        public override ZoneLevelType ZoneLevelType() 
        { 
            return Field.Zone.ZoneLevelType.LevelOne; 
        }

        #endregion
    }
}
