namespace Interfaces
{
    /// <summary>
    /// Defines every class that wants to receive notifications about collection
    /// </summary>
    public interface ICollectable
    {
        public void Collect(Entities.BaseEntity source);
    }
}