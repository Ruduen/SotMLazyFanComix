using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace LazyFanComix.Thri
{
    public class CallTheShotCardController : CardController
    {
        public CallTheShotCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            this.AddAsPowerContributor();
        }

        public override void AddTriggers()
        {
            this.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, (PhaseChangeAction pca) => this.GameController.DestroyCard(this.DecisionMaker, this.Card, true, cardSource: this.GetCardSource()), new TriggerType[] { TriggerType.DestroyCard, TriggerType.DestroySelf });
            this.AddWhenDestroyedTrigger((DestroyCardAction dca) => this.GameController.SelectAndUsePower(this.DecisionMaker, cardSource: this.GetCardSource()), TriggerType.UsePower);
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            if (cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && !cardController.Card.Owner.IsIncapacitated && !cardController.Card.IsFlipped)
            {
                return new Power[] { new Power(cardController.HeroTurnTakerController, cardController, "Your hero deals 1 target 2 projectile damage.", DealDamageResponse(cardController), 0, null, this.GetCardSource()) };
            }
            return null;
        }

        private IEnumerator DealDamageResponse(CardController characterCard)
        {
            int[] powerNumerals = new int[] {
                this.GetPowerNumeral(0, 1),
                this.GetPowerNumeral(1, 2)
            };
            return this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, characterCard.Card), powerNumerals[1], DamageType.Projectile, powerNumerals[0], false, powerNumerals[1], cardSource: this.GetCardSource());
        }
    }
}