using GlobalBullet;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreBullet
{
    public abstract class BulletDataBase
    {
        public static List<BulletDataBase> AllMoreBullets = new List<BulletDataBase>();

        public string Name { get; }
        public Dictionary<GlobalBulletType, BulletData> BulletDatas { get; }

        public BulletDataBase(string name)
        {
            Name = name;
            AllMoreBullets.Add(this);
        }

        public BulletDataBase(string name, Dictionary<GlobalBulletType, BulletData> datas) : this(name)
        {
            BulletDatas = datas;
        }

        public bool ContainsKey(GlobalBulletType type) => BulletDatas.ContainsKey(type);

        public BulletData this[GlobalBulletType type] => BulletDatas.TryGetValue(type, out BulletData data) ? data : null;
    }

    public class BulletData
    {
        public Action<UnitModel> Bullet;
        public Sprite SlotSprite;
        public Texture2D CursorTexture;
        public bool TargetWorker;
        public bool TargetCreature;
        public Action Sound;
        public Action<Vector2> Effect;

        /// <param name="bullet">Treatment of the target hit by the bullet</param>
        /// <param name="slot">UI Image</param>
        /// <param name="cursor">Cursor Image</param>
        /// <param name="worker">Include worker in Target</param>
        /// <param name="creature">Include creature in Target</param>
        /// <param name="sound">Sound generated when a bullet hits</param>
        /// <param name="effect">Effect</param>
        public BulletData(Action<UnitModel> bullet, Sprite slot, Texture2D cursor, bool worker = true, bool creature = false, Action sound = null, Action<Vector2> effect = null)
        {
            Bullet = bullet;
            SlotSprite = slot;
            CursorTexture = cursor;
            TargetWorker = worker;
            TargetCreature = creature;
            Sound = sound ?? (() => { });
            Effect = effect ?? ((pos) => { });
        }
    }
}