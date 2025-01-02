using GlobalBullet;
using UnityEngine;

namespace MoreBullet
{
    public class VanillaBullet : BulletDataBase
    {
        public static VanillaBullet Instance = new VanillaBullet();

        public VanillaBullet() : base("VanillaBullet")
        {
            BulletDatas.Add(GlobalBulletType.RECOVER_HP, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("RecoverHPBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_HP.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_H.P recovery.png"),
                sound: Heal, effect: (pos) => Effect(GlobalBulletType.RECOVER_HP, pos)
            ));
            BulletDatas.Add(GlobalBulletType.RECOVER_MENTAL, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("RecoverMentalBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_Mental.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_M.P recovery.png"),
                sound: Heal, effect: (pos) => Effect(GlobalBulletType.RECOVER_MENTAL, pos)
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_R, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("ResistRBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_R_Shield.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_Red Shield.png"),
                sound: Shield, effect: (pos) => Effect(GlobalBulletType.RESIST_R, pos)
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_W, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("ResistWBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_W_Shield.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_White Shield.png"),
                sound: Shield, effect: (pos) => Effect(GlobalBulletType.RESIST_W, pos)
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_B, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("ResistBBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_B_Shield.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_Black Shield.png"),
                sound: Shield, effect: (pos) => Effect(GlobalBulletType.RESIST_B, pos)
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_P, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("ResistPBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_P_Shield.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_Pale Shield.png"),
                sound: Shield, effect: (pos) => Effect(GlobalBulletType.RESIST_P, pos)
            ));
            BulletDatas.Add(GlobalBulletType.SLOW, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("SlowBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_SpeedDown.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_Slow.png"), false, true,
                () => SoundEffectPlayer.PlayOnce("Bullet/Bullet_Slow", Vector2.zero),
                (pos) => Effect(GlobalBulletType.SLOW, pos)
            ));
            BulletDatas.Add(GlobalBulletType.EXCUTE, new BulletData(
                (target) => GlobalBulletManager.instance.RunMethod("ExcuteBullet", target),
                Harmony_Patch.GetSprite("VanillaBullet/Bullet_Execution.png"),
                Harmony_Patch.GetTexture2D("VanillaBullet/Aim_Execute.png"),
                sound: () => SoundEffectPlayer.PlayOnce("Bullet/Bullet_Execution", Vector2.zero),
                effect: (pos) => Effect(GlobalBulletType.EXCUTE, pos)
            ));
        }

        public void Heal() => SoundEffectPlayer.PlayOnce("Bullet/Bullet_Heal", Vector2.zero);
        public void Shield() => SoundEffectPlayer.PlayOnce("Bullet/Bullet_Shield", Vector2.zero);
        public void Effect(GlobalBulletType type, Vector2 pos) => GlobalBulletEffect.GenEffect(type, pos);
    }
}
