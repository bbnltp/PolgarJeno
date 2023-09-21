using System;
using System.Collections.Generic;
using System.Linq;
using SimCity.Model.Population;
using SimCity.Model.Table;
using SimCity.Model.Table.Field.PublicFacility;
using SimCity.Model.Table.Field.Zone;

namespace SimCity.Model
{
    public class GamePopulation
    {
		#region Fields

		private int happinessStreak;
        private int negaviteBudgetStreak;
        private HappinessLevel happinessLevel;

        private List<Person> allResidents;
        private List<Person> retiredResidents;
        private List<Person> workingResidents;

        #endregion

        #region Consturctors

        public GamePopulation() 
        {
            happinessStreak      = 0;
            negaviteBudgetStreak = 0;
            happinessLevel       = HappinessLevel.Normal;

            allResidents     = new List<Person>();
            retiredResidents = new List<Person>();
            workingResidents = new List<Person>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The game's overall happiness level.
        /// </summary>
        public HappinessLevel HappinessLevel
        {
            get { return happinessLevel; }
            set
            { 
                if (happinessLevel == value)
                {
                    HappinessStreak += 1;
                }
                else
                {
                    happinessLevel  = value;
                    HappinessStreak = 0;
                }
            }
        }


        /// <summary>
        /// The number of consecutive days of the negative budget.
        /// </summary>
        public int NegaviteBudgetStreak
        {
            get { return negaviteBudgetStreak; }
            set { negaviteBudgetStreak = value; }
        }

        /// <summary>
        /// The number of consecutive days of the happiness.
        /// </summary>
        public int HappinessStreak
        {
            get { return happinessStreak; }
            set { happinessStreak = value; }
        }

        /// <summary>
        /// The total population (workers + retireds) counter.
        /// </summary>
        public int PopulationCounter
        {
            get { return allResidents.Count; }
        }

        /// <summary>
        /// The total number of retired residents.
        /// </summary>
        public int RetiredCounter
        {
            get { return retiredResidents.Count; }
        }

        /// <summary>
        /// The total number of working residents.
        /// </summary>
        public int WorkingCounter
        {
            get { return workingResidents.Count; }
        }

        /// <summary>
        /// The list of all residents in the game.
        /// </summary>
        public List<Person> AllResidents
        {
            get { return allResidents; }
        }

        /// <summary>
        /// The list of all retired residents in the game.
        /// </summary>
        public List<Person> RetiredResidents
        {
            get { return retiredResidents; }
        }

        /// <summary>
        /// The list of all working residents in the game.
        /// </summary>
        public List<Person> WorkingResidents
        {
            get { return workingResidents; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Increases the ages of all residents and handles the retired residents.
        /// </summary>
        /// <param name="gameTable">The modell's gameTable.</param>
        public void YearHavePassed(GameTable gameTable)
        {
            Random rand = new Random();
            List<Person> retiredResidentsToRemove = new List<Person>();
            List<Person> workingResidnetsToAdd    = new List<Person>();

            foreach (Person resident in allResidents)
            {
                // Increase the age.
                resident.Age += 1;

                if (resident.IsRetired && resident.WorkZone is not null)
                {
                    // Remove resident from the workZone.
                    resident.WorkZone?.RemoveResident(resident);
                    resident.WorkZone = null;

                    // Relocate the resident in the retired residents.
                    workingResidents.Remove(resident);
                    retiredResidents.Add(resident);
                }
                else if (resident.IsRetired && resident.Age > rand.Next(65, 95) /*resident.Age >= 66*/)
                {
                    // The retired resident have died.
                    Person newResident = new Person(18);
                    workingResidnetsToAdd.Add(newResident);
                    retiredResidentsToRemove.Add(resident);
                }
            }

            // Removing the retired residents.
            foreach (Person retired in retiredResidentsToRemove)
            {
                retired.ResidentialZone?.ResidentList.Remove(retired);
                retiredResidents.Remove(retired);
                allResidents.Remove(retired);


            }

            // Adding the new resident.
            foreach (Person newResident in workingResidnetsToAdd)
            {
                allResidents.Add(newResident);
                workingResidents.Add(newResident);
                AccommodateOrRemoveResidents(
                    new List<Person>() { newResident },
                    gameTable, false
                );
            }
        }

        /// <summary>
        /// Calculates the happiness of every residents in the game.
        /// It also removes the critical residnets as well.
        /// </summary>
        /// <param name="gameTable">The modell's gameTable.</param>
        /// <param name="gameEconomy">The modell's gameEconomy.</param>
        public void CalculateGameHappiness(GameTable gameTable, GameEconomy gameEconomy)
        {
            int globalHappinessFactors = 0;

            //=================== TaxRate factor ======================

            bool isTaxRateLow = gameEconomy.TaxRate <= 20;
            globalHappinessFactors += isTaxRateLow ? 1 : -1;

            //=============== Negavite budget factor ==================

            int negativeBudgetDifference = 0;

            if (gameEconomy.GameFunds < 0)
            {
                NegaviteBudgetStreak += 1;
                negativeBudgetDifference = negaviteBudgetStreak *
                                           gameEconomy.GameFunds / 10000 - 1;
            }
            else
            {
                NegaviteBudgetStreak = 0;
            }
            globalHappinessFactors += negativeBudgetDifference;

            //======================= Szolgáltatások és ipari termelés aránya ==================

            double commertialWorkersCount = 0;

            foreach (CommertialZone commertial in gameTable.CommertialZones)
            {
                commertialWorkersCount += commertial.RetiredCounter;
            }

            double commertailPercent =
                workingResidents.Count > 0 ? commertialWorkersCount / workingResidents.Count : 0.5;

            bool commertailIndustrailBalanced
                = 0.4 <= commertailPercent && commertailPercent <= 0.6;

            globalHappinessFactors += commertailIndustrailBalanced ? 0 : -1;

            //=================== Közbiztonság, tűzoltóság és stadion ========================

            Dictionary<Zone, int> zoneFacilityFactors = new Dictionary<Zone, int>();

            foreach (PublicFacility facility in gameTable.PublicFacilities)
            {
                foreach (var discoveredZone
                         in gameTable.DiscoverZonesWithBFS(facility.TableRowPosition, facility.TableColumnPosition))
                {
                    int distance = gameTable.Distance(facility, discoveredZone.Key);

                    if (distance <= facility.RadiusOfPositiveEffect())
                    {
                        int facilityFactor = 0;
                        bool contains =
                            zoneFacilityFactors.TryGetValue(discoveredZone.Key, out facilityFactor);

                        facilityFactor +=
                                facility is PoliceDepartment ? PopulationCounter / 10 + 1 : 1;

                        if (!contains)
                        {
                            zoneFacilityFactors.Add(discoveredZone.Key, facilityFactor);
                        }
                        else
                        {
                            zoneFacilityFactors[discoveredZone.Key] = facilityFactor;
                        }
                    }
                }
            }

            //================= Zone based factor. ====================

            List<Person> residentsToRemove = new List<Person>();
            double allResidentsHappinessSum = 0;

            foreach (ResidentialZone residentialZone in gameTable.ResidentialZones)
            {
                //====================== Residentail zone factor  ===========================
                //=============== (nearby industrial zone and facility factor) ==============

                int residentialZoneFactors = 1;

                foreach (IndustrialZone industrial in gameTable.IndustrialZones)
                {
                    int distance = gameTable.Distance(residentialZone, industrial);
                    int maximumDistance = 5;

                    if (distance <= maximumDistance)
                    {
                        residentialZoneFactors = -1;
                    }
                }

                int facilityFactor = 0;
                zoneFacilityFactors.TryGetValue(residentialZone, out facilityFactor);

                residentialZoneFactors += facilityFactor;

                //=========================== Residents happines calculation =========================

                int happinessFactors = globalHappinessFactors + residentialZoneFactors;

                foreach (Person resident in residentialZone.ResidentList)
                {

                    //======================= Work factors (facility and distance) ================

                    int workZoneFactor = 0;
                    Zone? workZone = resident.WorkZone;

                    if (workZone is not null)
                    {
                        int workDistance = residentialZone.WorkZones[workZone].distance;
                        int maximumDistance = 10;

                        workZoneFactor += workDistance <= maximumDistance ? 1 : -1;

                        int workZoneFacilityFactor = 0;
                        zoneFacilityFactors.TryGetValue(workZone, out workZoneFacilityFactor);

                        workZoneFactor += workZoneFacilityFactor;
                    }

                    //======================== HappinessDuration factor ==================

                    int residentHappinessLevel = (int)resident.HappinessLevel;
                    int happinessDurationFactor = 0;

                    if (resident.HappinessDuration >= 10)
                    {
                        happinessDurationFactor +=
                            residentHappinessLevel >= (int)HappinessLevel.Normal ? 2 : -2;
                    }
                    else if (resident.HappinessDuration >= 5)
                    {
                        happinessDurationFactor +=
                            residentHappinessLevel >= (int)HappinessLevel.Normal ? 1 : -1;
                    }

                    //======================== Computing the happiness ==================

                    int happinessNumber = residentHappinessLevel +
                                          happinessFactors +
                                          workZoneFactor +
                                          happinessDurationFactor;

                    allResidentsHappinessSum += happinessNumber;

                    resident.HappinessLevel = ConvertToHappinessLevel(happinessNumber);

                    //============ Collecting the the critical residents to remove ============

                    int maximumToleratedDuration = 15;

                    if (resident.HappinessLevel == HappinessLevel.Critical &&
                        resident.HappinessDuration > maximumToleratedDuration &&
                        !resident.IsRetired)
                    {
                        residentsToRemove.Add(resident);
                    }
                }
            }

            //=========================== Calculating the avergae of the happiness =============

            if (PopulationCounter != 0)
            {
                HappinessLevel = ConvertToHappinessLevel(
                    Convert.ToInt32(Math.Ceiling(allResidentsHappinessSum / PopulationCounter)) // *100
                );
            }
            else
            {
                happinessStreak += 1;
            }

            //========================= Removing the critical residents =========================

            HashSet<Zone> zonesThatAreChanged = new HashSet<Zone>();

            foreach (Person resident in residentsToRemove)
            {
                // Workzone is effected.
                if (resident.WorkZone is not null)
                {
                    zonesThatAreChanged.Add(resident.WorkZone);
                }

                // Residential zone is effected.
                if (resident.ResidentialZone is not null)
                {
                    zonesThatAreChanged.Add(resident.ResidentialZone);
                }

                RemoveResident(resident);
            }

            // Update the zones that are changed.
            foreach (Zone zone in zonesThatAreChanged)
            {
                gameTable.OnTableFieldChanged(
                    zone.TableRowPosition,
                    zone.TableColumnPosition,
                    zone,
                    false
                );
            }
        }

        /// <summary>
        /// Adds new residents to the zones.
        /// </summary>
        /// <param name="gameTable">The modell's gameTable.</param>
        public void AddNewResidents(GameTable gameTable)
        {
            // The desired number of residents to be added.
            int totalNumOfNewResidents = NumberOfIncomingResidents();

            // The zones that are changed.
            HashSet<Zone> zonesThatAreChanged = new HashSet<Zone>();

            foreach (ResidentialZone residentalZone in gameTable.ResidentialZones)
            {
                //======================= Zone validation ======================

                // No more incoming new residents.
                if (totalNumOfNewResidents == 0) break;

                int residentalSpace = residentalZone.AvailableSpace;

                // The residentialZone has no available space.
                if (residentalSpace == 0) continue;

                //======================= Number of incoming residents =====================

                int numberOfResidentToBeAdded = totalNumOfNewResidents;
                if (HasNerbyIndustrialZone(residentalZone, gameTable))
                {
                    numberOfResidentToBeAdded /= 2;
                }

                int industrialAvailable   = 0;
                int commertialAvailable   = 0;
                int industrialWorkerCount = 0;
                int commertialWorkerCount = 0;
                PriorityQueue<IndustrialZone, int> industrials = new PriorityQueue<IndustrialZone, int>();
                PriorityQueue<CommertialZone, int> commertials = new PriorityQueue<CommertialZone, int>();

                foreach (KeyValuePair<Zone, (int distance, int workerCount)> workZonePair
                         in residentalZone.WorkZones)
                {
                    Zone workZone = workZonePair.Key;
                    (int distance, int workerCount) data = workZonePair.Value;

                    if (workZone is CommertialZone commertial)
                    {
                        if (commertial.AvailableSpace != 0)
                        {
                            commertials.Enqueue(commertial, data.distance);
                        }

                        commertialAvailable   += commertial.AvailableSpace;
                        commertialWorkerCount += commertial.ResidentCounter;
                    }
                    else if (workZone is IndustrialZone industrial)
                    {
                        if (industrial.AvailableSpace != 0)
                        {
                            industrials.Enqueue(industrial, data.distance);
                        }


                        industrialAvailable   += industrial.AvailableSpace;
                        industrialWorkerCount += industrial.ResidentCounter;
                    }
                }

                int toCommertial = 0;
                int toIndustrial = 0;

                // The number of residents to be added.
                numberOfResidentToBeAdded = Math.Min(
                    numberOfResidentToBeAdded,
                    Math.Min(residentalZone.AvailableSpace, industrialAvailable + commertialAvailable)
                );

                // If one of the zone types is not available.
                if (commertialAvailable == 0)
                {
                    toCommertial = 0;
                    toIndustrial = numberOfResidentToBeAdded;
                }
                else if (industrialAvailable == 0)
                {
                    toCommertial = numberOfResidentToBeAdded;
                    toIndustrial = 0;
                }

                // Filling up the difference between workers.
                if (industrialWorkerCount > commertialWorkerCount)
                {
                    toCommertial = Math.Min(
                        numberOfResidentToBeAdded,
                        Math.Min(commertialAvailable, industrialWorkerCount - commertialWorkerCount)
                    );
                    commertialAvailable -= toCommertial;
                }
                else
                {
                    toIndustrial = Math.Min(
                        numberOfResidentToBeAdded,
                        Math.Min(industrialAvailable, commertialWorkerCount - industrialWorkerCount)
                    );
                    industrialAvailable -= toIndustrial;
                }

                numberOfResidentToBeAdded -= toCommertial + toIndustrial;

                
                if (commertialAvailable > 0 && industrialAvailable > 0)
                {
                    toIndustrial = Math.Min(
                        Math.Min(numberOfResidentToBeAdded, industrialAvailable),
                        toIndustrial + Convert.ToInt32(Math.Floor((double)numberOfResidentToBeAdded / 2))
                    );
                    toCommertial = numberOfResidentToBeAdded - toIndustrial;
                }

                //========================= Beköltöztetés =========================

                IndustrialZone targetIndustrial = null;
                CommertialZone targetCommertial = null;

                // Adding workers to the industrial zones.
                while (toIndustrial > 0)
                {
                    // Getting the nearest industrialZone.
                    if (targetIndustrial is null ||
                        targetIndustrial.AvailableSpace == 0)
                    {
                        targetIndustrial = industrials.Dequeue();
                    }

                    // Adding the new Person.
                    Person resident  = new Person();
                    bool resSuccess  = residentalZone.AddResident(resident);
                    bool workSuccess = targetIndustrial.AddResident(resident);

                    // If the addition is unsuccessfull.
                    if (!resSuccess || !workSuccess)
                    {
                        residentalZone.RemoveResident(resident);
                        targetIndustrial.RemoveResident(resident);
                        continue;
                    }

                    // The zones are changed.
                    zonesThatAreChanged.Add(residentalZone);
                    zonesThatAreChanged.Add(targetIndustrial);

                    // Successfull addition.
                    allResidents.Add(resident);
                    workingResidents.Add(resident);
                    toIndustrial -= 1;
                }

                // Adding workers to the commertial zones.
                while (toCommertial > 0)
                {
                    // Getting the nearest commertial zone.
                    if (targetCommertial is null ||
                        targetCommertial.AvailableSpace == 0)
                    {
                        targetCommertial = commertials.Dequeue();
                    }

                    // Adding the new Person.
                    Person resident  = new Person();
                    bool resSuccess  = residentalZone.AddResident(resident);
                    bool workSuccess = targetCommertial.AddResident(resident);

                    // If the addition is unsuccessfull.
                    if (!resSuccess || !workSuccess)
                    {
                        residentalZone.RemoveResident(resident);
                        targetCommertial.RemoveResident(resident);
                        continue;
                    }

                    // The zones are changed.
                    zonesThatAreChanged.Add(residentalZone);
                    zonesThatAreChanged.Add(targetCommertial);

                    // Successfull addition.
                    allResidents.Add(resident);
                    workingResidents.Add(resident);
                    toCommertial -= 1;
                }
            }

            // Update the zones that are changed.
            foreach (Zone zone in zonesThatAreChanged)
            {
                gameTable.OnTableFieldChanged(
                    zone.TableRowPosition,
                    zone.TableColumnPosition,
                    zone,
                    false
                );
            }
        }

        /// <summary>
        /// Tries to accommodates the residents in the zones
        /// or removes them if no space is available.
        /// </summary>
        /// <param name="residentList">The list of residents to accommode.</param>
        /// <param name="gameTable">The modell's gameTable.</param>
        /// <param name="changeHappiness">Tells wether to change the residents happiness.</param>
        public void AccommodateOrRemoveResidents(List<Person> residentList, GameTable gameTable, bool changeHappiness)
        {
            List<Person> retiredResidents = new List<Person>();
            List<Person> workingResidents = new List<Person>();

            HashSet<Zone> zonesThatAreChanged = new HashSet<Zone>();

            // Sorting the residents to two groups.
            foreach (Person resident in residentList)
            {
                // Remove the resident from it's zones.
                if (resident.WorkZone is not null)
                {
                    zonesThatAreChanged.Add(resident.WorkZone);
                    resident.WorkZone.RemoveResident(resident);
                }
                if (resident.ResidentialZone is not null)
                {
                    zonesThatAreChanged.Add(resident.ResidentialZone);
                    resident.ResidentialZone.RemoveResident(resident);
                }

                // Change the happinessLevel of the resident.
                if (changeHappiness)
                {
                    if (resident.HappinessLevel >= HappinessLevel.High)
                    {
                        resident.HappinessLevel = HappinessLevel.Normal;
                    }
                    else if (resident.HappinessLevel >= HappinessLevel.Normal)
                    {
                        resident.HappinessLevel = HappinessLevel.Low;
                    }
                    else
                    {
                        resident.HappinessLevel = HappinessLevel.Critical;
                    }
                }

                // Sorting the residents.
                if (resident.IsRetired)
                {
                    retiredResidents.Add(resident);
                }
                else
                {
                    workingResidents.Add(resident);
                }
            }

            foreach (ResidentialZone residential in gameTable.ResidentialZones)
            {
                bool hasAvailableSpace = residential.AvailableSpace != 0;
                bool allAccommodated   = retiredResidents.Count == 0 &&
                                         workingResidents.Count == 0;

                // If there are no more residents.
                if (allAccommodated) break;

                // If the residential zone is full.
                if (!hasAvailableSpace) continue;

                // Accommodating the workers first.
                foreach (var workZonePair in residential.WorkZones)
                {
                    Zone workZone     = workZonePair.Key;
                    hasAvailableSpace = workZone.AvailableSpace != 0 &&
                                        workZonePair.Value.distance > 0;
                    allAccommodated   = workingResidents.Count == 0;

                    // If there are no more working residents.
                    if (allAccommodated) break;

                    // If the work zone is full.
                    if (!hasAvailableSpace) continue;

                    // Add zones to the zonesThatAreChanged
                    zonesThatAreChanged.Add(residential);
                    zonesThatAreChanged.Add(workZone);

                    while (!allAccommodated && hasAvailableSpace)
                    {
                        // Getting the first working resident.
                        Person worker = workingResidents[0];
                        workingResidents.RemoveAt(0);

                        // Accommodating the worker.
                        residential.AddResident(worker);
                        workZone.AddResident(worker);

                        allAccommodated   = workingResidents.Count  == 0;
                        hasAvailableSpace = workZone.AvailableSpace != 0;
                    }
                }

                hasAvailableSpace = residential.AvailableSpace != 0;
                allAccommodated   = retiredResidents.Count == 0;

                // If there are no more residents.
                if (allAccommodated) break;

                // If the residential zone is full.
                if (!hasAvailableSpace) continue;

                // Add residential to the zonesThatAreChanged.
                zonesThatAreChanged.Add(residential);

                while (!allAccommodated && hasAvailableSpace)
                {
                    // Getting the first retired resident.
                    Person retired = retiredResidents[0];
                    retiredResidents.RemoveAt(0);

                    // Remove the resident's previous residential zone.
                    //retired.ResidentialZone?.RemoveResident(retired);

                    // Accommodating the retired resident.
                    residential.AddResident(retired);

                    allAccommodated   = retiredResidents.Count == 0;
                    hasAvailableSpace = residential.AvailableSpace != 0;
                }
            }

            // Remove the remaining retired residents.
            foreach (Person retired in retiredResidents)
            {
                //retired.ResidentialZone?.RemoveResident(retired);

                AllResidents.Remove(retired);
                RetiredResidents.Remove(retired);
            }

            // Remove the remaining working residents.
            foreach (Person working in workingResidents)
            {
                //working.WorkZone?.RemoveResident(working);
                //working.ResidentialZone?.RemoveResident(working);

                AllResidents.Remove(working);
                WorkingResidents.Remove(working);
            }

            // Update the zones on the view.
            foreach (Zone zone in zonesThatAreChanged)
            {
                gameTable.OnTableFieldChanged(
                    zone.TableRowPosition, 
                    zone.TableColumnPosition, 
                    zone, 
                    false
                );
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Removes the resident from the it's residential and work zones.
        /// </summary>
        /// <param name="resident">The resident to remove.</param>
        private void RemoveResident(Person resident)
        {
            if (resident is null) return;

            resident.WorkZone?.RemoveResident(resident);
            resident.ResidentialZone?.RemoveResident(resident);
            
            if (resident.IsRetired)
            {
                retiredResidents.Remove(resident);
            }
            else
            {
                workingResidents.Remove(resident);
            }

            allResidents.Remove(resident);
        }

        /// <summary>
        /// Tells wether the given residential zone has a nearby industrial zone.
        /// </summary>
        /// <param name="residential">The residential zone.</param>
        /// <param name="gameTable">The modell's gameTable.</param>
        /// <returns>If the zone has a nearby industrial zone.</returns>
        private bool HasNerbyIndustrialZone(ResidentialZone residential, GameTable gameTable)
        {
            bool hasNearbyIndustrialZone = false;
            foreach (IndustrialZone industrial in gameTable.IndustrialZones)
            {
                if (hasNearbyIndustrialZone) break;

                hasNearbyIndustrialZone = gameTable.Distance(residential, industrial) < 5;
            }

            return hasNearbyIndustrialZone;
        }

        /// <summary>
        /// Calculates how many new residents would come.
        /// </summary>
        /// <returns>The number of incoming residents.</returns>
        private int NumberOfIncomingResidents()
        {
            int numOfNewResidents = 0;

            switch (happinessLevel)
            {
                case HappinessLevel.Normal: numOfNewResidents = 10; break;
                case HappinessLevel.High:   numOfNewResidents = 20; break;
                case HappinessLevel.Superb: numOfNewResidents = 30; break;
            }

            
            return numOfNewResidents;
        }

        /// <summary>
        /// Converts the integer representation of the happiness 
        /// into the enumeration representation.
        /// </summary>
        /// <param name="happinessNumber">The integer representation of the happiness.</param>
        /// <returns>The enu representation of the happiness.</returns>
        private HappinessLevel ConvertToHappinessLevel(int happinessNumber)
        {
            if (happinessNumber < (int)HappinessLevel.Low)
            {
                return HappinessLevel.Critical;
            }
            else if (happinessNumber < (int)HappinessLevel.Normal)
            {
                return HappinessLevel.Low;
            }
            else if (happinessNumber < (int)HappinessLevel.High)
            {
                return HappinessLevel.Normal;
            }
            else if (happinessNumber < (int)HappinessLevel.Superb)
            {
                return HappinessLevel.High;
            }
            else
            {
                return HappinessLevel.Superb;
            }
        }

        #endregion
    }
}
