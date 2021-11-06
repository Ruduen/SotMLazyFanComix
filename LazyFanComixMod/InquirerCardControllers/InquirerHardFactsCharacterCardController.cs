﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.Inquirer
{
    public class InquirerHardFactsCharacterCardController : HeroCharacterCardController
    {
        private readonly List<Card> actedDistortions;

        public InquirerHardFactsCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.actedDistortions = new List<Card>();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<int> numerals = new List<int>(){
                            this.GetPowerNumeral(0, 3),  // Max HP
                            this.GetPowerNumeral(1, 1),  // Number of Targets
                            this.GetPowerNumeral(2, 1)   // Damage.
            };

            // You may play an ongoing.
            IEnumerator coroutine;
            coroutine = this.SelectAndPlayCardsFromHand(this.HeroTurnTakerController, 1, false, 0, new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing", true));
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            // Make distortions HP targets. (Note that this does not apply to pre-existing targets - this is a quirk of how the engine currently applies those effects.)
            MakeTargetStatusEffect makeTargetStatusEffect = new MakeTargetStatusEffect(numerals[0], false);
            makeTargetStatusEffect.CardsToMakeTargets.HasAnyOfTheseKeywords = new List<string>() { "distortion" };
            makeTargetStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
            coroutine = this.AddStatusEffect(makeTargetStatusEffect, true);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            SelectCardsDecision selectCardsDecision = new SelectCardsDecision(this.GameController, this.HeroTurnTakerController, (Card c) => c.IsInPlay && c.IsDistortion, SelectionType.CardToDealDamage, null, false, null, true, true, false, new Func<int>(this.NumDistortionsToDamage), null, null, null, this.GetCardSource(null));
            coroutine = this.GameController.SelectCardsAndDoAction(selectCardsDecision, (SelectCardDecision sc) => this.DistortionDamageResponse(sc, numerals[1], numerals[2]), null, null, this.GetCardSource(null), null, false, null);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            this.actedDistortions.Clear();
        }

        private int NumDistortionsToDamage()
        {
            if (!this.Card.IsIncapacitatedOrOutOfGame)
            {
                int num = this.GameController.FindCardsWhere((Card c) => c.IsDistortion && c.IsInPlay).Except(this.actedDistortions).Count<Card>();
                return this.actedDistortions.Count<Card>() + num;
            }
            return 0;
        }

        private IEnumerator DistortionDamageResponse(SelectCardDecision sc, int numberOfTargets, int damageAmount)
        {
            Card selectedCard = sc.SelectedCard;
            this.actedDistortions.Add(selectedCard);
            IEnumerator coroutine = this.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, new DamageSource(this.GameController, selectedCard), (Card c) => new int?(damageAmount), DamageType.Psychic, () => numberOfTargets, false, new int?(numberOfTargets), cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // TODO: Replace with something more unique!
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        coroutine = this.SelectHeroToPlayCard(this.HeroTurnTakerController);
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 1:
                    {
                        coroutine = base.GameController.SelectHeroToUsePower(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
                case 2:
                    {
                        coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, cardSource: this.GetCardSource());
                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                        break;
                    }
            }
            yield break;
        }
    }
}