using System;
using System.Collections.Generic;
using System.Linq;

namespace SimCity.Model.Table.Field.Zone
{
    public abstract class Zone : PositionBasedTableField
    {
        #region Fields

        // Level information
        protected ZoneLevel zoneLevel;

		// Residential information
		protected List<Person> residentList;

        #endregion

        #region Constructors

        /// <summary>
        /// Zone constructor.
        /// </summary>
        /// <param name="tableRowPosition">The table row.</param>
        /// <param name="tableColumnPosition">The table column.</param>
        protected Zone(int tableRowPosition, int tableColumnPosition) : 
                  base(tableRowPosition, tableColumnPosition)
        {
            zoneLevel = ZoneLevelOne.Instance();

            residentList = new List<Person>();
        }

		#endregion

		#region Properties

        /// <summary>
        /// Property for the zone level.
        /// </summary>
        public ZoneLevel ZoneLevel
        {
            get { return zoneLevel;  }
            set { zoneLevel = value; }
        }

        /// <summary>
        /// Property for the resident list.
        /// </summary>
        public List<Person> ResidentList
        {
            get { return residentList; }
            private set { residentList = value; }
        }

        /// <summary>
        /// Property for the resident counter.
        /// </summary>
        public int ResidentCounter
        {
            get { return residentList.Count; }
        }

        /// <summary>
        /// Property for the retired counter.
        /// </summary>
        public int RetiredCounter
        {
            get
            {
                int retiredCounter = 0;
                foreach (Person resident in residentList)
                {
                    if (resident.IsRetired)
                    {
                        retiredCounter += 1;
                    }
                }
                return retiredCounter;
            }
        }

        /// <summary>
        /// Property for the maximum capacity.
        /// </summary>
        public int MaximumCapacity
        {
            get { return zoneLevel.MaximumCapacity(); }
        }

        /// <summary>
        /// Property for the available space.
        /// </summary>
        public int AvailableSpace
        {
            get { return MaximumCapacity - ResidentCounter; }
        }


        #endregion

        #region Public methods

        /// <summary>
        /// The size of the zone.
        /// </summary>
        public override int Size() { return 4; }

        /// <summary>
        /// The income per resident in the zone.
        /// </summary>
        public virtual int IncomePerResident() { return 0; }

        /// <summary>
        /// The annual reservation expence of the zone.
        /// </summary>
        public override int AnnualReservationExpence() { return 0; }


        /// <summary>
        /// Calculates the total amount of taxes collected.
        /// </summary>
        public int TaxesCollected()
        {
            return (ResidentCounter - RetiredCounter) * IncomePerResident();
        }

        /// <summary>
        /// Calculates the total amount of retirement expence.
        /// </summary>
        public int RetirementExpences()
        {
            int pensionExpences = 0;

            foreach (Person person in residentList)
            {
                if (person.IsRetired)
                {
                    pensionExpences += Convert.ToInt32(person.Pension.Average());
                }
            }

            return pensionExpences;
        }

        /// <summary>
        /// Update the zone's level to the next.
        /// </summary>
        public void UpgradeZoneLevel()
        {
            if (zoneLevel is ZoneLevelOne)
            {
                zoneLevel = ZoneLevelTwo.Instance();
            }
            else // zoneLevel is LevelTwo or LevelThree.
            {
                zoneLevel = ZoneLevelThree.Instance();
            }
        }

        /// <summary>
        /// Adds the resident to the zone.
        /// </summary>
        /// <param name="resident">The resident.</param>
        /// <returns>Returns true if the addition was successfull, otherwise false.</returns>
        public bool AddResident(Person resident)
        {
            bool containsResident = residentList.Contains(resident);
            if (containsResident) return false;

            if (this is ResidentialZone zone)
            {
                Zone? workZone = resident.WorkZone;

                if (workZone is not null)
                {
                    (int distance, int workerCount) data;
                    bool contains = zone.WorkZones.TryGetValue(workZone, out data);

                    if (contains) 
                        zone.WorkZones[workZone] = (data.distance, data.workerCount + 1);
                }

                resident.ResidentialZone = zone;
            }
            else
            {
                ResidentialZone? residentialZone = resident.ResidentialZone;

                if (residentialZone is not null)
                {
                    (int distance, int workerCount) data;
                    bool contains = residentialZone.WorkZones.TryGetValue(this, out data);

                    if (contains) 
                        residentialZone.WorkZones[this] = (data.distance, data.workerCount + 1);
                }

                resident.WorkZone = this;
            }

            residentList.Add(resident);
            return true;
        }

        /// <summary>
        /// Removes the resident from the zone.
        /// </summary>
        /// <param name="resident">The resident.</param>
        /// <returns>Returns true if the removal was successfull, otherwise false.</returns>
        public bool RemoveResident(Person resident)
        {
            bool residentRemoved = residentList.Remove(resident);
            if (!residentRemoved) return residentRemoved;

            if (this is ResidentialZone zone)
            {
                Zone? workZone = resident.WorkZone;

                if (workZone is not null)
                {
                    (int distance, int workerCount) data;
                    bool contains = zone.WorkZones.TryGetValue(workZone, out data);

                    if (contains)
                        zone.WorkZones[workZone] = (data.distance, data.workerCount - 1);
                }

                resident.ResidentialZone = null;
            }
            else
            {
                ResidentialZone? residentialZone = resident.ResidentialZone;

                if (residentialZone is not null)
                {
                    (int distance, int workerCount) data;
                    bool contains = residentialZone.WorkZones.TryGetValue(this, out data);

                    if (contains)
                        residentialZone.WorkZones[this] = (data.distance, data.workerCount - 1);
                }

                resident.WorkZone = null;
            }

            return residentRemoved;
        }

        #endregion
    }
}
