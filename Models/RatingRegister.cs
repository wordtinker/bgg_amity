using System;

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
            
            matrix = new byte[10, 2];
            // TODO STUB
            matrix[9, 0] = 8; matrix[9, 1] = 10;
            matrix[8, 0] = 8; matrix[8, 1] = 10;
            matrix[7, 0] = 8; matrix[7, 1] = 10;
            //matrix[6, 0] = 6; matrix[6, 1] = 9;
            Variation = 3;
            // TODO setter (i, min, max), check bounds
        }

        // Members
        private byte[,] matrix;
        // properties
        public Tuple<byte, byte> this[byte index]
        {
            get
            {
                index--;
                if (index < 0 || index > 9) throw new ArgumentOutOfRangeException();
                if (matrix[index, 0] == 0 || matrix[index, 1] == 0) return null;
                return Tuple.Create(matrix[index, 0], matrix[index, 1]);
            }
        }
        public int Max
        {
            get
            {
                for (int i = 9; i >= 0; i--)
                {
                    if (matrix[i, 0] != 0) return i+1;
                }
                return 0;
            }
        }
        public int Min
        {
            get
            {
                for (int i = 0; i < 10; i++)
                {
                    if (matrix[i, 0] != 0) return i+1;
                }
                return 0;
            }
        }
        public byte Variation { get; set; }
    }
}
