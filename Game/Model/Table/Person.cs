using System;
using SimCity.Model.Population;
using SimCity.Model.Table.Field.Zone;
using System.Collections.Generic;

namespace SimCity.Model.Table
{

    public class Person
    {
        #region Fields

        private Zone? workZone;
        private ResidentialZone? residentialZone;
        
        private int age;
        private string name;
        private List<int> pension;

        private HappinessLevel happinessLevel;
        private int happinessDuration;

        private static string[] names =
        {
            "Gipsz Jakab",
            "Nagy Eszter",
            "Nagy András",
            "Polgár Jenő",
            "Török Andrea",
            "Hajdu Barnabás",
            "Pap Endre",
            "Balázs Zsombor",
            "Hegedűs Áron",
            "Bíró Nikoletta",
            "Barna Vincze",
            "Nemes Márton",
            "Egedi Viktor",
            "Varga Balázs",
            "Halmai Tímea",
            "Molnár Richárd",
            "Fazekas Emőke"
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Person constructor.
        /// </summary>
        /// <param name="age">The age of the person.</param>
        /// <param name="name">Then name of the person.</param>
        /// <param name="happinessLevel">The happiness level of the person.</param>
        /// <param name="happinessDuration">The happiness duration of the person.</param>
        /// <param name="workZone">The work zone of the person.</param>
        /// <param name="residentialZone">The residential zone of the person.</param>
        public Person(int age, string name, 
                      HappinessLevel happinessLevel, int happinessDuration, 
                      Zone? workZone, ResidentialZone? residentialZone)
        {
            // Name and age settings
            this.age  = age;
            this.name = name;

            // Happiness settings
            this.happinessLevel    = happinessLevel;
            this.happinessDuration = happinessDuration;

            // Zone settings
            this.workZone        = workZone;
            this.residentialZone = residentialZone;
            
            // Pensions
            pension = new List<int>();
        }

        /// <summary>
        /// Person constructor.
        /// </summary>
        /// <param name="age">The age of the person.</param>
        /// <param name="name">The name of the person.</param>
        public Person(int age , string name)
            : this(age, name, HappinessLevel.Normal, 0, null, null)
        { }

        /// <summary>
        /// Person constructor.
        /// </summary>
        /// <param name="age">The age of the person.</param>
        public Person(int age)
        {
            Random rand = new Random();

            // Name and age settings
            name = names[rand.Next(0, names.Length)];
            this.age = age;

            // Happinesslevel settings
            happinessLevel    = HappinessLevel.Normal;
            happinessDuration = 0;

            // Work and living zone settings
            workZone        = null;
            residentialZone = null;

            // Pensions.
            pension = new List<int>();
        }

        /// <summary>
        /// Person constructor with default parameters.
        /// </summary>
        public Person() 
        {
            Random rand = new Random();

            // Name and age settings
            name = names[rand.Next(0, names.Length)]; 
            age  = rand.Next(18, 61);

            // Happinesslevel settings
            happinessLevel    = HappinessLevel.Normal;
            happinessDuration = 0;

            // Work and living zone settings
            workZone        = null;
            residentialZone = null;

            // Pensions.
            pension = new List<int>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The list of pension.
        /// </summary>
        public List<int> Pension
        {
            get { return pension; }
            set { pension = value; }
        }

        /// <summary>
        /// The work zone of the resident.
        /// </summary>
        public Zone? WorkZone
        {
            get { return workZone;  }
            set { workZone = value; }
        }

        /// <summary>
        /// The residential zone of the resident.
        /// </summary>
        public ResidentialZone? ResidentialZone
        {
            get { return residentialZone;  }
            set { residentialZone = value; }
        }

        /// <summary>
        /// The age of the resident.
        /// </summary>
        public int Age
        {
            get { return age;  }
            set { age = value; }
        }

        /// <summary>
        /// The name of the resident.
        /// </summary>
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        /// <summary>
        /// Tells if the resident is retired.
        /// </summary>
        public bool IsRetired
        {
            get { return age >= 65; }
        }

        /// <summary>
        /// The number of consecutive days for the happiness.
        /// </summary>
        public int HappinessDuration
        {
            get { return happinessDuration; }
            private set { happinessDuration = value; }
        }

        /// <summary>
        /// The happiness level of the resident.
        /// </summary>
        public HappinessLevel HappinessLevel
        {
            get { return happinessLevel; }
            set
            {
                if (happinessLevel == value)
                {
                    happinessDuration += 1;
                }
                else
                {
                    happinessLevel    = value;
                    happinessDuration = 0;
                }
            }
        }

        #endregion
    }
}
