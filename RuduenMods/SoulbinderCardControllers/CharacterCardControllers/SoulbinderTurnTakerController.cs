﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuduenWorkshop.Soulbinder
{
    public class SoulbinderTurnTakerController : HeroTurnTakerController
    {
        private CharacterCardController _instructions = null;
        private Boolean _initializeDone = false;

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

            _initializeDone = true;

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

            // Shared soulbinder logic - Move one of the three into play.
            coroutine = this.GameController.SelectCardFromLocationAndMoveIt(this, this.HeroTurnTaker.OffToTheSide, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && ShardIdentifiers.Contains(c.Identifier), "soulshard"), new List<MoveCardDestination> { new MoveCardDestination(this.TurnTaker.PlayArea) }, true, cardSource: new CardSource(InstructionsCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // For remaining cards, incapacitate and move into play.
            IEnumerable<CardController> ssCardControllers = this.GameController.FindCardControllersWhere((Card c) => c.Owner == this.TurnTaker && c.Location == this.HeroTurnTaker.OffToTheSide && ShardIdentifiers.Contains(c.Identifier));

            // Move remaining cards under the instruction card.
            coroutine = this.GameController.MoveCards(this, ssCardControllers.Select((CardController cc) => cc.Card), InstructionsCardController.Card.UnderLocation, cardSource: new CardSource(InstructionsCardController));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        public override Card CharacterCard
        {
            get
            {
                if (_initializeDone)
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

        public override CharacterCardController IncapacitationCardController
        {
            get
            {
                return InstructionsCardController;
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