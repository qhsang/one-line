﻿using UnityEngine;
using System.Collections;

namespace EasyMobile.Editor
{
    public static class EM_Constants
    {
        // Product name
        public const string ProductName = "Easy Mobile";
        public const string Copyright = "© 2017-2018 SgLib Games LLC. All Rights Reserved.";

        // Current version
        public const string versionString = "1.1.0";
        public const int versionInt = 0x01100;

        // Folder
        public const string RootPath = "Assets/OneLine/_Scripts/EM";
        public const string EditorFolder = RootPath + "/Editor";
        public const string PackagesFolder = RootPath + "/Packages";
        public const string AssetsPluginsAndroidFolder = "Assets/Plugins/Android";
        public const string AssetsPluginsIOSFolder = "Assets/Plugins/iOS";
        public const string SkinFolder = RootPath + "/GUISkins";
        public const string SkinTextureFolder = SkinFolder + "/Textures";

        // Asset and stuff
        public const string PluginSettingsFilePath = EditorFolder + "/EasyMobileSettings.txt";

        // UnityPackages
        public const string PlayServicersResolverPackagePath = PackagesFolder + "/play-services-resolver.unitypackage";

        // URLs
        public const string DocumentationURL = "https://sglibgames.gitbooks.io/easy-mobile-user-guide/content/";
        public const string SupportEmail = "sglib.games@gmail.com";

        // ProjectSettings keys
        public const string PSK_EMVersionString = "VERSION";
        public const string PSK_EMVersionInt = "VERSION_INT";
        public const string PSK_ImportedPlayServicesResolver = "IMPORTED_PLAY_SERVICES_RESOLVER";
    }
}

