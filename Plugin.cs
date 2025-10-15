using BepInEx;
using BepInEx.Logging;
using EFT;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace ftomove
{
    [BepInPlugin("com.alimoncul.ftomove", "ftomove", "1.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal GameObject moverObject;

        protected void Awake()
        {
            new NewGamePatch().Enable();
            Logger = base.Logger;
            Logger.LogInfo("com.alimoncul.ftomove loaded");

        }
    }
    internal class NewGamePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));

        [PatchPrefix]
        public static void PatchPrefix()
        {
            Mover.Enable();
        }
    }
}
