﻿using Marathon.Formats.Archive;
using Marathon.Formats.Script.Lua;
using Marathon.IO;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace MarathonRandomiser
{
    internal class Helpers
    {
        /// <summary>
        /// Fills out the custom CheckedListBox elements from text files containing comma seperated values.
        /// </summary>
        /// <param name="file">Text file containing comma seperated values.</param>
        /// <param name="listBox">The CheckedListBox element to populate.</param>
        public static void FillCheckedListBox(string file, CheckedListBox listBox)
        {
            // Load text file into string array.
            string[] list = file.Split("\r\n");

            // Loop through every entry in the text file.
            foreach (string line in list)
            {
                // Split this entry on the comma values.
                string[] split = line.Split(',');

                // Parse this entry into a CheckedListBoxItem.
                CheckedListBoxItem item = new()
                {
                    DisplayName = split[0],
                    Tag = split[1],
                    Checked = bool.Parse(split[2])
                };

                // Add this parsed entry into the CheckedListBox.
                listBox.Items.Add(item);
            }
        }
        
        /// <summary>
        /// Hacky workaround to fix the custom CheckedListBox element not visually updating if addressed in code.
        /// </summary>
        /// <param name="listBox">The CheckedListBox element to update.</param>
        /// <param name="edit">Whether we're also doing editing first</param>
        /// <param name="checkAll">Whether to check or uncheck the elements if we are editing.</param>
        public static void InvalidateCheckedListBox(CheckedListBox listBox, bool edit = false, bool checkAll = true)
        {
            if (edit)
            {
                foreach (CheckedListBoxItem item in listBox.Items)
                {
                    if (checkAll)
                        item.Checked = true;
                    else
                        item.Checked = false;
                }
            }

            // Get a standalone list of all the items in the target CheckedListBox.
            List<CheckedListBoxItem> listItems = listBox.Items.ToList();

            // Clear out the existing list of items in the target CheckedListBox.
            listBox.Items.Clear();

            // Loop through and add the items back with their updated status.
            foreach (CheckedListBoxItem item in listItems)
                listBox.Items.Add(item);
        }

        /// <summary>
        /// Loops through every item in a CheckedListBox element and return a list of their tags if checked.
        /// </summary>
        /// <param name="listBox">The CheckedListBox element to loop through.</param>
        /// <returns>The string list to return.</returns>
        public static List<string> EnumerateCheckedListBox(CheckedListBox listBox)
        {
            // Set up our string list.
            List<string> list = new();

            // Loop through each item in the CheckedListBox element.
            foreach (CheckedListBoxItem item in listBox.Items)
            {
                // If the item is checked, add its tag value to the list.
                if (item.Checked)
                    list.Add(item.Tag);
            }

            // Return the string list.
            return list;
        }

        /// <summary>
        /// Decompiles a Lua Binary file.
        /// </summary>
        /// <param name="path">Path to the Lua Binary to decompile.</param>
        public static async Task LuaDecompile(string path)
        {
            // Marathon throws an exception if a Lua is already decompiled, so catch any to prevent a program crash.
            try
            {
                LuaBinary lub = new(path, true);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Replaces illegal characters from a path with underscores.
        /// </summary>
        /// <param name="text">The string to strip out.</param>
        public static string UseSafeFormattedCharacters(string text)
        {
            return text.Replace(@"\", "_")
                       .Replace("/", "_")
                       .Replace(":", "_")
                       .Replace("*", "_")
                       .Replace("?", "_")
                       .Replace("\"", "_")
                       .Replace("<", "_")
                       .Replace(">", "_")
                       .Replace("|", "_");
        }

        /// <summary>
        /// Unpacks a Sonic '06 U8 Archive to the temporary directory.
        /// </summary>
        /// <param name="archive">The path to the archive to unpack.</param>
        /// <returns>The location of the unpacked archive.</returns>
        public static async Task<string> ArchiveHandler(string archive)
        {
            // Determine whether this archive is in a win32, xenon or ps3 directory.
            string archiveLocation = "";
            if (archive.Contains("\\win32"))
                archiveLocation = archive.Remove(archive.IndexOf("\\win32"));
            if (archive.Contains("\\xenon"))
                archiveLocation = archive.Remove(archive.IndexOf("\\xenon"));
            if (archive.Contains("\\ps3"))
                archiveLocation = archive.Remove(archive.IndexOf("\\ps3"));

            // Check if the archive is already extracted to the temporary directory. If not, then unpack it.
            if (!Directory.Exists($@"{MainWindow.TemporaryDirectory}{archive[0..^4].Replace(archiveLocation, "")}"))
            {
                U8Archive arc = new(archive, ReadMode.IndexOnly);
                arc.Extract($@"{MainWindow.TemporaryDirectory}{archive[0..^4].Replace(archiveLocation, "")}");
            }
            // Return the path to the unpacked archive as a string.
            return $@"{MainWindow.TemporaryDirectory}{archive[0..^4].Replace(archiveLocation, "")}";
        }

        /// <summary>
        /// Repacks a directory to a U8 Archive. Currently using Optimal compression due to a Marathon bug.
        /// </summary>
        /// <param name="path">The path to the repack.</param>
        /// <param name="savePath">Where to save the packed archive to.</param>
        public static async Task ArchiveHandler(string path, string savePath)
        {
            U8Archive arc = new(path, true, CompressionLevel.Optimal);
            arc.Save(savePath);
        }
    }
}