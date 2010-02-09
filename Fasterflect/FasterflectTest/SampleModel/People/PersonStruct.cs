using System.Collections.Generic;

namespace FasterflectTest.Model
{
    public struct PersonStruct
    {
		#pragma warning disable 0169, 0649
        private static int totalPeopleCreated;
        private string name;
        private int age;
        private double metersTravelled;
        private readonly Dictionary<string, PersonStruct?> friends;
		#pragma warning restore 0169, 0649

        public static int TotalPeopleCreated
        {
            get { return totalPeopleCreated; }
            set { totalPeopleCreated = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public double MetersTravelled
        {
            get { return metersTravelled; }
            set { metersTravelled = value; }
        }

        internal PersonStruct( PersonStruct other ) : this( other.Name, other.Age )
        {
        }

        internal PersonStruct( string name, int age, out int totalPeopleCreated )
            : this( name, age )
        {
            totalPeopleCreated = PersonStruct.totalPeopleCreated;
        }

        internal PersonStruct( string name, int age ) : this()
        {
            this.name = name;
            this.age = age;
            totalPeopleCreated++;
        }

        private void Walk( double meters )
        {
            this.metersTravelled += meters;
        }

        internal void Walk( double meters, out double metersTravelled )
        {
            this.metersTravelled += meters;
            metersTravelled = this.metersTravelled;
        }

        public PersonStruct AddFriend( PersonStruct friend )
        {
            return friend;
        }

        public static int GetTotalPeopleCreated()
        {
            return totalPeopleCreated;
        }

        public static int AdjustTotalPeopleCreated( int delta )
        {
            totalPeopleCreated += delta;
            return totalPeopleCreated;
        }

        public PersonStruct? this[ string name ]
        {
            get { return friends[ name ]; }
            set { friends[ name ] = value; }
        }

        public PersonStruct? this[ string name, int age ]
        {
            get
            {
                var person = friends[ name ];
                return person == null
                           ? null
                           : person.Value.Age == age
                                 ? person
                                 : null;
            }
        }
    }
}