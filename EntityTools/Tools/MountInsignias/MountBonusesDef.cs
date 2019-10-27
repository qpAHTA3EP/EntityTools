using System.Xml.Serialization;
using InsigniaSet = EntityTools.Tools.Triple<EntityTools.Tools.MountInsignias.InsigniaType, EntityTools.Tools.MountInsignias.InsigniaType, EntityTools.Tools.MountInsignias.InsigniaType>;

namespace EntityTools.Tools.MountInsignias
{
    /// <summary>
    /// JОпиание бонуса скакуна
    /// </summary>
    public class MountBonusDef //: IComparable 
    {
        public int Barbed { get; set; }
        public int Crescent { get; set; }
        public int Illuminated { get; set; }
        public int Enlightened { get; set; }
        public int Regal { get; set; }

        public string InternalName { get; set;}
        [XmlIgnore]
        public string Name
        {
            get
            {
                /*if (Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName == "Dragon")
                    return NameRU;
                else*/ return NameGL;
            }
        }
        public string NameGL { get; set; }
        public string NameRU { get; set; }
        [XmlIgnore]
        public string Description
        {
            get
            {
                /*if (Game.CharacterSelectionData.CharacterChoices.LastPlayedCharacter.ShardName == "Dragon")
                    return DescriptionRU;
                else*/ return DescriptionGL;
            }
        }
        public string DescriptionGL { get; set; }
        public string DescriptionRU { get; set; }

        public InsigniaSet GetInsigniaSet()
        {
            InsigniaSet iSet = new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };

            int storedInsignia = 0;
            switch(Barbed)
            {
                case 0:
                    break;
                case 1:
                    iSet.First = InsigniaType.Barbed;
                    storedInsignia = 1;
                    break;
                case 2:
                    iSet.First = InsigniaType.Barbed;
                    iSet.Second = InsigniaType.Barbed;
                    storedInsignia = 2;
                    break;
                case 3:
                    iSet.First = InsigniaType.Barbed;
                    iSet.Second = InsigniaType.Barbed;
                    iSet.Third = InsigniaType.Barbed;
                    storedInsignia = 3;
                    break;
                default:
                    // Недопустимое сочетание
                    return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };

            }
            switch (Crescent)
            {
                case 0:
                    break;
                case 1:
                    switch(storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Crescent;
                            break;
                        case 1:
                            iSet.Second= InsigniaType.Crescent;
                            break;
                        case 2:
                            iSet.Third = InsigniaType.Crescent;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 1;
                    break;
                case 2:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Crescent;
                            iSet.Second = InsigniaType.Crescent;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Crescent;
                            iSet.Third = InsigniaType.Crescent;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 2;
                    break;
                case 3:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Crescent;
                            iSet.Second = InsigniaType.Crescent;
                            iSet.Third = InsigniaType.Crescent;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 3;
                    break;
                default:
                    // Недопустимое сочетание
                    return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
            }
            switch (Illuminated)
            {
                case 0:
                    break;
                case 1:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Illuminated;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Illuminated;
                            break;
                        case 2:
                            iSet.Third = InsigniaType.Illuminated;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 1;
                    break;
                case 2:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Illuminated;
                            iSet.Second = InsigniaType.Illuminated;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Illuminated;
                            iSet.Third = InsigniaType.Illuminated;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 2;
                    break;
                case 3:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Illuminated;
                            iSet.Second = InsigniaType.Illuminated;
                            iSet.Third = InsigniaType.Illuminated;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 3;
                    break;
                default:
                    // Недопустимое сочетание
                    return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
            }
            switch (Enlightened)
            {
                case 0:
                    break;
                case 1:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Enlightened;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Enlightened;
                            break;
                        case 2:
                            iSet.Third = InsigniaType.Enlightened;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 1;
                    break;
                case 2:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Enlightened;
                            iSet.Second = InsigniaType.Enlightened;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Enlightened;
                            iSet.Third = InsigniaType.Enlightened;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 2;
                    break;
                case 3:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Enlightened;
                            iSet.Second = InsigniaType.Enlightened;
                            iSet.Third = InsigniaType.Enlightened;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 3;
                    break;
                default:
                    // Недопустимое сочетание
                    return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
            }
            switch (Regal)
            {
                case 0:
                    break;
                case 1:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Regal;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Regal;
                            break;
                        case 2:
                            iSet.Third = InsigniaType.Regal;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 1;
                    break;
                case 2:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Regal;
                            iSet.Second = InsigniaType.Regal;
                            break;
                        case 1:
                            iSet.Second = InsigniaType.Regal;
                            iSet.Third = InsigniaType.Regal;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 2;
                    break;
                case 3:
                    switch (storedInsignia)
                    {
                        case 0:
                            iSet.First = InsigniaType.Regal;
                            iSet.Second = InsigniaType.Regal;
                            iSet.Third = InsigniaType.Regal;
                            break;
                        default:
                            // Недопустимое сочетание
                            return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
                    }
                    storedInsignia += 3;
                    break;
                default:
                    // Недопустимое сочетание
                    return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };
            }

            if (storedInsignia == 2 || storedInsignia == 3)
                return iSet;
            else return new InsigniaSet() { First = InsigniaType.Empty, Second = InsigniaType.Empty, Third = InsigniaType.Empty };

        }

        //public override bool Equals(object o)
        //{
        //    MountBonusesDef bonus = o as MountBonusesDef;
        //    if (bonus != null)
        //    {
        //        (Barbed - bonus.Barbed) 
        //            + (Crescent - bonus.Crescent) 
        //            + (Crescent - );
        //    }
        //    return false;
        //}

        //public int CompareTo(object obj)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
