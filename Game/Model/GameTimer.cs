using System;
using System.Timers;

namespace SimCity.Model
{
    public class GameTimer 
    {
        #region Fields

        private Timer _timer;
        private int _currentYear;
        private Month _currentMonth;
        private int _currentDay;
        private PlaySpeedType _playSpeed;

        #endregion

        #region Events

        public static EventHandler<TimerEventArgs>? GameTimerElapsed;
        public EventHandler<TimerEventArgs>? GameTimerWorking;

        #endregion

        #region Constructor

        /// <summary>
        /// GameTimer constructor.
        /// </summary>
        /// <param name="currentYear">The current year.</param>
        /// <param name="currentMonth">The current month.</param>
        /// <param name="currentDay">The current day.</param>
        /// <param name="playSpeed">The play speed.</param>
        public GameTimer(int currentYear, Month currentMonth, int currentDay, PlaySpeedType playSpeed)
        {
            _currentYear  = currentYear;
            _currentMonth = currentMonth;
            _currentDay   = currentDay;
            _playSpeed    = playSpeed;

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(UpdateGameDate);

            if (_playSpeed != PlaySpeedType.GamePaused)
            {
                _timer.Interval = Convert.ToDouble(_playSpeed);
                _timer.Start();
            }
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Property for the playspeed.
        /// </summary>
        public PlaySpeedType PlaySpeed
        {
            get { return _playSpeed; }
            set
            {
                _playSpeed = value;

                if (_playSpeed == PlaySpeedType.GamePaused)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Interval = Convert.ToDouble(_playSpeed);
                    _timer.Start();
                }
            }
        }

        /// <summary>
        /// Property for the current year.
        /// </summary>
        public int CurrentYear
        {
            get { return _currentYear; }
            set { _currentYear = value; }
        }

        /// <summary>
        /// Property for the current month.
        /// </summary>
        public Month CurrentMonth
        {
            get { return _currentMonth; }
            set { _currentMonth = value; }
        }

        /// <summary>
        /// Property for the current day.
        /// </summary>
        public int CurrentDay
        {
            get { return _currentDay; }
            set { _currentDay = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Steps the date to the next day.
        /// </summary>
        private void StepDay()
        {
            if ((CurrentMonth == Month.January && CurrentDay < 31) || (CurrentMonth == Month.February && CurrentDay < 28) || (CurrentMonth == Month.March && CurrentDay < 31) || (CurrentMonth == Month.April && CurrentDay < 30) || (CurrentMonth == Month.May && CurrentDay < 31) || (CurrentMonth == Month.June && CurrentDay < 30) || (CurrentMonth == Month.July && CurrentDay < 31) || (CurrentMonth == Month.August && CurrentDay < 31) || (CurrentMonth == Month.September && CurrentDay < 30) || (CurrentMonth == Month.October && CurrentDay < 31) || (CurrentMonth == Month.November && CurrentDay < 30) || (CurrentMonth == Month.December && CurrentDay < 31))
            {
                CurrentDay += 1;
            }
            else if (CurrentMonth == Month.December)
            {
                CurrentYear += 1;
                CurrentMonth = Month.January;
                CurrentDay = 1;
            }
            else
            {
                CurrentDay = 1;
                CurrentMonth += 1;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for the timer.Elapsed event.
        /// </summary>
        public void UpdateGameDate(object? sender, ElapsedEventArgs? e)
        {
            StepDay();
            GameTimerWorking?.Invoke(this, new TimerEventArgs(CurrentYear, CurrentMonth, CurrentDay, PlaySpeed));
        }

        /// <summary>
        /// Event invoker for the GameTimerWorking event.
        /// </summary>
        public void RaiseTimeChanged()
        {
            GameTimerWorking?.Invoke(this, new TimerEventArgs(CurrentYear, CurrentMonth, CurrentDay, PlaySpeed));
        }


        #endregion
    }
}
