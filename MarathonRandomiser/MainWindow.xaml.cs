﻿using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MarathonRandomiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Generate the path to a temp directory we can use for the Randomisation process.
        public static string TemporaryDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Set up the Randomiser.
        public static Random Randomiser = new();

        public MainWindow()
        {
            InitializeComponent();
            GenerateDirectories();
            SetDefaults();
            
            #if DEBUG
            System.Diagnostics.Debug.WriteLine($"Randomiser's current temporary path is: {TemporaryDirectory}");
            TextBox_General_Seed.Text = "WPF Test";
            #else
            TabItem_Debug.Visibility = System.Windows.Visibility.Collapsed;
            #endif
        }

        private void GenerateDirectories()
        {
            // Create the Voice Packs directory.
            if (!Directory.Exists($@"{Environment.CurrentDirectory}\VoicePacks"))
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\VoicePacks");

            // Create the XMA Cache Directory.
            if (!Directory.Exists($@"{Environment.CurrentDirectory}\Cache\XMA"))
                Directory.CreateDirectory($@"{Environment.CurrentDirectory}\Cache\XMA");
        }
        private void SetDefaults()
        {
            // Load consistent settings.
            TextBox_General_ModsDirectory.Text = Properties.Settings.Default.ModsDirectory;
            TextBox_General_GameExecutable.Text = Properties.Settings.Default.GameExecutable;

            // Generate a seed to use.
            TextBox_General_Seed.Text = Randomiser.Next().ToString();
            
            // Fill in the configuration CheckListBox elements.
            Helpers.FillCheckedListBox(Properties.Resources.EnemyTypes, CheckedList_SET_EnemyTypes);
            Helpers.FillCheckedListBox(Properties.Resources.CharacterTypes, CheckedList_SET_Characters);
            Helpers.FillCheckedListBox(Properties.Resources.ItemTypes, CheckedList_SET_ItemCapsules);
            Helpers.FillCheckedListBox(Properties.Resources.CommonPropTypes, CheckedList_SET_CommonProps);
            Helpers.FillCheckedListBox(Properties.Resources.PathPropTypes, CheckedList_SET_PathProps);
            Helpers.FillCheckedListBox(Properties.Resources.VoiceTypes, CheckedList_SET_Hints);
            Helpers.FillCheckedListBox(Properties.Resources.DoorTypes, CheckedList_SET_Doors);
            Helpers.FillCheckedListBox(Properties.Resources.EventLighting, CheckedList_Event_Lighting);
            Helpers.FillCheckedListBox(Properties.Resources.EventTerrain, CheckedList_Event_Terrain);
            Helpers.FillCheckedListBox(Properties.Resources.EnvMaps, CheckedList_Scene_EnvMaps);
            Helpers.FillCheckedListBox(Properties.Resources.MiscSongs, CheckedList_Misc_Songs);
            Helpers.FillCheckedListBox(Properties.Resources.MiscLanguages, CheckedList_Misc_Languages);

            // Get all the voice pack zip files in the Voice Packs directory.
            string[] voicePacks = Directory.GetFiles($@"{Environment.CurrentDirectory}\VoicePacks", "*.zip", SearchOption.TopDirectoryOnly);

            // Loop through and add the name of each pack to the CheckedList_Custom_Vox element.
            foreach (string voicePack in voicePacks)
            {
                CheckedListBoxItem item = new()
                {
                    DisplayName = Path.GetFileNameWithoutExtension(voicePack),
                    Tag = Path.GetFileNameWithoutExtension(voicePack),
                    Checked = false
                };
                CheckedList_Custom_Vox.Items.Add(item);
            }

            // Get all the patch files in the user's Mod Manager data.
            string[] patches = Directory.GetFiles($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Unify\\Patches\\", "*.mlua", SearchOption.TopDirectoryOnly);

            foreach (string patch in patches)
            {
                string[] mlua = File.ReadAllLines(patch);
                string[] split = mlua[1].Split('\"');

                CheckedListBoxItem item = new()
                {
                    DisplayName = split[1],
                    Tag = Path.GetFileName(patch),
                    Checked = true
                };

                // Auto uncheck patches which don't fit too well with the Randomiser.
                if (Path.GetFileName(patch) == "EnableDebugMode.mlua" || Path.GetFileName(patch) == "Disable2xMSAA.mlua" || Path.GetFileName(patch) == "Disable4xMSAA.mlua" ||
                    Path.GetFileName(patch) == "DisableCharacterDialogue.mlua" || Path.GetFileName(patch) == "DisableCharacterUpgrades.mlua" || Path.GetFileName(patch) == "DisableHintRings.mlua" ||
                    Path.GetFileName(patch) == "DisableHUD.mlua" || Path.GetFileName(patch) == "DisableMusic.mlua" || Path.GetFileName(patch) == "DisableShadows.mlua" ||
                    Path.GetFileName(patch) == "DisableTalkWindowInStages.mlua" || Path.GetFileName(patch) == "DoNotCarryElise.mlua" || Path.GetFileName(patch) == "DoNotEnterMachSpeed.mlua" ||
                    Path.GetFileName(patch) == "DoNotUseTheSnowboard.mlua" || Path.GetFileName(patch) == "OmegaBlurFix.mlua" || Path.GetFileName(patch) == "TGS2006Menu.mlua")
                {
                    item.Checked = false;
                }

                CheckedList_Misc_Patches.Items.Add(item);
            }
        }

        #region Text Box Functions
        private void ModsDirectory_Browse(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog FolderBrowser = new()
            {
                Description = "Select Mods Directory",
                UseDescriptionForTitle = true
            };

            if (FolderBrowser.ShowDialog() == true)
                TextBox_General_ModsDirectory.Text = FolderBrowser.SelectedPath;
        }
        private void ModsDirectory_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.ModsDirectory = TextBox_General_ModsDirectory.Text;
            Properties.Settings.Default.Save();
        }

        private void GameExecutable_Browse(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog OpenFileDialog = new()
            {
                Title = "Select Game Executable",
                Multiselect = false,
                Filter = "Xbox 360 Executable|default.xex|PlayStation 3 Executable|EBOOT.BIN"
            };

            if (OpenFileDialog.ShowDialog() == true)
                TextBox_General_GameExecutable.Text = OpenFileDialog.FileName;
        }
        private void GameExecutable_Update(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.GameExecutable = TextBox_General_GameExecutable.Text;
            Properties.Settings.Default.Save();
        }

        private void Seed_Reroll(object sender, RoutedEventArgs e)
        {
            TextBox_General_Seed.Text = Randomiser.Next().ToString();
        }

        private void CustomMusic_Browse(object sender, EventArgs e)
        {
            VistaOpenFileDialog OpenFileDialog = new()
            {
                Title = "Select Songs",
                Multiselect = true,
                Filter = "All Types|*.*"
            };

            // If the selections are valid, add them to the list of text in the custom music textbox.
            if (OpenFileDialog.ShowDialog() == true)
            {
                // Don't erase the box, just add a seperator.
                if (TextBox_Custom_Music.Text.Length != 0)
                    TextBox_Custom_Music.Text += "|";

                // Add selected files to the text box.
                for (int i = 0; i < OpenFileDialog.FileNames.Length; i++)
                    TextBox_Custom_Music.Text += $"{OpenFileDialog.FileNames[i]}|";

                // Remove the extra comma added at the end.
                TextBox_Custom_Music.Text = TextBox_Custom_Music.Text.Remove(TextBox_Custom_Music.Text.LastIndexOf('|'));
            }
        }
        #endregion

        #region Form Helper Functions
        /// <summary>
        /// Disables and enables certain other elements based on the toggled status of a CheckBox
        /// </summary>
        private void Dependency_CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // Get the new state of this CheckBox.
            bool? NewCheckedStatus = ((CheckBox)sender).IsChecked;

            // Get the name of this Checkbox.
            string CheckBoxName = ((CheckBox)sender).Name;

            // Check the name of the Checkbox.
            switch(CheckBoxName)
            {
                case "CheckBox_SET_Enemies": CheckBox_SET_Enemies_NoBosses.IsEnabled = (bool)NewCheckedStatus; break;
                case "CheckBox_SET_Enemies_Behaviour": CheckBox_SET_Enemies_Behaviour_NoEnforce.IsEnabled = (bool)NewCheckedStatus; break;

                case "CheckBox_Event_Voices":
                    CheckBox_Event_Voices_Japanese.IsEnabled = (bool)NewCheckedStatus;
                    CheckBox_Event_Voices_Gameplay.IsEnabled = (bool)NewCheckedStatus;
                    break;

                case "CheckBox_Scene_Light_Direction": CheckBox_Scene_Light_Direction_Enforce.IsEnabled = (bool)NewCheckedStatus; break;

                case "CheckBox_Misc_EnemyHealth":
                    Label_Misc_EnemyHealth_Min.IsEnabled = (bool)NewCheckedStatus;
                    NumericUpDown_Misc_EnemyHealth_Min.IsEnabled = (bool)NewCheckedStatus;
                    Label_Misc_EnemyHealth_Max.IsEnabled = (bool)NewCheckedStatus;
                    NumericUpDown_Misc_EnemyHealth_Max.IsEnabled = (bool)NewCheckedStatus;
                    CheckBox_Misc_EnemyHealth_Bosses.IsEnabled = (bool)NewCheckedStatus;
                    break;
                case "CheckBox_Misc_Collision": CheckBox_Misc_Collision_PerFace.IsEnabled = (bool)NewCheckedStatus; break;

                case "CheckBox_Misc_Patches":
                    Label_Misc_Patches_Weight.IsEnabled = (bool)NewCheckedStatus;
                    NumericUpDown_Misc_Patches_Weight.IsEnabled = (bool)NewCheckedStatus;
                    break;
            }
        }

        /// <summary>
        /// Checks and unchecks every element in a CheckedListBox control.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown if the Grid Name or Selected Tab Index doesn't exist in the list.</exception>
        private void CheckedListBox_SelectionToggle(object sender, RoutedEventArgs e)
        {
            // Check if the button that called this event was a Select All one, doing it this way means I don't have to name them or check a large list of button names.
            bool selectAll = (string)((Button)sender).Content == "Select All";

            // Get the grid element this button was a part of.
            var buttonParent = ((Button)sender).Parent;

            // Check the name of the parent grid element.
            // In most cases, we then check for the relevant tab control's selected index to determine the next action.
            switch (((Grid)buttonParent).Name)
            {
                case "Grid_ObjectPlacement":
                    switch (TabControl_ObjectPlacement.SelectedIndex)
                    {
                        case 0: Helpers.InvalidateCheckedListBox(CheckedList_SET_EnemyTypes, true, selectAll); break;
                        case 1: Helpers.InvalidateCheckedListBox(CheckedList_SET_Characters, true, selectAll); break;
                        case 2: Helpers.InvalidateCheckedListBox(CheckedList_SET_ItemCapsules, true, selectAll); break;
                        case 3: Helpers.InvalidateCheckedListBox(CheckedList_SET_CommonProps, true, selectAll); break;
                        case 4: Helpers.InvalidateCheckedListBox(CheckedList_SET_PathProps, true, selectAll); break;
                        case 5: Helpers.InvalidateCheckedListBox(CheckedList_SET_Hints, true, selectAll); break;
                        case 6: Helpers.InvalidateCheckedListBox(CheckedList_SET_Doors, true, selectAll); break;
                        default: throw new NotImplementedException();
                    }
                    break;

                case "Grid_Event":
                    switch (TabControl_Event.SelectedIndex)
                    {
                        case 0: Helpers.InvalidateCheckedListBox(CheckedList_Event_Lighting, true, selectAll); break;
                        case 1: Helpers.InvalidateCheckedListBox(CheckedList_Event_Terrain, true, selectAll); break;
                        default: throw new NotImplementedException();
                    }
                    break;

                case "Grid_Scene":
                    switch (TabControl_Scene.SelectedIndex)
                    {
                        case 0: Helpers.InvalidateCheckedListBox(CheckedList_Scene_EnvMaps, true, selectAll); break;
                        default: throw new NotImplementedException();
                    }
                    break;

                case "Grid_Miscellaneous":
                    switch (TabControl_Miscellaneous.SelectedIndex)
                    {
                        case 0: Helpers.InvalidateCheckedListBox(CheckedList_Misc_Songs, true, selectAll); break;
                        case 1: Helpers.InvalidateCheckedListBox(CheckedList_Misc_Languages, true, selectAll); break;
                        case 2: Helpers.InvalidateCheckedListBox(CheckedList_Misc_Patches, true, selectAll); break;
                        default: throw new NotImplementedException();
                    }
                    break;

                case "Grid_Custom": Helpers.InvalidateCheckedListBox(CheckedList_Custom_Vox, true, selectAll); break;

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Debug Functions
        /// <summary>
        /// Opens the temporary directory. Will open the documents folder if the temporary directory does not exist.
        /// </summary>
        private void Debug_OpenTempDir(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", TemporaryDirectory);
        }

        // Unpack and repack scripts.arc to test archive handling.
        private void Debug_ScriptsArchive(object sender, RoutedEventArgs e)
        {
            string ScriptsArchivePath = Helpers.ArchiveHandler(@$"{Path.GetDirectoryName(TextBox_General_GameExecutable.Text)}\xenon\archives\scripts.arc");
            Helpers.ArchiveHandler(ScriptsArchivePath, $@"{TemporaryDirectory}\scripts.arc");
            Directory.Delete($@"{TemporaryDirectory}\xenon", true);
        }

        // Unpacks scripts.arc and decompiles mission_2001.lub to test Lua Binary handling.
        private void Debug_MissionLua(object sender, RoutedEventArgs e)
        {
            string ScriptsArchivePath = Helpers.ArchiveHandler(@$"{Path.GetDirectoryName(TextBox_General_GameExecutable.Text)}\xenon\archives\scripts.arc");
            Helpers.LuaDecompile($@"{ScriptsArchivePath}\xenon\scripts\mission\2000\mission_2001.lub");
            System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", $@"{ScriptsArchivePath}\xenon\scripts\mission\2000\");
        }
        #endregion

        private void Randomise(object sender, RoutedEventArgs e)
        {
            // Check that our mods directory and game executable actually exist.
            if (!Directory.Exists(TextBox_General_ModsDirectory.Text) || !File.Exists(TextBox_General_GameExecutable.Text))
            {
                MessageBox.Show("Either your Game Executable or Mods Directory don't exist, please check your general settings.",
                                "Sonic '06 Randomiser Suite",
                                MessageBoxButton.OK);
                return;
            }

            // Set up a new Randomiser variable with the new seed.
            Randomiser = new Random(TextBox_General_Seed.Text.GetHashCode());

            // Get a list of all the archives based on the location of the game executable.
            string[] archives = Directory.GetFiles($@"{Path.GetDirectoryName(TextBox_General_GameExecutable.Text)}", "*.arc", SearchOption.AllDirectories);

            // Create Mod Directory (prompting the user if they want to delete it first or cancel if it already exists.)
            if (Directory.Exists($@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({Helpers.UseSafeFormattedCharacters(TextBox_General_Seed.Text)})"))
            {
                MessageBoxResult check = MessageBox.Show($"A mod with the seed {TextBox_General_Seed.Text} already exists.\nDo you want to replace it?",
                                             "Sonic '06 Randomiser Suite",
                                             MessageBoxButton.YesNo);

                if (check == MessageBoxResult.Yes)
                    Directory.Delete($@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({Helpers.UseSafeFormattedCharacters(TextBox_General_Seed.Text)})", true);

                if (check == MessageBoxResult.No)
                    return;
            }

            Directory.CreateDirectory($@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({Helpers.UseSafeFormattedCharacters(TextBox_General_Seed.Text)})");

            // Write mod configuration ini.
            using (Stream configCreate = File.Open(Path.Combine($@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({Helpers.UseSafeFormattedCharacters(TextBox_General_Seed.Text)})", "mod.ini"), FileMode.Create))
            using (StreamWriter configInfo = new(configCreate))
            {
                configInfo.WriteLine("[Details]");
                configInfo.WriteLine($"Title=\"Sonic '06 Randomised ({TextBox_General_Seed.Text})\"");
                configInfo.WriteLine($"Version=\"{TextBox_General_Seed.Text}\"");
                configInfo.WriteLine($"Date=\"{DateTime.Now:dd/MM/yyyy}\"");
                configInfo.WriteLine($"Author=\"Sonic '06 Randomiser Suite\"");

                if (TextBox_General_GameExecutable.Text.ToLower().EndsWith(".xex"))
                    configInfo.WriteLine($"Platform=\"Xbox 360\"");

                if (TextBox_General_GameExecutable.Text.ToLower().EndsWith(".bin"))
                    configInfo.WriteLine($"Platform=\"PlayStation 3\"");

                configInfo.WriteLine("\n[Filesystem]");
                configInfo.WriteLine($"Merge=\"False\"");
                configInfo.WriteLine($"CustomFilesystem=\"False\"");

                configInfo.Close();
            }

            // TODO: Reimplement Wildcard Configuration

            // Enumerate the Checked List Boxes for the user's settings on lists.
            List<string> SetEnemies = Helpers.EnumerateCheckedListBox(CheckedList_SET_EnemyTypes);
            List<string> SetCharacters = Helpers.EnumerateCheckedListBox(CheckedList_SET_Characters);
            List<string> SetItemCapsules = Helpers.EnumerateCheckedListBox(CheckedList_SET_ItemCapsules);
            List<string> SetCommonProps = Helpers.EnumerateCheckedListBox(CheckedList_SET_CommonProps);
            List<string> SetPathProps = Helpers.EnumerateCheckedListBox(CheckedList_SET_PathProps);
            List<string> SetHints = Helpers.EnumerateCheckedListBox(CheckedList_SET_Hints);
            List<string> SetDoors = Helpers.EnumerateCheckedListBox(CheckedList_SET_Doors);

            List<string> EventLighting = Helpers.EnumerateCheckedListBox(CheckedList_Event_Lighting);
            List<string> EventTerrain = Helpers.EnumerateCheckedListBox(CheckedList_Event_Terrain);

            List<string> SceneEnvMaps = Helpers.EnumerateCheckedListBox(CheckedList_Scene_EnvMaps);

            List<string> MiscMusic = Helpers.EnumerateCheckedListBox(CheckedList_Misc_Songs);
            List<string> MiscLanguages = Helpers.EnumerateCheckedListBox(CheckedList_Misc_Languages);
            List<string> MiscPatches = Helpers.EnumerateCheckedListBox(CheckedList_Misc_Patches);

            string[] CustomMusic = TextBox_Custom_Music.Text.Split('|');
            List<string> CustomVoxPacks = Helpers.EnumerateCheckedListBox(CheckedList_Custom_Vox);

            // Custom Music
            if (TextBox_Custom_Music.Text.Length != 0)
                Custom.Music(CustomMusic, $@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({TextBox_General_Seed.Text})", CheckBox_Custom_Music_XMACache.IsChecked, MiscMusic, archives);

            // Voice Packs
            if (CustomVoxPacks.Count > 0)
            {
                // Insert the patched voice_all_e.sbk file into sound.arc first.
                foreach (string archive in archives)
                    if (Path.GetFileName(archive).ToLower() == "sound.arc")
                        File.Copy($@"{Environment.CurrentDirectory}\ExternalResources\voice_all_e.sbk", $@"{Helpers.ArchiveHandler(archive)}\xenon\sound\voice_all_e.sbk", true);

                Custom.VoicePacks(CustomVoxPacks, $@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({TextBox_General_Seed.Text})", archives, SetHints);
            }

            // Disable options if they have nothing to pick from.
            if (SetEnemies.Count == 0)
                CheckBox_SET_Enemies.IsChecked = false;
            if (SetCharacters.Count == 0)
                CheckBox_SET_Characters.IsChecked = false;
            if (SetItemCapsules.Count == 0)
                CheckBox_SET_ItemCapsules.IsChecked = false;
            if (SetCommonProps.Count == 0)
                CheckBox_SET_CommonProps.IsChecked = false;
            if (SetPathProps.Count == 0)
                CheckBox_SET_PathProps.IsChecked = false;
            if (SetHints.Count == 0)
                CheckBox_SET_Hints.IsChecked = false;
            if (SetDoors.Count == 0)
                CheckBox_SET_Doors.IsChecked = false;

            if (EventLighting.Count == 0)
                CheckBox_Event_Lighting.IsChecked = false;
            if (EventTerrain.Count == 0)
                CheckBox_Event_Terrain.IsChecked = false;

            if (SceneEnvMaps.Count == 0)
                CheckBox_Scene_EnvMaps.IsChecked = false;

            if (MiscMusic.Count == 0)
                CheckBox_Misc_Music.IsChecked = false;
            if (MiscLanguages.Count == 0)
                CheckBox_Misc_Text.IsChecked = false;
            if (MiscPatches.Count == 0)
                CheckBox_Misc_Patches.IsChecked = false;

            // Object Placement
            if ((bool)CheckBox_SET_Enemies.IsChecked || (bool)CheckBox_SET_Enemies_Behaviour.IsChecked || (bool)CheckBox_SET_Characters.IsChecked || (bool)CheckBox_SET_ItemCapsules.IsChecked ||
                (bool)CheckBox_SET_CommonProps.IsChecked || (bool)CheckBox_SET_PathProps.IsChecked || (bool)CheckBox_SET_Hints.IsChecked || (bool)CheckBox_SET_Doors.IsChecked ||
                (bool)CheckBox_SET_DrawDistance.IsChecked || (bool)CheckBox_SET_Cosmetic.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "scripts.arc")
                    {
                        ObjectPlacementRandomiser.Load(Helpers.ArchiveHandler(archive), (bool)CheckBox_SET_Enemies.IsChecked, (bool)CheckBox_SET_Enemies_NoBosses.IsChecked,
                                                       (bool)CheckBox_SET_Enemies_Behaviour.IsChecked, (bool)CheckBox_SET_Enemies_Behaviour_NoEnforce.IsChecked, (bool)CheckBox_SET_Characters.IsChecked,
                                                       (bool)CheckBox_SET_ItemCapsules.IsChecked, (bool)CheckBox_SET_CommonProps.IsChecked, (bool)CheckBox_SET_PathProps.IsChecked,
                                                       (bool)CheckBox_SET_Hints.IsChecked, (bool)CheckBox_SET_Doors.IsChecked, (bool)CheckBox_SET_DrawDistance.IsChecked,
                                                       (bool)CheckBox_SET_Cosmetic.IsChecked, SetEnemies, SetCharacters, SetItemCapsules, SetCommonProps, SetPathProps, SetHints, SetDoors,
                                                       (int)NumericUpDown_SET_DrawDistance_Min.Value, (int)NumericUpDown_SET_DrawDistance_Max.Value);

                        // If we have voices enabled or the enemy list contains a boss, then patch them.
                        if ((bool)CheckBox_SET_Hints.IsChecked || SetEnemies.Contains("eCerberus") || SetEnemies.Contains("eGenesis") || SetEnemies.Contains("eWyvern") || SetEnemies.Contains("firstIblis") || SetEnemies.Contains("secondIblis") ||
                            SetEnemies.Contains("thirdIblis") || SetEnemies.Contains("firstmefiress") || SetEnemies.Contains("secondmefiress") || SetEnemies.Contains("solaris01") || SetEnemies.Contains("solaris02"))
                            ObjectPlacementRandomiser.BossPatch(Helpers.ArchiveHandler(archive), (bool)CheckBox_SET_Enemies.IsChecked, (bool)CheckBox_SET_Hints.IsChecked, SetHints);

                        // If we have Characters or Enemies (with Bosses) enabled, then edit the stage luas to handle lua spawns.
                        if (((bool)CheckBox_SET_Enemies.IsChecked && (bool)!CheckBox_SET_Enemies_NoBosses.IsChecked) || (bool)CheckBox_SET_Characters.IsChecked)
                            ObjectPlacementRandomiser.LuaPlayerStartRandomiser(Helpers.ArchiveHandler(archive), (bool)CheckBox_SET_Characters.IsChecked, SetCharacters, (bool)CheckBox_SET_Enemies.IsChecked, (bool)CheckBox_SET_Enemies_NoBosses.IsChecked);
                    }

                    // Patch voice_all_e.sbk to include every in game voice.
                    // Only do this if we have no voice packs installed, as the voice pack installation process already handles this.
                    if (CustomVoxPacks.Count == 0)
                    {
                        if (Path.GetFileName(archive).ToLower() == "sound.arc")
                            File.Copy($@"{Environment.CurrentDirectory}\ExternalResources\voice_all_e.sbk", $@"{Helpers.ArchiveHandler(archive)}\xenon\sound\voice_all_e.sbk", true);
                    }
                }
            }
            
            // Event Randomisation
            if ((bool)CheckBox_Event_Lighting.IsChecked || (bool)CheckBox_Event_XRotation.IsChecked || (bool)CheckBox_Event_YRotation.IsChecked || (bool)CheckBox_Event_ZRotation.IsChecked ||
                (bool)CheckBox_Event_XPosition.IsChecked || (bool)CheckBox_Event_YPosition.IsChecked || (bool)CheckBox_Event_ZPosition.IsChecked ||
                (bool)CheckBox_Event_Terrain.IsChecked || (bool)CheckBox_Event_Order.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "cache.arc")
                    {
                        EventRandomiser.Load(Helpers.ArchiveHandler(archive), (bool)CheckBox_Event_Lighting.IsChecked, EventLighting, (bool)CheckBox_Event_Terrain.IsChecked, EventTerrain,
                                             (bool)CheckBox_Event_XRotation.IsChecked, (bool)CheckBox_Event_YRotation.IsChecked, (bool)CheckBox_Event_ZRotation.IsChecked,
                                             (bool)CheckBox_Event_XPosition.IsChecked, (bool)CheckBox_Event_YPosition.IsChecked, (bool)CheckBox_Event_ZPosition.IsChecked,
                                             (bool)CheckBox_Event_Order.IsChecked, $@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({TextBox_General_Seed.Text})",
                                             TextBox_General_GameExecutable.Text);
                    }
                }
            }
            
            if((bool)CheckBox_Event_Voices.IsChecked)
                EventRandomiser.ShuffleVoiceLines(TextBox_General_GameExecutable.Text, (bool)CheckBox_Event_Voices_Japanese.IsChecked, (bool)CheckBox_Event_Voices_Gameplay.IsChecked, CustomVoxPacks,
                                                  $@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({TextBox_General_Seed.Text})");

            // Scene Randomisation
            if ((bool)CheckBox_Scene_Light_Ambient.IsChecked || (bool)CheckBox_Scene_Light_Main.IsChecked || (bool)CheckBox_Scene_Light_Sub.IsChecked || (bool)CheckBox_Scene_Light_Direction.IsChecked ||
                (bool)CheckBox_Scene_Fog_Colour.IsChecked || (bool)CheckBox_Scene_Fog_Density.IsChecked|| (bool)CheckBox_Scene_EnvMaps.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "scripts.arc")
                    {
                        SceneRandomiser.Load(Helpers.ArchiveHandler(archive), (bool)CheckBox_Scene_Light_Ambient.IsChecked, (bool)CheckBox_Scene_Light_Main.IsChecked,
                                             (bool)CheckBox_Scene_Light_Sub.IsChecked, (bool)CheckBox_Scene_Light_Direction.IsChecked, (bool)CheckBox_Scene_Light_Direction_Enforce.IsChecked,
                                             (bool)CheckBox_Scene_Fog_Colour.IsChecked, (bool)CheckBox_Scene_Fog_Density.IsChecked, (bool)CheckBox_Scene_EnvMaps.IsChecked, SceneEnvMaps);
                    }
                }
            }

            // Miscellaneous Randomisation
            // Music
            if ((bool)CheckBox_Misc_Music.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "scripts.arc")
                    {
                        MiscellaneousRandomisers.MusicRandomiser(Helpers.ArchiveHandler(archive), MiscMusic);
                    }
                }
            }

            // Enemy Health
            if ((bool)CheckBox_Misc_EnemyHealth.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "enemy.arc")
                    {
                        MiscellaneousRandomisers.EnemyHealthRandomiser(Helpers.ArchiveHandler(archive), (int)NumericUpDown_Misc_EnemyHealth_Min.Value, (int)NumericUpDown_Misc_EnemyHealth_Max.Value,
                                                                       (bool)CheckBox_Misc_EnemyHealth_Bosses.IsChecked);
                    }
                }
            }

            // Collision
            if ((bool)CheckBox_Misc_Collision.IsChecked)
            {
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "stage.arc")
                    {
                        MiscellaneousRandomisers.SurfaceRandomiser(Helpers.ArchiveHandler(archive), (bool)CheckBox_Misc_Collision_PerFace.IsChecked);
                    }
                }
            }

            // Text
            if ((bool)CheckBox_Misc_Text.IsChecked)
            {
                string eventArc = "";
                string textArc = "";
                // Get event.arc and text.arc, as we need both for Text Randomisation.
                foreach (string archive in archives)
                {
                    if (Path.GetFileName(archive).ToLower() == "event.arc")
                        eventArc = Helpers.ArchiveHandler(archive);

                    if (Path.GetFileName(archive).ToLower() == "text.arc")
                        textArc = Helpers.ArchiveHandler(archive);
                }

                MiscellaneousRandomisers.TextRandomiser(eventArc, textArc, MiscLanguages);
            }

            // Patches.
            // TODO: Allow the user to choose patches to allow and disallow for this process.
            if ((bool)CheckBox_Misc_Patches.IsChecked)
            {
                MiscellaneousRandomisers.PatchRandomiser($@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({TextBox_General_Seed.Text})", MiscPatches,
                                                         (int)NumericUpDown_Misc_Patches_Weight.Value);
            }


            // Repack Archives
            // This feels kinda dodgy honestly...
            foreach (string archive in archives)
            {
                if (Directory.Exists($@"{TemporaryDirectory}{archive[0..^4].Replace(Path.GetDirectoryName(TextBox_General_GameExecutable.Text), "")}"))
                {
                    string saveDir = $@"{TextBox_General_ModsDirectory.Text}\Sonic '06 Randomised ({Helpers.UseSafeFormattedCharacters(TextBox_General_Seed.Text)}){archive[0..^4].Replace(Path.GetDirectoryName(TextBox_General_GameExecutable.Text), "")}";
                    saveDir = saveDir.Remove(saveDir.LastIndexOf('\\'));

                    Directory.CreateDirectory(saveDir);
                    Helpers.ArchiveHandler($@"{TemporaryDirectory}{archive[0..^4].Replace(Path.GetDirectoryName(TextBox_General_GameExecutable.Text), "")}", $@"{saveDir}\{Path.GetFileName(archive)}");
                }
            }
        }
    }
}
