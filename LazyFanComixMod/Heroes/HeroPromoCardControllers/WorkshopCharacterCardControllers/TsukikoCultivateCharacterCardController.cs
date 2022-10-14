//using Handelabra.Sentinels.Engine.Controller;
//using Handelabra.Sentinels.Engine.Model;
//using LazyFanComix.HeroPromos;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using SpookyGhostwriter.Tsukiko;

//namespace LazyFanComix.Tsukiko
//{
//    public class TsukikoCultivateCharacterCardController : PromoDefaultCharacterCardController
//    {
//        public TsukikoCultivateCharacterCardController(Card card, TurnTakerController turnTakerController)
//            : base(card, turnTakerController)
//        {
//        }

//        public override IEnumerator UsePower(int index = 0)
//        {
//            List<Function> list = new List<Function>();
//            int powerNum = this.GetPowerNumeral(0, 5); // Amount of health.

//            return PutTopCardFaceDown(powerNum);
            
            
//        }

//        private IEnumerator PutTopCardFaceDown(int powerNum)
//        {
//            IEnumerator coroutine;
//            // Reset deck if necessary. 
//            if (this.HeroTurnTaker.Deck.IsEmpty)
//            {
//                if (this.HeroTurnTaker.Trash.IsEmpty)
//                {
//                    coroutine = this.GameController.SendMessageAction(this.DecisionMaker.Name + " has no cards in their deck or trash, so no card can be put into play face-down.", Priority.Low, this.GetCardSource());
//                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
//                }
//                else
//                {
//                    coroutine = this.GameController.ShuffleTrashIntoDeck(this.DecisionMaker, cardSource: this.GetCardSource());
//                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
//                }
//            }

//            // Attempt to move top card in with appropriate flip. 
//            Card cardToPlay = this.HeroTurnTaker.Deck.TopCard;
//            if (cardToPlay != null)
//            {

//                // Select a suit. 
//                List<SelectWordDecision> swdResults = new List<SelectWordDecision>();
//                coroutine = base.GameController.SelectWord(this.DecisionMaker, TsukikoCardController.allSuits.ToArray(), SelectionType.NaturalistForm, swdResults, false, null, base.GetCardSource(null));
//                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

//                // Manipulate top card.

//                //if (cardToPlay.IsMissionCard)
//                //{
//                //    coroutine = this.GameController.SendMessageAction("The top card of the deck is a mission, so it will be played.", Priority.Low, this.GetCardSource());
//                //    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
//                //}
//                //else
//                //{
//                //    coroutine = this.GameController.FlipCard(this.FindCardController(cardToPlay), cardSource: this.GetCardSource());
//                //    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
//                //}

//                List<MoveCardAction> mcaResults = new List<MoveCardAction>();
//                coroutine = this.GameController.MoveCard(this.DecisionMaker, cardToPlay, this.HeroTurnTaker.PlayArea, false, false, cardToPlay.IsMissionCard, responsibleTurnTaker: this.TurnTaker, storedResults: mcaResults, cardSource: this.GetCardSource());
//                if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }


//                // If card is played, set up 'while in play' status effect for health and suit. 
//                if (mcaResults.Any((MoveCardAction mca) => mca.WasCardMoved))
//                {
//                    MakeTargetStatusEffect mtse = new MakeTargetStatusEffect(powerNum, false);
//                    mtse.CardsToMakeTargets.IsSpecificCard = cardToPlay;
//                    mtse.UntilCardLeavesPlay(cardToPlay);
//                    coroutine = this.AddStatusEffect(mtse, true);
//                    if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

//                    string selectedWord = this.GetSelectedWord(swdResults);
//                    if (selectedWord != null)
//                    {
//                        ActivateEffectStatusEffect aese = new ActivateEffectStatusEffect(this.TurnTaker, this.Card, selectedWord);
//                        aese.UntilCardLeavesPlay(cardToPlay);
//                        coroutine = this.AddStatusEffect(aese, true);
//                        if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }
//                    }
//                }
//            }
//        }
//    }
//}