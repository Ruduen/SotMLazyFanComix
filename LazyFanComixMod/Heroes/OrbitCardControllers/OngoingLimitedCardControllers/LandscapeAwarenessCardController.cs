using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class LandscapeAwarenessCardController : CardController
    {
        public LandscapeAwarenessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource?.Card == this.CharacterCard && dd.Target.IsHero);
            this.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource?.Card == this.CharacterCard && dd.Target.IsHero,
                (DealDamageAction dd) => this.GameController.MakeDamageUnincreasable(dd, base.GetCardSource()), TriggerType.MakeDamageUnincreasable, TriggerTiming.Before);
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = this.GetPowerNumeral(0, 2);
            return this.GameController.DrawCards(this.DecisionMaker, powerNumeral, false, cardSource: this.GetCardSource());
        }

    }
}