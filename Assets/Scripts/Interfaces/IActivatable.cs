namespace Interfaces
{
    /// <summary>
    /// Defines every class that wants to receive notifications about activation
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        /// Notifies the class that the caller became <paramref name="isActive"/>
        /// </summary>
        /// <param name="isActive">New activity status of the caller</param>
        public void SetActive(bool isActive);
    }
}