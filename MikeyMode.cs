using System;
using System.Reflection;

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

    // TODO: also lie and say MikeyRush when inside GhostPlayback.LoadLevelData
    // will also need to adjust SaveCompressed to save w/ an id of 1UL
    class ThisPatchHasToApplyAtLaunchSmile {
        [HarmonyPatch(typeof(LevelRush), "GetCurrentLevelRushType")]
        [HarmonyPostfix]
        public static void ItsMikeyTime(ref LevelRush.LevelRushType __result) {
            if (ThankYouProZD.are_we_lying) {
                __result = LevelRush.LevelRushType.MikeyRush;
            }
        }

        [HarmonyPatch(typeof(LevelRush), "IsLevelRush")]
        [HarmonyPostfix]
        public static void ItsMikeyTimeBaby(ref bool __result) {
            if (ThankYouProZD.are_we_lying) {
                __result = true;
            }
        }
    }

    class ThankYouProZD {
        public static bool are_we_lying = false;

        [HarmonyPatch(typeof(GhostPlayback), "LoadLevelData")]
        [HarmonyPrefix]
        public static void OnEnterLoadShit() {
            are_we_lying = MikeyMode.use_mikey_ghosts.Value;
        }

        // LoadLevelData does a callback rather than exit -> flag set to false, so we just hook the callback and set to false there instead
        [HarmonyPatch(typeof(GhostPlayback), "OnLevelDataLoaded")]
        [HarmonyPrefix]
        public static void OnExitLoadShit() {
            are_we_lying = false;
        }

        [HarmonyPatch(typeof(CardPickup), "SetCard")]
        [HarmonyPrefix]
        public static void OnEnterCardShit() {
            are_we_lying = true;
        }

        [HarmonyPatch(typeof(CardPickup), "SetCard")]
        [HarmonyPostfix]
        public static void OnExitCardShit() {
            are_we_lying = false;
        }

        [HarmonyPatch(typeof(CardPickup), "Spawn")]
        [HarmonyPrefix]
        public static void OnEnterCardSpawnShit() {
            are_we_lying = true;
        }

        [HarmonyPatch(typeof(CardPickup), "Spawn")]
        [HarmonyPostfix]
        public static void OnExitCardSpawnShit() {
            are_we_lying = false;
        }

        [HarmonyPatch(typeof(CardPickup), "SpawnPickupVendor")]
        [HarmonyPrefix]
        public static void OnEnterVendorShit() {
            are_we_lying = true;
        }

        [HarmonyPatch(typeof(CardPickup), "SpawnPickupVendor")]
        [HarmonyPostfix]
        public static void OnExitVendorShit() {
            are_we_lying = false;
        }

        [HarmonyPatch(typeof(GhostRecorder), "SaveCompressed", new Type[] { } )]
        [HarmonyPrefix]
        public static bool SaveMikeyGhosts(GhostRecorder __instance) {
            FieldInfo recording_frames = typeof(GhostRecorder).GetField("m_recordingFrames", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo recording_index = typeof(GhostRecorder).GetField("m_recordingIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            GhostFrame[] frames = (GhostFrame[]) recording_frames.GetValue(__instance);
            int index = (int)recording_index.GetValue(__instance);
            
            // and now we save.... 
            string path = "";
            GhostUtils.GetPath(GhostUtils.GhostType.PersonalGhost, ref path);
            GhostRecorder.SaveCompressed(frames, index, path, 1UL, true);
            return false;
        }

        [HarmonyPatch(typeof(GhostUtils), "LoadLevelTotalTimeCompressed")]
        [HarmonyPrefix]
        public static void AllanPleaseAddID(ref ulong saveId) {
            saveId = 1UL;
        }
    }

    public class MikeyMode : MelonMod {
        public static MelonPreferences_Category mikey_rush;
        public static MelonPreferences_Entry<bool> is_enabled;
        public static MelonPreferences_Entry<bool> use_mikey_ghosts;
        public static bool was_enabled = false;

        public override void OnApplicationLateStart() {
            mikey_rush = MelonPreferences.CreateCategory("Mikey Mode Practice");
            is_enabled = mikey_rush.CreateEntry("Enable mikey rush mode (must restart to turn off)" , false);
            use_mikey_ghosts = mikey_rush.CreateEntry("Load mikey ghosts when enabled", true);
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
