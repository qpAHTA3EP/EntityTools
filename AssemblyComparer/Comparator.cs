using System;
using System.Linq;

namespace AssemblyComparer
{
    public static class Comparator
    {
        /// <summary>
        /// Вычисление коэффициента схожести Джаккарта
        /// <see href="https://habr.com/ru/company/skillfactory/blog/566414/"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Jaccard<T>(T a, T b)
        {
            var aa = a.ToString()/*.ToLower()*/.ToCharArray();
            var bb = b.ToString()/*.ToLower()*/.ToCharArray();

            double intersect = aa.Intersect(bb).Count();
            double union = aa.Union(bb).Count();

            return union > 0 ? intersect / union : 0;
        }
        /// <summary>
        /// Вычисление коэффициента схожести Джаккарта
        /// <see href="https://habr.com/ru/company/skillfactory/blog/566414/"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Jaccard(string a, string b)
        {
            var aa = a.ToLower().ToCharArray();
            var bb = b.ToLower().ToCharArray();

            double intersect = aa.Intersect(bb).Count();
            double union = aa.Union(bb).Count();

            return union > 0 ? intersect / union : 0;
        }
        /// <summary>
        /// Вычисление коэффициента схожести Танимото
        /// <see href="https://grishaev.me/2012/10/05/1/"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Tanimoto<T>(T a, T b)
        {
            var aa = a.ToString()/*.ToLower()*/.ToCharArray();
            var bb = b.ToString()/*.ToLower()*/.ToCharArray();

            double common = 0;
            foreach (var c in aa)
            {
                if (bb.Contains(c))
                    common++;
            }

            return common / (aa.Length + bb.Length - common);
        }
        /// <summary>
        /// Вычисление коэффициента схожести Танимото
        /// <see href="https://grishaev.me/2012/10/05/1/"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Tanimoto(string a, string b)
        {
            var aa = a/*.ToLower()*/.ToCharArray();
            var bb = b/*.ToLower()*/.ToCharArray();

            double common = 0;
            
            foreach (var c in aa)
            {
                if (bb.Contains(c))
                    common++;
            }

            return common / (aa.Length + bb.Length - common);
        }


        /// <summary>
        /// Вычисление расстояния Ливенштейна по алгоритму Вагнера-Фишера
        /// <see href="https://neerc.ifmo.ru/wiki/index.php?title=%D0%97%D0%B0%D0%B4%D0%B0%D1%87%D0%B0_%D0%BE_%D1%80%D0%B5%D0%B4%D0%B0%D0%BA%D1%86%D0%B8%D0%BE%D0%BD%D0%BD%D0%BE%D0%BC_%D1%80%D0%B0%D1%81%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D0%B8,_%D0%B0%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC_%D0%92%D0%B0%D0%B3%D0%BD%D0%B5%D1%80%D0%B0-%D0%A4%D0%B8%D1%88%D0%B5%D1%80%D0%B0"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Levenstein(string a, string b)
        {
            var aa = a/*.ToLower()*/.ToCharArray();
            var bb = b/*.ToLower()*/.ToCharArray();

            var D = new int[aa.Length+1, bb.Length+1];

            D[0, 0] = 0;
            for (int j = 1; j <= bb.Length; j++)
            {
                D[0, j] = D[0, j - 1] + 1;
            }

            for (int i = 1; i <= aa.Length; i++)
            {
                D[i, 0] = D[i - 1, 0] + 1;
                for (int j = 1; j <= bb.Length; j++)
                {
                    if (aa[i-1] != bb[j-1])
                    {
                        D[i, j] = Math.Min(D[i - 1, j] + 1,
                                           Math.Min(D[i, j - 1] + 1,
                                                    D[i - 1, j - 1] + 1));
                    }
                    else
                    {
                        D[i, j] = D[i - 1, j - 1];
                    }
                }
            }

            double d = D[a.Length, b.Length];
            return d == 0 ? 1d : 1d / d;
        }
    }
}