using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace RuduenWorkshop.Trailblazer
{
    public class ClimbingHarnessCardController : TrailblazerOnPlayPositionCardController
    {
        public ClimbingHarnessCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        protected override TriggerType ResponseTriggerType()
        {
            return TriggerType.IncreaseDamage;
        }

        protected override IEnumerator ResponseAction(CardEntersPlayAction cepa)
        {
            // Increase the next damage dealt by Trailblazer by 1. 
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(2);
            StatusEffect se = idse;
            se.Identifier = "IncreaseNextDamageDealtBy" + this.DecisionMaker.CharacterCard.ToString() + "ThanksTo" + this.Card.Identifier;
            idse.SourceCriteria.IsSpecificCard = this.DecisionMaker.CharacterCard;
            idse.NumberOfUses = 1;
            IEnumerator coroutine = this.GameController.AddStatusEffect(se, true, this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}