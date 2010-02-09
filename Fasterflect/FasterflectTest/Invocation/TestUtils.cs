namespace FasterflectTest.Invocation
{
    public static class TestUtils
    {
        public static string FirstCharUpper( this string str )
        {
            if( string.IsNullOrEmpty( str ) )
            {
                return str;
            }
            return str.Substring( 0, 1 ).ToUpper() + str.Substring( 1 );
        }
    }
}