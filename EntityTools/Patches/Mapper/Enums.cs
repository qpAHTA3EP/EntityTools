namespace EntityTools.Enums
{
    public enum MappingType
    {
        Bidirectional,
        Unidirectional,
        Stoped
    }

    /// <summary>
    /// Режим перетаскивания якорей CustomRegion'a
    /// </summary>
    public enum DragMode
    {
        None, // перетаскивание якорей разрешено, но не производится
        TopLeft,
        TopCenter,
        TopRight,
        Left,
        Center,
        Right,
        DownLeft,
        DownCenter,
        DownRight,
        Disabled // перетаскивание якорей запрещено
    }
}