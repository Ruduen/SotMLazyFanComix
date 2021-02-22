using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.Soulbinder;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
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
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(SoulbinderCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Soulbinder { get { return FindHero("Soulbinder"); } }

        protected Card[] Soulshards { get { return new Card[] { GetCard("SoulshardOfLightningCharacter"), GetCard("SoulshardOfMercuryCharacter"), GetCard("SoulshardOfIronCharacter") }; } }

        protected Card SoulbinderMortal { get { return GetCard("SoulbinderMortalFormCharacter"); } }

        protected Card SoulbinderInstruction { get { return GetCard("SoulbinderCharacter"); } }

        [Test(Description = "Basic Setup and Health")]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Soulbinder);
            Assert.IsInstanceOf(typeof(HeroTurnTakerController), Soulbinder);
            Assert.IsInstanceOf(typeof(SoulbinderCharacterCardController), Soulbinder.CharacterCardController);

            // No character cards, so just assert the four exist. 
            AssertNumberOfCardsInHand(Soulbinder, 4);
        }

        #region Basic Setup Tests

        [Test]
        public void TestSetupBasic()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Megalopolis");

            DecisionSelectCard = Soulshards[0];

            ResetDecisions();
            StartGame(false);

            AssertNotInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

        }

        [Test]
        public void TestSetupMortal()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Megalopolis");

            ResetDecisions();
            DecisionSelectCard = Soulshards[0];

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);

            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);
        }

        #endregion Basic Setup Tests

        #region Multi-Character and Incap Tests

        [Test]
        public void TestIncapBasicAll()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "Megalopolis");

            ResetDecisions();
            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertFlipped(Soulshards[0]);
            AssertIsInPlay(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertFlipped(Soulshards[1]);
            AssertIsInPlay(Soulshards[2]);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            DestroyCard(Soulshards[2]);
            AssertFlipped(Soulshards[2]);
            AssertIsInPlay(Soulshards[2]);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapBasicOneRemoved()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();
            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            Card card = PlayCard("UnforgivingWasteland");
            DealDamage(card, Soulshards[0], 100, DamageType.Melee);
            AssertOutOfGame(Soulshards[0]);

            //AssertUnderCard(SoulbinderInstruction, Soulshards[1]);

            AssertIncapacitated(Soulbinder);

            GoToStartOfTurn(Soulbinder);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(Soulbinder, Phase.End);
        }

        [Test]
        public void TestIncapMortalAll()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "Megalopolis");

            ResetDecisions();
            List<Card> SoulshardsDecision = new List<Card>();
            SoulshardsDecision.AddRange(Soulshards);
            SoulshardsDecision.AddRange(Soulshards);
            DecisionSelectCards = SoulshardsDecision;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertFlipped(Soulshards[0]);
            AssertIsInPlayAndNotUnderCard(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertFlipped(Soulshards[1]);
            AssertIsInPlayAndNotUnderCard(Soulshards[2]);

            DestroyCard(Soulshards[2]);
            AssertIsInPlayAndNotUnderCard(Soulshards[2]);
            AssertNotFlipped(SoulbinderMortal);

            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            DestroyCard(SoulbinderMortal);
            AssertOutOfGame(Soulshards);
            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapMortalFirst()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "Megalopolis");

            ResetDecisions();
            List<Card> SoulshardsDecision = new List<Card>
            {
                Soulshards[0]
            };
            SoulshardsDecision.AddRange(Soulshards);
            DecisionSelectCards = SoulshardsDecision;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            DestroyCard(SoulbinderMortal);

            AssertOutOfGame(Soulshards);
            AssertIsInPlay(SoulbinderMortal);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        [Test]
        public void TestIncapMortalRemoved()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            DecisionSelectCards = Soulshards;

            StartGame(false);

            AssertIsInPlay(SoulbinderMortal);
            AssertIsInPlay(Soulshards);
            AssertIsInPlayAndNotUnderCard(Soulshards[0]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[1]);
            AssertUnderCard(SoulbinderInstruction, Soulshards[2]);
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            Card card = PlayCard("UnforgivingWasteland");
            DealDamage(card, SoulbinderMortal, 100, DamageType.Melee);
            AssertOutOfGame(SoulbinderMortal);

            // Still in play - one soulshard left!
            AssertNotIncapacitatedOrOutOfGame(Soulbinder);

            // First is destroyed, second flips to take its place.
            DestroyCard(Soulshards[0]);
            AssertFlipped(Soulshards[0]);
            AssertIsInPlayAndNotUnderCard(Soulshards[1]);

            DestroyCard(Soulshards[1]);
            AssertFlipped(Soulshards[1]);
            AssertIsInPlayAndNotUnderCard(Soulshards[2]);

            DestroyCard(Soulshards[2]);
            AssertFlipped(Soulshards[2]);

            AssertIncapacitated(Soulbinder);

            GoToUseIncapacitatedAbilityPhase(Soulbinder);
        }

        #endregion Multi-Character and Incap Tests

        #region Powers
        [Test]
        public void TestPowerBasic()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();
            DecisionSelectCard = Soulshards[1];

            StartGame(false);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(Soulshards[1], mdp);
            UsePower(SoulbinderInstruction);
            QuickHPCheck(-1, -3);

        }

        [Test]
        public void TestPowerMortalNoRitual()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder/SoulbinderMortalCharacter", "Legacy", "TheFinalWasteland");

            ResetDecisions();

            StartGame(false);

            AssertNextMessage("There are no rituals with Ritual Tokens in play.");

            QuickHandStorage(Soulbinder);
            UsePower(SoulbinderMortal);
            QuickHandCheck(1);

            AssertExpectedMessageWasShown();
        }

        [Test]
        public void TestPowerWood()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.Soulbinder", "Legacy", "TheFinalWasteland");

            ResetDecisions();
            DecisionSelectCards = new Card[] { Soulshards[1] };

            StartGame(false);

            ResetDecisions();

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            Card wood = PlayCard("WoodenEffigy");

            DecisionSelectCards = new Card[] { wood, mdp };

            DealDamage(wood, wood, 3, DamageType.Melee);

            QuickHPStorage(wood, mdp);
            UsePower(wood);
            QuickHPCheck(2, -2);

        }
        #endregion Powers
    }
}