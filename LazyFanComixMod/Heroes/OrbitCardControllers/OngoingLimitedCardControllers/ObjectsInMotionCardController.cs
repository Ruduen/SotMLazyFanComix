using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace LazyFanComix.Orbit
{
    public class ObjectsInMotionCardController : CardController
    {
        public ObjectsInMotionCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddIncreaseDamageTrigger((DealDamageAction dda) => dda?.DamageSource?.Card != null && dda.DamageSource.Card.IsTarget && dda.DamageSource.Card != this.CharacterCard && this.GameController.ActiveTurnTaker == this.TurnTaker, 1);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            return this.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: this.GetCardSource());
        }

    }
}