using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Soulbinder;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class SoulbinderTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(SoulbinderCharacterCardController))); // replace with your own namespace
        }

        private int InitialRitual { get { return 3; } }

        protected HeroTurnTakerController Soulbinder { get { return FindHero("Soulbinder"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Soulbinder);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Soulbinder);
            Assert.IsInstanceOf(typeof(SoulbinderCharacterCardController), Soulbinder.CharacterCardController);

            AssertNumberOfCardsInDeck(Soulbinder, 36);
            AssertNumberOfCardsInHand(Soulbinder, 4);
        }

        #region Basic Setup Tests

        [Test]
        public void TestSetupBasic()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Megalopolis");

            ResetDecisions();
            StartGame(false);

            AssertIsInPlay(Soulbinder.CharacterCard);
        }

        #endregion Basic Setup Tests

        #region Powers Cards

        [Test]
        public void TestPowerBasic()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(Soulbinder.CharacterCard, mdp);
            UsePower(Soulbinder);
            QuickHPCheck(-1, -3);
        }

        [Test]
        public void TestPowerMortalNoRitual()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder/SoulbinderMortalFormCharacter", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            AssertNextMessage("There are no rituals with Ritual Tokens in play.");

            QuickHandStorage(Soulbinder);
            UsePower(Soulbinder);
            QuickHandCheck(1);

            AssertExpectedMessageWasShown();
        }

        [Test]
        public void TestPowerMortalRitual()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder/SoulbinderMortalFormCharacter", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            Card card = PlayCard("RitualOfCatastrophe");

            QuickHandStorage(Soulbinder);
            UsePower(Soulbinder);
            QuickHandCheck(1);

            AssertTokenPoolCount(card.TokenPools.FirstOrDefault(), InitialRitual - 1); // One removed from 4.
        }

        [Test]
        public void TestPowerClay()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame(false);
            DiscardAllCards(Soulbinder);
            Card[] rituals = new Card[] { PutInHand("RitualOfSalvation"), PutInHand("RitualOfTransferrence") };
            Card clay = PutInHand("ClaySoulsplinter");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { null, rituals[0] };
            DecisionSelectFunction = 1;
            QuickHPStorage(mdp);
            PlayCard(clay);
            QuickHandStorage(Soulbinder);
            UsePower(clay);
            QuickHandCheck(1); // 1 Drawn,
        }

        [Test]
        public void TestPowerWood()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();
            Card wood = PutInHand("WoodenSoulsplinter");

            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { null };

            QuickHandStorage(Soulbinder);
            PlayCard(wood);
            QuickHandCheck(-1); // 1 Played.

            DealDamage(wood, wood, 3, DamageType.Melee);

            ResetDecisions();
            QuickHPStorage(wood);
            UsePower(wood);
            QuickHPCheck(1);
        }

        [Test]
        public void TestPowerStraw()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            DiscardAllCards(Soulbinder);
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card straw = PutInHand("StrawSoulsplinter");

            DecisionSelectCards = new Card[] { straw, mdp };
            DecisionSelectFunction = 1;

            QuickHandStorage(Soulbinder);
            PlayCard(straw);
            QuickHandCheck(-1); // 1 Played.
            DealDamage(straw, straw, 3, DamageType.Melee);

            QuickHPStorage(straw, mdp);
            UsePower(straw);
            QuickHPCheck(2, -2);
        }

        #endregion Powers Cards

        #region Rituals

        //[Test]
        //public void TestCardRitualEndRemoved()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

        //    ResetDecisions();
        //    DecisionSelectCards = Soulshards;

        //    StartGame(false);

        //    ResetDecisions();

        //    Card ritual = PlayCard("RitualOfCatastrophe");

        //    GoToEndOfTurn(Soulbinder);
        //    AssertTokenPoolCount(ritual.TokenPools.FirstOrDefault(), InitialRitual - 1);

        //    GoToStartOfTurn(Soulbinder);

        //    // Bonus Damage
        //    UsePower(legacy);
        //    GoToEndOfTurn(Soulbinder);
        //    AssertTokenPoolCount(ritual.TokenPools.FirstOrDefault(), InitialRitual - 3);

        //    // Negated Damage
        //    GoToStartOfTurn(Soulbinder);
        //    PlayCard("HeroicInterception");
        //    GoToEndOfTurn(Soulbinder);
        //    AssertTokenPoolCount(ritual.TokenPools.FirstOrDefault(), InitialRitual - 3);
        //}

        [Test]
        public void TestCardRitualPower()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            Card[] rituals = new Card[] { PlayCard("RitualOfCatastrophe"), PlayCard("RitualOfKnowledge") };

            AssertTokenPoolCount(rituals[0].TokenPools.FirstOrDefault(), InitialRitual);
            UsePower(rituals[0]);
            AssertTokenPoolCount(rituals[0].TokenPools.FirstOrDefault(), InitialRitual - 1);

            PlayCard("ClaySoulsplinter");
            AssertTokenPoolCount(rituals[1].TokenPools.FirstOrDefault(), InitialRitual);
            UsePower(rituals[1]);
            AssertTokenPoolCount(rituals[1].TokenPools.FirstOrDefault(), InitialRitual - 2);
        }

        [Test]
        public void TestCardRitualOfCatastrophe()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            Card ritual = PlayCard("RitualOfCatastrophe");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            RemoveTokensFromPool(ritual.TokenPools[0], InitialRitual);
            QuickHPCheck(-3);
            AssertInTrash(ritual);
        }

        [Test]
        public void TestCardRitualOfKnowledge()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            Card ritual = PlayCard("RitualOfKnowledge");
            Card[] cards = new Card[] { PutInHand("RitualOfCatastrophe") };
            DecisionSelectCards = cards;
            DecisionSelectPower = cards[0];

            QuickHandStorage(Soulbinder);
            RemoveTokensFromPool(ritual.TokenPools[0], 5);
            QuickHandCheck(3); // Draw 4, play 1, use a power.
            AssertIsInPlay(cards[0]);
            AssertNumberOfUsablePowers(Soulbinder, 1); // Only ritual power remaining.
            AssertInTrash(ritual);
        }

        [Test]
        public void TestCardRitualOfKnowledgePossibleRepeatedTrigger()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame(false);

            Card ritual = PlayCard("RitualOfKnowledge");
            DecisionSelectCards = new Card[] { null };
            DecisionSelectPower = ritual;

            QuickHandStorage(Soulbinder);
            RemoveTokensFromPool(ritual.TokenPools[0], InitialRitual);
            QuickHandCheck(4); // Draw 4
            AssertInTrash(ritual);
        }

        [Test]
        public void TestCardRitualOfTransferrence()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "Ra", "TheFinalWasteland");

            StartGame();

            Card ritual = PlayCard("RitualOfTransferrence");
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card[] targets = new Card[] { Soulbinder.CharacterCard, legacy.CharacterCard, ra.CharacterCard, mdp };
            DealDamage(baron.CharacterCard, targets, 3, DamageType.Melee);

            DecisionSelectCards = targets;
            QuickHPStorage(targets);
            RemoveTokensFromPool(ritual.TokenPools[0], InitialRitual);
            QuickHPCheck(2, 2, 2, -6);
            AssertInTrash(ritual);
        }

        [Test]
        public void TestCardRitualOfSalvation()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "Ra", "TheFinalWasteland");

            StartGame();
            Card ritual = PlayCard("RitualOfSalvation");
            Card[] targets = new Card[] { Soulbinder.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            DealDamage(baron.CharacterCard, targets, 4, DamageType.Melee);

            DecisionSelectCards = targets;
            QuickHPStorage(targets);
            QuickHandStorage(Soulbinder, legacy, ra);
            RemoveTokensFromPool(ritual.TokenPools[0], InitialRitual);
            QuickHPCheck(3, 3, 3);
            QuickHandCheck(1, 1, 1);
            AssertInTrash(ritual);
        }

        [Test]
        public void TestCardRitualComponents()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "Ra", "TheFinalWasteland");

            StartGame();

            Card card = PlayCard("RitualComponents");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            Card ritualA = PlayCard("RitualOfSalvation");
            UsePower(card);
            AssertTokenPoolCount(ritualA.TokenPools[0], InitialRitual - 1);

            DecisionSelectCards = new Card[] { null };

            PutInHand(card);
            PlayCard(card);
            PutInHand(ritualA);
            PlayCard(ritualA);

            Card ritualB = PlayCard("RitualOfCatastrophe");
            ResetDecisions();

            DecisionSelectCards = new Card[] { mdp, ritualA };

            QuickHPStorage(mdp);
            UsePower(card);
            QuickHPCheck(-2);
            AssertTokenPoolCount(ritualA.TokenPools[0], InitialRitual - 1); // Selected reduced.
            AssertTokenPoolCount(ritualB.TokenPools[0], InitialRitual); // Other not reduced.
        }

        //[Test]
        //public void TestCardRitualComponentsTriggered()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "Ra", "TheFinalWasteland");

        //    ResetDecisions();
        //    DecisionSelectCards = Soulshards;

        //    StartGame(false);

        //    ResetDecisions();
        //    DecisionSelectCards = new Card[] { null };

        //    PlayCard("ClaySoulsplinter");
        //    PlayCard("StrawSoulsplinter");

        //    ResetDecisions();

        //    Card[] playRituals = new Card[] { PutInHand("RitualOfCatastrophe"), PutInHand("RitualOfSalvation") };

        //    Card card = PlayCard("RitualComponents");
        //    Card ritual = PlayCard("RitualOfCausality");
        //    RemoveTokensFromPool(ritual.TokenPools[0], 1);

        //    DecisionSelectCards = new Card[] { playRituals[0], playRituals[1], playRituals[0] };
        //    UsePower(card);
        //    AssertInTrash(ritual);
        //    AssertIsInPlay(playRituals);
        //    // Power removed 1 again.
        //    AssertTokenPoolCount(playRituals[0].TokenPools[0], 2);
        //    AssertTokenPoolCount(playRituals[1].TokenPools[0], 2);
        //}

        #endregion Rituals

        #region Other Cards

        //[Test]
        //public void TestCardReliquary()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

        //    ResetDecisions();
        //    DecisionSelectCards = Soulshards;

        //    StartGame(false);

        //    DecisionYesNo = true;

        //    QuickHandStorage(Soulbinder);
        //    PlayCard("Reliquary");

        //    AssertIsInPlayAndNotUnderCard(Soulshards[0]);
        //    AssertIsInPlayAndNotUnderCard(Soulshards[1]);

        //    DestroyCard(Soulshards[0]);
        //    AssertNotFlipped(Soulshards[0]);
        //}

        [Test]
        public void TestCardArcaneDetonation()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { mdp };

            DiscardAllCards(Soulbinder);
            QuickHPStorage(Soulbinder.CharacterCard, mdp);
            PlayCard("ArcaneDetonation");
            QuickHPCheck(-2, -2);
        }

        [Test]
        public void TestCardSacrificialRite()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            ResetDecisions();

            Card[] rituals = new Card[] { PlayCard("RitualOfCatastrophe"), PlayCard("RitualOfTransferrence") };
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { mdp, rituals[0] };

            DiscardAllCards(Soulbinder);
            QuickHPStorage(Soulbinder.CharacterCard, mdp);
            PlayCard("SacrificialRite");
            QuickHPCheck(-2, -2);
            AssertTokenPoolCount(rituals[0].TokenPools.FirstOrDefault(), InitialRitual - 1);
            AssertTokenPoolCount(rituals[1].TokenPools.FirstOrDefault(), InitialRitual - 1);
        }

        [Test]
        public void TestCardKeystoneOfSpirit()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "Ra", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            ResetDecisions();

            Card[] targets = new Card[] { Soulbinder.CharacterCard, ra.CharacterCard };
            DealDamage(baron.CharacterCard, targets, 3, DamageType.Melee);

            QuickHPStorage(Soulbinder.CharacterCard, ra.CharacterCard);

            PlayCard("KeystoneOfSpirit");

            QuickHPCheck(-2, 1); // -1 for play, -1 for power.
            AssertNotUsablePower(Soulbinder, Soulbinder.CharacterCard);
        }

        [Test]
        public void TestCardFinalEruption()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            ResetDecisions();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card soulsplinter = PlayCard("ClaySoulsplinter");

            DecisionSelectCards = new Card[] { mdp, soulsplinter };

            DiscardAllCards(Soulbinder);
            SetHitPoints(soulsplinter, 2);
            QuickHPStorage(mdp);
            PlayCard("FinalEruption");
            QuickHPCheck(-4);
            AssertInTrash(soulsplinter);
        }

        [Test]
        public void TestCardSpiritialResonance()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            StartGame();

            DecisionSelectCards = new Card[] { null };
            DiscardAllCards(Soulbinder);

            QuickHPStorage(Soulbinder.CharacterCard);
            QuickHandStorage(Soulbinder);
            PlayCard("SpiritialResonance");
            QuickHPCheck(-1);
            QuickHandCheck(2);
            AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("ritual") || c.DoKeywordsContain("soulsplinter"), 1);
        }

        [Test]
        public void TestCardDebtOfTheSoulless()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            ResetDecisions();
            PlayCard("DebtOfTheSoulless");

            QuickHPStorage(Soulbinder.CharacterCard);
            QuickHandStorage(Soulbinder);
            DealDamage(Soulbinder.CharacterCard, Soulbinder.CharacterCard, 1, DamageType.Melee);
            DealDamage(Soulbinder.CharacterCard, Soulbinder.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }

        #endregion Other Cards
    }
}