namespace MoodLift.Core.Interfaces
{
    /// <summary>
    /// Provides functionality to retrieve the Google user identifier associated with the current authenticated user.
    /// </summary>
    public interface ICurrentUserService
    {
        string GetGoogleUserId(); 
    }
}
