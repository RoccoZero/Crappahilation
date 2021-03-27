using Ensage;
using SharpDX;

namespace TechiesCrappahilationPaid.BombsType.DrawBehaviour
{
    public class CanDrawRange : DrawBombRangeHelper
    {
        public CanDrawRange(Unit me) : base(me)
        {
        }

        public CanDrawRange(Unit me, float range, Color clr) : base(me, range, clr)
        {
            Particle.DrawRange(Me, $"{Me.Handle}Range", range, clr);
        }

        public override void Draw(float range, Color clr)
        {
            Particle.DrawRange(Me, $"{Me.Handle}Range", range, clr);
        }
    }
}