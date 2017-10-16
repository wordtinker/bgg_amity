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
            // TODO Later Persistence
            // TODO Later Manage Register via GUI
            
            matrix = new byte[10, 2];
            // TODO Later STUB
            SetRatingSpan(10, 8, 10);
            SetRatingSpan(9, 8, 10);
            SetRatingSpan(8, 8, 10);
            SetRatingSpan(4, 3, 5);
            SetRatingSpan(2, 1, 3);
            Variation = 4;
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
        private void SetRatingSpan(byte rating, byte low, byte high)
        {
            if (rating < 1 || rating > 10) throw new ArgumentOutOfRangeException();
            if (low < 1 || low > 10) throw new ArgumentOutOfRangeException();
            if (high < 1 || high > 10) throw new ArgumentOutOfRangeException();
            if (low > high) throw new ArgumentOutOfRangeException();

            matrix[rating - 1, 0] = low;
            matrix[rating - 1, 1] = high;
        }
    }
}
