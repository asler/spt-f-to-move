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

        public void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        protected void Start()
        {
            _logger?.LogInfo("com.alimoncul.ftomove: Mover Start called!");
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

        protected void Update()
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
                    NotificationManagerClass.DisplayWarningNotification(new GClass3726(item, destroyResult.ItemsToDestroy).GetLocalizedDescription(), ENotificationDurationType.Default);
                    return;
                }

                traderControllerClass.RunNetworkTransaction(findResult.Value, null);

                Singleton<GUISounds>.Instance.PlayItemSound(item.ItemSound, EInventorySoundType.pickup, false);
            }
        }
    }
}
