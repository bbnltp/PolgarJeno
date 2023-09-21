using System;

namespace SimCity.Model.Table.Field.Zone
{
    public class ZoneLevelThree : ZoneLevel
    {
        #region Static fields and methods

        private static ZoneLevelThree? instance;

        /// <summary>
        /// Factory method for the level three zone.
        /// </summary>
        public static ZoneLevelThree Instance()
        {
            if (instance == null) instance = new ZoneLevelThree();
            return instance;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// The ZoneLevelThree constructor.
        /// </summary>
        private ZoneLevelThree() : base() { }

        #endregion

        #region Public methods

        /// <summary>
        /// The maximum capacity of the level three zone.
        /// </summary>
        public override int MaximumCapacity() { return 30; }

        /// <summary>
        /// The zone level type of the level three zone.
        /// </summary>
        public override ZoneLevelType ZoneLevelType()
        {
            return Field.Zone.ZoneLevelType.LevelThree;
        }

        #endregion
    }
}
