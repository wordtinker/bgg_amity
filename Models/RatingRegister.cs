namespace Amity.Models
{
    public class RatingRegister
    {
        // Singleton 
        private static readonly RatingRegister instance = new RatingRegister();
        public static RatingRegister Instance
        {
            get
            {
                return instance;
            }
        }
        // singleton ctor
        private RatingRegister()
        {
            matrix = new bool[10, 10];
        }

        // Members
        private bool[,] matrix;
        // properties
        public int Max { get { return 10; } } // TODO STUB
        public int Min { get { return 7; } } // TODO STUB
    }
}
