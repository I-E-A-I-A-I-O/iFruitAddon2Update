using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using GTA;
using iFruitAddon2.Properties;


/*
    Changelog:
        2.1.0 (05/03/2018): - Changed the way contact index is stored to allow multiple mods to share the value (it wasn't working as expected).
                            - Added a "Bold" option to contacts. It sets the contact text in bold or not.
                            - New contacts font is not bold by default anymore. It is now the same as native contacts.

        2.0.1 (30/01/2018): - Possible to close the phone (if the contact opens a menu, avoid using controls to navigate in the menu AND in the phone)
                            - At the moment, it is mandatory to close the phone in order to be compatible with RPH

        2.0.0 (28/01/2018): Initial release


    TODO :
    ------
    - Supprimer la notification seulement si elle correspond à CELL_LEFT_SESS
    - Permettre de choisir de mettre le contact en "Bold" (= nom du textureDictionary en minuscule)
    
    X Utiliser Game.GetGXTEntry au lieu de _GET_LABEL_TEXT

    X Gérer les menus NativeUI en parallèle du téléphone => Possibilité de fermer le téléphone quand le script ouvre le menu
    X Ajouter un timer dans la fonction Close() pour éviter d'avoir à gérer ça côté script
    - Téléphone qui se ferme quand on appel (CELL_LEFT_SESS) :
        > Réouvrir le téléphone dans la foulée ?
        > Pour éviter qu'il ne se ferme, il faudrait kill "appcontacts" avant d'appeler mais dans ce cas on ne peut plus se déplacer dans les contacts
        > RPH : Reste ouvert sans icône de contact (appel d'un contact inconnu géré par RPH), impossible de fermer le téléphone sans le détruire et tuer les scripts
    
*/
namespace iFruitAddon2
{
    class IFruitAddon2 : Script
    {
        internal static bool Initialized { get; private set; }

        internal static int GamePid { get; private set; }

        private static readonly string MainDir = AppDomain.CurrentDomain.BaseDirectory + "\\iFruitAddon2";
        private static readonly string ConfigFile = MainDir + "\\config.ini";

        private static string? _tempFilePath;

        private static int _contactIndex = 40;
        public static int ContactIndex { get => _contactIndex; internal set => _contactIndex = value; }

        public static ScriptSettings? Config { get; private set; }

        private bool _checkForUpdates = true;


        public IFruitAddon2()
        {
            Tick += Initialize;
        }

        public static string? GetTempFilePath()
        {
            if (!Directory.Exists(MainDir))
            {
                Logger.Log("Creating main directory.");
                Directory.CreateDirectory(MainDir);
            }

            GamePid = Process.GetProcessesByName("GTA5")[0]?.Id ?? 0;
            return MainDir + "\\" + GamePid + ".tmp";
        }

        private void Initialize(object sender, EventArgs e)
        {
            // Get the process ID of the game and creating temp file
            _tempFilePath = GetTempFilePath();

            // Removing old temp files (if the game has crashed, the file were not deleted)
            foreach (var file in Directory.GetFiles(MainDir, "*.tmp"))
            {
                if (_tempFilePath != null)
                {
                    FileInfo tempFileInfo = new(_tempFilePath), fileInfo = new(file);
                    if (tempFileInfo.Name != fileInfo.Name && File.Exists(file))
                    {
                        // Reset log file
                        Logger.ResetLogFile();

                        // Remove old temp file
                        File.Delete(file);
                    }
                }
            }

            while (Game.IsLoading)
                Yield();
            while (GTA.UI.Screen.IsFadingIn)
                Yield();

            LoadConfigValues();
            if (_checkForUpdates)
                if (IsUpdateAvailable()) NotifyNewUpdate();

            Initialized = true;

            Tick -= Initialize;
        }
        
        // Dispose Event
        protected void Dispose(bool a0)
        {
            if (!a0) return;
            if (!File.Exists(_tempFilePath)) return;
            if (_tempFilePath != null)
                File.Delete(_tempFilePath);
        }

        private void LoadConfigValues()
        {
            if (!Directory.Exists(MainDir))
            {
                Logger.Log("Creating main directory.");
                Directory.CreateDirectory(MainDir);
            }
            if (!File.Exists(ConfigFile))
            {
                Logger.Log("Creating config file.");
                File.WriteAllText(ConfigFile, Resources.config);
            }

            Config = ScriptSettings.Load(ConfigFile);
            _contactIndex = Config.GetValue("General", "StartIndex", 40);
            _checkForUpdates = Config.GetValue("General", "CheckForUpdates", true);
        }

        private bool IsUpdateAvailable()
        {
            try
            {
                var client = new WebClient();
                var downloadedString = client.DownloadString("https://raw.githubusercontent.com/Bob74/iFruitAddon2/master/version");

                downloadedString = downloadedString.Replace("\r", "");
                downloadedString = downloadedString.Replace("\n", "");

                var onlineVersion = new Version(downloadedString);

                client.Dispose();

                if (onlineVersion.CompareTo(Assembly.GetExecutingAssembly().GetName().Version) > 0)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                Logger.Log("Error: IsUpdateAvailable - " + e.Message);
            }

            return false;
        }

        private void NotifyNewUpdate()
        {
            GTA.UI.Notification.Show("iFruitAddon2: A new update is available!", true);
        }
    }
}
