using System;

using MelonLoader;
using HarmonyLib;
using UnityEngine;

namespace Mikey {
    class DisablePBUpdating_Patch {
        [HarmonyPatch(typeof(LevelStats), "UpdateTimeMicroseconds")]
        [HarmonyPrefix]
        static bool SkipUpdatingPb(LevelStats __instance, long newTime) {
            __instance._timeLastMicroseconds = newTime;
            return false;
        }
    }

    class ThisPatchHasToApplyAtLaunchSmile {
        [HarmonyPatch(typeof(LevelRush), "GetCurrentLevelRushType")]
        [HarmonyPostfix]
        public static void ItsMikeyTime(ref LevelRush.LevelRushType __result) {
            if (ThankYouProZD.inSetCard) {
                __result = LevelRush.LevelRushType.MikeyRush;
            }
        }
    }

    class ThankYouProZD {
        public static bool inSetCard = false;

        [HarmonyPatch(typeof(CardPickup), "SetCard")]
        [HarmonyPrefix]
        public static void OnEnterCardShit() {
            inSetCard = true;
        }

        [HarmonyPatch(typeof(CardPickup), "SetCard")]
        [HarmonyPostfix]
        public static void OnExitCardShit() {
            inSetCard = false;
        }

        [HarmonyPatch(typeof(GhostRecorder), "SaveCompressed", new Type[] { } )]
        [HarmonyPrefix]
        public static bool PreventGhosts() {
            return false;
        }
    }

    public class MikeyMode : MelonMod {
        MelonPreferences_Category mikey_rush;
        MelonPreferences_Entry<bool> is_enabled;
        public static bool was_enabled = false;
        public override void OnApplicationLateStart() {
            mikey_rush = MelonPreferences.CreateCategory("Mikey Mode Practice");
            is_enabled = mikey_rush.CreateEntry("Enable mikey rush mode (must restart to turn off)" , false);
            HarmonyInstance.PatchAll(typeof(ThisPatchHasToApplyAtLaunchSmile));
            DoPatch();
        }

        public override void OnPreferencesSaved() {
            DoPatch();
        }

        private void DoPatch() {
            if (is_enabled.Value) {
                was_enabled = true;
                GameDataManager.powerPrefs.dontUploadToLeaderboard = true;
                HarmonyInstance.PatchAll(typeof(DisablePBUpdating_Patch));
                HarmonyInstance.PatchAll(typeof(ThankYouProZD));
            }
        }

        public GUIStyle DefaultTextStyle() {
            GUIStyle style = new GUIStyle();
            style.fixedHeight = 20;
            style.fontSize = 20;
            return style;
        }

        public void DrawText(int x_offset, int y_offset, string s, Color c) {
            GUIStyle style = DefaultTextStyle();
            style.normal.textColor = c;

            GUIStyle outline_style = DefaultTextStyle();
            outline_style.normal.textColor = Color.black;
            int outline_strength = 2;

            Rect r = new Rect(x_offset, y_offset, 120, 30);

            for (int i = -outline_strength; i <= outline_strength; ++i) {
                GUI.Label(new Rect(r.x - outline_strength, r.y + i, r.width, r.height), s, outline_style);
                GUI.Label(new Rect(r.x + outline_strength, r.y + i, r.width, r.height), s, outline_style);
            }
            for (int i = -outline_strength + 1; i <= outline_strength - 1; ++i) {
                GUI.Label(new Rect(r.x + i, r.y - outline_strength, r.width, r.height), s, outline_style);
                GUI.Label(new Rect(r.x + i, r.y + outline_strength, r.width, r.height), s, outline_style);
            }
            GUI.Label(r, s, style);
        }

        public override void OnGUI() {
            if (is_enabled.Value) {
                DrawText(50, 5, "Mikey Practice mod enabled", Color.cyan);
            } else if (was_enabled) {
                DrawText(50, 5, "Need to restart for PBs to re-enable!", Color.red);
            }
        }
    }
}
