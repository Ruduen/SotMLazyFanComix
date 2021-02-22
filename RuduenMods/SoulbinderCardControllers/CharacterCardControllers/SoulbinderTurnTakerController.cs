using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderTurnTakerController : HeroTurnTakerController
    {
        private CharacterCardController _instructions = null;
        private CharacterCardController InstructionsCardController
        {
            get
            {
                if (this._instructions == null)
                {
                    Card card = this.TurnTaker.FindCard("SoulbinderCharacter", false);
                    if (card != null)
                    {
                        this._instructions = (CharacterCardController)this.FindCardController(card);
                    }
                }
                return this._instructions;
            }
        }
        private string[] ShardIdentifiers { get { return new string[] { "SoulshardOfLightningCharacter", "SoulshardOfMercuryCharacter", "SoulshardOfIronCharacter" }; } }

        public SoulbinderTurnTakerController(TurnTaker tt, GameController gc) : base(tt, gc)
        {
        }

        public override IEnumerator StartGame()
        {

            IEnumerator coroutine;

            // Base character card means no promo identifier. 

            IEnumerable<Card> heroCards = this.GameController.FindCardsWhere((Card c) => c.Owner == this.TurnTaker && c.Identifier == "SoulbinderMortalFormCharacter");

            if (this.TurnTaker.PromoIdentifier == "SoulbinderMortalCharacter")
            {
                coroutine = this.GameController.MoveCards(this, heroCards, this.TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: new CardSource(InstructionsCardController));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }
            else
            {
                coroutine = this.GameController.MoveCards(this, heroCards, this.TurnTaker.OutOfGame, isPutIntoPlay: true, cardSource: new CardSource(InstructionsCardController));
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // TODO: Verify if other promo identifiers cause issues. 

            // Shared soulbinder logic - Move one of the three into play.
            coroutine = this.GameController.SelectCardFromLocationAndMoveIt(this, this.HeroTurnTaker.OffToTheSide, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && ShardIdentifiers.Contains(c.Identifier), "soulshard"), new List<MoveCardDestination> { new MoveCardDestination(this.TurnTaker.PlayArea) }, true, cardSource: new CardSource(InstructionsCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // For remaining cards, incapacitate and move into play. 
            IEnumerable<CardController> ssCardControllers = this.GameController.FindCardControllersWhere((Card c) => c.Owner == this.TurnTaker && c.Location == this.HeroTurnTaker.OffToTheSide && ShardIdentifiers.Contains(c.Identifier));

            // Incapacitate and move remaining cards. 
            coroutine = this.GameController.FlipCards(ssCardControllers, cardSource: new CardSource(InstructionsCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.GameController.MoveCards(this, ssCardControllers.Select((CardController cc) => cc.Card), this.TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: new CardSource(InstructionsCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


        }
        public override Card CharacterCard
        {
            get
            {
                if (this.HasMultipleCharacterCards)
                {
                    Log.Warning("There was a request for Soulbinder's character card, which is null.");
                    return null;
                }
                else
                {
                    Log.Warning("There was a request for Soulbinder's character card prior to Setup. Returning the instructions card to avoid null reference errors.");
                    return base.CharacterCard;
                }
            }
        }

        public override bool IsIncapacitated
        {
            get
            {
                if (this.TurnTaker.Identifier == "Soulbinder")
                {
                    return this.FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c.IsActive && c.IsInPlayAndHasGameText).Count() == 0 || this.IncapacitationCardController.Card.IsFlipped;
                }
                return base.IsIncapacitated;
            }
        }
        public override bool IsIncapacitatedOrOutOfGame
        {
            get
            {
                return this.IsIncapacitated || this.IncapacitationCardController.Card.IsOutOfGame;
            }
        }
    }
}