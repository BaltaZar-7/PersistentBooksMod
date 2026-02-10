#nullable disable
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;

namespace PersistentBooksMod
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("PersistentBooksMod loaded!");
        }

        internal static class BookUtils
        {
            private static readonly HashSet<string> BookGearNames = new()
    {
        "GEAR_BookA",
        "GEAR_BookB",
        "GEAR_BookBopen",
        "GEAR_BookC",
        "GEAR_BookD",
        "GEAR_BookE",
        "GEAR_BookEopen",
        "GEAR_BookF",
        "GEAR_BookFopen",
        "GEAR_BookG",
        "GEAR_BookH",
        "GEAR_BookHopen",
        "GEAR_BookI",
        "GEAR_BookLabE_01",
        "GEAR_BookLabE_02",
        "GEAR_BookLabE_03",
        "GEAR_BookLabE_Open_01",
        "GEAR_BookManual"
    };

            internal static bool IsPersistentBook(GearItem gi)
            {
                return gi != null && BookGearNames.Contains(gi.name);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerManager))]
    [HarmonyPatch(nameof(PlayerManager.TryAddToExistingStackable))]
    [HarmonyPatch(
    new System.Type[] {
        typeof(GearItem),
        typeof(float),
        typeof(int),
        typeof(GearItem)
    },
    new ArgumentType[] {
        ArgumentType.Normal,
        ArgumentType.Normal,
        ArgumentType.Normal,
        ArgumentType.Ref
    }
)]
    internal static class PlayerManager_TryAddToExistingStackable_BlockBooks
    {
        static bool Prefix(ref GearItem gearToAdd, ref GearItem existingGearItem)
        {
            if (gearToAdd == null)
                return true;

            if (!Main.BookUtils.IsPersistentBook(gearToAdd))
                return true;

            MelonLogger.Msg($"[PersistentBooksMod] Prevent stacking book: {gearToAdd.name}");

            // 🔑 ne legyen stack target → játék stack logikája skip
            existingGearItem = null;

            return false;
        }
    }
}