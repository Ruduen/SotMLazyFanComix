//using Handelabra.Sentinels.Engine.Controller;
//using Handelabra.Sentinels.Engine.Model;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;

//namespace LazyFanComix.Vagabond
//{
//    public class SerpentsBiteCardController : SharedSoloCardController
//    {
//        public SerpentsBiteCardController(Card card, TurnTakerController turnTakerController)
//            : base(card, turnTakerController)
//        {
//        }

//        public override IEnumerator Play()
//        {
//    //        {
//    //            "identifier": "SerpentsBite",
//    //  "count": 4,
//    //  "title": "Serpent's Bite'",
//    //  "keywords": [ "equipment", "limited" ],
//    //  "powers": "{Vagabond} deals 1 target 1 melee damage, 1 toxic damage, and 1 infernal damage.",
//    //  "body": [ "When this card enters play, if {Vagabond} is the only active hero, increase all damage dealt by {Vagabond} by 1 until the start of your next turn." ],
//    //  "icons": [ "IncreaseDamageDealt", "HasPower", "DealDamageMelee" ],
//    //}

//            IEnumerator coroutine;
//            if (this.IsSolo())
//            {
//                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
//                idse.SourceCriteria.IsSpecificCard = this.CharacterCard;
//                idse.UntilStartOfNextTurn(this.TurnTaker);
//                idse.UntilCardLeavesPlay(this.CharacterCard);
//                return this.AddStatusEffect(idse);
//            }
//            else
//            {
//                return this.GameController.SendMessageAction(this.CharacterCard.AlternateTitleOrTitle + " is not the only active Hero, so damage will not be increased.", Priority.Low, this.GetCardSource());
//            }
//        }

//        public override IEnumerator UsePower(int index = 0)
//        {
//            IEnumerator coroutine;
//            int[] powerNumerals = new int[] {
//                this.GetPowerNumeral(0, 1),
//                this.GetPowerNumeral(1, 1),
//                this.GetPowerNumeral(2, 1),
//                this.GetPowerNumeral(2, 1)
//            };

//            List<DealDamageAction> ddas = new List<DealDamageAction> {
//                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.CharacterCard), null, powerNumerals[1], DamageType.Melee),
//                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.CharacterCard), null, powerNumerals[2], DamageType.Toxic),
//                new DealDamageAction(this.GetCardSource(), new DamageSource(this.GameController, this.CharacterCard), null, powerNumerals[3], DamageType.Infernal),
//            };

//            coroutine = this.SelectTargetsAndDealMultipleInstancesOfDamage(ddas, null, null, powerNumerals[0], powerNumerals[0], false, null, null, true, false, null);
//            if (this.UseUnityCoroutines) { yield return this.GameController.StartCoroutine(coroutine); } else { this.GameController.ExhaustCoroutine(coroutine); }

//        }

//    }
//}