using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Vagabond;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class VagabondText : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(VagabondCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Vagabond { get { return FindHero("Vagabond"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Vagabond);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Vagabond);
            Assert.IsInstanceOf(typeof(VagabondCharacterCardController), Vagabond.CharacterCardController);

            Assert.AreEqual(24, Vagabond.CharacterCard.HitPoints);
            AssertNumberOfCardsInDeck(Vagabond, 36);
            AssertNumberOfCardsInHand(Vagabond, 4);
        }

        #region Solo Card Tests

        [Test()]
        public void TestAlchemicalCollector()
        {
            SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();

            SetHitPoints(new TurnTakerController[] { Vagabond, legacy }, 15);

            QuickHPStorage(Vagabond, legacy);
            PlayCard("AlchemicCollector");
            QuickHPCheck(2, 2);

        }

        [Test()]
        public void TestAlchemicalCollectorMissInfoSolo()
        {
            SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("IsolatedHero");

            SetHitPoints(new TurnTakerController[] { Vagabond, legacy }, 15);

            QuickHPStorage(Vagabond, legacy);
            PlayCard("AlchemicCollector");
            QuickHPCheck(2 - 3, 0);

        }


        [Test()]
        public void TestAlchemicalCollectorSolo()
        {
            SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("TakingCareOfSomething");

            SetHitPoints(new TurnTakerController[] { Vagabond, legacy }, 15);

            QuickHPStorage(Vagabond, legacy);
            PlayCard("AlchemicCollector");
            QuickHPCheck(2 - 3, 0);

        }


        [Test()]
        public void TestImbuedDetonation()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            Card play = PutInTrash("ImbuedDetonation");
            DrawCard(Vagabond);

            DestroyNonCharacterVillainCards();
            QuickHPStorage(baron);
            PlayCard(play);
            QuickHPCheck(-3);

            PlayCard("TakingCareOfSomething");
            DecisionYesNo = true;
            PlayCard(play);
            QuickHPCheck(-3 - 3 - 3);

        }

        [Test()]
        public void TestBreakTheKeystone()
        {
            SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            Card[] clues = new Card[] { PlayCard("SuspiciousMalfunction"), PlayCard("MisplacedMemo") };

            PlayCard("BreakTheKeystone");
            AssertInTrash(clues);

            PlayCard("TakingCareOfSomething");
            PlayCards(clues);

            PlayCard("BreakTheKeystone");
            AssertInTrash(clues);
            AssertNotUsablePower(Vagabond, Vagabond.CharacterCard);
        }

        [Test()]
        public void TestImprovise()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            Card play = PutInTrash("Improvise");

            QuickHandStorage(Vagabond, legacy);
            PlayCard(play);
            QuickHandCheck(1, 1);


            PlayCard("TakingCareOfSomething");

            Card[] playFromTop = new Card[] { PutOnDeck("AlchemicCollector"), PutOnDeck("ImbuedDetonation") };
            Card draw = PutOnDeck("BreakTheKeystone");
            QuickHandStorage(Vagabond, legacy);
            PlayCard(play);
            QuickHandCheck(1, 0);
            AssertInHand(draw);
            AssertInTrash(playFromTop);
        }


        [Test()]
        public void TestUnifiedAssault()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();

            PlayCard("UnifiedAssault");
            AssertNotUsablePower(Vagabond, Vagabond.CharacterCard);
            AssertNotUsablePower(legacy, legacy.CharacterCard);

            GoToStartOfTurn(baron);
            DestroyNonCharacterVillainCards();

            PlayCard("TakingCareOfSomething");

            QuickHPStorage(baron);
            PlayCard("UnifiedAssault");
            AssertNotUsablePower(Vagabond, Vagabond.CharacterCard);
            AssertUsablePower(legacy, legacy.CharacterCard);
            QuickHPCheck(-2 - 4);
        }


        [Test()]
        public void TestProbingStrike()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();
            DestroyNonCharacterVillainCards();

            Card play = PutInTrash("ProbingStrike");

            QuickHandStorage(Vagabond);
            QuickHPStorage(baron);
            PlayCard(play);
            QuickHandCheck(3);
            QuickHPCheck(-1);

            PlayCard("TakingCareOfSomething");

            Card toPlay = PutInHand("Kiwi");
            DecisionSelectCardToPlay = toPlay;
            QuickHandStorage(Vagabond);
            QuickHPStorage(baron);
            PlayCard(play);
            QuickHandCheck(3 - 1);
            QuickHPCheck(-1);
            AssertIsInPlay(toPlay);

        }

        #endregion Solo Card Tests

        #region Equipment Tests
        //[Test()]
        //public void TestBottledLightning()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();

        //    QuickHPStorage(Vagabond, baron);
        //    PlayCard("BottledLightning");
        //    QuickHPCheck(-1, -1);

        //    UsePower("BottledLightning");
        //    QuickHPCheck(-2, -2);

        //    GoToEndOfTurn(Vagabond);
        //    UsePower("BottledLightning");
        //    QuickHPCheck(-1, -1);

        //}

        [Test()]
        public void TestBottledLightning()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            SetHitPoints(Vagabond, 10);

            DecisionGainHP = Vagabond.CharacterCard;

            QuickHPStorage(Vagabond, baron);
            PlayCard("BottledLightning");
            DealDamage(Vagabond, baron, 1, DamageType.Fire);
            QuickHPCheck(2, -1 - 1);

            UsePower("BottledLightning");
            DealDamage(Vagabond, baron, 1, DamageType.Fire);
            QuickHPCheck(2, -1 - 2);

            GoToEndOfTurn(Vagabond);
            UsePower("BottledLightning");
            DealDamage(Vagabond, baron, 1, DamageType.Fire);
            QuickHPCheck(2, -1 - 1);

        }

        [Test()]
        public void TestEndlessPouches()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            Card card = PutInTrash("EndlessPouches");
            Card kiwi = PutInTrash("Kiwi");
            DecisionYesNo = true;

            // First time, Kiwi goes out. Second time, draw.
            QuickHandStorage(Vagabond);
            PlayCard(card);
            AssertIsInPlay(kiwi);
            QuickHandCheck(1);

            UsePower(card);
            QuickHandCheck(1); // Only one more draw.

            // Draw Twice.
            DestroyNonCharacterVillainCards();
            GoToStartOfTurn(Vagabond);
            UsePower(card);
            QuickHandCheck(2);

            // Where'd Kiwi go?
            MoveCard(Vagabond, kiwi, Vagabond.TurnTaker.OutOfGame);
            UsePower(card);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestFlashStepBoots()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();

            Card play = PutInHand("Improvise");
            DecisionSelectCardToPlay = play;

            PlayCard("FlashStepBoots");
            AssertInTrash(play);
        }


        [Test()]
        public void TestTwinFangDagger()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();

            UsePower(legacy);

            QuickHPStorage(baron);
            PlayCard("TwinFangDagger");
            QuickHPCheck(-2 - 1 - 1 - 1);
        }

        #endregion Equipment Tests

        #region Isolate Tests
        [Test()]
        public void TestIsolateGeneric()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();
            GoToEndOfTurn(baron);
            DestroyNonCharacterVillainCards();

            PlayCard("TakingCareOfSomething");

            UsePower(legacy);
            QuickHPStorage(Vagabond);
            DealDamage(Vagabond, Vagabond, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSoloClandestineExecutioner()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();
            GoToEndOfTurn(baron);
            DestroyNonCharacterVillainCards();


            PlayCard("ClandestineExecutioner");
            DecisionSelectCardToPlay = PutInHand("Kiwi");
            QuickHPStorage(baron);

            GoToStartOfTurn(Vagabond);
            QuickHPCheck(-2 - 1);
            AssertIsInPlay("Kiwi");
        }

        [Test()]
        public void TestSoloMakeshiftDiversion()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();
            GoToEndOfTurn(baron);
            DestroyNonCharacterVillainCards();


            PlayCard("MakeshiftDiversion");
            DecisionSelectCardToPlay = PutInHand("Kiwi");
            QuickHPStorage(Vagabond);
            DealDamage(Vagabond.CharacterCard, Vagabond.CharacterCard, 2, DamageType.Fire);
            QuickHPCheck(-1);

            Card top = PutOnDeck("SelfDestructSequence");


            GoToStartOfTurn(Vagabond);
            AssertIsInPlay("Kiwi");
            AssertOnTopOfDeck(baron, top);
        }

        [Test()]
        public void TestSoloTakingCareOfSomething()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();
            GoToEndOfTurn(baron);
            DestroyNonCharacterVillainCards();

            Card destroy = PlayCard("MobileDefensePlatform");
            SetHitPoints(destroy, 5);
            PlayCard("TakingCareOfSomething");
            DecisionSelectCardToPlay = PutInHand("Kiwi");

            GoToStartOfTurn(Vagabond);
            AssertInTrash(destroy);
            AssertIsInPlay("Kiwi");
        }



        #endregion Isolate Tests


        #region Other Test
        [Test()]
        public void TestFeignDeath()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            Card solo = PutInDeck("TakingCareOfSomething");
            DecisionSelectCard = solo;

            Card feign = PlayCard("FeignDeath");
            DestroyCard(Vagabond.CharacterCard);
            AssertHitPoints(Vagabond.CharacterCard, 7);
            AssertNotIncapacitatedOrOutOfGame(Vagabond);
            AssertIsInPlay(solo);


            PutInDeck(solo);
            PlayCard(feign);
            DealDamage(Vagabond, Vagabond, 500, DamageType.Fire);
            AssertHitPoints(Vagabond.CharacterCard, 7);
            AssertNotIncapacitatedOrOutOfGame(Vagabond);
            AssertIsInPlay(solo);


            QuickHPStorage(baron);
            PutInDeck(solo);
            PlayCard(feign);
            UsePower(feign);
            QuickHPCheck(-2);
            AssertIsInPlay(solo);


        }

        [Test()]
        public void TestKiwi()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("Kiwi");
            DecisionYesNo = true;

            QuickHPStorage(baron);
            UsePower(Vagabond);
            QuickHPCheck(-2 - 2);

            UsePower(Vagabond);
            QuickHPCheck(-2);

            GoToStartOfTurn(Vagabond);
            UsePower(Vagabond);
            QuickHPCheck(-2 - 2);

        }


        [Test()]
        public void TestKiwiGrantedPower()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "CaptainCosmic", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("Kiwi");
            PlayCard("CosmicWeapon");
            PlayCard("CosmicWeapon", 1);
            DecisionYesNo = true;

            QuickHPStorage(baron);
            UsePower(Vagabond, 2);
            QuickHPCheck(-3 - 3);

            UsePower(Vagabond, 2);
            QuickHPCheck(-3);

            GoToStartOfTurn(Vagabond);
            UsePower(Vagabond, 2);
            QuickHPCheck(-3 - 3);

        }

        [Test()]
        public void TestKiwiGrantedPowerMultiplePowersAdded()
        {
            SetupGameController("PlagueRat", "CaptainCosmic", "LazyFanComix.Vagabond", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("Kiwi");
            DecisionYesNo = true;

            Card[] infections = new Card[] { PlayCard("Infection"), PlayCard("Infection", 2) };

            FlipCard(plague);

            UsePower(Vagabond, 2);
            AssertInTrash(infections);

            PlayCards(infections);
            UsePower(Vagabond, 2);
            AssertInTrash(infections[0]);
            AssertIsInPlay(infections[1]);

            GoToStartOfTurn(Vagabond);
            PlayCards(infections);
            UsePower(Vagabond, 2);
            AssertInTrash(infections);
        }

        [Test()]
        public void TestKiwiGrantedPowerLost()
        {
            SetupGameController("PlagueRat", "LazyFanComix.Vagabond", "CaptainCosmic", "WagnerMarsBase");

            StartGame();

            DestroyNonCharacterVillainCards();
            PlayCard("Kiwi");
            DecisionYesNo = true;

            Card[] infections = new Card[] { PlayCard("Infection"), PlayCard("Infection", 2) };

            FlipCard(plague);

            UsePower(Vagabond, 2);
            AssertInTrash(infections[0]);
            AssertIsInPlay(infections[1]);

            GoToStartOfTurn(Vagabond);
            PlayCards(infections);
            UsePower(Vagabond, 2);
            AssertInTrash(infections[0]);
            AssertIsInPlay(infections[1]);
        }
        #endregion Other Test

        #region Deprecated Tests

        //[Test()]
        //public void TestSerpentsBite()
        //{
        //    SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    Card target = PlayCard("OldLadyInTheStreet");

        //    QuickHPStorage(target);

        //    Card power = PlayCard("SerpentsBite");
        //    UsePower(power);

        //    QuickHPCheck(-3);
        //}

        //[Test()]
        //public void TestSerpentsBiteSolo()
        //{
        //    SetupGameController("MissInformation", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    Card target = PlayCard("OldLadyInTheStreet");
        //    DecisionSelectTurnTaker = Vagabond.HeroTurnTaker;
        //    PlayCard("IsolatedHero");

        //    QuickHPStorage(target);

        //    Card power = PlayCard("SerpentsBite");
        //    UsePower(power);

        //    QuickHPCheck(-6);
        //}


        //[Test()]
        //public void TestKiwiMultiPowerComplete()
        //{
        //    // TODO: Rewrite to better handle possibility of multiple powers due to outside source (cosmic/damage?). 
        //    SetupGameController("BaronBlade", "LazyFanComix.Vagabond", "Legacy", "WagnerMarsBase");

        //    StartGame();

        //    DestroyNonCharacterVillainCards();
        //    PlayCard("Kiwi");
        //    DecisionsYesNo = new bool[] { false, true, false };

        //    Card[] selections = new Card[] { PutInHand("EndlessPouches"), PutInHand("BottledLightning"), baron.CharacterCard };

        //    DecisionSelectCards = selections;
        //    PlayCard("FlashStepBoots");

        //    AssertIsInPlay(selections[0], selections[1]);

        //}

        #endregion Deprecated Tests
    }
}