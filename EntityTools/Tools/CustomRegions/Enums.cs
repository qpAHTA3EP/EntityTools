namespace EntityTools.Enums
{
    public enum InclusionType
    {
        Ignore,
#if true
        Union,
        Intersection,
        Exclusion 
#else
        Merge,
        Intersect,
        Exclude 
#endif
    }
}