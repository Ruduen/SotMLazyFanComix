using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.BreachMage;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RuduenModsTest
{
    [TestFixture]
    public class BreachMageTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(BreachMageCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController BreachMage { get { return FindHero("BreachMage"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(BreachMage);
            Assert.IsInstanceOf(typeof(BreachMageTurnTakerController), BreachMage);
            Assert.IsInstanceOf(typeof(BreachMageCharacterCardController), BreachMage.CharacterCardController);

            Assert.AreEqual(27, BreachMage.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBreachMageInitialBreachesWork()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card[] breaches = this.GameController.FindCardsWhere((Card c) => c.DoKeywordsContain("breach") && c.Owner == BreachMage.HeroTurnTaker && c.IsInPlay).ToArray();
            // Note that due to not explicitly being a mission, character, shield, or any other special cards, we must use the special check for closed/open breaches! Yes, this is a pain. Engine restrictions! 
            int[] initFocus = new int[] { 0, 0, 3, 4 };

            Assert.IsTrue(breaches.Count() == 4);

            for (int i = 0; i < 4; i++)
            {
                AssertTokenPoolCount(breaches[i].FindTokenPool("FocusPool"), initFocus[i]);
            }
        }

        [Test()]
        public void TestBreachMagePlaySpell()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card card = PutIntoPlay("FlareCascade");
            Card breach = FindCardInPlay("BreachI");

            AssertNextToCard(card, breach);
        }

        [Test()]
        public void TestBreachMageCastSpell()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card card = PutIntoPlay("FlareCascade");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            GoToStartOfTurn(BreachMage);
            QuickHPCheck(-3); // MDP took 3 damage.
            AssertInTrash(card);
        }

        [Test()]
        public void TestBreachMageCastSpellPotent()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card breach = FindCardInPlay("BreachIII");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            // Use three times to open. 
            UsePower(breach);
            UsePower(breach);
            UsePower(breach);

            DecisionSelectTarget = mdp;
            DecisionSelectCard = breach;

            Card card = PutIntoPlay("FlareCascade");


            QuickHPStorage(mdp);
            GoToStartOfTurn(BreachMage);
            QuickHPCheck(-4); // MDP took 4 damage.
            AssertInTrash(card);
        }

        [Test()]
        public void TestChargeAuraCharm()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            PlayCard("AuraCharm");

            // Confirm base power was used, so only breaches remain.
            AssertNumberOfUsablePowers(BreachMage, 4);  
        }

        [Test()]
        public void TestChargeHammerCharm()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();
            Card ongoing = PlayCard("LivingForceField");
            DecisionDestroyCard = ongoing;

            PlayCard("HammerCharm");
            AssertInTrash(ongoing); // Ongoing destroyed.
        }

        [Test()]
        public void TestSpellFlareCascade()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card spell = PlayCard("FlareCascade");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            GoToStartOfTurn(BreachMage);
            QuickHPCheck(-3); // Damage Dealt.
            AssertInTrash(spell); // Spell destroyed.
        }

        [Test()]
        public void TestSpellFlareCascadeChargeTwice()
        {
            SetupGameController("BaronBlade", "RuduenWorkshop.BreachMage", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card[] charges = FindCardsWhere((Card c) => c.Identifier == "HammerCharm").ToArray();

            PutInHand(charges);
            PlayCards(charges);

            Card spell = PlayCard("FlareCascade");

            DecisionSelectCards = new List<Card>() { mdp, charges[0], mdp, charges[1], mdp, null };

            QuickHPStorage(mdp);
            GoToStartOfTurn(BreachMage);
            QuickHPCheck(-9); // Damage Dealt.
            AssertInTrash(spell); // Spell destroyed.
        }
    }
}