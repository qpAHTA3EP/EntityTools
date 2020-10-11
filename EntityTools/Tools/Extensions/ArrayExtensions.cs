namespace EntityTools.Extensions
{
    public static class ArrayExtensions
    {
        public static void SetAll<T>(this T[] array, T value)
        {
            int iter = array.Length / 32;
            for(int i = 0; i < iter; i++)
            {
                array[i] = value;
                array[i + 1] = value;
                array[i + 2] = value;
                array[i + 3] = value;
                array[i + 4] = value;
                array[i + 5] = value;
                array[i + 6] = value;
                array[i + 7] = value;

                array[i + 8] = value;
                array[i + 9] = value;
                array[i + 10] = value;
                array[i + 11] = value;
                array[i + 12] = value;
                array[i + 13] = value;
                array[i + 14] = value;
                array[i + 15] = value;

                array[i + 16] = value;
                array[i + 17] = value;
                array[i + 18] = value;
                array[i + 19] = value;
                array[i + 20] = value;
                array[i + 21] = value;
                array[i + 22] = value;
                array[i + 23] = value;

                array[i + 24] = value;
                array[i + 25] = value;
                array[i + 26] = value;
                array[i + 27] = value;
                array[i + 28] = value;
                array[i + 29] = value;
                array[i + 30] = value;
                array[i + 31] = value;
            }
            for (long i = iter * 32; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
    }
}
