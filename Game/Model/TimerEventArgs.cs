using System;

namespace SimCity.Model
{
    public class TimerEventArgs : EventArgs
    {
        private int _gameYear;
        private Month _gameMonth;
        private int _gameDay;
        private PlaySpeedType _gamePlaySpeed;

        /// <summary>
        /// TimerEventArgs constructor.
        /// </summary>
        /// <param name="gameYear">The game year.</param>
        /// <param name="gameMonth">The game month.</param>
        /// <param name="gameDay">The game day.</param>
        /// <param name="gamePlaySpeed">The game play speed.</param>
        public TimerEventArgs(int gameYear, Month gameMonth, int gameDay, PlaySpeedType gamePlaySpeed)
        {
            _gameYear = gameYear;
            _gameMonth = gameMonth;
            _gameDay = gameDay;
            _gamePlaySpeed = gamePlaySpeed;
        }

        /// <summary>
        /// Property for the game year.
        /// </summary>
        public int GameYear 
        { 
            get { return _gameYear; } 
        }

        /// <summary>
        /// Property for the game month.
        /// </summary>
        public Month GameMonth 
        { 
            get { return _gameMonth; } 
        }

        /// <summary>
        /// Property for the game day.
        /// </summary>
        public int GameDay 
        { 
            get { return _gameDay; } 
        }

        /// <summary>
        /// Property for the game play speed.
        /// </summary>
        public PlaySpeedType GamePlaySpeed 
        { 
            get {  return _gamePlaySpeed; } 
        }

    }
}
