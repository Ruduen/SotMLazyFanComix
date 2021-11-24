using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.TheTurfWar;
using System.Collections.Generic;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;

namespace LazyFanComixTest
{
    [TestFixture]
    public class TheTurfWarTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(TheTurfWarInstructionsCardController))); // replace with your own namespace
        }

        #region Load Tests


        protected TurnTakerController TurfWar { get { return FindVillain("TheTurfWar"); } }
        protected Card Calin { get { return FindCardInPlay("Calin"); } }
        protected Card Kanya { get { return FindCardInPlay("Kanya"); } }
        protected Card Sez { get { return FindCardInPlay("Sez"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(TurfWar);
            Assert.IsInstanceOf(typeof(TurnTakerController), TurfWar);
            Assert.IsInstanceOf(typeof(TheTurfWarInstructionsCardController), TurfWar.CharacterCardController);
        }

        [Test()]
        public void TestSetupWorks()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();
            AssertIsInPlay("Calin");
            AssertIsInPlay("Kanya");
            AssertIsInPlay("Sez");

            AssertNumberOfCardsInDeck(TurfWar, 20); // Default villain card count. 
        }

        #endregion Load Tests

        #region Victory Tests
        [Test()]
        public void TestPartialVictory()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            DestroyCard(Calin);
            DestroyCard(Kanya);

            GoToStartOfTurn(TurfWar);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test()]
        public void TestTrueVictory()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            DestroyCard(Calin);
            DestroyCard(Kanya);
            DestroyCard(Sez);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }
        #endregion Victory Tests

        #region Instructions Card Test

        [Test()]
        public void TestGrabsEnvironment()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "TheEnclaveOfTheEndlings");

            StartGame();
            Card[] envCards = new Card[] { PlayCard("Bloogo"), PlayCard("Orbo") };
            DestroyCard(envCards[0]);
            AssertUnderCard(Calin, envCards[0]);
            PlayCard("Zenith");
            DestroyCard(envCards[1]);
            AssertUnderCard(Sez, envCards[1]);

            // What happens when you destroy something and it would be re-grabbed? It actually goes away!
            DestroyCard(envCards[1]);
            AssertInTrash(envCards[1]);
        }

        [Test()]
        public void TestFlippedActivation()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(TurfWar);

            GoToStartOfTurn(TurfWar);
        }

        [Test()]
        public void TestFlippedActivationDestroyBasic()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            FlipCard(TurfWar);

            Card[] cards = env.TurnTaker.Deck.Cards.ToArray();

            MoveCard(env, cards[0], Calin.UnderLocation);
            MoveCard(env, cards[1], Kanya.UnderLocation);
            MoveCard(env, cards[2], Kanya.UnderLocation);
            MoveCard(env, cards[3], Kanya.UnderLocation);

            DecisionDestroyCards = new Card[] { cards[0], cards[1], cards[2], null };

            GoToStartOfTurn(TurfWar);

            AssertInTrash(cards[0], cards[1], cards[2]);
            AssertIsInPlay(cards[3]);
        }

        [Test()]
        public void TestFlippedActivationDestroySavedFromDestruction()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "Legacy", "Megalopolis");

            StartGame();

            // TODO: Need to figure out the convoluted circumstance which could have something be 'not destroyed', then be destroyed, then re-played and is valid for destruction. 
            // No, I can't yet think of such a case. Yes, it's a pain.
        }

        [Test()]
        public void TestFlippedActivationFigureheadDamageIsDestroyFailsDiscover()
        {
            /// Scenario where the figurehead is destroyed along the way. 
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Megalopolis");

            StartGame();

            GoToEndOfTurn(env);

            SetHitPoints(Calin, 2);
            PlayCard("DrivingMantis");
            DecisionRedirectTarget = Calin;
            FlipCard(TurfWar);

            GoToStartOfTurn(TurfWar);

            AssertNotInPlay((Card c) => c.IsCultist);
        }


        #endregion Instructions Card Test

        #region Figurehead Tests


        #endregion Figurehead Tests
        [Test()]
        public void TestCardCalinActive()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Sez);
            DestroyCard(Kanya);

            QuickHPStorage(Calin, fixer.CharacterCard);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-2, -2);
        }

        [Test()]
        public void TestCardCalinIncap()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "TheWraith", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Calin);

            AssertNotTarget(Calin);
            GoToEndOfTurn(TurfWar);

            // One end-of-turn card added.
            AssertNumberOfCardsUnderCard(Calin, 1);

            GoToStartOfTurn(TurfWar);

            DecisionSelectTarget = Kanya;
            PlayCard("ThroatJab");
            DecisionSelectTarget = Sez;
            PlayCard("ThroatJab");
            ResetDecisions();

            MoveCards(env, env.TurnTaker.Deck.GetTopCards(2), Calin.UnderLocation);
            GoToEndOfTurn(TurfWar);

            AssertIsTarget(Calin);
            AssertHitPoints(Calin, 15);
            AssertNumberOfCardsUnderCard(Calin, 0);
        }

        [Test()]
        public void TestCardKanyaActive()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Sez);
            DestroyCard(Calin);

            Card otherTarget = PlayCard("Bloogo");
            QuickHPStorage(Kanya, fixer.CharacterCard, otherTarget);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(0, -1, -1);
        }


        [Test()]
        public void TestCardKanyaIncap()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "TheWraith", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Kanya);

            AssertNotTarget(Kanya);
            GoToEndOfTurn(TurfWar);

            // One end-of-turn card added.
            AssertNumberOfCardsUnderCard(Kanya, 1);

            MoveCards(env, env.TurnTaker.Deck.GetTopCards(1), Kanya.UnderLocation);

            GoToPlayCardPhase(TurfWar);

            DecisionSelectTarget = Calin;
            PlayCard("ThroatJab");
            DecisionSelectTarget = Sez;
            PlayCard("ThroatJab");
            ResetDecisions();

            QuickHPStorage(wraith);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-3);
            AssertNumberOfCardsUnderCard(Kanya, 0);

        }


        [Test()]
        public void TestCardSezIncap()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "TheWraith", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Sez);

            AssertNotTarget(Sez);
            GoToEndOfTurn(TurfWar);

            // One end-of-turn card added.
            AssertNumberOfCardsUnderCard(Sez, 1);

            GoToPlayCardPhase(TurfWar);

            DecisionSelectTarget = Calin;
            PlayCard("ThroatJab");
            DecisionSelectTarget = Kanya;
            PlayCard("ThroatJab");
            ResetDecisions();

            Card thug = PlayCard("Zenith");
            DealDamage(thug, thug, 10, DamageType.Fire);

            // Flip to avoid environment destroy trigger.
            FlipCard(TurfWar);

            QuickHPStorage(thug);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(3);
            AssertNumberOfCardsInPlay(env, 1);
            AssertNumberOfCardsUnderCard(Sez, 0);

        }


        [Test()]
        public void TestCardSezActive()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Legacy", "TheWraith", "Guise", "TheEnclaveOfTheEndlings");

            StartGame();

            // Put trigger timing here so we don't have to deal with other active triggers.
            GoToPlayCardPhase(TurfWar);

            DestroyCard(Kanya);
            DestroyCard(Calin);
            DestroyCard(guise);

            Card otherLowTarget = PlayCard("Bloogo");
            Card otherHighTarget = PlayCard("Orbo");
            QuickHPStorage(Sez, fixer.CharacterCard, legacy.CharacterCard, wraith.CharacterCard, otherLowTarget, otherHighTarget);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(0, -3, -3, -3, 0, -3);
        }

        #region Target Tests

        [Test()]
        public void TestCardMoveUnderNotValid()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("Zenith");

            // Target is Active - Put Under.
            GoToEndOfTurn(TurfWar);
            AssertNumberOfCardsAtLocation(Sez.UnderLocation, 1);

            // Target is not Active - Don't Put Under
            DestroyCard(Sez);
            GoToEndOfTurn(TurfWar);

            // Sez will destroy with End of Turn effect - make sure there isn't a new one added!
            AssertNumberOfCardsAtLocation(Sez.UnderLocation, 0);
        }


        [Test()]
        public void TestCardTheLeeches()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card leeches = PlayCard("TheLeeches");

            QuickHPStorage(fixer.CharacterCard, leeches);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-3, -3);
            GoToEndOfTurn(env);

            Card[] ongoings = new Card[] { PlayCard("Harmony"), PlayCard("DrivingMantis"), PlayCard("BloodyKnuckles") };
            DecisionSelectCards = ongoings;
            GoToStartOfTurn(TurfWar);

            AssertUnderCard(Calin, ongoings[0]);
            AssertUnderCard(Calin, ongoings[1]);
            AssertIsInPlayAndNotUnderCard(ongoings[2]);
        }


        [Test()]
        public void TestCardTheConduit()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card conduit = PlayCard("TheConduit");

            QuickHPStorage(fixer.CharacterCard, conduit);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-2, -2);
            GoToEndOfTurn(env);

            PlayCard("HypersonicAssault");
            DestroyCard(conduit); PlayCard(conduit);

            PlayCard("Harmony"); PlayCard("RivetingCrane"); PlayCard("PushingTheLimits");

            QuickHPStorage(fixer.CharacterCard, conduit);
            GoToStartOfTurn(TurfWar);
            QuickHPCheck(-3, -3);
        }



        [Test()]
        public void TestCardChalice()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            Card chalice = PlayCard("Chalice");

            QuickHPStorage(chalice);
            DealDamage(chalice, chalice, 5, DamageType.Fire);
            QuickHPCheck(-3);

            // Target is Active - Put Under.
            PlayCard("RivetingCrane");
            GoToEndOfTurn(TurfWar);
            AssertNumberOfCardsAtLocation(Calin.UnderLocation, 1);

        }

        [Test()]
        public void TestCardTheShockTroopers()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card troopers = PlayCard("TheShockTroopers");

            QuickHPStorage(fixer.CharacterCard, troopers, Calin);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-2, 0, -2);
            GoToEndOfTurn(env);

            Card[] equip = new Card[] { PlayCard("DualCrowbars"), PlayCard("ToolBox"), PlayCard("GreaseGun") };
            DecisionSelectCards = equip;
            GoToStartOfTurn(TurfWar);

            AssertUnderCard(Kanya, equip[0]);
            AssertUnderCard(Kanya, equip[1]);
            AssertIsInPlayAndNotUnderCard(equip[2]);
        }

        [Test()]
        public void TestCardTheFlashpoint()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card flashpoint = PlayCard("TheFlashpoint");

            QuickHPStorage(fixer.CharacterCard, flashpoint);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-1, 0);
            GoToEndOfTurn(env);

            PlayCard("HypersonicAssault");
            DestroyCard(flashpoint); PlayCard(flashpoint);

            PlayCard("DualCrowbars"); PlayCard("ToolBox"); PlayCard("HUDGoggles");

            QuickHPStorage(fixer.CharacterCard, flashpoint);
            GoToStartOfTurn(TurfWar);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestCardShortCircuit()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            Card shortCircuit = PlayCard("ShortCircuit");

            QuickHPStorage(shortCircuit);
            DealDamage(shortCircuit, shortCircuit, 5, DamageType.Fire);
            QuickHPCheck(-6);

            // Target is Active - Put Under.
            PlayCard("DualCrowbars");
            GoToEndOfTurn(TurfWar);
            AssertNumberOfCardsAtLocation(Kanya.UnderLocation, 1);

        }

        [Test()]
        public void TestCardTheBrutes()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheWraith", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card brutes = PlayCard("TheBrutes");

            QuickHPStorage(fixer.CharacterCard, tachyon.CharacterCard, wraith.CharacterCard, brutes, Calin);
            GoToEndOfTurn(TurfWar);
            // The two enemies and the highest hero.
            QuickHPCheck(-4, 0, 0, 0, -4);
            GoToEndOfTurn(env);

            GoToStartOfTurn(TurfWar);
            AssertNumberOfCardsUnderCard(Sez, 2);
        }

        [Test()]
        public void TestCardTheSpike()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "Tachyon", "TheWraith", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HypersonicAssault");
            Card spike = PlayCard("TheSpike");

            QuickHPStorage(fixer.CharacterCard, tachyon.CharacterCard, wraith.CharacterCard, Calin, spike);
            GoToEndOfTurn(TurfWar);
            QuickHPCheck(-3, 0, 0, -3, 0);
            GoToEndOfTurn(env);

            PlayCard("HypersonicAssault");
            DestroyCard(spike); PlayCard(spike);

            PlayCard("Bloogo");
            PlayCard("Orbo");

            RestoreToMaxHP(fixer);
            QuickHPStorage(fixer.CharacterCard, tachyon.CharacterCard, wraith.CharacterCard, Calin, spike);
            GoToStartOfTurn(TurfWar);
            QuickHPCheck(-5, 0, 0, -5, 0);
        }

        [Test()]
        public void TestCardZenith()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("Zenith");

            Card damager = PlayCard("Bloogo");
            QuickHPStorage(Sez);
            DealDamage(damager, Sez, 5, DamageType.Fire);
            QuickHPCheck(-3);

            // Target is Active - Put Under.
            GoToEndOfTurn(TurfWar);
            AssertNumberOfCardsAtLocation(Sez.UnderLocation, 1);

        }

        #endregion Target Tests

        #region Non-Specific Cards Test

        [Test()]
        public void TestCardReinforcements()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("Reinforcements");
            AssertNumberOfCardsInPlay(TurfWar, 7); // Should now have 7 cards: Instructions, 3 chars, 3 targets.
        }

        [Test()]
        public void TestCardPlotsRevealed()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            Card envCard = PlayCard("Bloogo");
            Card target = PutOnDeck("Zenith");

            QuickHPStorage(Calin, Sez, fixer.CharacterCard);
            PlayCard("PlotsRevealed");
            QuickHPCheck(-2, 0, -2);
            AssertUnderCard(Sez, envCard);


        }

        [Test()]
        public void TestCardTheRealmsChoice()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            FlipCard(TurfWar); // Flip so destroy trigger isn't too nuts.
            PlayCard("TheRealmsChoice");
            Card destroyTarget = PlayCard("Bloogo");

            GoToEndOfTurn(TurfWar);
            AssertInTrash(destroyTarget);
            AssertNumberOfCardsInPlay(env, 2);
        }

        [Test()]
        public void TestCardHoldingTheHighGround()
        {
            SetupGameController("LazyFanComix.TheTurfWar", "MrFixer", "TheEnclaveOfTheEndlings");

            StartGame();

            PlayCard("HoldingTheHighGround");

            QuickHPStorage(Calin, Kanya, fixer.CharacterCard);
            GoToEndOfTurn(TurfWar);
            // Damage Totals: 
            // Calin: Calin 2, Kanya 1+1, Sez 0, Heal 5.
            // Kanya: Calin 2+1, Kanya 0, Sez 3+1
            // Fixer: Calin 2, Kanya 1, Sez 0
            QuickHPCheck(0, -7, -3);

            GoToStartOfTurn(TurfWar);
            QuickHPCheck(0, -3, -2); // Start of turn, Calin damages

        }
        #endregion Non-Specific Cards Test
    }
}