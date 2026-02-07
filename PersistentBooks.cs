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

    [HarmonyPatch(typeof(PlayerManager), "AddItemToPlayerInventory")]
    internal static class PlayerManager_AddItemToPlayerInventory_BookNoStack
    {
        static bool Prefix(
            PlayerManager __instance,
            GearItem gi,
            bool trackItemLooted,
            bool enableNotificationFlag
        )
        {
            if (gi == null) return true;
            if (!Main.BookUtils.IsPersistentBook(gi)) return true;

            MelonLogger.Msg($"[PersistentBooksMod] Intercept pickup: {gi.name}");

            Inventory inv = GameManager.GetInventoryComponent();

            int units = 1;
            StackableItem si = gi.GetComponent<StackableItem>();
            if (si != null && si.m_Units > 0)
                units = si.m_Units;

            for (int i = 0; i < units; i++)
            {
                GearItem clone = UnityEngine.Object.Instantiate(gi);
                clone.name = gi.name;

                StackableItem cloneSI = clone.GetComponent<StackableItem>();
                if (cloneSI != null)
                {
                    cloneSI.m_Units = 1;
                    cloneSI.m_DefaultUnitsInItem = 1;
                }

                inv.AddGear(clone, false);
            }

            // 🔑 KRITIKUS FIX: ne azonnal destroy
            MelonCoroutines.Start(DestroyNextFrame(gi));

            return false;
        }

        private static System.Collections.IEnumerator DestroyNextFrame(GearItem gi)
        {
            yield return null;

            if (gi != null && gi.gameObject != null)
            {
                UnityEngine.Object.Destroy(gi.gameObject);
            }
        }
    }
}