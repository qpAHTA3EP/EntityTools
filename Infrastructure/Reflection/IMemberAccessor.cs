using System.Reflection;

namespace Infrastructure.Reflection
{
    /// <summary>
    /// Инферфейс объекта, предоставляющего доступ к члену типа <typeparamref name="T"/>
    /// </summary>
    public interface IMemberAccessor<T>
    {
        /// <summary>
        /// Описатель, содержащий сведения об атрибутах члена и предоставляет доступ к его метаданным.
        /// </summary>
        MemberInfo MemberInfo { get; }
        /// <summary>
        /// Флаг корректности 
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// Свойство для чтения и записи значения члена класса
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// Чтение значение члена класса
        /// </summary>
        T GetValue();
        /// <summary>
        /// Присваивание члену класса значения <paramref name="value"/>
        /// </summary>
        void SetValue(T value);
    }


    /// <summary>
    /// Инферфейс объекта, предоставляющего доступ к члену типа <typeparamref name="T"/>,
    /// инкапсулированного в экземпляр типа <typeparamref name="C"/>
    /// </summary>
    public interface IInstanceMemberAccessor<C, T> : IMemberAccessor<T>
    {
        /// <summary>
        /// Экземпляр типа <typeparamref name="C"/>, к члену которого предоставляется доступ
        /// </summary>
        C Instance 
        { 
            get; 
            //set;
        }
        /// <summary>
        /// Чтение значение члена объекта <paramref name="instance"/>
        /// </summary>
        T GetValueFrom(C instance);
        /// <summary>
        /// Присваивание члену объекта <paramref name="instance"/> значения <paramref name="value"/>
        /// </summary>
        void SetValueTo(C instance, T value);
        /// <summary>
        /// Свойство для чтения и записи значения объекта <paramref name="instance"/>
        /// </summary>
        T this[C instance] { get; set; }
    }

    /// <summary>
    /// Инферфейс объекта, предоставляющего доступ к члену типа <typeparamref name="T"/>
    /// </summary>
    public interface IInstanceMemberAccessor<T> : IInstanceMemberAccessor<object, T> { }
}
