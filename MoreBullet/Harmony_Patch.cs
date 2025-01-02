using GlobalBullet;
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace MoreBullet
{
    public class Harmony_Patch
    {
        public static string DirectoryPath;
        public static string ConfigPath => $"{DirectoryPath}/Config.xml";
        public static string LogPath => $"{DirectoryPath}/Log.txt";
        public static string ImagePath => $"{DirectoryPath}/Image";

        public Harmony_Patch()
        {
            DirectoryPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                File.AppendAllText($"{Application.dataPath}/BaseMods/Error.txt", $"[MoreBullet] Path could not be found!\n");
                return;
            }
            File.WriteAllText(LogPath, "");

            HarmonyInstance instance = HarmonyInstance.Create("Lobotomy.Glaceon471.MoreBullet");
            Type patchs = typeof(Harmony_Patch);
            instance.Patch(typeof(GameManager).GetMethod("StartGame", AccessTools.all), null, new HarmonyMethod(patchs.GetMethod("GameManagerStartGamePostfix", AccessTools.all)));
            instance.Patch(typeof(GlobalBulletManager).GetMethod("ActivateBullet", AccessTools.all), new HarmonyMethod(patchs.GetMethod("GlobalBulletManagerActivateBulletPrefix", AccessTools.all)), null);
            instance.Patch(typeof(GlobalBulletWindow).GetMethod("Update", AccessTools.all), null, new HarmonyMethod(patchs.GetMethod("GlobalBulletWindowUpdatePostfix", AccessTools.all)));
            instance.Patch(typeof(GlobalBulletWindow).GetMethod("UpdateSniping", AccessTools.all), new HarmonyMethod(patchs.GetMethod("GlobalBulletWindowUpdateSnipingPrefix", AccessTools.all)), null);
            instance.Patch(typeof(CursorManager).GetMethod("CursorSet", AccessTools.all), new HarmonyMethod(patchs.GetMethod("CursorManagerCursorSetPrefix", AccessTools.all)), null);

            if (!File.Exists(ConfigPath))
            {
                XmlDocument document = new XmlDocument();
                XmlDeclaration child = document.CreateXmlDeclaration("1.0", "UTF-8", null);
                document.AppendChild(child);
                XmlElement element1 = document.CreateElement("MoreBullet");
                document.AppendChild(element1);
                XmlElement element2 = document.CreateElement("UseBulletsName");
                element1.AppendChild(element2);
                XmlElement element3 = document.CreateElement("add");
                element3.InnerText = "VanillaBullet";
                element2.AppendChild(element3);
                XmlComment element4 = document.CreateComment("<add>MoreBullet</add>");
                element2.AppendChild(element4);
                document.Save(ConfigPath);
            }
        }

        public static void GameManagerStartGamePostfix()
        {
            MoreBulletManager manager = MoreBulletManager.Instance;
            manager.LoadMoreBullets();
            manager.ResetBulletMode();
            manager.CreateBulletChangeButton();
        }

        public static bool GlobalBulletManagerActivateBulletPrefix(GlobalBulletManager __instance, ref bool __result, GlobalBulletType type, List<UnitModel> targets)
        {
            __result = MoreBulletManager.Instance.ActivateBullet(__instance, type, targets);
            return false;
        }

        public static void GlobalBulletWindowUpdatePostfix()
        {
            if (GameManager.currentGameManager.state != GameState.STOP && ConsoleScript.instance != null && !ConsoleScript.instance.ConsoleWnd.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Alpha9))
                    MoreBulletManager.Instance.ToggleBulletMode();
            }
        }

        public static bool GlobalBulletWindowUpdatePointerPrefix(GlobalBulletWindow __instance)
        {
            MoreBulletManager.Instance.UpdateSniping(__instance);
            return false;
        }

        public static bool CursorManagerCursorSetPrefix(CursorManager __instance, MouseCursorType type)
        {
            MoreBulletManager.Instance.CursorSet(__instance, type);
            return false;
        }

        public static void LogWrite(string text)
        {
            File.AppendAllText(LogPath, $"{text}\n");
        }

        private static readonly Dictionary<string, Tuple<Sprite, Texture2D>> SpriteCache = new Dictionary<string, Tuple<Sprite, Texture2D>>();
        
        public static Sprite GetSprite(string path) => GetSprite(path, out var _);
        
        public static Texture2D GetTexture2D(string path)
        {
            GetSprite(path, out var texture);
            return texture;
        }
        
        public static Sprite GetSprite(string path, out Texture2D texture)
        {
            if (SpriteCache.TryGetValue(path, out Tuple<Sprite, Texture2D> tuple))
            {
                texture = tuple.Item2;
                return tuple.Item1;
            }
            texture = new Texture2D(2, 2);
            ImageConversion.LoadImage(texture, File.ReadAllBytes($"{ImagePath}/{texture}"));
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            SpriteCache.Add(path, new Tuple<Sprite, Texture2D>(sprite, texture));
            return sprite;
        }
    }
}
