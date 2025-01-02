namespace MoreBullet
{
    public static class MoreBulletBufs
    {
        public const UnitBufType ChangeDamgeBufId = (UnitBufType)415321;
        public const UnitBufType SppedUpBufId = (UnitBufType)415322;

        public class ChangeDamgeBuf : UnitBuf
        {
            private readonly float Time;
            private readonly RwbpType DamageType;

            public ChangeDamgeBuf(RwbpType rwbp) : this(15, rwbp)
            { }

            public ChangeDamgeBuf(float time, RwbpType rwbp)
            {
                type = ChangeDamgeBufId;
                duplicateType = BufDuplicateType.ONLY_ONE;
                Time = time;
                DamageType = rwbp;
            }

            public override void Init(UnitModel model)
            {
                base.Init(model);
                remainTime = Time;
            }

            public override bool OnGiveDamage(UnitModel actor, UnitModel target, ref DamageInfo dmg)
            {
                dmg.type = DamageType;
                return true;
            }
        }

        public class SppedUpBuf : UnitBuf
        {
            private readonly float Time;

            public SppedUpBuf(float time)
            {
                type = SppedUpBufId;
                duplicateType = BufDuplicateType.ONLY_ONE;
                Time = time;
            }

            public override void Init(UnitModel model)
            {
                base.Init(model);
                remainTime = Time;
            }

            public override float MovementScale()
            {
                return 1.5f;
            }
        }
    }
}
