using GlobalBullet;

namespace MoreBullet
{
    public class MoreBullet : BulletDataBase
    {
        public static MoreBullet Instance = new MoreBullet();

        public MoreBullet() : base("MoreBullet")
        {
            BulletDatas.Add(GlobalBulletType.RECOVER_HP, new BulletData(
                HPDamgeBullet, Harmony_Patch.GetSprite("MoreBullet/Bullet_HP_Damage.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_H.P recovery.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.RECOVER_MENTAL, new BulletData(
                MPDamgeBullet, Harmony_Patch.GetSprite("MoreBullet/Bullet_Mental_Damage.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_M.P recovery.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_R, new BulletData(
                (target) => ChangeDamageBullet(target, RwbpType.R),
                Harmony_Patch.GetSprite("MoreBullet/Bullet_Change_R.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Change Red.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_W, new BulletData(
                (target) => ChangeDamageBullet(target, RwbpType.W),
                Harmony_Patch.GetSprite("MoreBullet/Bullet_Change_W.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Change White.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_B, new BulletData(
                (target) => ChangeDamageBullet(target, RwbpType.B),
                Harmony_Patch.GetSprite("MoreBullet/Bullet_Change_B.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Change Black.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.RESIST_P, new BulletData(
                (target) => ChangeDamageBullet(target, RwbpType.P),
                Harmony_Patch.GetSprite("MoreBullet/Bullet_Change_P.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Change Pale.png"), true, true
            ));
            BulletDatas.Add(GlobalBulletType.SLOW, new BulletData(
                SpeedUpBullet,
                Harmony_Patch.GetSprite("MoreBullet/Bullet_SpeedUp.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Spped.png")
            ));
            BulletDatas.Add(GlobalBulletType.EXCUTE, new BulletData(
                RecoveryBullet,
                Harmony_Patch.GetSprite("MoreBullet/Bullet_Recovery.png"),
                Harmony_Patch.GetTexture2D("MoreBullet/Aim_Recovery.png")
            ));
        }

        public static void HPDamgeBullet(UnitModel target)
        {
            int num = 25;
            if (ResearchDataModel.instance.IsUpgradedAbility("upgrade_recover_bullet")) num += 15;
            target.TakeDamage(new DamageInfo(RwbpType.R, num - 5, num));
        }

        public static void MPDamgeBullet(UnitModel target)
        {
            int num = 25;
            if (ResearchDataModel.instance.IsUpgradedAbility("upgrade_recover_bullet")) num += 15;
            target.TakeDamage(new DamageInfo(RwbpType.W, num - 5, num));
        }

        public static void ChangeDamageBullet(UnitModel target, RwbpType type)
        {
            target.AddUnitBuf(new MoreBulletBufs.ChangeDamgeBuf(type));
        }

        public static void SpeedUpBullet(UnitModel target) => target.AddUnitBuf(new MoreBulletBufs.SppedUpBuf(10f));

        public static void RecoveryBullet(UnitModel target)
        {
            if (target is WorkerModel worker)
            {
                int num = 10;
                if (ResearchDataModel.instance.IsUpgradedAbility("upgrade_recover_bullet")) num += 10;
                if (!worker.HasUnitBuf(UnitBufType.QUEENBEE_SPORE)) worker.RecoverHP(num);
                if (!worker.IsPanic()) worker.RecoverMental(num);
            }
        }
    }
}
