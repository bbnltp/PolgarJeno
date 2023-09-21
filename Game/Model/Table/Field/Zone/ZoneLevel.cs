using System;

namespace SimCity.Model.Table.Field.Zone
{
    public abstract class ZoneLevel
    {
        /// <summary>
        /// ZoneLevel constructor.
        /// </summary>
        protected ZoneLevel() { }

        /// <summary>
        /// The upgrade expence of the zone.
        /// </summary>
        public virtual int UpgradeExpence()  { return 0; }

        /// <summary>
        /// The maximum capacity of the zone.
        /// </summary>
        public virtual int MaximumCapacity() { return 0; }

        /// <summary>
        /// The zone level type of the zone.
        /// </summary>
        public virtual ZoneLevelType ZoneLevelType() 
        {
            return Field.Zone.ZoneLevelType.LevelOne; 
        }
    }
}
