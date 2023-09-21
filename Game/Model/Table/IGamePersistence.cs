using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCity.Model.Table
{
    public interface IGamePersistence
    {
        public List<String> SavedSimulations();
        public Task LoadRoads(StreamReader reader, GameTable table);
        public Task LoadPublicFacilites(StreamReader reader, GameTable table);
        public Task LoadZones(StreamReader reader, GameTable table);
        public Task<GameEconomy> LoadEconomy(StreamReader reader);
        public Task<GamePopulation> LoadPopulation( StreamReader reader, GameTable table);
        public Task<GameTimer> LoadTimer(StreamReader reader, GameTimer timer);
        public Task<GameTable> LoadAsync(StreamReader reader);
        public Task SaveRoads(StreamWriter writer, GameTable table);
        public Task SavePublicFacilities(StreamWriter writer, GameTable table);
        public Task SaveZones(StreamWriter writer, GameTable table);
        public Task SaveEconomy(StreamWriter write, GameEconomy economy);
        public Task SavePopulation(StreamWriter write, GameTable table, GamePopulation population);
        public Task SaveTimer(StreamWriter write, GameTimer timer);
        public Task SaveAsync(StreamWriter write, GameTable map);
        public Task SaveAsync(String path, GameTable map);
    }
}
