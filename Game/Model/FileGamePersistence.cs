using SimCity.Model.Population;
using SimCity.Model.Table;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.PublicFacility;
using SimCity.Model.Table.Field.Zone;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace SimCity.Model
{
    public class FileGamePersistence : IGamePersistence
    {
        public List<String> _saved;

        public async Task LoadRoads(StreamReader reader, GameTable table)
        {
            var line = await reader.ReadLineAsync();
            var splitData = line.Split(" ");
            int num = Int32.Parse(line);
            int col, row;
            Road road = Road.Instance();
            for (int i = 0; i < num; i++)
            {
                //következő sor út beolvasás
                line = await reader.ReadLineAsync();
                splitData = line.Split(" ");
                row = Int32.Parse(splitData[0]);
                col = Int32.Parse(splitData[1]);
                table.InsertFieldToTable(row, col, road);
                
            }
        }

        public async Task LoadPublicFacilites(StreamReader reader, GameTable table)
        {
            var line = await reader.ReadLineAsync();
            var splitData = line.Split(" ");
            int num = Int32.Parse(line);
            FireDepartment fire;
            PoliceDepartment police;
            Stadium stadium;
            int row, col;
            TableFieldType fieldType;
            for (int i = 0; i < num; i++)
            {
                line = await reader.ReadLineAsync();
                splitData = line.Split(" ");
                fieldType = (TableFieldType)Int32.Parse(splitData[0]);
                row = Int32.Parse(splitData[1]);
                col = Int32.Parse(splitData[2]);
                switch (fieldType)
                {
                    case TableFieldType.PoliceDepartment:
                        police = new PoliceDepartment(row, col);
                        table.InsertFieldToTable(row, col, police);
                        table.PublicFacilities.Add(police);
                        break;
                    case TableFieldType.FireDepartment:
                        fire = new FireDepartment(row, col);
                        table.InsertFieldToTable(row, col, fire);
                        table.PublicFacilities.Add(fire);

                        break;
                    case TableFieldType.Stadium:
                        stadium = new Stadium(row, col);
                        table.InsertFieldToTable(row, col, stadium);
                        table.PublicFacilities.Add(stadium);

                        break;
                }
            }
        }

        public async Task LoadZones(StreamReader reader, GameTable table)
        {
            
            var line = await reader.ReadLineAsync();
            var splitData = line.Split(" ");
            int num = Int32.Parse(line);
            FireDepartment fire;
            PoliceDepartment police;
            Stadium stadium;
            int row, col;
            num = Int32.Parse(line);
            CommertialZone com;
            for (int i = 0; i < num; i++)
            {
                line = await reader.ReadLineAsync();
                splitData = line.Split(" ");
                row = Int32.Parse(splitData[0]);
                col = Int32.Parse(splitData[1]);
                
                com = new CommertialZone(row, col);
                table.InsertFieldToTable(row, col, com);

                //fejlesztés
                for (int j = 1; j < Int32.Parse(splitData[2]); j++)
                {
                    table.UpgradeZone(row, col);
                }
                table.CommertialZones.Add(com);
                table.WorkZones.Add(com);
            }

            //Industrial zone
            //sor beolvasása
            line = await reader.ReadLineAsync();
            //hányat kell olvasni
            num = Int32.Parse(line);
            IndustrialZone ind;
            for (int i = 0; i < num; i++)
            {
                line = await reader.ReadLineAsync();
                splitData = line.Split(" ");
                row = Int32.Parse(splitData[0]);
                col = Int32.Parse(splitData[1]);
                ind = new IndustrialZone(row, col);
                table.InsertFieldToTable(row, col, ind);

                //fejlesztés
                for (int j = 1; j < Int32.Parse(splitData[2]); j++)
                {
                    table.UpgradeZone(row, col);
                }
                table.IndustrialZones.Add(ind);
                table.WorkZones.Add(ind);

            }

            line = await reader.ReadLineAsync();
            num = Int32.Parse(line);
            int worknum, level;
            int workIndex, distance, workerCount;
            ResidentialZone res;
            //Residential zone
            for (int i = 0; i < num; i++)
            {
                line = await reader.ReadLineAsync();
                splitData = line.Split(" ");
                row = Int32.Parse(splitData[0]);
                col = Int32.Parse(splitData[1]);
                level = Int32.Parse(splitData[2]);
                res = new ResidentialZone(row, col);
                
                worknum = Int32.Parse(splitData[3]);
                for (int j = 0; j < worknum; j++)
                {
                    line = await reader.ReadLineAsync();
                    splitData = line.Split(" ");
                    workIndex = Int32.Parse(splitData[0]);
                    distance = Int32.Parse(splitData[1]);
                    workerCount = Int32.Parse(splitData[2]);
                    res.WorkZones.Add(table.WorkZones[workIndex], (distance, workerCount));
                }

               table.InsertFieldToTable(row, col, res);
                //fejlesztés
                for (int j = 0; j < level; j++)
                {
                    table.UpgradeZone(row, col);
                }
                table.ResidentialZones.Add(res);

            }

        }
        public async Task<GameEconomy> LoadEconomy(StreamReader reader)
        {
            try
            {
                    GameEconomy economy = new GameEconomy();
                    var line = await reader.ReadLineAsync();
                    var splitData = line.Split(" ");
                    economy.TaxRate = Int32.Parse(splitData[0]);
                    economy.TaxesCollected= Int32.Parse(splitData[1]);
                    economy.GameFunds = Int32.Parse(splitData[2]);
                    economy.RetirementExpenses= Int32.Parse(splitData[3]);
                    line = await reader.ReadLineAsync();
                KeyValuePair<TableFieldType, int> pair = new KeyValuePair<TableFieldType, int>
                    (TableFieldType.FireDepartment, Int32.Parse(line));
                economy.TableFieldExpenses.Add(pair.Key, pair.Value);

                line = await reader.ReadLineAsync();
                pair = new KeyValuePair<TableFieldType, int>
                    (TableFieldType.PoliceDepartment, Int32.Parse(line));
                economy.TableFieldExpenses.Add(pair.Key, pair.Value);

                line = await reader.ReadLineAsync();
                pair = new KeyValuePair<TableFieldType, int>
                    (TableFieldType.Stadium, Int32.Parse(line));
                economy.TableFieldExpenses.Add(pair.Key, pair.Value);
                line = await reader.ReadLineAsync();
                pair = new KeyValuePair<TableFieldType, int>
                    (TableFieldType.Road, Int32.Parse(line));
                economy.TableFieldExpenses.Add(pair.Key, pair.Value);
                return economy;
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GameTimer> LoadTimer(StreamReader reader, GameTimer _timer)
        {
            try
            {
                var line = await reader.ReadLineAsync();
                var splitData = line.Split(" ");
                _timer.CurrentYear= Int32.Parse(splitData[0]);
                _timer.CurrentMonth = (Month)Int32.Parse(splitData[1]);
                _timer.CurrentDay = Int32.Parse(splitData[2]);
                _timer.PlaySpeed= (PlaySpeedType)Int32.Parse(splitData[3]);
                

                return _timer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GamePopulation> LoadPopulation(StreamReader reader, GameTable table)
        {
            try
            {
                GamePopulation population = new GamePopulation();
                var line = await reader.ReadLineAsync();
                var splitData = line.Split(" ");
                population.HappinessStreak = Int32.Parse(splitData[0]);
                population.NegaviteBudgetStreak= Int32.Parse(splitData[1]);
                population.HappinessLevel = (HappinessLevel)Int32.Parse(splitData[2]);
                

                Person person;
                //population.AllResidents.Count = num;
                string name;
                int age, happinessdrt, num, pension, resnum, worknum;
                line = await reader.ReadLineAsync();
                num = Int32.Parse(line);
                HappinessLevel happinesslvl;
                ResidentialZone? res;
                Zone? workzone;
                for (int i = 0; i < num; i++)
                {
                    //new line
                    line = await reader.ReadLineAsync();
                    splitData = line.Split(" ");
                    //basic data
                    if (splitData.Length == 8)
                    {
                        name = splitData[0] + splitData[1];
                        age = Int32.Parse(splitData[2]);
                        happinesslvl = (HappinessLevel)Int32.Parse(splitData[3]);
                        happinessdrt = Int32.Parse(splitData[4]);
                        pension = Int32.Parse(splitData[5]);
                        resnum = Int32.Parse(splitData[6]);
                        worknum = Int32.Parse(splitData[7]);
                    }
                    else
                    {
                        name = splitData[0];
                        age = Int32.Parse(splitData[1]);
                        happinesslvl = (HappinessLevel)Int32.Parse(splitData[2]);
                        happinessdrt = Int32.Parse(splitData[3]);
                        pension = Int32.Parse(splitData[4]);
                        resnum = Int32.Parse(splitData[5]);
                        worknum = Int32.Parse(splitData[6]);
                    }
                    
                    
                    //living and working place
                    res = table.ResidentialZones[resnum];

                    if (worknum == -1)
                    {
                        workzone = null;
                    }
                    else
                    {
                        workzone = table.WorkZones[worknum];
                    }

                    //create person
                    person = new Person(age, name, happinesslvl, happinessdrt, workzone, res);

                    //assign pension
                    if(pension>0)
                    {
                        line = await reader.ReadLineAsync();
                        splitData = line.Split(" ");

                        for (int j = 0; j<pension; j++)
                        {
                            person.Pension.Add(Int32.Parse(splitData[j]));
                        }
                    }
                    
                    //add to lists
                    population.AllResidents.Add(person);
                    if (person.IsRetired) population.RetiredResidents.Add(person);
                    else population.WorkingResidents.Add(person);

                    if (workzone is not null)
                    {
                        workzone.ResidentList.Add(person);
                    }
                    res.ResidentList.Add(person);
                }

                return population;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<GameTable> LoadAsync(StreamReader reader)
        {
            try
            {
                
                    GameTable table = new GameTable();
                    await LoadRoads(reader, table);
                    await LoadPublicFacilites(reader, table);
                    await LoadZones(reader, table);

                    return table;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task SaveRoads(StreamWriter writer, GameTable table)
        {

            int counter = 0;
            for (int i=0; i<table.Height; i++)
            {
                for (int j=0; j<table.Widht; j++)
                {
                    if (table.Table[i,j].TableFieldType()==TableFieldType.Road )
                    {
                        counter++;
                    }
                }
            }
            await writer.WriteLineAsync(counter.ToString());
            for (int i = 0; i < table.Height; i++)
            {
                for (int j = 0; j < table.Widht; j++)
                {
                    if (table.Table[i, j].TableFieldType() == TableFieldType.Road)
                    {
                        await writer.WriteLineAsync(i.ToString()+" "+j.ToString());
                    }
                }
            }
        }
        public async Task SavePublicFacilities(StreamWriter writer, GameTable table)
        {
            await writer.WriteLineAsync(table.PublicFacility.Count.ToString());
            int row, col, type;
            for (int i=0; i<table.PublicFacility.Count; i++)
            {
                row = table.PublicFacility[i].TableRowPosition;
                col = table.PublicFacility[i].TableColumnPosition;
                type = (int)table.PublicFacility[i].TableFieldType();
                await writer.WriteLineAsync(type.ToString()+" "+
                    row.ToString()+" "+col.ToString());
                
            }
        }
        public async Task SaveZones(StreamWriter writer, GameTable table)
        {
            await writer.WriteLineAsync(table.CommertialZones.Count.ToString());
            int row, col;
            int type;
            CommertialZone com;

            for(int i=0; i<table.CommertialZones.Count; i++)
            {
                com  = table.CommertialZones[i];
                row  = com.TableRowPosition;
                col  = com.TableColumnPosition;
                type = (int)com.ZoneLevel.ZoneLevelType();
                
                await writer.WriteLineAsync(row.ToString() + " " + col.ToString()+" " + type.ToString());

            }

            IndustrialZone ind;
            await writer.WriteLineAsync(table.IndustrialZones.Count.ToString());
            for(int i=0; i<table.IndustrialZones.Count;i++)
            {
                ind  = table.IndustrialZones[i];
                row  = ind.TableRowPosition;
                col  = ind.TableColumnPosition;
                type = (int)ind.ZoneLevel.ZoneLevelType();

                await writer.WriteLineAsync(row.ToString() + " " + col.ToString() +  " " + type.ToString());

            }

            ResidentialZone res;
            await writer.WriteLineAsync(table.ResidentialZones.Count.ToString());
            
            for(int i=0; i<table.ResidentialZones.Count;i++)
            {
                res  = table.ResidentialZones[i];
                row  = res.TableRowPosition;
                col  = res.TableColumnPosition;
                type = (int)res.ZoneLevel.ZoneLevelType();

                await writer.WriteLineAsync( 
                    row.ToString() + " " + col.ToString() + " " +
                    type.ToString() + " " +
                    res.WorkZones.Count.ToString()
                );
                
                foreach(KeyValuePair<Zone, (int distance, int workerCount)> pair in res.WorkZones)
                {
                    await writer.WriteLineAsync(
                        table.WorkZones.FindIndex((zone)=>zone.Equals(pair.Key)).ToString() + " " + 
                        pair.Value.distance.ToString() + " " + 
                        pair.Value.workerCount.ToString()
                   );                 
                }

                
            }
        }
        public async Task SavePopulation(StreamWriter writer, GameTable table, GamePopulation population)
        {
            await writer.WriteLineAsync(
                population.HappinessStreak.ToString() + " " + 
                population.NegaviteBudgetStreak.ToString() + " " + 
                (int)population.HappinessLevel
            );
            await writer.WriteLineAsync(population.AllResidents.Count.ToString());

            for (int i=0; i<population.AllResidents.Count; i++)
            {
                
                await writer.WriteLineAsync(
                    population.AllResidents[i].Name + " " + 
                    population.AllResidents[i].Age.ToString() + " " +
                    (int)population.AllResidents[i].HappinessLevel + " " + 
                    population.AllResidents[i].HappinessDuration.ToString() + " " +
                    population.AllResidents[i].Pension.Count.ToString() + " " + 
                    table.ResidentialZones.FindIndex(
                        (zone) => zone.Equals(population.AllResidents[i].ResidentialZone)
                    ).ToString() + " " +
                    table.WorkZones.FindIndex(
                        (zone) => zone.Equals(population.AllResidents[i].WorkZone)
                    ).ToString()
                );
               
                for(int j=0; j<population.AllResidents[i].Pension.Count; j++)
                {
                    writer.Write( population.AllResidents[i].Pension[j].ToString() + " " );
                }
                writer.Write("\n");

            }
        }
        public async Task SaveEconomy(StreamWriter writer, GameEconomy economy)
        {
            await writer.WriteLineAsync(
                economy.TaxRate.ToString() + " " + 
                economy.TaxesCollected.ToString() + " " +
                economy.GameFunds.ToString() + " " + 
                economy.RetirementExpenses.ToString()
            );
            await writer.WriteLineAsync(economy.TableFieldExpenses.GetValueOrDefault(TableFieldType.FireDepartment).ToString());
            await writer.WriteLineAsync(economy.TableFieldExpenses.GetValueOrDefault(TableFieldType.PoliceDepartment).ToString());
            await writer.WriteLineAsync(economy.TableFieldExpenses.GetValueOrDefault(TableFieldType.Stadium).ToString());
            await writer.WriteLineAsync(economy.TableFieldExpenses.GetValueOrDefault(TableFieldType.Road).ToString());



        }
        public async Task SaveTimer(StreamWriter writer, GameTimer timer)
        {
            await writer.WriteLineAsync(
                timer.CurrentYear.ToString() + " " + 
                (int)timer.CurrentMonth + " " + 
                timer.CurrentDay.ToString() + " " + 
                (int)timer.PlaySpeed
            ); 
        }
        public async Task SaveAsync(StreamWriter writer, GameTable table)
        {
            try
            {
                await SaveRoads(writer, table);
                await SavePublicFacilities(writer, table);
                await SaveZones(writer, table);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task SaveAsync(String path, GameTable table)
        {
            try
            {
            //FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
                using(StreamWriter writer = new StreamWriter(path))
                {
                    await SaveRoads(writer, table);
                    await SavePublicFacilities(writer, table);
                    await SaveZones(writer, table);
                }
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }

        public List<string> SavedSimulations()
        {
            return _saved;
        }
    }
}
