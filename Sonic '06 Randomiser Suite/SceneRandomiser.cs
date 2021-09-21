﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Sonic_06_Randomiser_Suite
{
    class SceneRandomiser
    {
        /// <summary>
        /// The main entry block for scene lua binary randomisation,all the other functions in this file are accessed through here.
        /// </summary>
        /// <param name="archivePath">The path to the already unpacked scripts.arc containing all the scene lua binary files.</param>
        /// <param name="ambient">Whether the ambient light values should be randomised.</param>
        /// <param name="main">Whether the main light values should be randomised.</param>
        /// <param name="sub">Whether the sub light values should be randomised.</param>
        /// <param name="direction">Whether the light direction values should be randomised.</param>
        /// <param name="enforceDirection">Whether the light direction is FORCED to come from above the player.</param>
        /// <param name="fogColour">Whether the fog colour values should be randomised.</param>
        /// <param name="fogDensity">Whether the fog density values should be randomised.</param>
        /// <param name="env">Whether the used environment map should be randomised.</param>
        /// <param name="envMaps">The valid environment map file paths to use.</param>
        public static void Load(string archivePath, bool ambient, bool main, bool sub, bool direction, bool enforceDirection, bool fogColour, bool fogDensity, bool env, List<string> envMaps)
        {
            // Get a list of all the scene lua binaries in scripts.arc.
            string[] sceneLuas = Directory.GetFiles(archivePath, "scene*.lub", SearchOption.AllDirectories);

            // Loop through all the lua binaries.
            foreach (string sceneLua in sceneLuas)
            {
                // Decompile this lua binary.
                System.Console.WriteLine($@"Randomising scene parameters in '{sceneLua}'.");
                LuaHandler.Decompile(sceneLua);

                // Read the decompiled lua file into a string array.
                string[] lua = File.ReadAllLines(sceneLua);

                // Loop through each line in this lua binary.
                for (int i = 0; i < lua.Length; i++)
                {
                    // Lighting Colours
                    if (lua[i].Contains("Ambient = {") && ambient)
                    {
                        // Alternate Lighting Setups have another line denoting their type, factor this in.
                        if (!lua[i + 1].Contains("Type"))
                            RGBA(lua, i+2, 6, true);
                        else
                            RGBA(lua, i+3, 6, true);
                    }

                    if (lua[i].Contains("Main = {") && !lua[i].Contains("FarDistance") && !lua[i].Contains("ClipDistance") && main)
                    {
                        // Check if this Lua doesn't have the final main block divided up because fuck you.
                        if (lua[i + 1].Contains("ClumpClipDistance") || lua[i + 1].Contains("FarDistance"))
                            continue;

                        // Alternate Lighting Setups have another line denoting their type, factor this in.
                        if (!lua[i + 1].Contains("Type"))
                            RGBA(lua, i + 2, 6, true);
                        else
                            RGBA(lua, i + 3, 6, true);
                    }

                    if (lua[i].Contains("Sub = {") && sub)
                    {
                        // Alternate Lighting Setups have another line denoting their type, factor this in.
                        if (!lua[i + 1].Contains("Type"))
                            RGBA(lua, i + 2, 6, true);
                        else
                            RGBA(lua, i + 3, 6, true);
                    }

                    // Lighting Direction
                    if (lua[i].Contains("Direction_3dsmax") && direction)
                        Direction(lua, i + 2, enforceDirection);

                    // Fog
                    // Colour
                    if (lua[i].Contains("BRay") && fogColour)
                        RGBA(lua, i + 1, 4, false);
                    // Density
                    if (lua[i].Contains("BRay") && fogDensity)
                    {
                        string[] power = lua[i + 4].Split(' ');
                        power[4] = $"{Form_Main.Randomiser.NextDouble() * (0.001 - 0) + 0}";
                        lua[i+  4] = string.Join(' ', power);
                    }

                    // Environment Maps
                    if (lua[i].Contains("EnvMap"))
                    {
                        string[] envMap = lua[i + 1].Split(' ');
                        envMap[4] = $"\"{envMaps[Form_Main.Randomiser.Next(envMaps.Count)]}\"";
                        lua[i + 1] = string.Join(' ', envMap);
                    }
                }

                // Save the updated lua binary.
                File.WriteAllLines(sceneLua, lua);
            }
        }

        /// <summary>
        /// Generates random RGBA values.
        /// </summary>
        /// <param name="lua">The string array we're using.</param>
        /// <param name="startPos">Where in the string array we should be.</param>
        /// <param name="splitLength">How long the split's length is (used by the fog colour).</param>
        /// <param name="usePower">Whether the power value should be handled too (used by the fog colour to avoid changing the fog density as well).</param>
        /// <returns></returns>
        static string[] RGBA(string[] lua, int startPos, int splitLength, bool usePower)
        {
            // Split the RGB values into string arrays.
            string[] rSplit     = lua[startPos].Split(' ');
            string[] gSplit     = lua[startPos + 1].Split(' ');
            string[] bSplit     = lua[startPos + 2].Split(' ');

            // Replace the value at the specified position with a random floating point number between 0 and 1.
            rSplit[splitLength]     = $"{Form_Main.Randomiser.NextDouble()},";
            gSplit[splitLength]     = $"{Form_Main.Randomiser.NextDouble()},";
            bSplit[splitLength]     = $"{Form_Main.Randomiser.NextDouble()},";

            // Rejoin the splits into the main string array.
            lua[startPos]     = string.Join(' ', rSplit);
            lua[startPos + 1] = string.Join(' ', gSplit);
            lua[startPos + 2] = string.Join(' ', bSplit);

            // Repeat the previous three steps for power if required.
            if(usePower)
            {
                string[] powerSplit     = lua[startPos + 3].Split(' ');
                powerSplit[splitLength] = $"{Form_Main.Randomiser.NextDouble()}";
                lua[startPos + 3]       = string.Join(' ', powerSplit);
            }

            // Return the edited string array.
            return lua;
        }

        /// <summary>
        /// Generates a random set of light direction values.
        /// </summary>
        /// <param name="lua">The string array we're using.</param>
        /// <param name="startPos">Where in the string array we should be.</param>
        /// <param name="enforce">Whether the direction should be enforced.</param>
        /// <returns></returns>
        static string[] Direction(string[] lua, int startPos, bool enforce)
        {
            // Split the XYZ values into string arrays.
            string[] xSplit = lua[startPos].Split(' ');
            string[] ySplit = lua[startPos + 1].Split(' ');
            string[] zSplit = lua[startPos + 2].Split(' ');

            // Generate random floating point numbers between -1 and 1, with six decimal places.
            xSplit[8] = $"{Math.Round(Form_Main.Randomiser.NextDouble() * (1 - -1) + -1, 6)},";
            ySplit[8] = $"{Math.Round(Form_Main.Randomiser.NextDouble() * (1 - -1) + -1, 6)},";
            zSplit[8] = $"{Math.Round(Form_Main.Randomiser.NextDouble() * (1 - -1) + -1, 6)}";

            // If the light value on the Z Axis is negative, flip it.
            if(zSplit[8].Contains('-') && enforce)
                zSplit[8] = zSplit[8].Replace("-", string.Empty);

            // Rejoin the splits into the main string array.
            lua[startPos]     = string.Join(' ', xSplit);
            lua[startPos + 1] = string.Join(' ', ySplit);
            lua[startPos + 2] = string.Join(' ', zSplit);

            // Return the edited string array.
            return lua;
        }
    }
}
