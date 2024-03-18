namespace Interfaces
{
    /// <summary>
    /// Defines every class that wants to receive notifications about activation
    /// </summary>
    public interface IActivatable
    {
        public void SetActive(bool isActive);
    }
}