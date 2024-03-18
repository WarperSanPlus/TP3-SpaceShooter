namespace Interfaces
{
    /// <summary>
    /// Defines every class that wants to receive notifications about <see cref="UtilityScripts.TTD"/>
    /// </summary>
    public interface ILifespan
    {
        /// <summary>
        /// Called when the lifespan ends
        /// </summary>
        public void OnLifeEnd();
    }
}