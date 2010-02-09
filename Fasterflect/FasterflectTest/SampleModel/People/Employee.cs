namespace FasterflectTest.Model
{
    public class Employee : Person, ISwimmable
    {
        private int employeeId;

        public int EmployeeId
        {
            get { return employeeId; }
            set { employeeId = value; }
        }

        public Employee[] Subordinates { get; private set; }

        public Employee( string name, int age ) : base( name, age )
        {
            employeeId = GetTotalPeopleCreated();
        }

        public Employee() : this( string.Empty, 0 )
        {
        }

        public Employee( Employee[] subordinates ) : this( string.Empty, 0 )
        {
            Subordinates = subordinates;
        }

        void ISwimmable.Swim( int meters )
        {
            metersTravelled += meters;
        }
    }
}