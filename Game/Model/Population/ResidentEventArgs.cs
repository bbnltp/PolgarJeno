using System;
using SimCity.Model.Table;

namespace SimCity.Model.Population
{
    public class ResidentEventArgs : EventArgs
    {
        #region Fields

        private Person resident;
        private ResidentAction action;

        #endregion

        #region Constructors

        public ResidentEventArgs(Person resident, ResidentAction action)
        {
            this.resident = resident;
            this.action   = action;
        }

        #endregion

        #region Properties

        public Person Resident
        {
            get { return resident; }
        }

        public ResidentAction Action
        {
            get { return action; }
        }

        #endregion
    }
}
