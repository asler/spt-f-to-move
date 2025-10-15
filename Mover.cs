using BepInEx.Logging;
using EFT.UI;
using EFT.InventoryLogic;
using Comfort.Common;
using UnityEngine;
using EFT.Communications;
using HarmonyLib;
using EFT;

namespace ftomove
{
    public class Mover : MonoBehaviour
    {
        private ManualLogSource _logger;
        private static GameWorld gameWorld;

        public Mover()
        {
            if (_logger == null)
            {
                _logger = BepInEx.Logging.Logger.CreateLogSource(nameof(Mover));
            }
        }

        public static void Enable()
        {
            if (Singleton<IBotGame>.Instantiated)
            {
                gameWorld = Singleton<GameWorld>.Instance;
                var mover = gameWorld.GetOrAddComponent<Mover>();
                mover._logger.LogInfo("Enabled");
            }
        }

        public static bool InRaid()
        {
            var instance = Singleton<AbstractGame>.Instance;
            return instance != null && instance.InRaid;
        }

        public static bool InInventoryScreen()
        {
            return ItemUiContext.Instance.ContextType == EItemUiContextType.InventoryScreen;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && InRaid() && InInventoryScreen())
            {
                var itemUiContextInstance = ItemUiContext.Instance;
                if (itemUiContextInstance == null)
                {
                    return;
                }

                ItemContextAbstractClass itemContextAbstractClass = itemUiContextInstance.ItemContextAbstractClass;
                if (itemContextAbstractClass == null)
                {
                    return;
                }

                var traderControllerClass = AccessTools.Field(typeof(ItemUiContext), "traderControllerClass").GetValue(itemUiContextInstance) as TraderControllerClass;
                if (traderControllerClass == null)
                {
                    return;
                }

                Item item = itemContextAbstractClass.Item;

                var findResult = ItemUiContext.Instance.QuickFindAppropriatePlace(itemContextAbstractClass, traderControllerClass, false, true, true);
                if (findResult.Failed || !traderControllerClass.CanExecute(findResult.Value))
                {
                    return;
                }

                if (findResult.Value is IDestroyResult destroyResult && destroyResult.ItemsDestroyRequired)
                {
                    NotificationManagerClass.DisplayWarningNotification(new GClass1583(item, destroyResult.ItemsToDestroy).GetLocalizedDescription(), ENotificationDurationType.Default);
                    return;
                }

                traderControllerClass.RunNetworkTransaction(findResult.Value, null);

                Singleton<GUISounds>.Instance.PlayItemSound(item.ItemSound, EInventorySoundType.pickup, false);
            }
        }
    }
}
