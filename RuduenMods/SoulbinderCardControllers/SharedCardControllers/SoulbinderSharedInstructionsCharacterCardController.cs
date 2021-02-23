using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using RuduenWorkshop.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Manually tested!

namespace RuduenWorkshop.Soulbinder
{
    public abstract class SoulbinderSharedInstructionsCharacterCardController : PromoDefaultCharacterCardController
    {
        private string[] ShardIdentifiers { get { return new string[] { "SoulshardOfLightningCharacter", "SoulshardOfMercuryCharacter", "SoulshardOfIronCharacter" }; } }
        public SoulbinderSharedInstructionsCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                // Flip if all chars are flipped. 
                this.AddSideTrigger(
                    this.AddTrigger<GameAction>(
                        (GameAction ga) => (ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !this.Card.IsFlipped && this.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.IsHeroCharacterCard && c.IsActive && c != base.Card, false, null, false).Count() == 0,
                        (GameAction ga) => this.GameController.FlipCard(this.FindCardController(this.Card), cardSource: this.GetCardSource()),
                        TriggerType.FlipCard, TriggerTiming.After
                    )
                );
                // General logic for flipping when incapping a relic. 
                this.AddSideTrigger(
                    this.AddTrigger<FlipCardAction>(
                        (FlipCardAction fca) => fca.CardToFlip.Card.Owner == this.TurnTaker && ShardIdentifiers.Contains(fca.CardToFlip.Card.Identifier) && !fca.CardToFlip.Card.IsFlipped,
                        FlipDifferentCardResponse,
                        new TriggerType[] { TriggerType.FlipCard, TriggerType.GainHP }, TriggerTiming.Before
                    )
                );
            }
            else
            {
                // Restore when unflipping. 
                this.AddSideTriggers(
                    this.AddTargetEntersPlayTrigger(
                        (Card c) => this.Card.IsFlipped && this.CharacterCards.Contains(c),
                        (Card c) => this.GameController.FlipCard(this.FindCardController(this.Card), cardSource: this.GetCardSource()),
                        TriggerType.Hidden, TriggerTiming.After, false, true
                    )
                );
            }
        }

        private IEnumerator FlipDifferentCardResponse(FlipCardAction fca)
        {
            IEnumerator coroutine;

            // Move a card from under this into play.
            coroutine = this.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, this.Card.UnderLocation, 1, 1, new LinqCardCriteria(), new List<MoveCardDestination>() { new MoveCardDestination(this.TurnTaker.PlayArea) }, true, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            this.RemoveAllTriggers();
            this.AddSideTriggers();
            yield break;
        }

    }
}