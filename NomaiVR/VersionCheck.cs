﻿using UnityEngine;
using System.Collections;
using System;

namespace NomaiVR
{
    public static class VersionCheck
    {
        const string SupportedVersion = "1.0.5";

        public static void CheckGameVersion()
        {
            if (!IsGameVersionSupported())
            {
                NomaiVR.LogError(
                    $"Fatal error: this version of NomaiVR only supports Outer Wilds {SupportedVersion}. " +
                    $"Make sure you are using the latest version of NomaiVR. " +
                    $"Currently installed version of Outer Wilds is {Application.version}. " +
                    $"You can force the game to start anyway by setting skipGameVersionCheck to true in NomaiVR/config.json."
                );
                Application.Quit();
            }
        }

        private static bool IsGameVersionSupported()
        {
            string[] gameVersionParts = SplitVersion(Application.version);
            string[] supportedVersionParts = SplitVersion(SupportedVersion);

            for (int i = 0; i < supportedVersionParts.Length; i++)
            {
                if (gameVersionParts[i] != supportedVersionParts[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static string[] SplitVersion(string version)
        {
            return version.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}