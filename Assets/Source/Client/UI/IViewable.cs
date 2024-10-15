namespace Deadfront.Client.UI
{
    /// <summary>
    /// Interface for viewable UI elements
    /// </summary>
    public interface IViewable
    {
        /// <summary>
        /// Check if the UI element can block raycasts
        /// </summary>
        /// <returns>True if the UI element can block raycasts, false otherwise</returns>
        bool IsBlocking();
    }
}