﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Inquirer;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class InquirerTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(InquirerCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Inquirer { get { return FindHero("Inquirer"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Inquirer);
            Assert.IsInstanceOf(typeof(InquirerTurnTakerController), Inquirer);
            Assert.IsInstanceOf(typeof(InquirerCharacterCardController), Inquirer.CharacterCardController);

            Assert.AreEqual(26, Inquirer.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestInnatePower()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");

            StartGame();

            QuickHandStorage(Inquirer.ToHero());
            UsePower(Inquirer.CharacterCard);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestInnateIncap()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Legacy", "Megalopolis");

            StartGame();

            Card card = PlayCard("TheLegacyRing");
            DestroyCard(Inquirer);
            UseIncapacitatedAbility(Inquirer, 2);
            AssertInHand(card); // Returned to hand.
        }

        [Test()]
        public void TestInnateLiesOnLiesPower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Inquirer", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "InquirerCharacter", "InquirerLiesOnLiesCharacter" }
            };
            SetupGameController(setupItems, false, promos);

            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = mdp;

            DealDamage(Inquirer, mdp, 2, DamageType.Melee); // 2 Damage for Setup.
            Card distortion = PlayCard("YoureLookingPale"); // 4 Damage.

            UsePower(Inquirer);

            // Only one card to return, and should destroy the thing, since movement is not destruction.
            AssertInTrash(mdp);
            AssertInPlayArea(baron, distortion); // Distortion handling logic should leave it in play near BB.
        }

        [Test()]
        public void TestInnateHardFactsPower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Inquirer", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "InquirerCharacter", "InquirerHardFactsCharacter" }
            };
            SetupGameController(setupItems, false, promos);

            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card distortion = PutInHand("YoureLookingPale");

            DecisionNextToCard = mdp;
            DecisionSelectCardToPlay = distortion;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);

            UsePower(Inquirer); // 4 damage from play, 1 more from distortion attack.

            QuickHPCheck(-5);
        }

        [Test()]
        public void TestHardFactsInnatePowerImbuedVitalityFirst()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Inquirer", "RealmOfDiscord"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "InquirerCharacter", "InquirerHardFactsCharacter" }
            };
            SetupGameController(setupItems, false, promos);

            StartGame();

            GoToPlayCardPhase(Inquirer);

            PlayCard("ImbuedVitality");

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card distortion = PutInHand("YoureLookingPale");

            DecisionNextToCard = mdp;
            DecisionSelectCards = new List<Card>() { distortion, mdp, distortion, mdp, mdp };

            QuickHPStorage(mdp);

            UsePower(Inquirer); // 4 damage from play, 2 more from 2 distortion attacks.

            QuickHPCheck(-6);
            AssertMaximumHitPoints(distortion, 6); // Ongoing affect re-applies over one-time effect.

            GoToStartOfTurn(Inquirer);
            AssertMaximumHitPoints(distortion, 6); // Return to Imbued Vitality.
        }

        [Test()]
        public void TestHardFactsInnatePowerImbuedVitalitySecond()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Inquirer", "RealmOfDiscord"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "InquirerCharacter", "InquirerHardFactsCharacter" }
            };
            SetupGameController(setupItems, false, promos);

            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card distortion = PutInHand("YoureLookingPale");

            UsePower(Inquirer); // Card played, power used. HP at 3.
            PlayCard("ImbuedVitality"); // Card destroyed, but HP updated to 6.
            PlayCard(distortion); // Replay card.

            AssertMaximumHitPoints(distortion, 6); // This effect came later and should be more relevant.

            DestroyCard("ImbuedVitality"); // Destroy, HP should return to 3.

            AssertNotTarget(distortion); // No longer a target. This isn't great - I'd expect it to go back to 3, but there's little to do outside of debugging Handlabra code.
        }

        [Test()]
        public void TestHardFactsInnatePowerDestroysTarget()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Inquirer", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "InquirerCharacter", "InquirerHardFactsCharacter" }
            };
            SetupGameController(setupItems, false, promos);

            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card distortion = PutInHand("YoureLookingPale");

            DealDamage(Inquirer, mdp, 5, DamageType.Melee);

            DecisionNextToCard = mdp;
            DecisionSelectCardToPlay = distortion;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);

            UsePower(Inquirer); // 4 damage from play, 1 more from distortion attack.

            AssertInTrash(mdp);
        }

        [Test()]
        public void TestTheLieTheyTellThemselves()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();
            GoToUsePowerPhase(Inquirer);

            // Put out example cards.
            PlayCard("TheLieTheyTellThemselves");

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = mdp;

            QuickHPStorage(mdp);
            PlayCard("YoureLookingPale");
            QuickHPCheck(-5); // 4 from self-damage, 1 from additional self-damage.
        }

        [Test()]
        public void TestUndeniableFacts()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();
            GoToUsePowerPhase(Inquirer);

            Card power = PlayCard("UndeniableFacts");

            Card[] cards = new Card[] { PutInHand("YoureLookingPale"), FindCardInPlay("MobileDefensePlatform"), PutInHand("IveFixedTheWound") };

            DecisionSelectCards = cards;

            UsePower(power);
            AssertIsInPlay(cards);

            DestroyCards(cards);

            QuickHandStorage(Inquirer);
            GoToEndOfTurn(Inquirer);
            QuickHandCheck(1); // Draw from no distortions.
        }

        [Test()]
        public void TestPersonas()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            PlayCard("ImAMolePerson");
            PlayCard("ImAVictorian");
            PlayCard("ImANinja");

            DealDamage(Inquirer, Inquirer, 3, DamageType.Melee);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp, Inquirer.CharacterCard);

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            AssertNumberOfCardsInTrash(Inquirer, 0);
            QuickHandStorage(Inquirer);
            GoToEndOfTurn(Inquirer);
            QuickHandCheck(0); // 1 Discarded, 1 Drawn
            AssertNumberOfCardsInTrash(Inquirer, 2); // Discards a card, activate the base power, draw a card, do not destroy to play.
            DiscardTopCards(Inquirer.HeroTurnTaker.Deck, 1); // Discard a third card to allow for a single success and two failures.

            GoToStartOfTurn(Inquirer);
            AssertNumberOfCardsInTrash(Inquirer, 2); // One successful shuffle, two failed shuffles.

            QuickHPCheck(-2, 2); // Two total damage - 1 base, 1 buff. 2 Healing - 1 base, 1 buff.
        }

        [Test()]
        public void TestPersonaVictorianFisticuffCrash()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            DecisionSelectCards = new Card[]{ null };

            PlayCard("ImAVictorian");
            PlayCard("Fisticuffs");
        }

        [Test()]
        public void TestBackupPlan()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);
            Card power = PlayCard("BackupPlan");

            DealDamage(Inquirer, Inquirer, 3, DamageType.Melee);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp, Inquirer.CharacterCard);
            QuickHandStorage(Inquirer);

            DecisionSelectTarget = mdp;
            UsePower(power);

            QuickHPCheck(-1, 1);
            AssertNumberOfCardsInTrash(Inquirer, 1); // Discarded.
            QuickHandCheck(0); // Discard and draw for net 0 change.
        }

        [Test()]
        public void TestYoureLookingPaleInitial()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = mdp;
            QuickHPStorage(mdp);

            PlayCard("YoureLookingPale");
            QuickHPCheck(-4); // 4 damage
        }

        [Test()]
        public void TestYoureLookingPaleAfter()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = mdp;
            QuickHPStorage(mdp);

            PlayCard("YoureLookingPale");

            GoToStartOfTurn(Inquirer);
            QuickHPCheck(-2); // 4 damage, healed 2.
        }

        [Test()]
        public void TestYoureOnOurSideInitial()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card bb = GetCardInPlay("BaronBladeCharacter");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = bb;
            QuickHPStorage(mdp, Inquirer.CharacterCard);

            PlayCard("YoureOnOurSide");
            QuickHPCheck(-2, 0); // 2 damage to others.
        }

        [Test()]
        public void TestYoureOnOurSideAfter()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card bb = GetCardInPlay("BaronBladeCharacter");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = bb;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp, Inquirer.CharacterCard);

            PlayCard("YoureOnOurSide");

            GoToStartOfTurn(Inquirer);
            QuickHPCheck(-2, -1); // 2 damage to others, -1 to Inquirer
        }

        [Test()]
        public void TestIveFixedTheWoundInitial()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            DealDamage(Inquirer, Inquirer, 10, DamageType.Melee);

            GoToPlayCardPhase(Inquirer);

            QuickHPStorage(Inquirer);
            PlayCard("IveFixedTheWound");
            QuickHPCheck(5); // 5 Healing
        }

        [Test()]
        public void TestIveFixedTheWoundAfter()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            DealDamage(Inquirer, Inquirer, 10, DamageType.Melee);

            GoToPlayCardPhase(Inquirer);

            QuickHPStorage(Inquirer);
            PlayCard("IveFixedTheWound");
            GoToStartOfTurn(Inquirer);
            QuickHPCheck(3); // 5 Healing, 2 Damage.
        }

        [Test()]
        public void TestLookADistractionInitial()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card bb = GetCardInPlay("BaronBladeCharacter");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = bb;
            QuickHPStorage(mdp, Inquirer.CharacterCard);

            PlayCard("LookADistraction");
            QuickHPCheck(-3, 0); // 3 damage to others.
        }

        [Test()]
        public void TestLookADistractionAfter()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card bb = GetCardInPlay("BaronBladeCharacter");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionNextToCard = bb;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp, Inquirer.CharacterCard);

            PlayCard("LookADistraction");

            GoToStartOfTurn(Inquirer);
            QuickHPCheck(-3, -1); // 3 damage to others, -1 to Inquirer
        }

        [Test()]
        public void TestUntilYouMakeIt()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            Card persona = GetCard("ImAMolePerson");
            Card safeCard = GetCardWithLittleEffect(Inquirer);

            MoveCard(Inquirer, persona, Inquirer.TurnTaker.Deck, true);
            PutInHand(safeCard);

            QuickHandStorage(Inquirer);

            List<Card> cards = new List<Card>
            {
                persona, // First search for persona.
                safeCard // Then play safe card.
            };
            DecisionSelectCards = ArrangeDecisionCards(cards);

            PlayCard("UntilYouMakeIt");
            QuickHandCheck(0); // Draw 1, Play 1, Net 0.
            AssertNumberOfCardsInPlay(Inquirer, 3); // Should now have character card, new persona, and card in play, since safe cards are preferred.
        }

        [Test()]
        public void TestUntilYouMakeItTiming()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            Card ninja = PutInDeck("ImANinja");
            Card punch = PutInHand("Fisticuffs");
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card discard = PutInHand("BackupPlan");
            DecisionSelectCards = new Card[] { ninja, punch, mdp, discard };

            QuickHPStorage(mdp);
            PlayCard("UntilYouMakeIt");
            QuickHPCheck(-4);
        }

        private IEnumerable<Card> ArrangeDecisionCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                yield return card;
            }
        }

        [Test()]
        public void TestFisticuffs()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Inquirer);

            DealDamage(Inquirer, Inquirer, 3, DamageType.Melee);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp, Inquirer.CharacterCard);
            QuickHandStorage(Inquirer);

            DecisionSelectTarget = mdp;
            PlayCard("Fisticuffs");

            QuickHPCheck(-3, 2);
            AssertNumberOfCardsInTrash(Inquirer, 2); // Discarded and played card.
            QuickHandCheck(-1); // Fisticuffs source is ambiguous, so don't check hand - just a net of -1.
        }

        [Test()]
        public void TestFisticuffsNoHand()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            DiscardAllCards(Inquirer);

            PlayCard("Fisticuffs");
        }


        [Test()]
        public void TestTheRightQuestions()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Inquirer", "Megalopolis");
            StartGame();

            Card ongoing = PlayCard("LivingForceField");
            Card distortion = GetCard("YoureLookingPale");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Inquirer.PutInHand(distortion);

            GoToPlayCardPhase(Inquirer);

            DecisionDestroyCard = ongoing;
            DecisionSelectCardToPlay = distortion;
            DecisionSelectCardsIndex = 4; // Most other decisions are set, but the fourth for the return must be the played distortion.
            DecisionSelectCard = distortion;
            DecisionNextToCard = mdp;

            QuickHPStorage(mdp);

            PlayCard("TheRightQuestions");

            AssertInTrash(ongoing); // Destroyed Ongoing.
            AssertInHand(distortion);  // Played and returned distortion.
            QuickHPCheck(-4); // All damage dealt, no destroy trigger hit.
        }

        
        #region Tribunal

        [Test()]
        public void TestTribunalCalledPower()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            AvailableHeroes = DeckDefinition.AvailableHeroes.Concat(new string[] { "LazyFanComix.Inquirer" });
            SelectFromBoxForNextDecision("LazyFanComix.InquirerCharacter", "LazyFanComix.Inquirer");

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("InquirerCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }

        #endregion Tribunal
    }
}