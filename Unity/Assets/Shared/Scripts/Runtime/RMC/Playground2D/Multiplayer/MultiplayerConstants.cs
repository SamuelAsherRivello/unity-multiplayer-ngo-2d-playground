namespace RMC.Playground2D.Shared.Multiplayer
{
    /// <summary>
    /// Helper methods for Multiplayer
    /// </summary>
    public static class MultiplayerConstants 
    {
        //  Constants ------------------------------------
        public const string Instructions01_Join = "Press 'H' to Host, 'C' to Client";
        public const string Instructions02_WaitingForHost = "Waiting For Host...";
        public const string Instructions03_Playing = "Touch Crate For Points\nPress 'Q' to Quit\nYou are <color=#B1C6FF>{0}</color>";
        public const string Instructions04_MustRestartApp = "Must Restart App";
        
        //  Methods ---------------------------------------
        public static string GetRoleNameByIsHost(bool isHost)
        {
            string roleName = "Client";
            if (isHost)
            {
                roleName = "Host";
            }
            return roleName;
        }
    }
}