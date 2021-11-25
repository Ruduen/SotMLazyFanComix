using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Collections.Generic;
using System.Reflection;

namespace LazyFanComixTest
{
    [TestFixture]
    public class HeroPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController))); // replace with your own namespace
        }

        [Test()]
        public void TestAbsoluteZeroOverloadPlay()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/LazyFanComix.AbsoluteZeroOverloadCharacter", "TheBlock");

            StartGame();


            Assert.IsTrue(az.CharacterCard.IsPromoCard);
            Card card = PutInTrash("IsothermicTransducer");

            DecisionSelectCard = card;

            QuickHPStorage(az);
            UsePower(az);
            QuickHPCheck(-3); // Damage dealt
            AssertInPlayArea(az, card);
        }

        [Test()]
        public void TestAbsoluteZeroOverloadTribunal()
        {
            SetupGameController("BaronBlade", "Guise", "TheCelestialTribunal");

            StartGame();
            SelectFromBoxForNextDecision("LazyFanComix.AbsoluteZeroOverloadCharacter", "AbsoluteZero");

            PlayCard("CalledToJudgement");

            Card representative = FindCardInPlay("AbsoluteZeroCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);
        }


        [Test()]
        public void TestAbsoluteZeroOverchillPlay()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/LazyFanComix.AbsoluteZeroOverchillCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(az.CharacterCard.IsPromoCard);
            Card card = PutInHand("CryoChamber");
            PutIntoPlay("IsothermicTransducer");

            DecisionSelectCard = card;
            DecisionSelectFunction = 0;

            QuickHPStorage(az);
            QuickHandStorage(az);
            UsePower(az);
            QuickHPCheck(-1); // Damage dealt through DR.
            QuickHandCheck(-1); // 1 Played, 0 Drawn.
            AssertInPlayArea(az, card);
        }

        [Test()]
        public void TestAbsoluteZeroOverchillDestroy()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/LazyFanComix.AbsoluteZeroOverchillCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(az.CharacterCard.IsPromoCard);
            List<Card> transducers = new List<Card>(this.GameController.FindCardsWhere((Card c) => c.Identifier == "IsothermicTransducer"));
            PlayCard(transducers[0]);
            DiscardAllCards(az);
            PutInHand(transducers[1]);

            DecisionSelectFunction = 1;

            // Only available card is a copy of a limited card. Play will fail, cause destroy.

            QuickHPStorage(az);
            QuickHandStorage(az);
            UsePower(az);
            QuickHPCheck(-1); // Damage dealt through DR.
            QuickHandCheck(3); // No play, draw 3.
            AssertInTrash(az, transducers[0]);
        }

        [Test()]
        public void TestAkashThriya()
        {
            SetupGameController("BaronBlade", "AkashThriya/LazyFanComix.AkashThriyaPlantEssenceCharacter", "TheEnclaveOfTheEndlings");

            StartGame();

            Assert.IsTrue(thriya.CharacterCard.IsPromoCard);

            Card tree = PutOnDeck("AkashFlora");
            DecisionSelectCard = tree;

            UsePower(thriya);
            AssertAtLocation(tree, thriya.HeroTurnTaker.Hand);

            DecisionSelectFunction = 1;
            UsePower(thriya);
            AssertOnTopOfDeck(env, tree, 2);

            PlayTopCard(env);
            PlayTopCard(env);
            AssertNotInPlay(tree);
            PlayTopCard(env);
            AssertIsInPlay(tree);            // Confirm tree exists at this point - no Environment shenanigan should destroy it.

            // Check nothing completely breaks when dealing with an empty hand.
            DiscardAllCards(thriya);
            UsePower(thriya);

            // Check nothing completely breaks if the environment deck is empty. 
            DiscardAllCards(thriya);
            PutInHand(tree);
            MoveAllCards(env, env.TurnTaker.Deck, env.TurnTaker.Trash);
            UsePower(thriya);
            AssertOnTopOfDeck(env, tree);

            // Check single card on top still works.
            Card pollen = PutInHand("HealingPollen");
            UsePower(thriya);
            AssertOnTopOfDeck(env, tree); // Tree then pollen, given previous deck was just tree.
            AssertOnTopOfDeck(env, pollen, 1);
        }

        [Test()]
        public void TestArgentAdeptPlaySafe()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/LazyFanComix.TheArgentAdeptAriaCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("DrakesPipes");

            DecisionSelectCard = card;

            UsePower(adept);
            AssertInPlayArea(adept, card);
        }

        [Test()]
        public void TestArgentAdeptPlayPerform()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/LazyFanComix.TheArgentAdeptAriaCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("ScherzoOfFrostAndFlame");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCard = card;

            QuickHPStorage(mdp);
            UsePower(adept);
            AssertInPlayArea(adept, card);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestArgentAdeptPlayPerformAccompany()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/LazyFanComix.TheArgentAdeptAriaCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(adept.CharacterCard.IsPromoCard);

            Card card = PutInHand("SyncopatedOnslaught");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { card, adept.CharacterCard, mdp, mdp };

            QuickHPStorage(mdp);
            UsePower(adept);
            AssertInPlayArea(adept, card);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestBenchmarkNoSoftware()
        {
            SetupGameController("BaronBlade", "Benchmark/LazyFanComix.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            DiscardAllCards(bench);

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(legacy);
            UsePower(bench);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBenchmarkSoftware()
        {
            SetupGameController("BaronBlade", "Benchmark/LazyFanComix.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card software = PutInHand("AutoTargetingProtocol");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectCardToPlay = software;

            QuickHPStorage(mdp);
            UsePower(bench);
            QuickHPCheck(-1);
            AssertInPlayArea(bench, software); // Card is in play.
        }

        // TODO: Fix if Handlabra fix!
        [Test(Description = "Failing Handlabra Case", ExpectedResult = false)]
        public bool TestBrokenBenchmarkSoftwareIndestructibleBounce()
        {
            SetupGameController("BaronBlade", "Benchmark/LazyFanComix.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(bench.CharacterCard.IsPromoCard);
            Card software = PutInHand("AutoTargetingProtocol");
            PutInHand("AllyMatrix"); // Add second one so the decision selection always has a choice.
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            // Play, damage, bounce, play.
            DecisionSelectCards = new Card[] { software, mdp, software, null };

            QuickHPStorage(mdp);
            UsePower(bench);
            PutIntoPlay("OverhaulLoadout");
            return (software.Location == bench.TurnTaker.PlayArea); // The card should be in the play area! Expect a fail right now.
        }

        [Test()]
        public void TestBunkerFullSalvoNoOtherPower()
        {
            SetupGameController("BaronBlade", "Bunker/LazyFanComix.BunkerFullSalvoCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            GoToUsePowerPhase(bunker);

            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(2); //  3 Drawn, 1 Discarded.

            AssertPhaseActionCount(0); // Powers used.
        }

        [Test()]
        public void TestBunkerFullSalvoOneOtherPower()
        {
            SetupGameController("BaronBlade", "Bunker/LazyFanComix.BunkerFullSalvoCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutIntoPlay("FlakCannon");

            DecisionSelectTarget = mdp;

            GoToUsePowerPhase(bunker);

            QuickHandStorage(bunker);
            UsePower(bunker);
            QuickHandCheck(1); //  3 Drawn, 2 Discarded
            AssertNumberOfCardsInTrash(bunker, 2); // 2 Discarded.
            AssertPhaseActionCount(1); // 1 Power Remaining
        }

        [Test()]
        public void TestBunkerModeShiftRecharge()
        {
            SetupGameController("BaronBlade", "Bunker/LazyFanComix.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card recharge = PutOnDeck("RechargeMode");
            DecisionSelectCard = recharge;

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(0); // Powers used.

            GoToDrawCardPhase(bunker);
            AssertPhaseActionCount(2); // 2 Draws
        }

        [Test()]
        public void TestBunkerModeShiftTurret()
        {
            SetupGameController("BaronBlade", "Bunker/LazyFanComix.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card turret = PutOnDeck("TurretMode");
            DecisionSelectCard = turret;

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(1); // 1 Use Remaining from Turret Mode.
        }

        [Test()]
        public void TestBunkerModeShiftUpgrade()
        {
            SetupGameController("BaronBlade", "Bunker/LazyFanComix.BunkerModeShiftCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(bunker.CharacterCard.IsPromoCard);

            Card upgrade = PutOnDeck("UpgradeMode");
            Card equip = PutInHand("FlakCannon");
            DecisionSelectCards = new Card[] { upgrade, equip };

            GoToUsePowerPhase(bunker);

            UsePower(bunker);
            AssertPhaseActionCount(0); // Powers used.
            AssertIsInPlay(equip);
        }

        [Test()]
        public void TestBunkerF6Tribunal()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Legacy/AmericasGreatestLegacyCharacter", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            SelectFromBoxForNextDecision("BunkerFreedomSixCharacter", "Bunker");
            PlayCard("CalledToJudgement");

            GoToStartOfTurn(legacy);
            DecisionSelectCard = FindCardInPlay("BunkerCharacter");
            UsePower(legacy);
        }


        [Test()]
        public void TestCaptainCosmicNoConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/LazyFanComix.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(cosmic.CharacterCard.IsPromoCard);

            QuickHandStorage(cosmic);
            UsePower(cosmic);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestCaptainCosmicConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/LazyFanComix.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(cosmic.CharacterCard.IsPromoCard);

            Card construct = PutIntoPlay("CosmicWeapon");

            QuickHandStorage(cosmic);
            QuickHPStorage(construct, cosmic.CharacterCard);
            UsePower(cosmic);
            QuickHandCheck(1); // Draw.
            DealDamage(legacy, construct, 2, DamageType.Melee);
            AssertNextToCard(construct, cosmic.CharacterCard);
            GoToStartOfTurn(cosmic);
            AssertInTrash(construct);
        }

        [Test()]
        public void TestChronoRanger()
        {
            SetupGameController("BaronBlade", "ChronoRanger/LazyFanComix.ChronoRangerHighNoonCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(chrono.CharacterCard.IsPromoCard);

            PlayCard("DefensiveDisplacement");

            Card card = PutInHand("TerribleTechStrike");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 1;
            DecisionSelectTarget = mdp;

            DecisionSelectCard = card;

            QuickHPStorage(chrono.CharacterCard, mdp);
            QuickHandStorage(chrono);
            UsePower(chrono);
            DealDamage(chrono.CharacterCard, mdp, 1, DamageType.Melee);
            DealDamage(mdp, chrono.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-1, -1); // Damage dealt through DR.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestExpatriettePowerDeck()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "Expatriette/LazyFanComix.ExpatrietteQuickShotCharacter", "Megalopolis");

            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);

            StartGame();

            Card equipment = PutOnDeck("Pride");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectPower = equipment;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(expatriette);
            AssertInPlayArea(expatriette, equipment); // Equipment played.
            QuickHPCheck(-1); // Damage dealt.
        }

        [Test()]
        public void TestExpatrietteGunPlayAndPower()
        {
            // Equipment Test
            SetupGameController("Omnitron", "Expatriette/LazyFanComix.ExpatrietteLiterallyAGunCharacter", "Megalopolis");

            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);

            StartGame();

            Card ammo=PlayCard("IncendiaryRounds");
            AssertNextToCard(ammo, expatriette.CharacterCard);

            QuickHPStorage(omnitron, expatriette);
            QuickHandStorage(expatriette);
            UsePower(expatriette);
            QuickHPCheck(-4, 0); // Shoot Omni twice.
            QuickHandCheck(1);

        }

        [Test()]
        public void TestExpatriettePowerNoDeck()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Expatriette/LazyFanComix.ExpatrietteQuickShotCharacter", "Megalopolis");
            Assert.IsTrue(expatriette.CharacterCard.IsPromoCard);

            StartGame();

            PutInTrash(expatriette.HeroTurnTaker.Deck.Cards); // Move all cards in deck to trash.
            Card ongoing = PutInHand("HairtriggerReflexes");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = ongoing;
            DecisionSelectTarget = mdp;

            AssertNumberOfCardsInDeck(expatriette, 0); // Deck remains empty.
            QuickHandStorage(expatriette);
            UsePower(expatriette);
            AssertNumberOfCardsInDeck(expatriette, 0); // Deck remains empty.
        }

        [Test()]
        public void TestFanaticDraw()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "Fanatic/LazyFanComix.FanaticZealCharacter", "Megalopolis");

            Assert.IsTrue(fanatic.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectPowers = new Card[] { null };
            DecisionSelectFunction = 1;

            QuickHPStorage(fanatic.CharacterCard, mdp);
            QuickHandStorage(fanatic);
            UsePower(fanatic);
            QuickHPCheck(-1, -1); // Damage dealt to BB (canceled), mdp, and self.
            QuickHandCheck(1); // Drawn.
        }

        [Test()]
        public void TestFanaticPower()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "Fanatic/LazyFanComix.FanaticZealCharacter", "Megalopolis");

            Assert.IsTrue(fanatic.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("Absolution");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 0;
            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, mdp }; // Attempt to attack MDP twice.

            QuickHPStorage(fanatic.CharacterCard, mdp);
            UsePower(fanatic);
            QuickHPCheck(-1, -4); // Damage dealt to BB (canceled), mdp, and self.
        }

        [Test()]
        public void TestGuiseOngoing()
        {
            SetupGameController("BaronBlade", "Guise/LazyFanComix.GuiseShenanigansCharacter", "Megalopolis");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);
            Card ongoing = PutInHand("GrittyReboot");

            DecisionSelectFunction = 0;
            DecisionSelectCard = ongoing;

            QuickHandStorage(guise);
            UsePower(guise);
            QuickHandCheck(0); // 1 Card Drawn, 1 Card Played.
            DestroyCard(ongoing);
            AssertIsInPlay(ongoing);
            GoToStartOfTurn(guise);
            AssertIsInPlay(ongoing); // First new turn, should survive self-destruct.
            GoToStartOfTurn(guise);
            AssertInTrash(ongoing); // Second new turn. Without a power, it's gone!
        }

        [Test()]
        public void TestGuiseOngoingAlreadyDestroyed()
        {
            SetupGameController("BaronBlade", "Guise/LazyFanComix.GuiseShenanigansCharacter", "Unity", "TimeCataclysm");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);
            Card bee = PlayCard("BeeBot");
            PlayCard("SurpriseShoppingTrip");
            Card ongoing = PutInHand("GrittyReboot");

            DecisionSelectFunction = 0;
            DecisionSelectCard = ongoing;

            AssertNextMessage("Guise does not have a played ongoing, so he cannot make it indestructible. Whoops!");
            QuickHandStorage(guise);
            UsePower(guise);
            QuickHandCheck(1); // 1 Card Draw, 1 Card Played, 1 Card Drawn from Gritty Reboot
            DestroyCard(ongoing);
            AssertInTrash(ongoing); // Second new turn. Without a power, it's gone!
        }

        [Test()]
        public void TestGuiseNoOngoing()
        {
            SetupGameController("BaronBlade", "Guise/LazyFanComix.GuiseShenanigansCharacter", "Megalopolis");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);

            DiscardAllCards(guise);
            PutOnDeck("GimmickyCharacter");

            QuickHandStorage(guise);
            UsePower(guise);
            QuickHandCheck(1); // 1 Card Drawn.
        }

        [Test()]
        public void TestGuiseTribunalFullWat()
        {
            IEnumerable<string> setupItems = new List<string>()
            {
                "BaronBlade", "Guise", "TheCelestialTribunal"
            };
            SetupGameController(setupItems);

            StartGame();

            SelectFromBoxForNextDecision("CompletionistGuiseCharacter", "Guise");
            PlayCard("RepresentativeOfEarth");

            SelectFromBoxForNextDecision("SantaGuiseCharacter", "Guise");
            DecisionSelectCard = FindCardInPlay("GuiseCharacter", 0);

            UsePower(FindCardInPlay("GuiseCharacter", 1));
        }



        [Test()]
        public void TestHaka()
        {
            SetupGameController("BaronBlade", "Haka/LazyFanComix.HakaVigorCharacter", "Megalopolis");
            Assert.IsTrue(haka.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(haka);
            PutInHand("VitalitySurge");
            GoToUsePowerPhase(haka);

            UsePower(haka);
            AssertNumberOfCardsInHand(haka, 1); // Make sure the net effect is 1 cards in hand, even if the played card results in a draw.
        }

        [Test()]
        public void TestHarpy()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/LazyFanComix.TheHarpyExtremeCallingCharacter", "Megalopolis");

            Assert.IsTrue(harpy.CharacterCard.IsPromoCard);

            DecisionSelectWord = "Flip 2 {arcana}";

            StartGame();
            QuickHPStorage(harpy);
            UsePower(harpy);
            QuickHPCheck(-2); // Damage dealt.
            ;
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool), 1);
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.AvianControlPool), 4);
        }

        [Test()]
        public void TestHarpyFancierTrigger()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/LazyFanComix.TheHarpyExtremeCallingCharacter", "Megalopolis");

            Assert.IsTrue(harpy.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("HarpyHex");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectWord = "Flip 1 {avian}";
            DecisionSelectTarget = mdp;

            QuickHPStorage(harpy.CharacterCard, mdp);
            UsePower(harpy);
            QuickHPCheck(-1, -1); // Damage dealt.
            ;
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool), 4);
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.AvianControlPool), 1);
        }

        [Test()]
        public void TestHarpyNumerology()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/LazyFanComix.TheHarpyExtremeCallingCharacter", "Megalopolis");

            Assert.IsTrue(harpy.CharacterCard.IsPromoCard);

            PutIntoPlay("AppliedNumerology");

            DecisionSelectWord = "Flip 1 {arcana}";

            StartGame();
            QuickHPStorage(harpy);
            UsePower(harpy);
            QuickHPCheck(-1); // Damage dealt.
            ;
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.ArcanaControlPool), 2);
            AssertTokenPoolCount(harpy.CharacterCard.FindTokenPool(TokenPool.AvianControlPool), 3);
        }

        [Test()]
        public void TestKnyfePower()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Knyfe/LazyFanComix.KnyfeKineticLoopCharacter", "Megalopolis");
            Assert.IsTrue(knyfe.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(knyfe);
            QuickHPStorage(knyfe);

            DecisionSelectFunction = 1;
            UsePower(knyfe);
            DealDamage(knyfe, knyfe, 1, DamageType.Energy);

            QuickHPCheck(0); // 1 damage, healed 1.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestKnyfePowerNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Knyfe/LazyFanComix.KnyfeKineticLoopCharacter", "TheBlock");
            Assert.IsTrue(knyfe.CharacterCard.IsPromoCard);

            StartGame();

            DealDamage(knyfe, knyfe, 5, DamageType.Energy);

            PutIntoPlay("DefensiveDisplacement");

            QuickHandStorage(knyfe);
            QuickHPStorage(knyfe);

            DecisionSelectFunction = 1;
            UsePower(knyfe);
            DealDamage(knyfe, knyfe, 1, DamageType.Energy);

            QuickHPCheck(0); // No damage or healing.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestKnyfePlay()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Knyfe/LazyFanComix.KnyfeKineticLoopCharacter", "Megalopolis");
            Assert.IsTrue(knyfe.CharacterCard.IsPromoCard);

            StartGame();

            DecisionSelectCard = PutInHand("IncidentalContact");

            QuickHandStorage(knyfe);
            QuickHPStorage(knyfe);

            UsePower(knyfe);

            QuickHPCheck(0); // 1 damage, healed 1.
            QuickHandCheck(-1); // Card played.
        }

        [Test()]
        public void TestLegacyPlayOneShot()
        {
            SetupGameController("BaronBlade", "Legacy/LazyFanComix.LegacyInTheFrayCharacter", "Megalopolis");
            Assert.IsTrue(legacy.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(legacy);
            Card card = PutInHand("FlyingSmash");
            GoToUsePowerPhase(legacy);

            DecisionSelectCardToPlay = card;

            QuickHandStorage(legacy);
            UsePower(legacy);
            AssertInTrash(card);
            QuickHandCheck(-1); // 1 played, 0 drawn.
        }

        [Test()]
        public void TestLegacyNotOneShot()
        {
            SetupGameController("BaronBlade", "Legacy/LazyFanComix.LegacyInTheFrayCharacter", "Megalopolis");
            Assert.IsTrue(legacy.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(legacy);
            Card card = PutInHand("TheLegacyRing");
            GoToUsePowerPhase(legacy);

            DecisionSelectFunction = 1;

            QuickHandStorage(legacy);
            UsePower(legacy);
            AssertInTrash(card);
            QuickHandCheck(0); // 1 Discarded, 1 Drawn
        }

        [Test()]
        public void TestLifelineNormalHit()
        {
            SetupGameController("BaronBlade", "Lifeline/LazyFanComix.LifelineEnergyTapCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(lifeline.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DealDamage(lifeline, lifeline.CharacterCard, 5, DamageType.Melee);

            QuickHPStorage(lifeline.CharacterCard, mdp);
            UsePower(lifeline);
            QuickHPCheck(1, -2); // One successful hit, one HP regained.
        }

        [Test()]
        public void TestLifelineRedirectedHit()
        {
            SetupGameController("BaronBlade", "Lifeline/LazyFanComix.LifelineEnergyTapCharacter", "Legacy", "Tachyon", "Megalopolis");
            Assert.IsTrue(lifeline.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutIntoPlay("SynapticInterruption");

            DecisionSelectCards = new Card[] { mdp, tachyon.CharacterCard, mdp };

            UsePower(legacy);
            UsePower(legacy); // Boost damage to 3 for redirect.

            DealDamage(lifeline, lifeline.CharacterCard, 5, DamageType.Melee);

            QuickHPStorage(lifeline.CharacterCard, tachyon.CharacterCard, mdp);
            UsePower(lifeline);
            QuickHPCheck(1, 0, -6); // Two hits but only one target damaged, so 1 HP.
        }

        [Test()]
        public void TestLuminaryNoDevice()
        {
            SetupGameController("BaronBlade", "Luminary/LazyFanComix.LuminaryReprogramCharacter", "Megalopolis");
            Assert.IsTrue(luminary.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(luminary);
            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            QuickHPStorage(baron);
            QuickHandStorage(luminary);
            UsePower(luminary);
            QuickHPCheck(-1);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestLuminaryDeviceDestroyed()
        {
            SetupGameController("BaronBlade", "Luminary/LazyFanComix.LuminaryReprogramCharacter", "Megalopolis");
            Assert.IsTrue(luminary.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(luminary);
            Card card = PutInHand("RegressionTurret");
            Card destroyed = PutIntoPlay("BacklashGenerator");
            GoToUsePowerPhase(luminary);

            DecisionSelectCards = new Card[] { destroyed, card };

            UsePower(luminary);
            AssertInTrash(destroyed);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestLuminaryDeviceNotDestroyed()
        {
            SetupGameController("BaronBlade", "Luminary/LazyFanComix.LuminaryReprogramCharacter", "Megalopolis");
            Assert.IsTrue(luminary.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(luminary);
            Card card = PutInHand("RegressionTurret");
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            GoToUsePowerPhase(luminary);

            DecisionSelectCards = new Card[] { mdp, card };

            UsePower(luminary);
            AssertIsInPlay(mdp);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestNaturalistAllIconsPowerSelectRhino()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);
            Card power = PutInHand("NaturalFormsPower");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = power;
            DecisionSelectWord = "{rhinoceros}";

            UsePower(naturalist);
            AssertIsInPlay(power);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);
            // To check triggers, confirm that 0 cards were drawn, Naturalist regained 2 HP, and targets (including MDP) were dealt 0 damage.
            UsePower(power);
            QuickHPCheck(2, 0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNaturalistAllIconsPlaySelectCrocodile()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);
            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("PrimalCharge");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;
            DecisionSelectWord = "{crocodile}";

            UsePower(naturalist);
            AssertInTrash(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);
            // To check triggers, confirm that 0 cards were drawn, Naturalist regained 0 HP, and targets (including MDP) were dealt 1 damage.
            UsePower(power);
            QuickHPCheck(0, -1);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNaturalistNoIcons()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);

            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("NaturalBornVigor");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;

            UsePower(naturalist);
            AssertIsInPlay(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);

            // To check triggers, confirm that no cards were drawn and no damage was healed or dealt.
            UsePower(power);

            QuickHPCheck(0, 0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNaturalistGazelle()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);

            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("CraftyAssault");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;

            UsePower(naturalist);
            AssertInTrash(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);

            // To check triggers, confirm that 2 cards were drawn and no damage was healed or dealt.
            UsePower(power);

            QuickHPCheck(0, 0);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestNaturalistRhino()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);

            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("IndomitableForce");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;

            UsePower(naturalist);
            AssertIsInPlay(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);

            // To check triggers, confirm that no cards were drawn and 2 damage was healed, but none was dealt.
            UsePower(power);

            QuickHPCheck(2, 0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNaturalistCrocodile()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);

            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("ThePredatorsEye");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCardToPlay = play;

            UsePower(naturalist);
            AssertInTrash(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);

            // To check triggers, confirm that no cards were drawn and 0 damage was healed, but 1 was dealt.
            UsePower(power);

            QuickHPCheck(0, -1);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNaturalistCombo()
        {
            SetupGameController("BaronBlade", "TheNaturalist/LazyFanComix.TheNaturalistVolatileFormCharacter", "Megalopolis");
            Assert.IsTrue(naturalist.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(naturalist);

            Card power = PutIntoPlay("NaturalFormsPower");
            Card play = PutInHand("ThePredatorsEye");
            Card mdp = FindCardInPlay("MobileDefensePlatform");
            PutIntoPlay("TheNimbleGazelle");

            DecisionSelectCardToPlay = play;

            UsePower(naturalist);
            AssertInTrash(play);

            DealDamage(naturalist, naturalist.CharacterCard, 5, DamageType.Toxic);

            QuickHPStorage(naturalist.CharacterCard, mdp);
            QuickHandStorage(naturalist);

            // To check triggers, confirm that 2 cards were drawn and 0 damage was healed, but 1 was dealt.
            UsePower(power);

            QuickHPCheck(0, -1);
            QuickHandCheck(2);
        }

        //[Test()]
        //public void TestMrFixerPowerA()
        //{
        //    // Tool in hand.
        //    SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
        //    Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

        //    StartGame();
        //    UsePower(legacy);
        //    Card tool = PutInHand("DualCrowbars");
        //    Card mdp = GetCardInPlay("MobileDefensePlatform");

        //    DecisionSelectCardToPlay = tool;
        //    DecisionSelectTarget = mdp;

        //    QuickHPStorage(fixer.CharacterCard, mdp);
        //    UsePower(fixer);
        //    QuickHPCheck(-1, -2);
        //    AssertInPlayArea(fixer, tool); // Card put into play.
        //}

        [Test()]
        public void TestMrFixerFlowingPower()
        {
            SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card harmony = PutOnDeck("Harmony");

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-2, -2);
            AssertInPlayArea(fixer, harmony); // Card put into play.
        }

        [Test()]
        public void TestMrFixerFlowingPowerNoDeck()
        {
            SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            MoveAllCards(fixer, fixer.TurnTaker.Deck, fixer.TurnTaker.Trash);

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-2, -2);
            AssertNumberOfCardsInDeck(fixer, 33); // Reshuffle forced.
        }

        [Test()]
        public void TestMrFixerFlowingPowerNoPlay()
        {
            SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PutOnDeck("Charge");
            PutOnDeck("Overdrive");
            PutOnDeck("GreaseGun");

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            AssertNextMessages("No Tool, Style, or copy of {Harmony} was discarded.");
            UsePower(fixer);
            QuickHPCheck(-2, -2);
            AssertNumberOfCardsInTrash(fixer, 3);
        }

        [Test()]
        public void TestMrFixerFocusedPowerB()
        {
            // Tool not in hand.
            SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFocusedStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            MoveAllCards(fixer, fixer.HeroTurnTaker.Hand, fixer.HeroTurnTaker.Deck);

            Card tool = PutInHand("DualCrowbars");

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-2, -2);
            AssertInPlayArea(fixer, tool); // Card put into play.
        }

        [Test()]
        public void TestMrFixerFocusedPowerC()
        {
            // Tool not in hand and empty deck.
            SetupGameController("BaronBlade", "MrFixer/LazyFanComix.MrFixerFocusedStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            MoveAllCards(fixer, fixer.HeroTurnTaker.Hand, fixer.HeroTurnTaker.Trash);
            MoveAllCards(fixer, fixer.HeroTurnTaker.Deck, fixer.HeroTurnTaker.Trash);

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            QuickHandStorage(fixer);
            UsePower(fixer);
            QuickHPCheck(-2, -2);
            QuickHandCheck(1);
            AssertNotInPlay((Card c) => c.IsTool);
        }

        [Test()]
        public void TestLaComodoraPower()
        {
            SetupGameController("Omnitron", "LaComodora/LazyFanComix.LaComodoraTemporalScavengeCharacter", "SkyScraper", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(comodora);
            // Stack Omnitron deck to avoid accidental plating block.
            PutOnDeck("DisintegrationRay");
            PutOnDeck("InterpolationBeam");

            Card equip = PutInHand("CannonPortal");
            GoToStartOfTurn(comodora);
            QuickHandStorage(comodora);
            UsePower(comodora);
            QuickHandCheck(1); // Card draw part. 
            DiscardAllCards(comodora);

            PlayCard(equip);


            // Cannon portal self-destructs at start of turn. Confirm it's now under.
            DecisionYesNo = true;
            GoToStartOfTurn(comodora);
            AssertUnderCard(comodora.CharacterCard, equip);

            // Played through power function 1.
            DecisionSelectFunction = 1;
            UsePower(comodora);
            AssertIsInPlayAndNotUnderCard(equip);

            // Make sure effect has been re-applied and therefore still works here.
            GoToStartOfTurn(comodora);
            AssertUnderCard(comodora.CharacterCard, equip);
            PlayCard(equip);

            GoToStartOfTurn(comodora);
            AssertInTrash(equip);

            // Post effect check: Make sure links are re-played!
            QuickHPStorage(omnitron);
            Card link = PlayCard("CompulsionCanister");
            QuickHPCheck(-2);

            UsePower(comodora);
            DestroyCard(link);
            UsePower(comodora);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestLaComodoraPowerGuise()
        {
            SetupGameController("BaronBlade", "LaComodora/LazyFanComix.LaComodoraTemporalScavengeCharacter", "Guise", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(comodora);
            DiscardAllCards(guise);

            Card equip = PlayCard("CannonPortal");

            DecisionYesNo = true;

            // Guise can do that too - but make sure it still goes under La Capitan. 
            PlayCard("ICanDoThatToo");

            GoToStartOfTurn(comodora);

            // Yep, it's actually under the card, just like with Idealist!
            AssertUnderCard(comodora.CharacterCard, equip);

            // Guise can play from under, too!
            PlayCard("ICanDoThatToo");
        }
        [Test()]
        public void TestLaComodoraPowerTribunal()
        {
            SetupGameController("BaronBlade", "TheWraith", "TheCelestialTribunal");

            StartGame();

            DiscardAllCards(wraith);

            DecisionYesNo = true;

            SelectFromBoxForNextDecision("LazyFanComix.LaComodoraTemporalScavengeCharacter", "LaComodora");

            Card equip = PutInHand("UtilityBelt");

            // Draw card, prepare.

            QuickHandStorage(wraith);
            PlayCard("CalledToJudgement");
            Card representative = FindCardInPlay("LaComodoraCharacter");
            AssertIsInPlay(representative);
            QuickHandCheck(1);

            // Destroy, under.
            DecisionYesNo = true;
            PlayCard(equip);
            DestroyCard(equip);
            AssertUnderCard(representative, equip);


            // Wraith power to play from under.
            DecisionSelectFunction = 1;
            PlayCard("CalledToJudgement");
            AssertIsInPlayAndNotUnderCard(equip);

            // Wear off effect, confirm to trash.
            GoToStartOfTurn(wraith);
            GoToStartOfTurn(wraith);
            DestroyCard(equip);
            AssertInTrash(equip);

            // Tribunal power without crashing.
            DecisionSelectFunction = 0;
            UsePower(representative);
            PlayCard(equip);

            // Destroy under.
            DestroyCard(equip);
            AssertUnderCard(representative, equip);

            // Representative can play this. 
            DecisionSelectFunction = 1;
            UsePower(representative);
            AssertIsInPlayAndNotUnderCard(equip);

            // Let time wear off.
            GoToStartOfTurn(wraith);
            GoToStartOfTurn(wraith);
            PlayCard(equip);
            DestroyCard(equip);
            AssertInTrash(equip);
        }

        [Test()]
        public void TestNightMistPowerDraw()
        {
            SetupGameController("BaronBlade", "NightMist/LazyFanComix.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(mist.CharacterCard.IsPromoCard);

            StartGame();

            DecisionSelectFunction = 1;

            QuickHandStorage(mist);
            QuickHPStorage(mist, legacy);
            UsePower(mist);
            DealDamage(mist, mist, 2, DamageType.Energy);
            DealDamage(mist, legacy, 2, DamageType.Energy);
            QuickHPCheck(-1, -1); // 1 Net Damage.
            QuickHandCheck(1); // Card drawn.
        }

        [Test()]
        public void TestNightMistPowerPlay()
        {
            SetupGameController("BaronBlade", "NightMist/LazyFanComix.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(mist.CharacterCard.IsPromoCard);

            StartGame();
            Card play = PutInHand("TomeOfElderMagic");

            DecisionSelectCardToPlay = play;

            QuickHandStorage(mist);
            QuickHPStorage(mist, legacy);
            UsePower(mist);
            DealDamage(mist, mist, 2, DamageType.Energy);
            DealDamage(mist, legacy, 2, DamageType.Energy);
            QuickHPCheck(-1, -1); // 1 Net Damage.
            AssertInPlayArea(mist, play);
        }

        [Test()]
        public void TestOmnitronXPower()
        {
            SetupGameController("BaronBlade", "OmnitronX/LazyFanComix.OmnitronXElectroShieldedSystemsCharacter", "Megalopolis");
            Assert.IsTrue(omnix.CharacterCard.IsPromoCard);

            StartGame();
            Card component = PutIntoPlay("ElectroDeploymentUnit");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionSelectFunction = 1;

            QuickHPStorage(omnix.CharacterCard, mdp);
            UsePower(omnix);
            QuickHPCheck(-1, -1);
            DestroyCard(component);
            AssertInPlayArea(omnix, component); // Card not destroyed.
            GoToStartOfTurn(omnix);
            DestroyCard(component);
            AssertInTrash(component); // New turn, effect wears off, destroyed.
        }

        [Test()]
        public void TestParsePowerNoDeck()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/LazyFanComix.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            MoveAllCards(parse, env.TurnTaker.Deck, env.TurnTaker.Trash);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            UsePower(parse);
            AssertInTrash(mdp); // Destroyed.
        }

        [Test()]
        public void TestParsePowerNoTrash()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/LazyFanComix.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(-1);
        }

        //[Test()]
        //public void TestParsePowerTrashSkipAttack()
        //{
        //    // Tool in hand.
        //    SetupGameController("BaronBlade", "Parse/LazyFanComix.ParseLaplaceShotCharacter", "Megalopolis");
        //    Assert.IsTrue(parse.CharacterCard.IsPromoCard);

        //    StartGame();

        //    DiscardTopCards(baron, 2);

        //    Card mdp = GetCardInPlay("MobileDefensePlatform");

        //    DecisionSelectTarget = null;

        //    QuickHPStorage(mdp);
        //    UsePower(parse);
        //    QuickHPCheck(0);
        //    AssertNumberOfCardsInTrash(baron, 2);
        //}

        [Test()]
        public void TestParsePowerTrashAttack()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/LazyFanComix.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            DiscardTopCards(env, 2);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(-3); // Base 1, 2 more.) 
            AssertNumberOfCardsInTrash(baron, 0);
        }

        [Test()]
        public void TestRaNormalPiercingHit()
        {
            SetupGameController("BaronBlade", "Ra/LazyFanComix.RaPiercingBlastCharacter", "TheBlock");
            Assert.IsTrue(ra.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("DefensiveDisplacement");

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(ra);
            QuickHPCheck(-1); // 1 Piercing Damage.
        }

        [Test()]
        public void TestRaNormalPiercingDestroy()
        {
            SetupGameController("BaronBlade", "Ra/LazyFanComix.RaPiercingBlastCharacter", "TheBlock");
            Assert.IsTrue(ra.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DealDamage(mdp, mdp, 9, DamageType.Energy);

            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard };

            PutIntoPlay("DefensiveDisplacement");

            QuickHPStorage(baron.CharacterCard);
            UsePower(ra);
            AssertInTrash(mdp); // MDP destroyed.
            QuickHPCheck(-1); // 1 Piercing Damage from repeat.
        }

        [Test()]
        public void TestRaNormalPiercingDestroyRedirect()
        {
            SetupGameController("BaronBlade", "Ra/LazyFanComix.RaPiercingBlastCharacter", "MrFixer", "TheBlock");
            Assert.IsTrue(ra.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DealDamage(mdp, mdp, 9, DamageType.Energy);

            DecisionSelectCards = new Card[] { fixer.CharacterCard, mdp, baron.CharacterCard };

            PutIntoPlay("DefensiveDisplacement");
            PutIntoPlay("DrivingMantis");

            QuickHPStorage(fixer.CharacterCard, baron.CharacterCard);
            UsePower(ra);
            AssertInTrash(mdp); // MDP destroyed.
            QuickHPCheck(0, -1); // 1 Piercing Damage from repeat.
        }

        [Test()]
        public void TestScholarNoElemental()
        {
            SetupGameController("BaronBlade", "TheScholar/LazyFanComix.TheScholarEquilibriumCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(scholar.CharacterCard.IsPromoCard);

            StartGame();

            UsePower(legacy);

            QuickHPStorage(scholar);
            QuickHandStorage(scholar);
            UsePower(scholar);
            QuickHandCheck(1);
            QuickHPCheck(-2); // 1 damage dealt for 1+1.
        }

        [Test()]
        public void TestScholarElemental()
        {
            SetupGameController("BaronBlade", "TheScholar/LazyFanComix.TheScholarEquilibriumCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(scholar.CharacterCard.IsPromoCard);

            StartGame();

            UsePower(legacy);
            PutIntoPlay("SolidToLiquid");

            QuickHPStorage(scholar);
            QuickHandStorage(scholar);
            UsePower(scholar);
            QuickHandCheck(2);
            QuickHPCheck(-4); // 3 damage dealt for 1+1+2.
        }

        [Test()]
        public void TestSetbackRunOfLuck()
        {
            SetupGameController("BaronBlade", "Setback/LazyFanComix.SetbackRunOfLuckCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(setback.CharacterCard.IsPromoCard);

            StartGame();

            Card[] plays = new Card[] {
                PutInHand ("SilverLining"),
                PutInHand("HighRiskBehavior")
            };

            DecisionSelectCards = plays;

            QuickHandStorage(setback);
            UsePower(setback);
            QuickHandCheck(1);
            AssertNotInPlay(plays);
            AssertTokenPoolCount(setback.CharacterCard.FindTokenPool(TokenPool.UnluckyPoolIdentifier), 2);

            GoToStartOfTurn(setback);
            QuickHandUpdate();
            AddTokensToPool(setback.CharacterCard.FindTokenPool(TokenPool.UnluckyPoolIdentifier), 10);
            UsePower(setback);
            AssertIsInPlay(plays); // Played ongoing card is in play.
            QuickHandCheck(-1); // 2 card played, 1 drawn.
        }

        [Test()]
        public void TestSentinels()
        {
            var promos = new Dictionary<string, string>();
            promos.Add("TheSentinelsInstructions", "LazyFanComix.TheSentinelsTagInInstructions");
            SetupGameController(new string[] { "Omnitron", "TheSentinels", "Legacy", "Megalopolis" }, false, promos);

            StartGame();

            Card instructions = FindCardController("TheSentinelsInstructions").Card;
            Assert.IsTrue(instructions.IsPromoCard);
            AssertIsInPlayAndNotUnderCard(mainstay);
            AssertIsInPlayAndNotUnderCard(medico);
            AssertUnderCard(instructions, idealist);
            AssertUnderCard(instructions, writhe);

            DiscardAllCards(sentinels);
            PutInHand("CoordinatedAssault");
            QuickHPStorage(omnitron);
            UsePower(instructions);
            QuickHPCheck(-4);

            AssertIsInPlayAndNotUnderCard(idealist);
            AssertUnderCard(instructions, medico);

            DestroyCard(idealist);
            DestroyCard(mainstay);

            AssertIncapacitated(sentinels);
        }

        [Test()]
        public void TestSentinelsDangItGuise()
        {
            var promos = new Dictionary<string, string>();
            promos.Add("TheSentinelsInstructions", "LazyFanComix.TheSentinelsTagInInstructions");
            SetupGameController(new string[] { "Omnitron", "TheSentinels", "Guise", "Megalopolis" }, false, promos);

            StartGame();

            Card instructions = FindCardController("TheSentinelsInstructions").Card;
            Assert.IsTrue(instructions.IsPromoCard);
            AssertIsInPlayAndNotUnderCard(mainstay);
            AssertIsInPlayAndNotUnderCard(medico);
            AssertUnderCard(instructions, idealist);
            AssertUnderCard(instructions, writhe);

            DecisionSelectPower = instructions;
            DiscardAllCards(guise);
            PlayCard("ICanDoThatToo");

            AssertIsInPlayAndNotUnderCard(idealist);
            AssertIsInPlayAndNotUnderCard(medico);
        }


        [Test()]
        public void TestSetbackDoubleOrNothingNoMatch()
        {
            SetupGameController("BaronBlade", "Setback/LazyFanComix.SetbackDoubleOrNothingCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(setback.CharacterCard.IsPromoCard);

            StartGame();
            Card play = PutInHand("FriendlyFire");
            Card[] bottom = new Card[] {
                PutOnDeck("ExceededExpectations", true),
                PutOnDeck("HighRiskBehavior", true)
            };

            DecisionSelectCardToPlay = play;

            QuickHandStorage(setback);
            UsePower(setback);
            AssertIsInPlay(play); // Played ongoing card is in play.
            QuickHandCheck(-1); // 1 card played, 0 drawn.
            AssertTokenPoolCount(setback.CharacterCard.FindTokenPool(TokenPool.UnluckyPoolIdentifier), 2); // 2 tokens added.
            AssertInTrash(bottom);
        }

        [Test()]
        public void TestSetbackDoubleOrNothingMatch()
        {
            SetupGameController("BaronBlade", "Setback/LazyFanComix.SetbackDoubleOrNothingCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(setback.CharacterCard.IsPromoCard);

            StartGame();
            Card play = PutInHand("FriendlyFire");
            Card[] bottom = new Card[] {
                PutOnDeck("ExceededExpectations", true),
                PutOnDeck("CashOut", true)
            };

            DecisionSelectCardToPlay = play;

            QuickHandStorage(setback);
            UsePower(setback);
            AssertIsInPlay(play); // Played ongoing card is in play.
            QuickHandCheck(1); // 1 card played, 1 drawn.
            AssertTokenPoolCount(setback.CharacterCard.FindTokenPool(TokenPool.UnluckyPoolIdentifier), 0); // 0 tokens added.
            AssertInHand(bottom);
        }

        //[Test()]
        //public void TestSetbackRunOfLuckNoMatch()
        //{
        //    SetupGameController("BaronBlade", "Setback/LazyFanComix.SetbackRunOfLuckCharacter", "Legacy", "Megalopolis");
        //    Assert.IsTrue(setback.CharacterCard.IsPromoCard);

        //    StartGame();
        //    Card play = PutInHand("FriendlyFire");
        //    Card[] top = new Card[] {
        //        PutOnDeck("ExceededExpectations"),
        //        PutOnDeck("HighRiskBehavior")
        //    };

        //    DecisionSelectCardToPlay = play;

        //    QuickHandStorage(setback);
        //    UsePower(setback);
        //    AssertInTrash(top);
        //}

        //[Test()]
        //public void TestSkyScraper()
        //{
        //    SetupGameController("BaronBlade", "SkyScraper/LazyFanComix.SkyScraperConsistentNormalCharacter", "Megalopolis");
        //    Assert.IsTrue(sky.CharacterCard.IsPromoCard);

        //    StartGame();
        //}

        [Test()]
        public void TestStuntmanOnTurn()
        {
            SetupGameController("BaronBlade", "Stuntman/LazyFanComix.StuntmanForeshadowCharacter", "Megalopolis");
            Assert.IsTrue(stunt.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(stunt);

            QuickHandStorage(stunt);
            UsePower(stunt);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestStuntmanOffTurn()
        {
            SetupGameController("BaronBlade", "Stuntman/LazyFanComix.StuntmanForeshadowCharacter", "Megalopolis");
            Assert.IsTrue(stunt.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(stunt);
            UsePower(stunt);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestTachyonControlledPacePowerNoTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/LazyFanComix.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.
        }

        [Test()]
        public void TestTachyonControlledPacePowerOngoingTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/LazyFanComix.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            Card lingering = PutInTrash(GetCardWithLittleEffect(tachyon));

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.

            AssertInPlayArea(tachyon, lingering);
        }

        [Test()]
        public void TestTachyonControlledPacePowerOneshotTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/LazyFanComix.TachyonControlledPaceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(tachyon.CharacterCard.IsPromoCard);

            Card oneshot = PutInTrash("SuckerPunch");

            GoToUsePowerPhase(tachyon);

            QuickHandStorage(tachyon);
            UsePower(tachyon);

            AssertPhaseActionCount(0); // Powers used.

            AssertOnBottomOfDeck(tachyon, oneshot);
        }

        [Test()]
        public void TestTempestNoEnvironmentNonTarget()
        {
            SetupGameController("BaronBlade", "Tempest/LazyFanComix.TempestRisingWindsCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(tempest.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("PlummetingMonorail");

            // Legacy to confirm damage is not boosted.
            UsePower(legacy);

            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            QuickHandStorage(tempest);
            QuickHPStorage(baron);
            UsePower(tempest);
            QuickHPCheck(0); //No hits due to no environment non-Targets.
            QuickHandCheck(1);
        }

        [Test()]
        public void TestTempestEnvironmentNonTarget()
        {
            SetupGameController("BaronBlade", "Tempest/LazyFanComix.TempestRisingWindsCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(tempest.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("PoliceBackup");

            // Legacy to confirm damage is not boosted.
            UsePower(legacy);

            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            QuickHandStorage(tempest);
            QuickHPStorage(baron);
            UsePower(tempest);
            QuickHPCheck(-1); // 1 hits due to no environment non-Targets.
            QuickHandCheck(1);
        }

        [Test()]
        public void TestTempestDestroyedLaterEnvironmentNonTarget()
        {
            SetupGameController("TheDreamer", "Tempest/LazyFanComix.TempestRisingWindsCharacter", "Legacy", "Unity", "MrFixer", "Megalopolis");
            Assert.IsTrue(tempest.CharacterCard.IsPromoCard);

            StartGame();

            DestroyCards((Card c) => c.IsInPlay && c.IsVillain && !c.IsCharacter); // Destroy all villain setup cards to remove non-Dreamer targets.

            // Make fixer lowest for Dreamer redirect.
            DealDamage(fixer, fixer.CharacterCard, 15, DamageType.Melee);

            PutIntoPlay("DrivingMantis");
            Card bee = PutIntoPlay("BeeBot");
            Card shootsFirst = PutIntoPlay("PoliceBackup");
            Card destroyed = PutIntoPlay("HostageSituation");

            DecisionsYesNo = new bool[] { true, false };

            DecisionSelectCards = new Card[] { shootsFirst, bee, null, destroyed };

            // Legacy to confirm damage is not boosted.
            UsePower(legacy);

            QuickHandStorage(tempest);
            QuickHPStorage(dreamer);
            UsePower(tempest);
            QuickHPCheck(0); // 0 Damage - first hit was redirected, second should've been cancelled.
            QuickHandCheck(1);
        }

        [Test()]
        public void TestTempestDestroyedEarlierEnvironmentNonTarget()
        {
            SetupGameController("TheDreamer", "Tempest/LazyFanComix.TempestRisingWindsCharacter", "Legacy", "Unity", "MrFixer", "Megalopolis");
            Assert.IsTrue(tempest.CharacterCard.IsPromoCard);

            StartGame();

            DestroyCards((Card c) => c.IsInPlay && c.IsVillain && !c.IsCharacter); // Destroy all villain setup cards to remove non-Dreamer targets.

            // Make fixer lowest for Dreamer redirect.
            DealDamage(fixer, fixer.CharacterCard, 15, DamageType.Melee);

            PutIntoPlay("DrivingMantis");
            Card bee = PutIntoPlay("BeeBot");
            Card shootsFirst = PutIntoPlay("PoliceBackup");
            PutIntoPlay("HostageSituation");

            DecisionsYesNo = new bool[] { true, false };

            DecisionSelectCards = new Card[] { shootsFirst, bee, null, shootsFirst };

            // Legacy to confirm damage is not boosted.
            UsePower(legacy);

            QuickHandStorage(tempest);
            QuickHPStorage(dreamer);
            UsePower(tempest);
            QuickHPCheck(-1); // 1 Damage - first hit was redirected, second was successful.
            QuickHandCheck(1);
        }

        [Test()]
        public void TestUnityPowerSpareParts()
        {
            SetupGameController("BaronBlade", "Unity/LazyFanComix.UnitySpareParts", "Megalopolis");
            Assert.IsTrue(unity.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(unity);
            UsePower(unity);
            QuickHandCheck(1); // Card draw, no play.
            AssertNumberOfCardsInPlay(unity, 1); // Only character card in play.
            AssertNumberOfCardsInTrash(unity, 0);
        }

        [Test()]
        public void TestUnityPowerSparePartsPlay()
        {
            SetupGameController("BaronBlade", "Unity/LazyFanComix.UnitySpareParts", "Megalopolis");
            Assert.IsTrue(unity.CharacterCard.IsPromoCard);

            StartGame();

            GoToUsePowerPhase(unity);

            PlayCard("ModularWorkbench", 0);
            PlayCard("ModularWorkbench", 1);
            PlayCard("ConstructionPylon", 0);

            Card play = PutInHand("BeeBot");

            DecisionSelectCardToPlay = play;

            QuickHandStorage(unity);
            UsePower(unity);
            QuickHandCheck(0); // Card draw, card play.
            AssertIsInPlay(play);
        }

        [Test()]
        public void TestUnityPowerToolkit()
        {
            SetupGameController("BaronBlade", "Unity/LazyFanComix.UnityToolkit", "Megalopolis");
            Assert.IsTrue(unity.CharacterCard.IsPromoCard);

            StartGame();

            // Set equipment so no unusual ones are used.
            Card card = PutOnDeck("ModularWorkbench");

            UsePower(unity);
            AssertIsInPlay(card);
        }

        public void TestVoidGuardIdealistNoConceptCards()
        {
            SetupGameController("BaronBlade", "VoidGuardTheIdealist/LazyFanComix.VoidGuardTheIdealistStreamOfConsciousnessCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(voidIdealist.CharacterCard.IsPromoCard);

            Card card = PutOnDeck("FlyingStabbyKnives");

            UsePower(voidIdealist);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestVoidGuardMedicoNormalChecks()
        {
            SetupGameController("BaronBlade", "VoidGuardDrMedico/LazyFanComix.VoidGuardDrMedicoOverdoseCharacter", "Megalopolis");
            Assert.IsTrue(voidMedico.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DealDamage(voidMedico, voidMedico.CharacterCard, 5, DamageType.Melee);

            DecisionSelectCards = new Card[] { voidMedico.CharacterCard, mdp };

            QuickHPStorage(voidMedico.CharacterCard, mdp);
            UsePower(voidMedico);
            QuickHPCheck(1, -4); // One successful heal, 1 max HP target damaged.
        }

        [Test()]
        public void TestVoidGuardMedicoTargetWasDamaged()
        {
            SetupGameController("BaronBlade", "VoidGuardDrMedico/LazyFanComix.VoidGuardDrMedicoOverdoseCharacter", "Tachyon", "Megalopolis");
            Assert.IsTrue(voidMedico.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutIntoPlay("SynapticInterruption");

            // First heal/damage Medico and MDP, then redirect to MDP so it no longer qualifies. What happens?
            DecisionSelectCards = new Card[] { tachyon.CharacterCard, mdp, tachyon.CharacterCard, mdp, mdp };

            QuickHPStorage(tachyon.CharacterCard, mdp);
            UsePower(voidMedico);
            QuickHPCheck(0, -4); // One successful heal, 1 max HP target damaged.
        }

        [Test()]
        public void TestVoidGuardIdealistConceptCards()
        {
            SetupGameController("BaronBlade", "VoidGuardTheIdealist/LazyFanComix.VoidGuardTheIdealistStreamOfConsciousnessCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(voidIdealist.CharacterCard.IsPromoCard);

            Card top = PutOnDeck("MonsterOfId"); // Remove from deck so damage doesn't warp tests.
            Card concept = PutIntoPlay("FlyingStabbyKnives");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = concept;

            Card fragment = PutIntoPlay("HedgyHogs");

            QuickHPStorage(mdp);
            UsePower(voidIdealist);
            QuickHPCheck(-2); // Damaged by play.
            AssertUnderCard(concept, fragment); // Moved back under the concept.
            AssertOnTopOfDeck(top); // Not played.
        }

        [Test()]
        public void TestVoidGuardMainstay()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "VoidGuardMainstay/LazyFanComix.VoidGuardMainstayShrugItOffCharacter", "Megalopolis");

            Assert.IsTrue(voidMainstay.CharacterCard.IsPromoCard);

            StartGame();

            QuickHPStorage(voidMainstay);
            QuickHandStorage(voidMainstay);
            UsePower(voidMainstay);
            DealDamage(voidMainstay, voidMainstay, 1, DamageType.Melee);
            DealDamage(voidMainstay, voidMainstay, 2, DamageType.Melee);
            QuickHandCheck(1); // Card drawn.
            QuickHPCheck(-2); // Only the 2 went through.
        }

        [Test()]
        public void TestVoidGuardMainstayIncreasePierces()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "VoidGuardMainstay/LazyFanComix.VoidGuardMainstayShrugItOffCharacter", "Legacy", "TheBlock");

            Assert.IsTrue(voidMainstay.CharacterCard.IsPromoCard);

            StartGame();

            UsePower(legacy);

            QuickHPStorage(voidMainstay);
            QuickHandStorage(voidMainstay);
            UsePower(voidMainstay);
            DealDamage(voidMainstay, voidMainstay, 0, DamageType.Melee);
            QuickHandCheck(1); // Card drawn.
            QuickHPCheck(0); // Increased damage should not work.
        }

        [Test()]
        public void TestVoidGuardMainstayGuiseDangIt()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "VoidGuardMainstay/LazyFanComix.VoidGuardMainstayShrugItOffCharacter", "Guise", "TheHarpy", "TheBlock");

            Assert.IsTrue(voidMainstay.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("AppliedNumerology");

            Card play = PutInHand("ICanDoThatToo");

            DecisionSelectTurnTaker = harpy.TurnTaker;
            DecisionSelectCard = voidMainstay.CharacterCard;
            DecisionYesNo = true;

            PutIntoPlay("UhYeahImThatGuy");

            QuickHPStorage(guise);
            QuickHandStorage(guise);
            PlayCard(play);
            DealDamage(guise, guise, 2, DamageType.Melee); // -1 from Numerology, then prevent. Numerology only works on cards in Guise's play area!
            QuickHandCheck(0); // Played and drawn.
            QuickHPCheck(0); // Prevented.
        }

        [Test()]
        public void TestVoidGuardWrithe()
        {
            SetupGameController("BaronBlade", "VoidGuardWrithe/LazyFanComix.VoidGuardWritheAmorphousGearCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(voidWrithe.CharacterCard.IsPromoCard);

            // Set equipment so no unusual ones are used.
            Card card = PutOnDeck("UmbralSiphon");

            UsePower(voidWrithe);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestVisionary()
        {
            SetupGameController("BaronBlade", "Legacy", "TheVisionary/LazyFanComix.TheVisionaryProphecyCharacter", "Megalopolis");

            Assert.IsTrue(visionary.CharacterCard.IsPromoCard);

            StartGame();

            Card play = GetCardWithLittleEffect(visionary);
            PutOnDeck(visionary, play);

            UsePower(visionary);
            AssertNumberOfCardsAtLocation(visionary.CharacterCard.UnderLocation, 1);

            DecisionSelectFunction = 1;

            UsePower(visionary);
            AssertIsInPlayAndNotUnderCard(play);
        }

        [Test()]
        public void TestWraithPlaySafe()
        {
            SetupGameController("BaronBlade", "TheWraith/LazyFanComix.TheWraithImprovisedGearCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(wraith.CharacterCard.IsPromoCard);

            Card card = PutInHand("MegaComputer");

            DecisionSelectCard = card;

            UsePower(wraith);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestWraithPlayPower()
        {
            SetupGameController("BaronBlade", "TheWraith/LazyFanComix.TheWraithImprovisedGearCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(wraith.CharacterCard.IsPromoCard);

            Card card = PutInHand("StunBolt");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = card;
            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(wraith);
            AssertInTrash(card);
            QuickHPCheck(-2); // Bolted twice.
        }

        [Test()]
        public void TestWeirdCaseRepresentative()
        {
            SetupGameController("BaronBlade", "Legacy", "TheCelestialTribunal");

            StartGame();

            SelectFromBoxForNextDecision("LazyFanComix.BunkerModeShiftCharacter", "Bunker");
            PlayCard("RepresentativeOfEarth");
            AssertIsInPlay("BunkerModeShiftCharacter");
        }

        #region Official Tests
        [Test()]
        public void TestTribunalCompletionistTurn()
        {
            SetupGameController("BaronBlade", "Guise/CompletionistGuiseCharacter", "TheWraith", "TheCelestialTribunal");

            StartGame();
            SelectFromBoxForNextDecision("LegacyCharacter", "Legacy");

            PlayCard("RepresentativeOfEarth");

            Card representative = FindCardInPlay("LegacyCharacter");
            AssertIsInPlay(representative);

            UsePower(representative);

            SelectCardsForNextDecision(representative);
            SelectFromBoxForNextDecision("YoungLegacyCharacter", "Legacy");
            UsePower(guise);

            ResetDecisions();
            SelectCardsForNextDecision(wraith.CharacterCard);
            UsePower(guise);
            ResetDecisions();

            DestroyCard(guise);
            DecisionSelectTurnTaker = representative.Owner;

            UseIncapacitatedAbility(guise, 2);
        }

        #endregion Official Tests
    }
}