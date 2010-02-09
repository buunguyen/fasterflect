using System.Collections.Generic;

namespace FasterflectTest.Model
{
    public class Person
    {
        private static int totalPeopleCreated;
        private string name;
        private int age;
        protected double metersTravelled;
        private readonly Dictionary<string, Person> friends;

        protected static int TotalPeopleCreated
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

        private double MetersTravelled
        {
            get { return metersTravelled; }
            set { metersTravelled = value; }
        }

        internal Person() : this( string.Empty, 0 )
        {
        }

        internal Person( Person other ) : this( other.Name, other.Age )
        {
        }

        internal Person( string name, int age, out int totalPeopleCreated )
            : this( name, age )
        {
            totalPeopleCreated = Person.totalPeopleCreated;
        }

        internal Person( string name, int age )
        {
            Name = name;
            Age = age;
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

        public Person AddFriend( Person friend )
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

        public Person this[ string name ]
        {
            get { return friends[ name ]; }
            set { friends[ name ] = value; }
        }

        public Person this[ string name, int age ]
        {
            get
            {
                var person = friends[ name ];
                return person == null
                           ? null
                           : person.Age == age
                                 ? person
                                 : null;
            }
        }
    }
}