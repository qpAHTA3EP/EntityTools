using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyComparer
{
    public class EnumPair : IEquatable<EnumPair>
    {
        public EnumPair(Type enum1)
        {
            Enum1Type = enum1 ?? throw new ArgumentNullException(nameof(enum1));
        }

        public Type Enum1Type { get; }

        public Type Enum2Type
        {
            get => enum2Type;
            set
            {
                differentElementSimilarityCalculated = false;
                enum2Type = value;
            }
        }
        private Type enum2Type;

        public ReadOnlyCollection<Tuple<string, string, double>> DifferentElementSimilarity
        {
            get
            {
                if (!differentElementSimilarityCalculated)
                {
                    differentElementSimilarity.Clear();
                    if (Enum2Type != null)
                    {
                        // Сопоставляем значение типов
                        var values1 = Enum.GetNames(Enum1Type).ToHashSet();
                        var values2 = Enum.GetNames(Enum2Type).ToHashSet();

                        // Объекты, у которых нет соответствий
                        var dif1 = values1.Except(values2).ToList();
                        var dif2 = values2.Except(values1).ToList();
                        // Флаг сопоставления элемента dif2 с элементом 
                        var dif2flags = new bool[dif2.Count];

                        foreach (var d1 in dif1)
                        {
                            double max = 0;
                            int maxInd = -1;
                            for (int i = 0; i < dif2.Count; i++)
                            {
                                var jaccard = Comparator.Jaccard(d1, dif2[i]);
                                if (jaccard > max)
                                {
                                    max = jaccard;
                                    maxInd = i;
                                }
                            }

                            if (maxInd >= 0)
                            {
                                differentElementSimilarity.Add(new Tuple<string, string, double>(d1, dif2[maxInd], max));
                                dif2flags[maxInd] = true;
                            }
                            else
                            {
                                differentElementSimilarity.Add(new Tuple<string, string, double>(d1, string.Empty, 0));
                            }
                        }

                        for (int i = 0; i < dif2.Count; i++)
                        {
                            if (!dif2flags[i])
                            {
                                differentElementSimilarity.Add(new Tuple<string, string, double>(string.Empty, dif2[i], 0));
                            }
                        }

                        differentElementSimilarityCalculated = true;
                    }
                    else
                    {

                    }
                }

                return differentElementSimilarity.AsReadOnly();
            }
        }

        private List<Tuple<string, string, double>> differentElementSimilarity =
            new List<Tuple<string, string, double>>();

        private bool differentElementSimilarityCalculated;
        public bool Equals(EnumPair other)
        {
            if (other is null)
                return false;
            var name1 = Enum1Type.FullName;
            var name2 = other.Enum2Type.FullName;

            if (string.IsNullOrEmpty(name1)
                || string.IsNullOrEmpty(name2))
                return false;

            return name1.Equals(name2, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnumPair) obj);
        }

        public override int GetHashCode()
        {

            return Enum1Type.FullName?.GetHashCode() ?? 0;
        }

        public static bool operator ==(EnumPair left, EnumPair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EnumPair left, EnumPair right)
        {
            return !Equals(left, right);
        }
    }
}