﻿using System;
using System.Reflection;
using HarmonyLib;
using KMod;
using oni_vietnamese.Config;
using oni_vietnamese.Utils;
using TMPro;

namespace oni_vietnamese {
    public class Patches : UserMod2 {
		private static readonly string ns = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
		public static string rootPath;
		private static FontConfig fc;
		private static TMP_FontAsset font;

		public override void OnLoad(Harmony harmony) {
            harmony.PatchAll();
            rootPath = mod.file_source.GetRoot();
            ConfigManager.Instance.configPath = mod.file_source.GetRoot();
            fc = ConfigManager.Instance.LoadConfigFile();
            font = FontUtil.LoadFontAsset(fc);

            if (font == null) {
                Debug.LogWarning($"[{ns}] Tải font thất bại.");
                return;
            }
        }

        [HarmonyPatch(typeof(Localization))]
        [HarmonyPatch(nameof(Localization.GetLocale))]
        [HarmonyPatch(new Type[] { typeof(string[]) })]
        public static class Localization_GetLocale_Patch {
            public static void Postfix(ref Localization.Locale __result) {
                try {
                    if (font == null) {
                        return;
                    }

                    var Language = fc.Code.Equals("zh") ? Localization.Language.Chinese : Localization.Language.Unspecified;
                    var Direction = fc.LeftToRight ? Localization.Direction.LeftToRight : Localization.Direction.RightToLeft;
                    __result = new Localization.Locale(Language, Direction, fc.Code, font.name);
                } catch (Exception ex) {
                    DebugUtil.LogWarningArgs(new object[] { ex });
                }
            }
        }
    }
}
