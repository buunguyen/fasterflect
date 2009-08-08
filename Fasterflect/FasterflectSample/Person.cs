namespace FasterflectSample
{
    class Person
    {
        private int id;
        private int milesTraveled;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Name { get; private set; }
        private static int InstanceCount;

        public Person() : this(0) {}

        public Person(int id) : this(id, string.Empty) { }

        public Person(int id, string name)
        {
            Id = id;
            Name = name;
            InstanceCount++;
        }

        public char this[int index]
        {
            get { return Name[index]; }
        }

        private void Walk(int miles) 
        {
            milesTraveled += miles;
        }

        private static void IncreaseInstanceCount()
        {
            InstanceCount++;
        }

        private static int GetInstanceCount()
        {
            return InstanceCount;
        }
    }
}
