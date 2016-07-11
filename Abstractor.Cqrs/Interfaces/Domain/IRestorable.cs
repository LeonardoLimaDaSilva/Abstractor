namespace Abstractor.Cqrs.Interfaces.Domain
{
    /// <summary>
    ///     Specifies that an object could have your internal state restored.
    /// </summary>
    /// <typeparam name="T">Type of the internal state.</typeparam>
    public interface IRestorable<T> where T : IMemento
    {
        /// <summary>
        ///     Restores the internal state of the object.
        /// </summary>
        /// <param name="memento">Represents the internal state.</param>
        void Restore(T memento);

        /// <summary>
        ///     Returns the internal state.
        /// </summary>
        /// <returns>Internal state.</returns>
        T Snapshot();
    }
}