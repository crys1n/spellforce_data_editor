﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public static class Settings
    {
        public static int IgnoredMipMapsCount { get; private set; } = 0;
        public static int MaximumAllowedTextureSize { get; private set; } = 2048;
        public static bool EnableShadows { get; set; } = true;
        public static int ShadowMapSize { get; private set; } = 1024;
        public static int FramesPerSecond { get; private set; } = 60;
        public static int AntiAliasingSamples { get; private set; } = 4;

        public static bool UnitsVisible { get; set; } = true;
        public static bool BuildingsVisible { get; set; } = true;
        public static bool ObjectsVisible { get; set; } = true;
        public static bool DecorationsVisible { get; set; } = true;
        public static bool LakesVisible { get; set; } = true;
        public static bool OverlaysVisible { get; set; } = true;
        public static bool VisualizeHeight { get; set; } = false;
        public static bool DisplayGrid { get; set; } = false;
        public static OpenTK.Vector4 GridColor { get; set; } = new OpenTK.Vector4(1, 1, 1, 1);

        public static string ExtractDirectory { get; set; } = "";
        public static bool ExtractAllInOne { get; set; } = false;

        public static string GameDirectory { get; set; } = "";
        public static int LanguageID { get; private set; } = 1;

        public static bool LogInfo { get; private set; } = true;
        public static bool LogWarning { get; private set; } = true;
        public static bool LogError { get; private set; } = true;

        public static string GameRunArguments { get; set; } = "";

        public static int Load()
        {
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Settings.Load() called");

            try
            {
                string[] settings = File.ReadAllLines("config.txt");
                bool ignore_rest = false;
                foreach(string s in settings)
                {
                    if (s.Length == 0)
                        continue;
                    if (s[0] == '#')
                        continue;

                    string[] words = s.Split(' ');
                    if (words.Length < 2)
                        continue;

                    switch(words[0])
                    {
                        case "!":
                            if (words[1] == "Ignore")
                                ignore_rest = true;
                            break;
                        case "IgnoredMipMapsCount":
                            IgnoredMipMapsCount = Utility.TryParseUInt8(words[1], (byte)IgnoredMipMapsCount);
                            if (IgnoredMipMapsCount > 2)
                                IgnoredMipMapsCount = 2;
                            break;
                        case "MaximumAllowedTextureSize":
                            MaximumAllowedTextureSize = Utility.TryParseUInt16(words[1], (ushort)MaximumAllowedTextureSize);
                            if (MaximumAllowedTextureSize < 64)
                                MaximumAllowedTextureSize = 64;
                            break;
                        case "ShadowMapSize":
                            ShadowMapSize = Utility.TryParseUInt16(words[1], (ushort)ShadowMapSize);
                            if (ShadowMapSize < 64)
                                ShadowMapSize = 64;
                            break;
                        case "FramesPerSecond":
                            FramesPerSecond = Utility.TryParseUInt16(words[1], (ushort)FramesPerSecond);
                            if (FramesPerSecond < 5)
                                FramesPerSecond = 5;
                            if (FramesPerSecond > 1000)
                                FramesPerSecond = 1000;
                            break;
                        case "EnableShadows":
                            EnableShadows = (words[1] == "YES");
                            break;
                        case "AntiAliasingSamples":
                            AntiAliasingSamples = Utility.TryParseUInt16(words[1], (ushort)AntiAliasingSamples);
                            if (AntiAliasingSamples > 4)
                                AntiAliasingSamples = 4;
                            AntiAliasingSamples = (AntiAliasingSamples / 2) * 2;
                            break;

                        case "UnitsVisible":
                            UnitsVisible = (words[1] == "YES");
                            break;
                        case "BuildingsVisible":
                            BuildingsVisible = (words[1] == "YES");
                            break;
                        case "ObjectsVisible":
                            ObjectsVisible = (words[1] == "YES");
                            break;
                        case "DecorationsVisible":
                            DecorationsVisible = (words[1] == "YES");
                            break;
                        case "LakesVisible":
                            LakesVisible = (words[1] == "YES");
                            break;
                        case "OverlaysVisible":
                            OverlaysVisible = (words[1] == "YES");
                            break;
                        case "VisualizeHeight":
                            VisualizeHeight = (words[1] == "YES");
                            break;
                        case "DisplayGrid":
                            DisplayGrid = (words[1] == "YES");
                            break;
                        case "GridColor":
                            byte r = 255;
                            byte g = 255;
                            byte b = 255;
                            if (words.Length == 4)
                            {
                                r = Utility.TryParseUInt8(words[1], 255);
                                g = Utility.TryParseUInt8(words[2], 255);
                                b = Utility.TryParseUInt8(words[3], 255);
                            }
                            GridColor = new OpenTK.Vector4(r, g, b, 255) / 255f;
                            break;

                        case "ExtractDirectory":
                            string es2 = words[1];
                            for (int i = 2; i < words.Length; i++)
                                es2 += " " + words[i];
                            ExtractDirectory = es2;
                            break;
                        case "ExtractAllInOne":
                            ExtractAllInOne = (words[1] == "YES");
                            break;
                        case "GameDirectory":
                            string s2 = words[1];
                            for (int i = 2; i < words.Length; i++)
                                s2 += " " + words[i];
                            GameDirectory = s2;
                            SFUnPak.SFUnPak.SpecifyGameDirectory(GameDirectory);
                            break;
                        case "LogInfo":
                            LogInfo = (words[1] == "YES");
                            if (LogInfo)
                                LogUtils.Log.SetOption(LogUtils.LogOption.INFO);
                            break;
                        case "LogWarning":
                            LogWarning = (words[1] == "YES");
                            if (LogWarning)
                                LogUtils.Log.SetOption(LogUtils.LogOption.WARNING);
                            break;
                        case "LogError":
                            LogError = (words[1] == "YES");
                            if (LogError)
                                LogUtils.Log.SetOption(LogUtils.LogOption.ERROR);
                            break;
                        case "LanguageID":
                            LanguageID = Utility.TryParseUInt8(words[1], (byte)LanguageID);
                            if (LanguageID > 4)
                                LanguageID = 1;
                            break;
                        case "GameRunArguments":
                            string s3 = words[1];
                            for (int i = 2; i < words.Length; i++)
                                s3 += " " + words[i];
                            GameRunArguments = s3;
                            break;
                    }
                    if (ignore_rest)
                        break;
                }
            }
            catch(Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Settings.Load(): failed to load settings from config.txt");
                return -1;
            }

            return 0;
        }

        public static void Save()
        {
            LogUtils.Log.Info(LogUtils.LogSource.Main, "Settings.Save() called");

            try
            {
                string[] settings = File.ReadAllLines("config.txt");
                List<string> new_settings = new List<string>();
                foreach (string s in settings)
                {
                    if(s.Length == 0)
                    {
                        new_settings.Add("");
                        continue;
                    }
                    string[] words = s.Split(' ');
                    if ((words.Length == 0) || (words[0].Length == 0))
                    {
                        new_settings.Add("");
                        continue;
                    }
                    if((s[0] == '#')||(s[0] == '!'))
                    {
                        new_settings.Add(s);
                        continue;
                    }

                    bool skipped_line = false;
                    switch (words[0])
                    {
                        case "IgnoredMipMapsCount":
                            words = new string[] { words[0], IgnoredMipMapsCount.ToString() };
                            break;
                        case "MaximumAllowedTextureSize":
                            words = new string[] { words[0], MaximumAllowedTextureSize.ToString() };
                            break;
                        case "ShadowMapSize":
                            words = new string[] { words[0], ShadowMapSize.ToString() };
                            break;
                        case "FramesPerSecond":
                            words = new string[] { words[0], FramesPerSecond.ToString() };
                            break;
                        case "EnableShadows":
                            words = new string[] { words[0], EnableShadows ? "YES" : "NO" };
                            break;
                        case "AntiAliasingSamples":
                            words = new string[] { words[0], AntiAliasingSamples.ToString() };
                            break;

                        case "UnitsVisible":
                            words = new string[] { words[0], UnitsVisible ? "YES" : "NO" };
                            break;
                        case "BuildingsVisible":
                            words = new string[] { words[0], BuildingsVisible ? "YES" : "NO" };
                            break;
                        case "ObjectsVisible":
                            words = new string[] { words[0], ObjectsVisible ? "YES" : "NO" };
                            break;
                        case "DecorationsVisible":
                            words = new string[] { words[0], DecorationsVisible ? "YES" : "NO" };
                            break;
                        case "LakesVisible":
                            words = new string[] { words[0], LakesVisible ? "YES" : "NO" };
                            break;
                        case "OverlaysVisible":
                            words = new string[] { words[0], OverlaysVisible ? "YES" : "NO" };
                            break;
                        case "VisualizeHeight":
                            words = new string[] { words[0], VisualizeHeight ? "YES" : "NO" };
                            break;
                        case "DisplayGrid":
                            words = new string[] { words[0], DisplayGrid ? "YES" : "NO" };
                            break;
                        case "GridColor":
                            words = new string[] { words[0], ((byte)(GridColor.X * 255)).ToString() ,
                                                             ((byte)(GridColor.Y * 255)).ToString() ,
                                                             ((byte)(GridColor.Z * 255)).ToString() };
                            break;

                        case "ExtractDirectory":
                            words = new string[] { words[0], ExtractDirectory };
                            break;
                        case "ExtractAllInOne":
                            words = new string[] { words[0], ExtractAllInOne ? "YES" : "NO" };
                            break;
                        case "GameDirectory":
                            words = new string[] { words[0], GameDirectory };
                            break;
                        case "LogInfo":
                            words = new string[] { words[0], LogInfo ? "YES" : "NO" };
                            break;
                        case "LogWarning":
                            words = new string[] { words[0], LogWarning ? "YES" : "NO" };
                            break;
                        case "LogError":
                            words = new string[] { words[0], LogError ? "YES" : "NO" };
                            break;
                        case "LanguageID":
                            words = new string[] { words[0], LanguageID.ToString() };
                            break;
                        case "GameRunArguments":
                            words = new string[] { words[0], GameRunArguments };
                            break;
                        default:
                            new_settings.Add(s);
                            skipped_line = true;
                            break;
                    }
                    if (skipped_line)
                        continue;
                    string s2 = words[0] + " " + words[1];
                    new_settings.Add(s2);
                }
                File.WriteAllLines("config.txt", new_settings.ToArray());
            }
            catch (Exception)
            {
                LogUtils.Log.Error(LogUtils.LogSource.Main, "Settings.Save(): failed to save settings to config.txt");
            }
        }
    }
}
