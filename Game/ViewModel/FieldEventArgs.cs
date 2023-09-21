using System;
using SimCity.Model.Table.Field;
using SimCity.Model.Table.Field.Zone;
using SimCity.Model.Table.Field.PublicFacility;

namespace SimCity.ViewModel
{
    public class FieldEventArgs : EventArgs
    {
        #region Fields

        Zone zone;
        PublicFacility facility;
        bool timed;

        #endregion

        #region Constructor

        public FieldEventArgs(Zone zone, PublicFacility facility, bool timed = false)
        {
            this.zone = zone;
            this.facility = facility;
            this.timed = timed;
        }

        #endregion;

        #region Properties

        public Zone Zone
        {
            get { return zone; }
        }
        public PublicFacility Facility
        {
            get { return facility; }
        }
        public bool Timed
        { 
            get { return timed; }
        }
        #endregion
    }
}