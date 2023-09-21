using System;

namespace SimCity.Model.Table.Field.Zone
{
    public class ZoneLevelTwo : ZoneLevel
    {
        #region Static fields and methods

        private static ZoneLevelTwo? instance;

        /// <summary>
        /// Factory method for the level two zone.
        /// </summary>
        public static ZoneLevelTwo Instance()
        {
            if (instance == null) instance = new ZoneLevelTwo();
            return instance;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// The ZoneLevelTwo constructor.
        /// </summary>
        private ZoneLevelTwo() : base() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The upgrade expence of the level two zone.
        /// </summary>
        public override int UpgradeExpence()  { return 15000; }

        /// <summary>
        /// The maximum capacity of the level two zone.
        /// </summary>
        public override int MaximumCapacity() { return 20; }

        /// <summary>
        /// The zone level type of the level two zone.
        /// </summary>
        public override ZoneLevelType ZoneLevelType()
        {
            return Field.Zone.ZoneLevelType.LevelTwo;
        }

        #endregion
    }
}
