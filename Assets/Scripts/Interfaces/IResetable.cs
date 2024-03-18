namespace Interfaces
{
    /// <summary>
    /// Determines every class that want to be reset
    /// </summary>
    public interface IResetable
    {
        /// <summary>
        /// Called upon a request of reset
        /// </summary>
        public void OnReset();
    }
}
