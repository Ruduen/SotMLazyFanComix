using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.Spellforge;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixText
{
    [TestFixture]
    public class SpellforgeTest : BaseTest
    {
        private readonly Dictionary<string, int> CardDamage = new Dictionary<string, int>()
        {
            { "Impact", -1 },
            { "Wave", -2 },
            { "Shock", -3 },
            { "Ray", -4 },
            { "Inspired", -1 },
            { "Controlled", -1 },
            { "OfResonance", -1 },
            { "OfDisruption", -1 },
            { "OfHealing", 3 }
        };

        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(SpellforgeCharacterCardController))); // replace with your own namespace
        }

        protected HeroTurnTakerController Spellforge { get { return FindHero("Spellforge"); } }

        [Test()]
        public void TestModWorks()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Spellforge);
            Assert.IsInstanceOf(typeof(SpellforgeTurnTakerController), Spellforge);
            Assert.IsInstanceOf(typeof(SpellforgeCharacterCardController), Spellforge.CharacterCardController);

            Assert.AreEqual(26, Spellforge.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDecreeInnatePower()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "SpellforgeCharacter", "SpellforgeDecreeCharacter" }
            };
            SetupGameController(setupItems, promoIdentifiers: promos);

            StartGame();

            DiscardAllCards(Spellforge);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, Spellforge.CharacterCard };

            QuickHPStorage(mdp);
            UsePower(Spellforge);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDecreeInnatePowerDiscardPrefix()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "SpellforgeCharacter", "SpellforgeDecreeCharacter" }
            };
            SetupGameController(setupItems, promoIdentifiers: promos);

            StartGame();

            Card prefix = PutInHand("Controlled");
            PutInHand("OfHealing");
            UsePower(legacy);
            UsePower(legacy);
            UsePower(legacy);

            DecisionSelectCards = new Card[] { prefix, null, Spellforge.CharacterCard, null };

            QuickHPStorage(Spellforge.CharacterCard);
            UsePower(Spellforge);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDecreeInnatePowerDiscardSuffix()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "SpellforgeCharacter", "SpellforgeDecreeCharacter" }
            };
            SetupGameController(setupItems, promoIdentifiers: promos);

            PutInHand("Controlled");
            Card suffix = PutInHand("OfHealing");
            UsePower(legacy);
            UsePower(legacy);
            UsePower(legacy);

            DecisionSelectCards = new List<Card>() { null, suffix, Spellforge.CharacterCard, null };

            QuickHPStorage(Spellforge.CharacterCard);
            UsePower(Spellforge);
            QuickHPCheck(-4 + CardDamage["OfHealing"]); // Hit for 4, healed 4.
        }

        [Test()]
        public void TestDecreeInnatePowerDiscardPrefixSuffix()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis"
            };
            Dictionary<string, string> promos = new Dictionary<string, string>
            {
                { "SpellforgeCharacter", "SpellforgeDecreeCharacter" }
            };
            SetupGameController(setupItems, promoIdentifiers: promos);

            StartGame();
            Card prefix = PutInHand("Controlled");
            Card suffix = PutInHand("OfHealing");
            UsePower(legacy);
            UsePower(legacy);
            UsePower(legacy);

            DecisionSelectCards = new List<Card>() { prefix, suffix, Spellforge.CharacterCard, null };

            DealDamage(Spellforge, Spellforge.CharacterCard, 4, DamageType.Melee);

            QuickHPStorage(Spellforge.CharacterCard);
            UsePower(Spellforge);
            QuickHPCheck(-1 + CardDamage["OfHealing"]); // Hit for 1, healed 4.
        }

        //[Test()]
        //public void TestRedefineInnatePower()
        //{
        //    IEnumerable<string> setupItems = new List<string>()
        //    {
        //        "BaronBlade", "LazyFanComix.Spellforge/SpellforgeRedefineCharacter", "Fanatic", "Megalopolis"
        //    };

        //    SetupGameController(setupItems);

        //    StartGame();

        //    DiscardAllCards(Spellforge);
        //    DiscardAllCards(fanatic);

        //    PutInHand("HolyNova");
        //    Card mdp = GetCardInPlay("MobileDefensePlatform");

        //    QuickHPStorage(mdp);
        //    UsePower(Spellforge);
        //    QuickHPCheck(-1);
        //}

        //[Test()]
        //public void TestRedefineInnatePowerDiscardPrefix()
        //{
        //    IEnumerable<string> setupItems = new List<string>()
        //    {
        //        "BaronBlade", "LazyFanComix.Spellforge/SpellforgeRedefineCharacter", "Fanatic", "Megalopolis"
        //    };

        //    SetupGameController(setupItems);

        //    StartGame();

        //    DiscardAllCards(Spellforge);
        //    DiscardAllCards(fanatic);
        //    PutInHand("Inspired");
        //    PutInHand("HolyNova");
        //    Card mdp = GetCardInPlay("MobileDefensePlatform");

        //    QuickHPStorage(mdp);
        //    UsePower(Spellforge);
        //    QuickHPCheck(-1 + CardDamage["Inspired"]);
        //}

        [Test()]
        public void TestEssenceNoDiscard()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard("Ray");
            QuickHPCheck(CardDamage["Ray"]);
        }

        [Test()]
        public void TestEssenceDiscardPrefix()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard("Ray");
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Ray"]);
        }

        [Test()]
        public void TestEssenceDiscardSuffix()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard("Ray");
            QuickHPCheck(CardDamage["Ray"] + CardDamage["OfDisruption"]); // 4 base damage, 1 self damage.
        }

        [Test()]
        public void TestEssenceDiscardPrefixSuffix()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard("Ray");
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Ray"] + CardDamage["Inspired"] + CardDamage["OfDisruption"]); // 5 base damage, 2 self damage.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardControlled()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "TheBlock");

            StartGame();

            UsePower(legacy);

            DiscardAllCards(Spellforge);
            PutInHand("Controlled");
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card play = PutInHand("Impact");

            QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
            PlayCard(play);
            QuickHPCheck(CardDamage["Controlled"], CardDamage["Controlled"], CardDamage["Impact"] - 1 + CardDamage["OfDisruption"]); // 1 controlled. 4 boosted to MDP, doubled by Resonance on the first hit only.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardInspired()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "TheBlock");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card play = PutInHand("Impact");

            QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
            PlayCard(play);
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"],
                CardDamage["Inspired"] + CardDamage["Impact"],
                CardDamage["Inspired"] + CardDamage["Impact"] + CardDamage["Inspired"] + CardDamage["OfDisruption"]); // 1 controlled. 2 boosted to MDP, doubled by Resonance.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardPiercing()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "TheBlock");

            StartGame();

            PutIntoPlay("DefensiveDisplacement");

            DiscardAllCards(Spellforge);
            PutInHand("Piercing");
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card play = PutInHand("Impact");

            QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
            PlayCard(play);
            QuickHPCheck(CardDamage["Impact"], CardDamage["Impact"], CardDamage["Impact"] + CardDamage["OfDisruption"]); // 1 base and irreducible. Self-damage is also irreducible.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardOfDisruption()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfDisruption");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card play = PutInHand("Impact");

            QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
            PlayCard(play);
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"],
                 CardDamage["Inspired"] + CardDamage["Impact"],
                 CardDamage["Inspired"] + CardDamage["Impact"] + CardDamage["Inspired"] + CardDamage["OfDisruption"]); // 2 base damage, 2 targetted damage for mdp only.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardOfDisruptionRedirect()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "MrFixer", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfDisruption");
            PutIntoPlay("DrivingMantis");

            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            Card bb = GetCardInPlay("BaronBladeCharacter");

            DecisionSelectTargets = new List<Card>() { fixer.CharacterCard, bb, Spellforge.CharacterCard, baron.CharacterCard }.ToArray();

            DecisionRedirectTarget = bb;

            Card play = PutInHand("Impact");

            QuickHPStorage(Spellforge.CharacterCard, fixer.CharacterCard, bb);
            PlayCard(play);
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"],
                0,
                (CardDamage["Inspired"] + CardDamage["Impact"] + CardDamage["Inspired"] + CardDamage["OfDisruption"]) * 2); // 2 base damage, 2 + 2 redirected to MDP + trigger, 2+2 direct to MDP without trigger.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardOfHealing()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfHealing");
            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(Spellforge.CharacterCard, mdp);
            PlayCard("Impact");
            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"] + CardDamage["OfHealing"], CardDamage["Inspired"] + CardDamage["Impact"]); // 2 damage, 3 healing to self; 2 damage to enemy.
        }

        [Test()]
        [Category("DiscardModifier")]
        public void TestDiscardOfAura()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

            StartGame();

            DiscardAllCards(Spellforge);
            PutInHand("Inspired");
            PutInHand("OfAura");
            Card play = PutInHand("Impact");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
            QuickHandStorage(Spellforge, legacy);

            PlayCard(play);

            QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"], CardDamage["Inspired"] + CardDamage["Impact"], CardDamage["Inspired"] + CardDamage["Impact"]); // 3 Damage each due to Inspired.
            QuickHandCheck(-2, 1); // 3 used, 1 drawn for Spellforge, 1 drawn for others.
        }

        //[Test()]
        //[Category("DiscardModifier")]
        //public void TestDiscardOfResonance()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "Megalopolis");

        //    StartGame();

        //    DiscardAllCards(Spellforge);
        //    PutInHand("Inspired");
        //    PutInHand("OfResonance");

        //    Card play = PutInHand("Impact");

        //    Card mdp = GetCardInPlay("MobileDefensePlatform");

        //    QuickHPStorage(Spellforge.CharacterCard, legacy.CharacterCard, mdp);
        //    PlayCard(play);
        //    QuickHPCheck(CardDamage["Inspired"] + CardDamage["Impact"],
        //        CardDamage["Inspired"] + CardDamage["Impact"],
        //        CardDamage["Inspired"] + CardDamage["Impact"] + CardDamage["Inspired"] + CardDamage["OfResonance"]); //  2 boosted to MDP, doubled by Resonance.
        //}

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayControlled()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "TheBlock");

            StartGame();
            PutInHand(FindCardsWhere((Card c) => c.Identifier == "Controlled").ToArray());

            Card safeish = PutInHand("OfDisruption");

            DecisionSelectCardToPlay = safeish;

            QuickHandStorage(Spellforge);
            PlayCard("Controlled");
            QuickHandCheck(-1); // 2 cards played, 1 card drawn.
        }

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayInspired()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Legacy", "TheBlock");

            StartGame();
            Card[] cards = FindCardsWhere((Card c) => c.Identifier == "Inspired").ToArray();
            PutInHand(cards);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = cards;

            QuickHandStorage(Spellforge);
            PlayCard("Inspired");
            QuickHandCheck(2); // 1 cards played, 3 cards drawn.
        }

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayPiercing()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "Ra", "TheBlock");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            // TODO: Find a better option!
            // For whatever reason, the power selection logic is always defaulting to Spellforge's, so just consume it so it can default otherwise.
            UsePower(Spellforge);

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            PlayCard("Piercing");
            QuickHPCheck(-2); // Damage was boosted.
        }

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayOfAura()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card card = PutInHand("OfAura");
            Card environment = PutIntoPlay("DefensiveDisplacement");

            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHandCheckZero(); // One played, one drawn.
            AssertInTrash(environment); // Destroyed.
        }

        //[Test]
        //[Category("DiscardModifier")]
        //public void TestPlayOfDisruption()
        //{
        //    SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

        //    StartGame();

        //    Card card = PutInHand("OfDisruption");
        //    Card ongoing = PutIntoPlay("LivingForceField");

        //    QuickHandStorage(Spellforge);
        //    PlayCard(card);
        //    QuickHandCheckZero(); // One played, one drawn.
        //    AssertInTrash(ongoing); // Destroyed.
        //}

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayOfHealing()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card card = PutInHand("OfHealing");

            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            QuickHPStorage(Spellforge);
            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHPCheck(2); // Heal 2.
            QuickHandCheckZero(); // One played, one drawn.
        }

        [Test]
        [Category("DiscardModifier")]
        public void TestPlayOfDisruption()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            // TODO: Find a better option!
            // For whatever reason, the power selection logic is always defaulting to Spellforge's, so just consume it so it can default otherwise.

            DecisionSelectTarget = mdp;

            Card card = PutInHand("OfDisruption");

            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            QuickHPStorage(mdp);
            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHPCheck(-2); // Deal 2.
            QuickHandCheckZero(); // One played, one drawn.
        }

        [Test]
        public void TestPlayArticulateTheLethologicalPrefix()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card card = PutInHand("ArticulateTheLethological");
            Card safeish = PutInHand("OfDisruption");

            DecisionSelectFunction = 0;
            DecisionSelectCardToPlay = safeish;

            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHandCheck(1); // 2 played, 2 added, 1 drawn.
        }

        [Test]
        public void TestPlayArticulateTheLethologicalEssence()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card card = PutInHand("ArticulateTheLethological");
            Card safeish = PutInHand("OfDisruption");

            DecisionSelectFunction = 1;
            DecisionSelectCardToPlay = safeish;

            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHandCheck(1); // 2 played, 2 added, 1 drawn.
        }

        [Test]
        public void TestPlayArticulateTheLethologicalSuffix()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card card = PutInHand("ArticulateTheLethological");
            Card safeish = PutInHand("OfDisruption");

            DecisionSelectFunction = 2;
            DecisionSelectCardToPlay = safeish;

            DealDamage(Spellforge, Spellforge.CharacterCard, 5, DamageType.Melee);

            QuickHandStorage(Spellforge);
            PlayCard(card);
            QuickHandCheck(1); // 2 played, 2 added, 1 drawn.
        }

        [Test]
        public void TestPlayArticulateTheLethologicalNoDeck()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card safeish = PutInHand("OfDisruption");

            DecisionSelectCardToPlay = safeish;

            MoveAllCards(Spellforge, Spellforge.HeroTurnTaker.Deck, Spellforge.HeroTurnTaker.Trash);
            MoveAllCards(Spellforge, Spellforge.HeroTurnTaker.Deck, Spellforge.HeroTurnTaker.Trash);

            QuickHandStorage(Spellforge);
            PlayCard("ArticulateTheLethological");
            QuickHandCheck(0); // 1 Played, 1 drawn. This should trigger a reshuffle.
            AssertNumberOfCardsInTrash(Spellforge, 2); // Cards that were used move to the trash after a reshuffle.
        }

        [Test]
        public void TestPlayMeanderingDissertation()
        {
            SetupGameController("BaronBlade", "LazyFanComix.Spellforge", "TheBlock");

            StartGame();

            Card[] discard = FindCardsWhere((Card c) => c.Identifier == "Controlled").ToArray();
            PutInHand(discard);
            Card safeish = PutInHand("OfHealing");

            DecisionSelectCards = new Card[] { discard[0], discard[1], null, safeish };

            QuickHandStorage(Spellforge);
            PlayCard("MeanderingDissertation");
            QuickHandCheck(0); // Equal discarded as drawn, 1 played and 1 drawn from that.
        }
    }
}