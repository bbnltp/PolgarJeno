using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCity.Model
{
    internal class GameEventArgs : EventArgs
    {
        private int _gameFunds;
        private String _currentDate;
        private bool _gameOver;

        /// <summary>
        /// Property for the game funds.
        /// </summary>
        public int GameFunds 
        { 
            get { return _gameFunds; } 
        }

        /// <summary>
        /// Property for the current date.
        /// </summary>
        public String CurrentDate 
        { 
            get { return _currentDate; } 
        }

        /// <summary>
        /// Property for the game over.
        /// </summary>
        public bool GameOver
        { 
            get { return _gameOver;} 
        }

        /// <summary>
        /// GameEventArgs constructor.
        /// </summary>
        /// <param name="gameFunds">The game funds.</param>
        /// <param name="currentDate">The current date.</param>
        /// <param name="GameOver">The game over.</param>
        public GameEventArgs(int gameFunds, String currentDate, bool GameOver)
        {
            _gameFunds = gameFunds;
            _currentDate = currentDate;
            _gameOver = GameOver;
        }

    }
}
