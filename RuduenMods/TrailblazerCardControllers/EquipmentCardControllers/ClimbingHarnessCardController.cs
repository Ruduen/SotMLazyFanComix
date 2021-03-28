using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace RuduenWorkshop.Trailblazer
{
    public class ClimbingHarnessCardController : CardController
    {
        private StatusEffect _statusEffect;

        public ClimbingHarnessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, ResponseAction, TriggerType.IncreaseDamage);
        }

        protected IEnumerator ResponseAction(PhaseChangeAction pca)
        {
            IEnumerator coroutine;

            if (_statusEffect == null)
            {
                IncreaseDamageStatusEffect idse;
                // Increase the next damage dealt by Trailblazer by 1.
                idse = new IncreaseDamageStatusEffect(1);
                idse.SourceCriteria.IsSpecificCard = this.DecisionMaker.CharacterCard;
                idse.NumberOfUses = 1;
                _statusEffect = idse;
                _statusEffect.Identifier = "IncreaseNextDamageDealtBy" + this.DecisionMaker.CharacterCard.ToString() + "ThanksTo" + this.Card.Identifier;
            }

            coroutine = this.GameController.AddStatusEffect(_statusEffect, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // If there is a position in play, increase the next damage by an additional 1.
            if (this.FindCardsWhere((Card c) => c.IsPosition && c.IsInPlay).Count() > 0)
            {
                coroutine = this.GameController.AddStatusEffect(_statusEffect, true, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
        }
    }
}