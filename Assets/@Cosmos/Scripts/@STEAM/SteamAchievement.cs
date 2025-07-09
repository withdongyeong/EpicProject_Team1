using Steamworks;

public static class SteamAchievement
{
    public static void Achieve(string apiName)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement(apiName, out bool isAchieved);

            if (!isAchieved)
            {
                SteamUserStats.SetAchievement(apiName);
                SteamUserStats.StoreStats();
            }
        }
    }
}