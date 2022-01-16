using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mono.Cecil;

namespace AssemblyComparer
{
    public class EnumDefinitionPair : IEquatable<EnumDefinitionPair>
    {
        public EnumDefinitionPair(TypeDefinition enum1)
        {
            Enum1Type = enum1 ?? throw new ArgumentNullException(nameof(enum1));
        }

        public TypeDefinition Enum1Type { get; }

        public TypeDefinition Enum2Type
        {
            get => enum2Type;
            set
            {
                differentElementSimilarityCalculated = false;
                enum2Type = value;
            }
        }
        private TypeDefinition enum2Type;

        public ReadOnlyCollection<Tuple<string, string, double, string, double>> DifferentElementSimilarity
        {
            get
            {
                if (!differentElementSimilarityCalculated)
                {
                    differentElementSimilarity.Clear();
                    if (Enum2Type != null)
                    {
                        // Сопоставляем значение типов
                        var values1 = Enum1Type.Fields.Where(f=>f.Constant != null).Select(f=>f.Name).ToHashSet();
                        var values2 = Enum2Type.Fields.Where(f => f.Constant != null).Select(f => f.Name).ToHashSet();

                        // Объекты, у которых нет соответствий
                        var dif1 = values1.Except(values2).ToList();
                        var dif2 = values2.Except(values1).ToList();
                        // Флаг сопоставления элемента dif2 с элементом 
                        var dif2flags = new bool[dif2.Count];

                        foreach (var d1 in dif1)
                        {
                            double maxiest = 0;
                            int maxiestInd = -1;
                            double max = 0;
                            int maxInd = -1;
                            for (int i = 0; i < dif2.Count; i++)
                            {
                                var k = Comparator.Levenstein(d1, dif2[i]);//Comparator.Tanimoto(d1, dif2[i]);//Comparator.Jaccard(d1, dif2[i]); //
                                if (k > maxiest)
                                {
                                    max = maxiest;
                                    maxInd = maxiestInd;
                                    maxiest = k;
                                    maxiestInd = i;
                                }
                                else if (k > max)
                                {
                                    max = k;
                                    maxInd = i;
                                }
                            }

                            if (maxiestInd >= 0)
                            {
                                if (maxInd >= 0)
                                {
                                    differentElementSimilarity.Add(new Tuple<string, string, double, string, double>(d1, dif2[maxiestInd], maxiest, dif2[maxInd], max));
                                    //dif2flags[maxInd] = true;
                                }
                                else
                                {
                                    differentElementSimilarity.Add(new Tuple<string, string, double, string, double>(d1, dif2[maxiestInd], maxiest, null, 0));
                                    dif2flags[maxiestInd] = true;
                                }
                                dif2flags[maxiestInd] = true;
                            }
                            else
                            {
                                differentElementSimilarity.Add(new Tuple<string, string, double, string, double>(d1, null, 0, null, 0));
                            }
                        }

                        for (int i = 0; i < dif2.Count; i++)
                        {
                            if (!dif2flags[i])
                            {
                                differentElementSimilarity.Add(new Tuple<string, string, double, string, double>(null, dif2[i], 0, null, 0));
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

        private List<Tuple<string, string, double, string, double>> differentElementSimilarity =
            new List<Tuple<string, string, double, string, double>>();

        private bool differentElementSimilarityCalculated;
        public bool Equals(EnumDefinitionPair other)
        {
            if (other is null)
                return false;
            var name1 = Enum1Type.FullName;
            var name2 = other.Enum1Type.FullName;

            if (string.IsNullOrEmpty(name1)
                || string.IsNullOrEmpty(name2))
                return false;

            return name1.Equals(name2, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EnumPair) obj);
        }

        public override int GetHashCode()
        {

            return Enum1Type.FullName?.GetHashCode() ?? 0;
        }

        public static bool operator ==(EnumDefinitionPair left, EnumDefinitionPair right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EnumDefinitionPair left, EnumDefinitionPair right)
        {
            return !Equals(left, right);
        }
    }
}