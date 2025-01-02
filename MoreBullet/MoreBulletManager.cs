using GlobalBullet;
using LobotomyBaseModLib;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace MoreBullet
{
    public class MoreBulletManager
    {
        private static MoreBulletManager _instance;
        public static MoreBulletManager Instance
        {
            get
            {
                if (_instance == null) _instance = new MoreBulletManager();
                return _instance;
            }
        }

        private GameObject BulletChangeButton;
        public int BulletModeId { get; private set; }
        public List<BulletDataBase> MoreBullets { get; private set; }
        public BulletDataBase CurrentBullet => MoreBullets[BulletModeId];

        private MoreBulletManager()
        {
            BulletModeId = 0;
            MoreBullets = new List<BulletDataBase>();
        }

        public void LoadMoreBullets()
        {
            MoreBullets = new List<BulletDataBase>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(File.ReadAllText(Harmony_Patch.ConfigPath));
            XmlNode root = document.SelectSingleNode("MoreBullet");
            if (root == null) return;
            XmlNode names = root.SelectSingleNode("UseBulletsName");
            if (names != null)
            {
                foreach (XmlNode name in names.ChildNodes)
                    MoreBullets.AddRange(BulletDataBase.AllMoreBullets.FindAll(x => x.Name == name.InnerText));
                Harmony_Patch.LogWrite($"MoreBullets Count is {MoreBullets.Count}");
            }
        }

        public void SetBulletMode(int id)
        {
            if (MoreBullets.Count == 0) MoreBullets = new List<BulletDataBase>() { VanillaBullet.Instance };
            BulletModeId = id % MoreBullets.Count;
            List<GlobalBulletUISlot> slots = GlobalBulletWindow.CurrentWindow.slots;
            for (int i = 0; i < slots.Count; i++)
            {
                GlobalBulletUISlot slot = slots[i];
                if (ActiveBullet(slot.SlotType))
                {
                    slot.SetAcitve(false);
                    continue;
                }
                Image image = slot.GetComponent<Image>();
                image.sprite = CurrentBullet[slot.SlotType].SlotSprite;
                image.rectTransform.sizeDelta = image.sprite.rect.size / 2f;
            }
            GlobalBulletWindow.CurrentWindow.RunMethod("UpdatePointer");
        }

        public void ResetBulletMode() => SetBulletMode(0);

        public void ToggleBulletMode() => SetBulletMode(BulletModeId + 1);

        public void CreateBulletChangeButton()
        {
            if (BulletChangeButton != null) Object.Destroy(BulletChangeButton);
            if (MoreBullets.Count <= 1) return;
            BulletChangeButton = new GameObject("BulletChangeButton");
            BulletChangeButton.transform.SetParent(GlobalBulletWindow.CurrentWindow.slots[0].transform.parent);
            Image image = BulletChangeButton.AddComponent<Image>();
            image.sprite = Harmony_Patch.GetSprite("BulletChangeButton.png", out Texture2D texture);
            image.rectTransform.sizeDelta = new Vector2(texture.width, texture.height) / 2f;
            image.color = GlobalBulletWindow.CurrentWindow.OrangeColor;
            Button button = BulletChangeButton.AddComponent<Button>();
            button.interactable = true;
            button.targetGraphic = image;
            button.onClick.AddListener(ToggleBulletMode);
            BulletChangeButton.SetActive(true);
            BulletChangeButton.layer = 999;
            BulletChangeButton.transform.position = GlobalBulletWindow.CurrentWindow.slots[7].transform.position + new Vector3(0f, 100f);
            BulletChangeButton.transform.localScale = Vector3.one;
            BulletChangeButton.transform.localRotation = Quaternion.identity;
        }

        public bool ActivateBullet(GlobalBulletManager manager, GlobalBulletType type, List<UnitModel> targets)
        {
            if (manager.currentBullet <= 0)
            {
                CursorManager.instance.CannotAnim();
                SoundEffectPlayer player1 = SoundEffectPlayer.PlayOnce("Bullet/Bullet_Empty", Vector2.zero);
                player1?.AttachToCamera();
                return false;
            }
            SoundEffectPlayer player2 = SoundEffectPlayer.PlayOnce("Bullet/Bullet_Fire", Vector2.zero);
            player2?.AttachToCamera();
            BulletData data = CurrentBullet[type];
            data.Sound();
            foreach (UnitModel target in targets)
                data.Bullet(target);
            manager.currentBullet--;
            manager.RunMethod("UpdateUI");
            return true;
        }

        public void UpdateSniping(GlobalBulletWindow window)
        {
            BulletData data = CurrentBullet[window.CurrentSelectedBullet];
            List<UnitModel> list = new List<UnitModel>();
            Vector2 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (UnitMouseEventManager.instance.isPointerEntered)
            {
                Collider2D[] array = Physics2D.OverlapPointAll(vector);
                foreach (Collider2D collider2D in array)
                {
                    UnitMouseEventTarget component = collider2D.GetComponent<UnitMouseEventTarget>();
                    if (component == null) continue;
                    if (data.TargetWorker && component.GetCommandTargetModel() is WorkerModel worker1) list.Add(worker1);
                    if (data.TargetCreature)
                    {
                        if (component.GetCommandTargetModel() is CreatureModel creature) list.Add(creature);
                        else if (component.GetCommandTargetModel() is WorkerModel worker2 && worker2.IsPanic()) list.Add(worker2);
                    }
                }
            }
            if (list.Count > 0) CursorManager.instance.OnEnteredTarget();
            else CursorManager.instance.OnExitTarget();
            if (Input.GetMouseButtonDown(0) && UnitMouseEventManager.instance.isPointerEntered && GlobalBulletManager.instance.ActivateBullet(window.CurrentSelectedBullet, list))
            {
                data.Effect(vector);
                window.OnShoot();
            }
        }

        public void CursorSet(CursorManager manager, MouseCursorType type)
        {
            if (manager.GetFieldValue<bool>("isLock") || manager.currentType == type) return;
            if ((int)type >= manager.cursorSprite.Count) manager.HideCursor();
            else if (manager.RunMethod<bool>("CheckCursorChangable", type))
            {
                Texture2D texture = manager.cursorSprite[(int)type];
                if (type >= MouseCursorType.BULLET_EXECUTION)
                {
                    manager.cursorMode = CursorMode.ForceSoftware;
                    GlobalBulletType bullet = type != MouseCursorType.BULLET_EXECUTION ? (GlobalBulletType)(type - 9) : GlobalBulletType.EXCUTE;
                    texture = WorkerSpriteManager.ScaleTexture(CurrentBullet[bullet].CursorTexture, (int)manager.BulletCursorSize.x, (int)manager.BulletCursorSize.y);
                }
                else manager.cursorMode = CursorMode.Auto;
                Vector2 hotspot = manager.GetHotspot(type, texture);
                manager.SetFieldValue("_currentHotspot", hotspot);
                manager.SetFieldValue("currentCursorTexture", texture);
                Cursor.SetCursor(texture, hotspot, manager.cursorMode);
                manager.SetFieldValue("_prevCursorType", manager.currentType);
                manager.currentType = type;
            }
        }

        private bool ActiveBullet(GlobalBulletType type)
        {
            if (!CurrentBullet.ContainsKey(type)) return false;
            switch (type)
            {
                case GlobalBulletType.RECOVER_HP: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_HP);
                case GlobalBulletType.RECOVER_MENTAL: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RECOVER_MENTAL);
                case GlobalBulletType.RESIST_R: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_R);
                case GlobalBulletType.RESIST_W: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_W);
                case GlobalBulletType.RESIST_B: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.RESIST_B);
                case GlobalBulletType.RESIST_P: return MissionManager.instance.ExistsFinishedBossMission(SefiraEnum.TIPERERTH1);
                case GlobalBulletType.SLOW: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.SLOW);
                case GlobalBulletType.EXCUTE: return ResearchDataModel.instance.IsUpgradedBullet(GlobalBulletType.EXCUTE);
                default: return false;
            }
        }
    }
}
