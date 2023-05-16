using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Thri
{
    public class LeadTheTargetCardController : CardController
    {
        public LeadTheTargetCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            this.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.HeroUsingPower == this.HeroTurnTakerController, MoveCardResponse, TriggerType.MoveCard, TriggerTiming.Before);
            this.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.DamageSource.IsHeroCharacterCard && dda.DamageSource.Card != this.CharacterCard && !dda.Target.IsHero && this.Card.UnderLocation.Cards.Select((Card c) => c.Owner).Contains(dda.DamageSource.Owner), DestroyCardsToIncreaseDamageResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.IncreaseDamage }, TriggerTiming.Before);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int[] powerNumerals = new int[]
            {
                this.GetPowerNumeral(0, 1)
            };
            return this.GameController.SelectAndUsePower(this.DecisionMaker, true, numberOfPowers: powerNumerals[0], cardSource: this.GetCardSource());
        }

        private IEnumerator MoveCardResponse(UsePowerAction upa)
        {
            IEnumerator coroutine;

            //coroutine = this.GameController.SelectLocationsAndDoAction(this.DecisionMaker,SelectionType.MoveCardToUnderCard, (Location l)=>l.IsHero && l.IsDeck,(Location l)=>this.GameController.MoveCards(this.DecisionMaker, l.BottomCard, this.Card.UnderLocation,playIfMovingToPlayArea: false, cardSource: this.GetCardSource()),requiredDecisions: )
            // TODO
            IEnumerable<LocationChoice> decks = this.FindLocationsWhere((Location l) => l.IsDeck && l.IsHero && l.OwnerTurnTaker != this.TurnTaker && l.BottomCard != null).Select((Location l) => new LocationChoice(l, null, true));
            if (decks.Count() > 0)
            {
                return coroutine = this.GameController.SelectLocationAndDoAction(
                    new SelectLocationDecision(this.GameController, this.DecisionMaker, decks, SelectionType.MoveUnderThisCard, true, cardSource: this.GetCardSource()),
                    (Location l) => this.GameController.MoveCard(this.DecisionMaker, l.BottomCard, this.Card.UnderLocation, playCardIfMovingToPlayArea: false, showMessage: true, cardSource: this.GetCardSource())
                );
            }
            return this.GameController.SendMessageAction("There are no Hero decks with cards to move.", Priority.Low, this.GetCardSource(), showCardSource: true);
        }

        private IEnumerator DestroyCardsToIncreaseDamageResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            int count;
            List<DestroyCardAction> dcas = new List<DestroyCardAction>();

            coroutine = this.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.Card.UnderLocation && c.Owner == dda.DamageSource.Owner), storedResults: dcas, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            count = dcas.Where((DestroyCardAction dca) => dca.WasCardDestroyed).Count();

            coroutine = this.GameController.IncreaseDamage(dda, count, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}