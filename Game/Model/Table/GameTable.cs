using System;
using System.Linq;
using System.Collections.Generic;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.Zone;
using SimCity.Model.Table.Field.PublicFacility;

namespace SimCity.Model.Table
{
    public class GameTable
    {
        #region Fields

        private Random rng = new Random();
        private TableField[,] table;
        
        private int roadCounter;

        private List<ResidentialZone> residentialZones;
        private List<IndustrialZone>  industrialZones;
        private List<CommertialZone>  commertialZones;
        private List<PublicFacility>  publicFacilities;
        private List<Zone> workZones;

        // Fields used for the fire generation.
        private Dictionary<PositionBasedTableField, FireDepartment?> onFire;
        private Dictionary<FireDepartment, List<(int row, int column)>?> fireTruckPositions;

        #endregion

        #region Constructors

        /// <summary>
        /// GameTable constructor.
        /// </summary>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="columnCount">The number of columns.</param>
        public GameTable(int rowCount, int columnCount) 
        {
            table = new TableField[rowCount, columnCount];

            for (int i = 0; i < 30; ++i)
            {
                for (int j = 0; j < 40; ++j)
                {
                    table[i, j] = EmptyField.Instance();
                }
            }

            roadCounter = 0;

            residentialZones = new List<ResidentialZone>();
            industrialZones  = new List<IndustrialZone>();
            commertialZones  = new List<CommertialZone>();
            publicFacilities = new List<PublicFacility>();
            workZones = new List<Zone>();

            onFire = new Dictionary<PositionBasedTableField, FireDepartment?>();
            fireTruckPositions = new Dictionary<FireDepartment, List<(int row, int column)>?>();
        }

        /// <summary>
        /// GameTable constructor with default parameters.
        /// </summary>
        public GameTable() : this(30, 40) {}

        #endregion

        #region Events

        /// <summary>
        /// The field changed event.
        /// </summary>
        public static event EventHandler<TableFieldChangedEventArgs>? TableFieldChanged;

        /// <summary>
        /// The fire changed event.
        /// </summary>
        public static event EventHandler<FireEventArgs>? TableFireChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Property for the height of the table.
        /// </summary>
        public int Height
        {
            get { return table.GetLength(0); }
        }

        /// <summary>
        /// Property for the widht of the table.
        /// </summary>
        public int Widht
        {
            get { return table.GetLength(1); }
        }

        /// <summary>
        /// Property for the road counter.
        /// </summary>
        public int RoadCounter
        {
            get { return roadCounter; }
            set { roadCounter = value; }
        }
            
        /// <summary>
        /// Property for the table.
        /// </summary>
        public TableField[,] Table
        {
            get { return table; }
        }

        /// <summary>
        /// Property for the residential zones.
        /// </summary>
        public List<ResidentialZone> ResidentialZones
        {
            get { return residentialZones; }
        }

        /// <summary>
        /// Property for the commertial zones.
        /// </summary>
        public List<CommertialZone> CommertialZones
        {
            get { return commertialZones; }
        }

        /// <summary>
        /// Property for the industrial zones.
        /// </summary>
        public List<IndustrialZone> IndustrialZones
        {
            get { return industrialZones; }
        }

        /// <summary>
        /// Property for the public facilities.
        /// </summary>
        public List<PublicFacility> PublicFacility
        {
            get { return publicFacilities; }
        }

        /// <summary>
        /// Property for the work zones.
        /// </summary>
        public List<Zone> WorkZones
        {
            get { return workZones; }
        }

        /// <summary>
        /// Property for the public facilities.
        /// </summary>
        public List<PublicFacility> PublicFacilities
        {
            get { return publicFacilities; }
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Computes the distance of two PositionBased, with respect of their sizes.
        /// </summary>
        /// <param name="first">The first field.</param>
        /// <param name="second">The second field.</param>
        /// <returns>The computed distance of the fields.</returns>
        public int Distance(PositionBasedTableField first, PositionBasedTableField second)
        {
            int rowDistance = first.TableRowPosition - second.TableRowPosition;
            int colDistance = first.TableColumnPosition - second.TableColumnPosition;

            int firstSizeMinusOne  = first.Size() - 1;
            int secondSizeMinusOne = second.Size() - 1;
            
            // Computing the fields locations to each other.
            bool firstAbove = rowDistance < 0;
            bool firstUnder = rowDistance > 0;
            bool firstLeft  = colDistance < 0;
            bool firstRight = colDistance > 0;

            // Pushing the fields top left corners closer to each other.
            if (firstAbove) rowDistance += firstSizeMinusOne;
            if (firstUnder) rowDistance -= secondSizeMinusOne;
            if (firstLeft)  colDistance += firstSizeMinusOne;
            if (firstRight) colDistance -= secondSizeMinusOne;

            // Computing the Chebyshev (infinit) distance of the positions.
            return Math.Max(Math.Abs(rowDistance), Math.Abs(colDistance));
        }

        /// <summary>
        /// Creates and places a specified TableField subclass
        /// object onto the specified empty table field.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="fieldType">The type of the field to be placed.</param>
        public void PlaceField(int row, int column, TableFieldType fieldType)
        {
            if (!IsTablePositionEmpty(row, column))
            {
                throw new ArgumentException("Invalid position or position isn't empty!");
            }

            TableField tableField;
            switch (fieldType)
            {
                case TableFieldType.Road:
                    tableField =  PlaceRoad(row, column);
                    RoadCounter++;
                    break;

                case TableFieldType.ResidentialZone:
                case TableFieldType.CommertialZone:
                case TableFieldType.IndustrialZone:
					tableField = PlaceZone(row, column, fieldType);
                    break;

                case TableFieldType.PoliceDepartment:
                case TableFieldType.FireDepartment:
                case TableFieldType.Stadium:
					tableField = PlacePublicFacitily(row, column, fieldType);
                    break;

                default:
                    throw new ArgumentException();
            
            }

            OnTableFieldChanged(row, column, tableField, false);
        }

        /// <summary>
        /// Removes the table object from the specified table field.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="forceRemove">Tells to force the remove.</param>
        public void RemoveField(int row, int column, bool forceRemove,
                                GamePopulation gamePopulation, GameEconomy gameEconomy)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException();
            }

            TableField tableField = table[row, column];
            TopLeftPositionUpdate(ref row, ref column);

            if (tableField is Road)
            {
                tableField = RemoveRoad(row, column, forceRemove, gamePopulation, gameEconomy);
                RoadCounter--;
            }
            else if (tableField is Zone)
            {
                tableField = RemoveZone(row, column, forceRemove, gamePopulation, gameEconomy);
            }
            else if (tableField is PublicFacility)
            {
                tableField = RemovePublicFacility(row, column);
            }

            OnTableFieldChanged(row, column, tableField, true);
        }


        /// <summary>
        /// Upgrades the given zone to the next level.
        /// </summary>
        /// <param name="row">The table row position.</param>
        /// <param name="column">The table column position.</param>
        public void UpgradeZone(int row, int column)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException("Not a valid position.");
            }

            Zone? zone = table[row, column] as Zone;

            if (zone is null)
            {
                throw new ArgumentException("Not a zone.");
            }

            if(zone.IsOnFire > 0)
            {
				throw new ArgumentException("The zone is under fire. You can't upgrade it now!");
			}

            if (zone.ZoneLevel.ZoneLevelType() == ZoneLevelType.LevelThree)
            {
                throw new ArgumentException("Can not upgraded more, because it is maxed level!");
            }

            zone.UpgradeZoneLevel();
            OnTableFieldChanged(zone.TableRowPosition, zone.TableColumnPosition, zone, false);
        }


		/// <summary>
		/// Returns the selected zone from the table.
		/// </summary>
		/// <param name="row">The table row.</param>
		/// <param name="column">The table column.</param>
		/// <returns>The selected zone.</returns>
		public Zone GetZone(int row, int column)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException();
            }

            TableField field = table[row, column];
            
            if (field is Zone zone) return zone;

            throw new ArgumentException("NOT A ZONE");
        }

        /// <summary>
        /// Returns the selected public facility from the table.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <returns>The selected public facility.</returns>
        public PublicFacility GetFacility(int row,int column)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException();
            }

            TableField field = table[row, column];

            if (field is PublicFacility facility) return facility;

            throw new ArgumentException("NOT A FACILITY");
        }

        #endregion

        #region Table field manipulation methods

        /// <summary>
        /// Creates a Road instance on an empty field.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        private TableField PlaceRoad(int row, int column)
        {
            Road road = Road.Instance();

            InsertFieldToTable(row, column, road);
            ConnectZonesWithRoad(row, column);
            return road;
        }

        /// <summary>
        /// Creates a new Zone instance on an empty field.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="fieldType">The type of the zone (residential, industrial, commertial).</param>
        private TableField PlaceZone(int row, int column, TableFieldType fieldType)
        {
            Zone createdZone;

			if (fieldType == TableFieldType.ResidentialZone)
            {
                ResidentialZone residential = new ResidentialZone(row, column);
                createdZone = residential;

                InsertFieldToTable(row, column, residential);
                residentialZones?.Add(residential);
            }
            else if (fieldType == TableFieldType.CommertialZone)
            {
                CommertialZone commertial = new CommertialZone(row, column);
                createdZone = commertial;

                InsertFieldToTable(row, column, commertial);
                commertialZones?.Add(commertial);
                workZones?.Add(commertial);
            }
            else // fieldType == TableFieldType.IndustrialZone
            {
                IndustrialZone industrial = new IndustrialZone(row, column);
                createdZone = industrial;

                InsertFieldToTable(row, column, industrial);
                industrialZones?.Add(industrial);
                workZones?.Add(industrial);
            }
            

            // Assinging the zone into a new network.
            ConnectZoneWithZones(row, column);

            return createdZone;
        }

        /// <summary>
        /// Creates a new public facility on an empty filed.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="fieldType">The type of the facility (stadium, fire or police department).</param>
        private TableField PlacePublicFacitily(int row, int column, TableFieldType fieldType)
        {
            PublicFacility facility;

            if (fieldType == TableFieldType.Stadium)
            {
                facility = new Stadium(row, column);
            }
            else if (fieldType == TableFieldType.PoliceDepartment)
            {
                facility = new PoliceDepartment(row, column);
            }
            else // fieldType == TableFieldType.FireDepartment
            {
                FireDepartment fireDepartment = new FireDepartment(row, column);
                facility = fireDepartment;

                // There is an ongoing fire.
                if (onFire.Count > 0)
                {
                    fireTruckPositions.Add(fireDepartment, null);
                }
            }

            // Inserting the facility into the table
            InsertFieldToTable(row, column, facility);

            // Add to the publicfacilities.
            publicFacilities.Add(facility);

            return facility;
        }

        /// <summary>
        /// Removes a Road object from the specified table position.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="forceRemove">True to force the remove.</param>
        /// <param name="gamePopulation">The modell's gamePopulation.</param>
        /// <param name="gameEconomy">The modell's gameEconomy.</param>
        private TableField RemoveRoad(int row, int column, bool forceRemove,
                                      GamePopulation gamePopulation, GameEconomy gameEconomy)
        {

            // Getting the road "from the table".
            Road road = Road.Instance();

            RoadRemovedCheckConnections(row, column, forceRemove, gamePopulation, gameEconomy);

            // Removing the field from the table.
            RemoveFieldFromTable(row, column, road);

            // Returning the removed road instance.
            return road;
        }

        /// <summary>
        /// Removes a Zone object from the specified table position.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="forceRemove">To force the remove.</param>
        /// <param name="gameEconomy">The modell's gameEconomy.</param>
        /// <param name="gamePopulation">The modell's gamePopulation.</param>
        private TableField RemoveZone(int row, int column, bool forceRemove,
                                      GamePopulation gamePopulation, GameEconomy gameEconomy)
        {
            // Getting the zone from the table
            Zone? zoneToRemove = table[row, column] as Zone;

            if (zoneToRemove is null)
            {
                throw new ArgumentException("Not a zone!");
            }

            bool isEmpty      = zoneToRemove.ResidentCounter == 0;
            bool isRemoveable = isEmpty || forceRemove;

            if (!isRemoveable)
            {
                throw new InvalidOperationException("The zone is not removeable!");
            }

            // Removing the zone from the table.
            RemoveFieldFromTable(row, column, zoneToRemove);

            // Remove zone from the target workzones.
            if (zoneToRemove is not ResidentialZone)
            {
                foreach (ResidentialZone residentialZone in residentialZones)
                {
                    residentialZone.WorkZones.Remove(zoneToRemove);
                }
            }

            // Accommodate or remove the residents from the zone.
            List<Person> residentList = new List<Person>(zoneToRemove.ResidentList);
            gamePopulation.AccommodateOrRemoveResidents(residentList, this, true);
            gameEconomy.ChargeExtraExpence(residentList.Count * 1000);


            // Delete the zone form the appropriate list.
            if (zoneToRemove is ResidentialZone residential)
            {
                residentialZones.Remove(residential);
            }
            else if (zoneToRemove is CommertialZone commertial)
            {
                commertialZones.Remove(commertial);
                WorkZones.Remove(commertial);
            }
            else if (zoneToRemove is IndustrialZone industrial)
            {
                industrialZones.Remove(industrial);
                WorkZones.Remove(industrial);
            }

            return zoneToRemove;
        }

        /// <summary>
        /// Removes a PublicFacililty object from the specified table position.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        private TableField RemovePublicFacility(int row, int column)
        {
            // Getting the publicfacility from the table
            PublicFacility? facililty = table[row, column] as PublicFacility;

            if (facililty is null)
            {
                throw new ArgumentException("Not a public facility!");
            }

            // Removing the public facility from the table.
            RemoveFieldFromTable(row, column, facililty);

            // Removing the public facility form the list.
            publicFacilities.Remove(facililty);

            return facililty;
        }


        #endregion

        #region Fire and catastrophy methods

        /// <summary>
        /// Eldönti hogy mennyi zone/facility kapjon lángra egyszerre
        /// </summary>
        /// <param name="createCatastrophy">katasztrófa által lett-e meghívva</param>
        public void SimulateFires(bool createCatastrophy, GamePopulation gamePopulation, GameEconomy gameEconomy)
        {
            int fireNumber = 0;

            if (createCatastrophy)
            {
                fireNumber = 4;
            }
            else
            {
                // If there is no fire currently
                // Chance for a new fire generation is 10%
                if (onFire.Count == 0 && rng.Next(0, 200) < 1)
                {
                    // Generate 1 or 2 fires.
                    fireNumber = rng.Next(1, 3);
                }
            }

            // Generate the fires.
            Catastrophy(fireNumber);

            // Manage the currently burning fires.
            FireTime(gamePopulation, gameEconomy);

            // Move the firetrucks.
            MoveFireTrucks();
        }


        /// <summary>
        /// Jegyzi, hogy hány napja van tűz alatt egy adott zone/facility
        /// </summary>
        /// <param name="gamePopulation"></param>
        /// <param name="gameEconomy"></param>
        public void FireTime(GamePopulation gamePopulation, GameEconomy gameEconomy)
        {
            if (onFire.Count != 0)
            {
                foreach (KeyValuePair<PositionBasedTableField, FireDepartment?> firePairs 
                         in onFire.ToList())
                {
                    PositionBasedTableField burningField = firePairs.Key;

                    if (burningField.IsOnFire == 20)
                    {
                        FireSpread(burningField);
                    } 
                    if (burningField.IsOnFire == 30)
                    {
                        burningField.IsOnFire = 0;
                        onFire.Remove(burningField);
                        RemoveField(
                            burningField.TableRowPosition,
                            burningField.TableColumnPosition,
                            true,
                            gamePopulation,
                            gameEconomy
                        );
                        continue;
                    }

                    burningField.IsOnFire += 1;
                }
            }
        }

        /// <summary>
        /// Tovább terjeszti a tűzet a környező zone-ra/facility-re
        /// </summary>
        /// <param name="tField"></param>
        public void FireSpread(PositionBasedTableField tField)
        {
            foreach (PositionBasedTableField field in 
                     SorroundingFields(tField.TableRowPosition, tField.TableColumnPosition, 2))
            {
                if (field.IsOnFire == 0 && field is not FireDepartment)
                {
                    field.IsOnFire = 1;
                    onFire.Add(field, null);
                }
            }
        }

        /// <summary>
        /// Megnézi, hogy van e-közeli FireDepartment(tűzoltóság)
        /// </summary>
        /// <param name="tField"></param>
        /// <returns>Visszatér igaz-zal ha van, hamissal ha nincs</returns>
        public bool IsThereCloseFireDepartment(TableField tField)
        {
            foreach (PublicFacility pFacility in PublicFacilities)
            {
                if (pFacility is FireDepartment fireDep)
                {
                    if (tField is Zone zone)
                    {
                        if (Distance(fireDep, zone) < fireDep.RadiusOfPositiveEffect())
                        {
                            return true;

                        }
                    }
                    else if (tField is PublicFacility facility)
                    {
                        if (Distance(fireDep, facility) < fireDep.RadiusOfPositiveEffect())
                        {
                            return true;
                        }
                    }

                }
            }

            return false;
        }

        /// <summary>
        /// Paraméter mennyiségű zone/facility-re generál tűzet  
        /// </summary>
        /// <param name="catastrophyNumber">hány zone/facility-t érint a tűzvész</param>
        public void Catastrophy(int catastrophyNumber) //Ide a tűzoltoság is
        {
            if (residentialZones is null ||
                commertialZones  is null ||
                industrialZones  is null ||
                publicFacilities is null ||
                onFire is null)
            {
                return;
            }

            int index;

            List<FireDepartment> fireDepartments
                = new List<FireDepartment>();
            List<PublicFacility> nonFireDepartments
                = new List<PublicFacility>();

            foreach (PublicFacility facility in publicFacilities)
            {
                if (facility is FireDepartment fireDep)
                {
                    fireDepartments.Add(fireDep);
                }
                else
                {
                    nonFireDepartments.Add(facility);
                }
            }

            catastrophyNumber = Math.Min(
                catastrophyNumber,
                residentialZones.Count + 
                commertialZones.Count +
                industrialZones.Count + 
                nonFireDepartments.Count - onFire.Count
            );

            bool catastrophyStarted = catastrophyNumber > 0;

            while (catastrophyNumber > 0)
            {
                switch (rng.Next(1, 5))
                {
                    case 1:
                        if (residentialZones.Count > 0)
                        {
                            index = rng.Next(0, residentialZones.Count);
                            ResidentialZone zone = residentialZones[index];

                            if (zone.IsOnFire == 0)
                            {
                                if (IsThereCloseFireDepartment(zone))
                                {
                                    zone.IsSetOnFire(5);
                                }
                                else zone.IsSetOnFire(0);

                                if (zone.IsOnFire == 1)
                                {
                                    onFire.Add(zone, null);
                                    catastrophyNumber--;
                                }
                            }
                        }
                        break;

                    case 2:
                        if (commertialZones.Count > 0)
                        {
                            index = rng.Next(0, commertialZones.Count);
                            CommertialZone zone = commertialZones[index];

                            if (zone.IsOnFire == 0)
                            {
                                if (IsThereCloseFireDepartment(zone))
                                {
                                    zone.IsSetOnFire(5);
                                }
                                else zone.IsSetOnFire(0);

                                if (zone.IsOnFire == 1)
                                {
                                    onFire.Add(zone, null);
                                    catastrophyNumber--;
                                }
                            }
                        }
                        break;

                    case 3:
                        if (industrialZones.Count > 0)
                        {
                            index = rng.Next(0, industrialZones.Count);
                            IndustrialZone zone = industrialZones[index];

                            if (zone.IsOnFire == 0)
                            {
                                if (IsThereCloseFireDepartment(zone))
                                {
                                    zone.IsSetOnFire(5);
                                }
                                else zone.IsSetOnFire(0);

                                if (zone.IsOnFire == 1)
                                {
                                    onFire.Add(zone, null);
                                    catastrophyNumber--;
                                }
                            }
                        }
                        break;

                    case 4:
                        if (nonFireDepartments.Count > 0)
                        {
                            index = rng.Next(0, nonFireDepartments.Count);
                            PublicFacility facility = nonFireDepartments[index];

                            if (facility.IsOnFire == 0)
                            {
                                if (IsThereCloseFireDepartment(facility))
                                {
                                    facility.IsSetOnFire(5);
                                }
                                else facility.IsSetOnFire(0);

                                if (facility.IsOnFire == 1)
                                {
                                    onFire.Add(facility, null);
                                    catastrophyNumber--;
                                }
                            }
                        }
                        break;
                }
            }

            if (catastrophyStarted)
            {
                foreach (FireDepartment fireDepartment in fireDepartments)
                {
                    fireTruckPositions.TryAdd(fireDepartment, null);
                }
            }
        }

        /// <summary>
        /// Finst the closes burning building to the fire truck.
        /// </summary>
        /// <param name="startRow">The truck's row position.</param>
        /// <param name="startColumn">The truck's column position.</param>
        /// <param name="fireDepartment">The fire department of the truck.</param>
        /// <returns>The list of positions to the destination, or the null if there is nothing to do.</returns>
        private List<(int row, int column)>? FindClosestBurning(
            int startRow,
            int startColumn,
            FireDepartment fireDepartment)
        {
            if (!IsValidTablePosition(startRow, startColumn))
            {
                throw new ArgumentException("The startposition is not a valid tablePosition!");
            }

            // Dictionary of the discovered road positions and their parent positions.
            Dictionary<(int row, int column), (int parentRow, int parentColumn)> discovered
                = new Dictionary<(int, int), (int, int)>();

            // Queue of the discovered positions and their distances.
            Queue<(int row, int column, int distance)> unprocessed 
                = new Queue<(int, int, int)>();

            // Refreshing the starting positions and strartField
            // TopLeftPositionUpdate(ref startRow, ref startColumn);
            TableField startField = table[startRow, startColumn];

            // If the startField is a Road or a PositionBasedTableField.
            if (startField is Road || startField is PositionBasedTableField)
            {
                unprocessed.Enqueue((startRow, startColumn, 0));
                discovered.Add((startRow, startColumn), (-1, -1));
            }

            // The found destination.
            (int row, int column, int distance) foundDestination = (-1, -1, -1);

            // Running the BFS-algorithm.
            while (unprocessed.Count > 0)
            {
                // Getting the parent positions for expantion.
                (int parentRow, int parentColumn, int parentDistance) = unprocessed.Dequeue();

                // Getting the parent field.
                TableField parentField = table[parentRow, parentColumn];

                // Expanding the parent positions to the sorrounding valid positions.
                foreach ((int r, int c) in
                         SorroundingOffDiagonalPositions(parentRow, parentColumn))
                {
                    int row    = r;
                    int column = c;
                    //TopLeftPositionUpdate(ref row, ref column);

                    // If the expanded child field is already discovered.
                    if (discovered.ContainsKey((row, column)))
                    {
                        continue;
                    }

                    // Increase the distance.
                    int childDistance = parentDistance + 1;

                    // The discovered child field.
                    TableField discoveredField = table[row, column];

                    // If the discovered field is a
                    // PositionBasedTableField and the parent is road.
                    if (parentField is Road &&
                        discoveredField is PositionBasedTableField field)
                    {
                        // The closest buring field has been found.
                        if (onFire.ContainsKey(field))
                        {
                            FireDepartment? other     = onFire[field];
                            bool otherTruckIsOnTheWay = other is not null;
                            bool thisTruckIsCloser    =
                                otherTruckIsOnTheWay &&
                                fireTruckPositions[other] is not null &&
                                childDistance < fireTruckPositions[other].Count - 1;

                            if (!otherTruckIsOnTheWay || thisTruckIsCloser)
                            {
                                // Claim the destination.
                                onFire[field]    = fireDepartment;
                                foundDestination = (row, column, childDistance);

                                // If the destination has been claimed from 
                                // an other truck on the way.
                                if (thisTruckIsCloser)
                                {
                                    (int row, int column) otherPos =
                                        fireTruckPositions[other][0];

                                    fireTruckPositions[other] =
                                        FindClosestBurning(otherPos.row, otherPos.column, other);
                                }

                                // Discovering the expanded field.
                                discovered.Add((row, column), (parentRow, parentColumn));

                                // Stop the processing
                                unprocessed.Clear();
                                break;
                            }
                        }
                        else if (field.Equals(fireDepartment) && foundDestination == (-1, -1, -1))
                        {
                            foundDestination = (row, column, childDistance);

                            // Discovering the expanded field.
                            discovered.Add((row, column), (parentRow, parentColumn));
                        }
                        continue;
                    }
                    else if (discoveredField is not Road)
                    {
                        continue;
                    }

                    // Discovering the expanded field.
                    discovered.Add((row, column), (parentRow, parentColumn));

                    // Adding the discovered field for furhter expantion.
                    unprocessed.Enqueue((row, column, childDistance));
                }
            }

            // The list of result positions
            List<(int row, int column)>? discoveredRoute = null;

            // Constructing the best route to the destination.
            if (foundDestination != (-1, -1, -1) &&
               !startField.Equals(table[foundDestination.row, foundDestination.column]))
            {
                discoveredRoute = new List<(int, int)>(foundDestination.distance + 1);

                int ind = foundDestination.distance;
                (int row, int column) parent = (foundDestination.row, foundDestination.column);

                // Inserting the route positions
                while (ind >= 0)
                {
                    discoveredRoute.Add(parent);
                    parent = discovered[parent];
                    ind -= 1;
                }

                discoveredRoute?.Reverse();
            }

            return discoveredRoute;
        }

        /// <summary>
        /// Mozgatja a pályán levő tűzoltókat az utakon.
        /// </summary>
        public void MoveFireTrucks()
        {
            if (fireTruckPositions is null) return;

            // The positions to update.
            List<PositionBasedTableField> justExtinguished = new List<PositionBasedTableField>();
            List<(int row, int column)> previousPositions  = new List<(int, int)>();
            List<(int row, int column)> currentPositions   = new List<(int, int)>();

            foreach (KeyValuePair<FireDepartment, List<(int row, int column)>?> firePair
                     in fireTruckPositions)
            {
                // The current position of the firetruck.
                // The default position is the firetruck's fire department position.
                (int row, int column) currentPosition;
                currentPosition.row    = firePair.Key.TableRowPosition;
                currentPosition.column = firePair.Key.TableColumnPosition;

                bool onTheRoad = firePair.Value is not null;

                if (onTheRoad)
                {
                    // Update the firetruck's current position.
                    currentPosition = firePair.Value[0];

                    // Update the firetruck's previous position.
                    previousPositions.Add(currentPosition);

                    // If the firetruck is next to it's destination.
                    if (firePair.Value.Count == 2)
                    {
                        (int row, int column) destination = firePair.Value[1];

                        // Getting the destination position.
                        PositionBasedTableField? destinationField
                            = table[destination.row, destination.column] as PositionBasedTableField;

                        // The destination is a burning building.
                        if (destinationField is not null &&
                            destinationField is not FireDepartment &&
                            onFire.ContainsKey(destinationField))
                        {
                            // Extinguis the fire.
                            destinationField.IsOnFire = 0;
                            onFire.Remove(destinationField);
                            justExtinguished.Add(destinationField);

                            // Add the current position.
                            currentPositions.Add(currentPosition);
                        }
                        else if (destinationField is not null)
                        {
                            currentPosition = (firePair.Key.TableRowPosition, firePair.Key.TableColumnPosition);
                        }

                        // The firetruck has no destinatination.
                        fireTruckPositions[firePair.Key] = null;
                    }
                    else
                    {
                        // Move the firetruck closer to the destination.
                        firePair.Value.RemoveAt(0);
                        currentPosition = firePair.Value[0];
                        currentPositions.Add(currentPosition);
                    }
                }

                bool thereAreFires  = onFire.Count > 0;
                bool hasDestination = fireTruckPositions[firePair.Key] is not null;
                bool destinationIsBurning = hasDestination;
                onTheRoad = table[currentPosition.row, currentPosition.column] is Road;

                if (hasDestination)
                {
                    // The destination's position.
                    (int row, int column) destinationPosition = 
                        fireTruckPositions[firePair.Key].Last();

                    // The destination field.
                    PositionBasedTableField? destinationField =
                        table[destinationPosition.row, destinationPosition.column]
                        as PositionBasedTableField;

                    hasDestination       = destinationField is not null;
                    destinationIsBurning = destinationField is not null && 
                                           destinationField.IsOnFire > 0;
                }

                /* Find new destination if either of the statements is true:
                 * On the road && has no destination.
                 * On the road && the destination is not reachable.
                 * On the road && the destination is not burning && there are fires.
                 * Not on the road && there are fires
                 */

                // Find new destination.
                if ( onTheRoad && (!hasDestination || !destinationIsBurning && thereAreFires) ||
                    !onTheRoad && thereAreFires)
                {
                    fireTruckPositions[firePair.Key] =
                        FindClosestBurning(
                            currentPosition.row,
                            currentPosition.column,
                            firePair.Key
                        );
                }
            }

            //
            if (onFire.Count != 0 ||
                justExtinguished.Count != 0 ||
                previousPositions.Count != 0 ||
                currentPositions.Count != 0)
            {
                // Invoke the TableFireChanged event.
                OnFireChanged(justExtinguished, previousPositions, currentPositions);
            }
            else
            {
                fireTruckPositions.Clear();
            }
        }

        #endregion

        #region Network manipulation methods

        /// <summary>
        /// Calculates the distances to the available Zones from the starting position.
        /// </summary>
        /// <param name="startRow">The starting row position.</param>
        /// <param name="startColumn">The starting column position.</param>
        /// <returns>A dicitionary of Zone and distance pairs.</returns>
        public Dictionary<Zone, int> DiscoverZonesWithBFS(int startRow, int startColumn)
        {
            // Dictionary of the discovered positions and their distances.
            Dictionary<(int row, int column), int> discovered
                = new Dictionary<(int, int), int>();

            return DiscoverZonesWithBFS(startRow, startColumn, ref discovered);
        }

        /// <summary>
        /// Calculates the distances to the available Zones from the starting position.
        /// </summary>
        /// <param name="startRow">The starting row position.</param>
        /// <param name="startColumn">The starting column position.</param>
        /// <param name="discovered">The dictionary of previously discovered positions and distances.</param>
        /// <returns>A dicitionary of Zone and distance pairs.</returns>
        public Dictionary<Zone, int> DiscoverZonesWithBFS(
            int startRow, 
            int startColumn,
            ref Dictionary<(int row, int column), int> discovered)
        {
            if (!IsValidTablePosition(startRow, startColumn))
            {
                throw new ArgumentException("The startposition is not a valid tablePosition!");
            }

            // Dictionary of the discovered Zones and their distances.
            Dictionary<Zone, int> discoveredZones
                = new Dictionary<Zone, int>();

            // Queue of the discovered positions and their distances.
            Queue<(int row, int column, int distance)> unprocessed
                = new Queue<(int, int, int)>();

            // Refreshing the starting positions and strartField
            TopLeftPositionUpdate(ref startRow, ref startColumn);
            TableField startField = table[startRow, startColumn];

            // If the startField is a Road and not already discovered.
            if ((startField is Road || startField is PositionBasedTableField) &&
                !discovered.ContainsKey((startRow, startColumn)))
            {
                unprocessed.Enqueue((startRow, startColumn, 0));

                if (startField is Zone startZone)
                {
                    discoveredZones.Add(startZone, 0);
                }
                else if (startField is Road)
                {
                    discovered.Add((startRow, startColumn), 0);
                }
            }

            // Running the BFS-algorithm.
            while (unprocessed.Count > 0)
            {
                // Getting the parent positions for expantion.
                (int parentRow, int parentColumn, int parentDistance) = unprocessed.Dequeue();

                // Parentfield
                TableField parentField = table[parentRow, parentColumn];

                // Expanding the parent positions to the sorrounding valid positions.
                foreach ((int r, int c) in
                         SorroundingOffDiagonalPositions(parentRow, parentColumn))
                {
                    int row    = r;
                    int column = c;
                    TopLeftPositionUpdate(ref row, ref column);

                    // If the expanded field is already discovered.
                    if (discovered.ContainsKey((row, column)))
                    {
                        continue;
                    }

                    // Increase the distance by one.
                    int distance = parentDistance + 1;
                    TableField discoveredField = table[row, column];

                    // NOTE: lehet-e a Zone típusú mező discovered?

                    // If the discovered field is a zone and the parent is road.
                    if (parentField is Road && discoveredField is Zone zone)
                    {
                        if (!discoveredZones.ContainsKey(zone))
                        {
                            discoveredZones.Add(zone, distance);
                        }
                        continue;
                    }
                    else if (discoveredField is not Road)
                    {
                        continue;
                    }

                    // Discovering the expanded field.
                    discovered.Add((row, column), distance);

                    // Adding the discovered field for furhter expantion.
                    unprocessed.Enqueue((row, column, distance));
                }
            }

            return discoveredZones;
        }


        /// <summary>
        /// Connects the newly placed zone with other zones.
        /// </summary>
        /// <param name="row">The row of the placed zone.</param>
        /// <param name="column">The column of the placed zone.</param>
        private void ConnectZoneWithZones(int row, int column)
        {
            // If the start positions is invalid.
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException();
            }

            // Getting the start Zone.
            Zone? start = table[row, column] as Zone;

            // If the startPositions is not a Zone.
            if (start is null)
            {
                throw new ArgumentException();
            }
            
            // For every discovered Zone.
            foreach (KeyValuePair<Zone, int> discoveredZone in
                     DiscoverZonesWithBFS(row, column))
            {
                Zone zone     = discoveredZone.Key;
                int  distance = discoveredZone.Value;

                Zone? workZone = null;
                ResidentialZone? residential = null;

                // Getting the workZone and the residential zone.
                if (start is ResidentialZone &&
                    zone  is not ResidentialZone)
                {
                    workZone    = zone;
                    residential = start as ResidentialZone;
                }
                else if (zone  is ResidentialZone && 
                         start is not ResidentialZone)
                {
                    workZone    = start;
                    residential = zone as ResidentialZone;
                }

                // Check for a workzone and residentialzone pair.
                if (workZone is null || residential is null)
                {
                    continue;
                }

                (int distance, int workerCount) currentField = (0, 0);
                bool contains =
                    residential.WorkZones.TryGetValue(workZone, out currentField);

                // Updating the residential's workZones if neccessarry.
                if (!contains || distance < currentField.distance)
                {
                    residential.WorkZones[workZone] = (distance, currentField.workerCount);
                }
            }
        }

        /// <summary>
        /// Connect the zones with other available zones.
        /// </summary>
        /// <param name="row">The row of the placed road.</param>
        /// <param name="column">The column of the placed road.</param>
        private void ConnectZonesWithRoad(int row, int column)
        {
            // If the start positions is invalid.
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException();
            }

            // Getting the start Road.
            Road? start = table[row, column] as Road;

            // If the startposition is not a Road.
            if (start is null)
            {
                throw new ArgumentException();
            }

            Dictionary<(int row, int column), int> positions = new Dictionary<(int, int), int>
            {
                { (row, column), 1 }
            };

            List<Dictionary<Zone, int>> resultBFS = new List<Dictionary<Zone, int>>();

            foreach ((int r, int c) in
                     SorroundingOffDiagonalPositions(row, column))
            {
                Dictionary<Zone, int> result = DiscoverZonesWithBFS(r, c, ref positions);

                if (result.Count > 0)
                {
                    resultBFS.Add(result);
                }
            }

            foreach (Dictionary<Zone, int> firstResult in resultBFS)
            {
                foreach (Dictionary<Zone, int> secondResult in resultBFS)
                {
                    if (firstResult != secondResult)
                    {
                        foreach (KeyValuePair<Zone, int> firstZone in firstResult)
                        {
                            if (firstZone.Key is ResidentialZone residential)
                            {
                                int residentialDist = firstZone.Value;

                                foreach (KeyValuePair<Zone, int> secondZone in secondResult)
                                {
                                    Zone workZone = secondZone.Key;
                                    int workDist = secondZone.Value;
                                    int newDist = workDist + residentialDist + 2;

                                    if (workZone is ResidentialZone)
                                    {
                                        continue;
                                    }

                                    (int distance, int workerCount) current = (0, 0);
                                    bool contains = residential.WorkZones.TryGetValue(workZone, out current);

                                    if (!contains || newDist < current.distance)
                                    {
                                        residential.WorkZones[workZone] = (newDist, current.workerCount);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the connections between zones.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        /// <param name="forceRemove">To force the remove.</param>
        /// <param name="gamePopulation">The modell's game population.</param>
        /// <param name="gameEconomy">The modell's game economy.</param>
        public void RoadRemovedCheckConnections(int row, int column, bool forceRemove,
                                                GamePopulation gamePopulation, GameEconomy gameEconomy)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException("Invalid position!");
            }

            Road? roadToRemove = table[row, column] as Road;

            if (roadToRemove is null)
            {
                throw new ArgumentException("Starting positions is not a Road!");
            }

            // The discovered positions and their distances.
            Dictionary<(int row, int column), int> positions = new Dictionary<(int, int), int>
            {
                { (row, column), -1 }
            };

            // The result of the initial BFS.
            List<Dictionary<Zone, int>> initialBFS = new List<Dictionary<Zone, int>>();

            // Run the initial BFS search from the sorround positions.
            foreach ((int r, int c) in
                     SorroundingOffDiagonalPositions(row, column))
            {
                Dictionary<Zone, int> result;

                if (table[r, c] is Zone sorroundingZone)
                {
                    result = new Dictionary<Zone, int>()
                    {
                        { sorroundingZone, 0 }
                    };
                }
                else
                {
                    result = DiscoverZonesWithBFS(r, c, ref positions);
                }

                if (result.Count > 0)
                {
                    initialBFS.Add(result);
                }
            }

            // The changes for the discovered residential zones.
            Dictionary<ResidentialZone, Dictionary<Zone, (int distance, int workerCount)>> changes
                = new Dictionary<ResidentialZone, Dictionary<Zone, (int, int)>>();

            bool forceIsNecesserry = false;

            foreach (Dictionary<Zone, int> first in initialBFS)
            {
                foreach (Dictionary<Zone, int> second in initialBFS)
                {
                    if (first == second) continue;

                    foreach (KeyValuePair<Zone, int> firstZone in first)
                    {
                        int residentialDist = firstZone.Value;
                        ResidentialZone? residential = firstZone.Key as ResidentialZone;
                        Dictionary<Zone, int>? secondaryBFS = null;

                        if (residential is null) continue;

                        foreach (KeyValuePair<Zone, int> secondZone in second)
                        {
                            Zone workZone = secondZone.Key;
                            int  workDist = secondZone.Value;

                            if (workZone is ResidentialZone) continue;

                            (int distance, int workerCount) data;
                            bool available = residential.WorkZones.TryGetValue(workZone, out data);

                            // Elérhető volt, dolgoznak is ott, ráadásul a legjobb út szűnt meg.
                            if (available && data.workerCount > 0 &&
                                data.distance >= residentialDist + workDist + 2)
                            {
                                if (secondaryBFS is null)
                                {
                                    secondaryBFS = DiscoverZonesWithBFS(
                                        residential.TableRowPosition, 
                                        residential.TableColumnPosition, 
                                        ref positions
                                    );
                                }

                                // Finding an alternative route between te residential and workzone.
                                available = secondaryBFS.TryGetValue(workZone, out data.distance);

                                changes.TryAdd(residential, new Dictionary<Zone, (int, int)>());

                                changes[residential].Add(
                                    workZone,
                                    (available ? data.distance : -1,
                                    data.workerCount)
                                );

                                forceIsNecesserry = !available;
                            }
                        }
                    }
                }
            }

            // Validate if the removeal should go through.
            if (forceIsNecesserry && !forceRemove)
            {
                throw new InvalidOperationException("Cannot remove road");
            }

            // Make the changes for every effected zone.
            foreach (var changePair in changes)
            {
                ResidentialZone residential = changePair.Key;

                foreach (var workZonePair in changePair.Value)
                {
                    Zone workZone = workZonePair.Key;
                    (int distance, int workerCount) data = workZonePair.Value;

                    residential.WorkZones[workZone] = (data.distance, data.workerCount);

                    // If the workzone is not reachable any more.
                    if (residential.WorkZones[workZone].distance <= 0)
                    {
                        // Getting the workers of that workzone.
                        List<Person> workers = new List<Person>();

                        foreach (Person worker in residential.ResidentList)
                        {
                            // If the worker works at the uncreachable workzone.
                            bool? worksThere = worker.WorkZone?.Equals(workZone);

                            if (worksThere is not null && worksThere.Value)
                            {
                                workers.Add(worker);
                            }
                        }

                        // Relocate the residents or remove them.
                        gamePopulation.AccommodateOrRemoveResidents(workers, this, true);
                        gameEconomy.ChargeExtraExpence(workers.Count * 1000);

                        // The workZone is not reachable any more.
                        residential.WorkZones.Remove(workZone);
                    }
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Validates the position by checking the bounds.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        private bool IsValidTablePosition(int row, int column)
        {
            return 0 <= row && row < table.GetLength(0) &&
                   0 <= column && column < table.GetLength(1);
        }

        /// <summary>
        /// Validates the position and checks if it's empty.
        /// </summary>
        /// <param name="row">The table row.</param>
        /// <param name="column">The table column.</param>
        private bool IsTablePositionEmpty(int row, int column)
        {
            return IsValidTablePosition(row, column) &&
                   table[row, column] is EmptyField;
        }

        /// <summary>
        /// Update the row, column positions to the top right position.
        /// </summary>
        /// <param name="row">The row position.</param>
        /// <param name="column">The column position.</param>
        private void TopLeftPositionUpdate(ref int row, ref int column)
        {
            if (IsValidTablePosition(row, column) &&
                table[row, column] is PositionBasedTableField field)
            {
                row    = field.TableRowPosition;
                column = field.TableColumnPosition;
            }
        }

        /// <summary>
        /// Validates the positions and then inserts the specified field into the table.
        /// </summary>
        /// <param name="row">The top right table row.</param>
        /// <param name="column">The top right table column.</param>
        /// <param name="field">The top field to remove</param>
        public void InsertFieldToTable(int row, int column, TableField field)
        {
            // Checking if all the table positions are valid and empty
            for (int r = row; r < row + field.Size(); ++r)
            {
                for (int c = column; c < column + field.Size(); ++c)
                {
                    if (!IsTablePositionEmpty(r, c)) 
                        throw new ArgumentException("InsertFieldToTable error");
                }
            }

            // Inserting the field into the empty positions
            for (int r = row; r < row + field.Size(); ++r)
            {
                for (int c = column; c < column + field.Size(); ++c)
                {
                    table[r, c] = field;
                }
            }
        }

        /// <summary>
        /// Validates the positions and then removes the specified field from the table.
        /// </summary>
        /// <param name="row">The top right table row.</param>
        /// <param name="column">The top right table column.</param>
        /// <param name="field">The top field to remove</param>
        private void RemoveFieldFromTable(int row, int column, TableField field)
        {
            // Update the given position to the field's top left corner position
            TopLeftPositionUpdate(ref row, ref column);

            // Checking if all the table positions are valid and equal to field
            for (int r = row; r < row + field.Size(); ++r)
            {
                for (int c = column; c < column + field.Size(); ++c)
                {
                    if ( !IsValidTablePosition(r, c) ||
                         !table[r, c].Equals(field) )
                    {
                        throw new ArgumentException();
                    }
                }
            }

            // Setting the table positions empty
            for (int r = row; r < row + field.Size(); ++r)
            {
                for (int c = column; c < column + field.Size(); ++c)
                {
                    table[r, c] = EmptyField.Instance();
                }
            }
        }

        /// <summary>
        /// Generates the valid sorrounding off-daigonal positions of a field.
        /// </summary>
        /// <param name="row">The top right table row.</param>
        /// <param name="column">The top right table column.</param>
        /// <returns>The sorrounding off-diagonal positions on the table.</returns>
        private List<(int row, int column)> SorroundingOffDiagonalPositions(int row, int column)
        {
            if (!IsValidTablePosition(row, column))
            {
                throw new ArgumentException("Invalid position");
            }

            TopLeftPositionUpdate(ref row, ref column);
            TableField field = table[row, column];

            List<(int row, int column)> positions = new List<(int row, int column)>();

            int fieldSize = field.Size();
            int rowStart  = row - 1;
            int rowEnd    = row + fieldSize;

            for (int r = rowStart; r <= rowEnd; ++r)
            {
                int columnStart, columnEnd, columnStep;

                if (r == rowStart || r == rowEnd)
                {
                    columnStart = column;
                    columnEnd   = column + fieldSize - 1;
                    columnStep  = 1;
                }
                else
                {
                    columnStart = column - 1;
                    columnEnd   = column + fieldSize;
                    columnStep  = fieldSize + 1;
                }

                for (int c = columnStart; c <= columnEnd; c += columnStep)
                {
                    if (IsValidTablePosition(r, c))
                    {
                        positions.Add((r, c));
                    }
                }
            }

            return positions;
        }

		/// <summary>
		/// Finds the sorrounding PositionBasedFields in a given radius.
		/// </summary>
		/// <param name="row">The top right table row.</param>
		/// <param name="column">The top right table column.</param>
		/// <param name="distance">The distance around the startfield.</param>
		/// <returns>The found sorrounding PositionBasedFields.</returns>
		private HashSet<PositionBasedTableField> SorroundingFields(int row, int column, int distance)
		{
			if (!IsValidTablePosition(row, column))
			{
				throw new ArgumentException("Invalid position");
			}

			TopLeftPositionUpdate(ref row, ref column);
			TableField startField = table[row, column];

			HashSet<PositionBasedTableField> sorroundingFields = new HashSet<PositionBasedTableField>();

			int fieldSize = startField.Size();
			int rowStart = row - distance;
			int rowEnd = row + fieldSize + distance - 1;
			int columnStart = column - distance;
			int columnEnd = column + fieldSize + distance - 1;

			for (int r = rowStart; r <= rowEnd; ++r)
			{
				int c = columnStart;
				int columnStep;

				while (c <= columnEnd)
				{
					if (IsValidTablePosition(r, c))
					{
						PositionBasedTableField? field = table[r, c] as PositionBasedTableField;

						if (field is not null && !sorroundingFields.Contains(field))
						{
							sorroundingFields.Add(field);
						}
					}

					if (r < row || r < row + fieldSize - 1 || column != c + 1)
					{
						columnStep = 1;
					}
					else
					{
						columnStep = fieldSize + 1;
					}

					c += columnStep;
				}
			}

			return sorroundingFields;
		}

		#endregion

		#region Event invokers

        /// <summary>
        /// Invokes the TableFieldChanged event.
        /// </summary>
        /// <param name="row">The field's row position.</param>
        /// <param name="column">The field's column position.</param>
        /// <param name="field">The table field instance.</param>
        /// <param name="isRemoved">True if the field has been removed.</param>
		public void OnTableFieldChanged(int row, int column, TableField field, bool isRemoved)
        {
            TableFieldChanged?.Invoke(this, new TableFieldChangedEventArgs(row, column, field, isRemoved));
        }

        /// <summary>
        /// Invokes the FireChanged event.
        /// </summary>
        /// <param name="justExtinguished">The list of just extinguished fields.</param>
        /// <param name="previousPositions">The list of previous truck positions.</param>
        /// <param name="currentPositions">The list of current truck positions.</param>
        private void OnFireChanged(
            List<PositionBasedTableField> justExtinguished,
            List<(int row, int column)> previousPositions,
            List<(int row, int column)> currentPositions)
        {
            TableFireChanged?.Invoke(
                this, 
                new FireEventArgs(onFire.Keys.ToList(), justExtinguished, previousPositions, currentPositions)
            );
        }

        #endregion
    }
}
