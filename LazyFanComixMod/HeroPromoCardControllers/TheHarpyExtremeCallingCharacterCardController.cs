using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using LazyFanComix.HeroPromos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LazyFanComix.TheHarpy
{
    public class TheHarpyExtremeCallingCharacterCardController : PromoDefaultCharacterCardController
    {
        public TheHarpyExtremeCallingCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
            TokenPool arcanaPool = this.Card.FindTokenPool(TokenPool.ArcanaControlPool);
            TokenPool avianPool = this.Card.FindTokenPool(TokenPool.AvianControlPool);
            this.SpecialStringMaker.ShowSpecialString(() => string.Format("{0} has {1} {2} and {3} {4} control tokens.",
                new object[] { this.Card.AlternateTitleOrTitle, arcanaPool.CurrentValue, "{arcana}", avianPool.CurrentValue, "{avian}" }),
                null, null);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            int powerNumeral = GetPowerNumeral(0, 1);

            // Draw a card.
            coroutine = this.DrawCards(this.DecisionMaker, 1);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

            coroutine = this.FlipAllControlTokenAndDamage(powerNumeral);
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }

        // Token: 0x060015E4 RID: 5604 RVA: 0x0003C07C File Offset: 0x0003A27C
        public IEnumerator FlipAllControlTokenAndDamage(int powerNumeral)
        {
            IEnumerator coroutine;

            List<SelectWordDecision> storedResultsWord = new List<SelectWordDecision>();
            List<FlipTokensAction> storedResultsToken = new List<FlipTokensAction>();
            TokenPool avianPool = this.Card.FindTokenPool(TokenPool.AvianControlPool);
            TokenPool arcanaPool = this.Card.FindTokenPool(TokenPool.ArcanaControlPool);
            int arcanaToFlip;
            int avianToFlip;
            int tokensFlipped = 0;

            if (avianPool == null || arcanaPool == null || !this.TurnTaker.IsHero)
            {
                TurnTaker turnTaker = this.FindTurnTakersWhere((TurnTaker tt) => tt.Identifier == "TheHarpy", false).FirstOrDefault<TurnTaker>();
                if (turnTaker != null)
                {
                    avianPool = turnTaker.CharacterCard.FindTokenPool(TokenPool.AvianControlPool);
                    arcanaPool = turnTaker.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool);
                }
            }
            if (avianPool != null && arcanaPool != null)
            {
                arcanaToFlip = (arcanaPool.CurrentValue > powerNumeral) ? (arcanaPool.CurrentValue) - powerNumeral : 0;
                avianToFlip = (avianPool.CurrentValue > powerNumeral) ? (avianPool.CurrentValue) - powerNumeral : 0;
                string[] words = new string[]
                        {
                            "Flip " + arcanaToFlip + " {arcana}",
                            "Flip " + avianToFlip + " {avian}"
                        };

                coroutine = this.GameController.SelectWord(this.DecisionMaker, words, SelectionType.HarpyTokenType, storedResultsWord, false, null, this.GetCardSource());
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

                string text = this.GetSelectedWord(storedResultsWord);

                if (text != null)
                {
                    TokenPool originPool;
                    TokenPool destinationPool;
                    if (text == words[0])
                    {
                        originPool = arcanaPool;
                        destinationPool = avianPool;
                        tokensFlipped = arcanaToFlip;
                    }
                    else
                    {
                        originPool = avianPool;
                        destinationPool = arcanaPool;
                        tokensFlipped = avianToFlip;
                    }
                    coroutine = this.FlipTokens(originPool, destinationPool, tokensFlipped, storedResultsToken);
                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
                }
            }
            else
            {
                coroutine = this.GameController.SendMessageAction("There are no control tokens in play.", Priority.Medium, this.GetCardSource(), null, true);
                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
            }

            // Deal self damage.
            coroutine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard), this.CharacterCard, storedResultsToken.Count(), DamageType.Psychic, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
        }
    }
}