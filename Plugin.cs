using BepInEx;
using UnityEngine;
using BepInEx.Logging;

namespace ftomove
{
    [BepInPlugin("com.alimoncul.ftomove", "ftomove", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        protected void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("com.alimoncul.ftomove loaded");

            var moverObject = new GameObject("Mover");
            var mover = moverObject.AddComponent<Mover>();

            mover.Initialize(Logger);
            DontDestroyOnLoad(moverObject);
        }
    }
}
