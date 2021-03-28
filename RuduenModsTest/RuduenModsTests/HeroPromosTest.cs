using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using RuduenWorkshop.HeroPromos;
using System.Collections.Generic;
using System.Reflection;

namespace RuduenModsTest
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
            ModHelper.AddAssembly("RuduenWorkshop", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController))); // replace with your own namespace
        }

        [Test()]
        public void TestAbsoluteZeroPlay()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/RuduenWorkshop.AbsoluteZeroOverchillCharacter", "TheBlock");

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
        public void TestAbsoluteZeroDestroy()
        {
            SetupGameController("BaronBlade", "AbsoluteZero/RuduenWorkshop.AbsoluteZeroOverchillCharacter", "TheBlock");

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
        public void TestAkashThriyaEnvDestroy()
        {
            SetupGameController("BaronBlade", "AkashThriya/RuduenWorkshop.AkashThriyaPlantEssenceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(thriya.CharacterCard.IsPromoCard);
            Card envCard = PutOnDeck("DefensiveDisplacement");

            MoveAllCards(thriya, thriya.HeroTurnTaker.Deck, thriya.HeroTurnTaker.Trash);

            UsePower(thriya);
            AssertInTrash(envCard);
        }

        [Test()]
        public void TestAkashThriyaMove()
        {
            SetupGameController("BaronBlade", "AkashThriya/RuduenWorkshop.AkashThriyaPlantEssenceCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(thriya.CharacterCard.IsPromoCard);
            Card envCard = PutOnDeck("DefensiveDisplacement");
            Card akCard = PutOnDeck("NoxiousPod");

            DecisionSelectFunction = 1;

            UsePower(thriya);
            AssertIsInPlay(envCard);
            AssertAtLocation(akCard, env.TurnTaker.Deck);
        }

        [Test()]
        public void TestArgentAdeptPlaySafe()
        {
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "TheArgentAdept/RuduenWorkshop.TheArgentAdeptAriaCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "Benchmark/RuduenWorkshop.BenchmarkDownloadManagerCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerFullSalvoCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerFullSalvoCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Bunker/RuduenWorkshop.BunkerModeShiftCharacter", "TheBlock");

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
        public void TestCaptainCosmicNoConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/RuduenWorkshop.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

            StartGame();

            Assert.IsTrue(cosmic.CharacterCard.IsPromoCard);

            QuickHandStorage(cosmic);
            UsePower(cosmic);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestCaptainCosmicConstruct()
        {
            SetupGameController("BaronBlade", "CaptainCosmic/RuduenWorkshop.CaptainCosmicCosmicShieldingCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "ChronoRanger/RuduenWorkshop.ChronoRangerHighNoonCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Expatriette/RuduenWorkshop.ExpatrietteQuickShotCharacter", "Megalopolis");

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
        public void TestExpatriettePowerNoDeck()
        {
            // No cards in deck test.
            SetupGameController("BaronBlade", "Expatriette/RuduenWorkshop.ExpatrietteQuickShotCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Fanatic/RuduenWorkshop.FanaticZealCharacter", "Megalopolis");

            Assert.IsTrue(fanatic.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectPowers = new Card[] { null };

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
            SetupGameController("BaronBlade", "Fanatic/RuduenWorkshop.FanaticZealCharacter", "Megalopolis");

            Assert.IsTrue(fanatic.CharacterCard.IsPromoCard);

            StartGame();

            PutIntoPlay("Absolution");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 1;
            DecisionSelectTargets = new Card[] { mdp, baron.CharacterCard, mdp }; // Attempt to attack MDP twice.

            QuickHPStorage(fanatic.CharacterCard, mdp);
            UsePower(fanatic);
            QuickHPCheck(-1, -4); // Damage dealt to BB (canceled), mdp, and self.
        }

        [Test()]
        public void TestGuiseOngoing()
        {
            SetupGameController("BaronBlade", "Guise/RuduenWorkshop.GuiseShenanigansCharacter", "Megalopolis");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);
            Card ongoing = PutInHand("GrittyReboot");

            DecisionSelectFunction = 0;
            DecisionSelectCard = ongoing;

            QuickHandStorage(guise);
            UsePower(guise);
            QuickHandCheck(-1); // 1 Card Played.
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
            SetupGameController("BaronBlade", "Guise/RuduenWorkshop.GuiseShenanigansCharacter", "Unity", "TimeCataclysm");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);
            Card bee = PlayCard("BeeBot");
            PlayCard("SurpriseShoppingTrip");
            Card ongoing = PutInHand("GrittyReboot");

            DecisionSelectFunction = 0;
            DecisionSelectCard = ongoing;

            AssertNextMessage("Guise does not have any valid Ongoings in play, so he cannot make any indestructible. Whoops!");
            QuickHandStorage(guise);
            UsePower(guise);
            QuickHandCheck(0); // 1 Card Played, 1 Card Drawn from Gritty Reboot
            DestroyCard(ongoing);
            AssertInTrash(ongoing); // Second new turn. Without a power, it's gone!
        }

        [Test()]
        public void TestGuiseNoOngoing()
        {
            SetupGameController("BaronBlade", "Guise/RuduenWorkshop.GuiseShenanigansCharacter", "Megalopolis");
            Assert.IsTrue(guise.CharacterCard.IsPromoCard);

            StartGame();
            GoToUsePowerPhase(guise);

            DiscardAllCards(guise);

            QuickHandStorage(guise);
            AssertNextMessage("Guise cannot play any ongoings, so they must draw 1 card.");
            UsePower(guise);
            QuickHandCheck(1); // 1 Card Drawn.
        }

        [Test()]
        public void TestHaka()
        {
            SetupGameController("BaronBlade", "Haka/RuduenWorkshop.HakaVigorCharacter", "Megalopolis");
            Assert.IsTrue(haka.CharacterCard.IsPromoCard);

            StartGame();

            DiscardAllCards(haka);
            PutInHand("VitalitySurge");
            GoToUsePowerPhase(haka);

            UsePower(haka);
            AssertNumberOfCardsInHand(haka, 2); // Make sure the net effect is 2 cards in hand, even if the played card results in a draw.
        }

        [Test()]
        public void TestHarpy()
        {
            // Equipment Test
            SetupGameController("BaronBlade", "TheHarpy/RuduenWorkshop.TheHarpyExtremeCallingCharacter", "Megalopolis");

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
            SetupGameController("BaronBlade", "TheHarpy/RuduenWorkshop.TheHarpyExtremeCallingCharacter", "Megalopolis");

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
            SetupGameController("BaronBlade", "TheHarpy/RuduenWorkshop.TheHarpyExtremeCallingCharacter", "Megalopolis");

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
            SetupGameController("BaronBlade", "Knyfe/RuduenWorkshop.KnyfeKineticLoopCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Knyfe/RuduenWorkshop.KnyfeKineticLoopCharacter", "TheBlock");
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
            SetupGameController("BaronBlade", "Knyfe/RuduenWorkshop.KnyfeKineticLoopCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Legacy/RuduenWorkshop.LegacyInTheFrayCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Legacy/RuduenWorkshop.LegacyInTheFrayCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Lifeline/RuduenWorkshop.LifelineEnergyTapCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "Lifeline/RuduenWorkshop.LifelineEnergyTapCharacter", "Legacy", "Tachyon", "Megalopolis");
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
            SetupGameController("BaronBlade", "Luminary/RuduenWorkshop.LuminaryReprogramCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Luminary/RuduenWorkshop.LuminaryReprogramCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Luminary/RuduenWorkshop.LuminaryReprogramCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheNaturalist/RuduenWorkshop.TheNaturalistVolatileFormCharacter", "Megalopolis");
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
        //    SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
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
        public void TestMrFixerPowerB()
        {
            // Tool not in hand.
            SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
            Assert.IsTrue(fixer.CharacterCard.IsPromoCard);

            StartGame();
            UsePower(legacy);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            MoveAllCards(fixer, fixer.HeroTurnTaker.Hand, fixer.HeroTurnTaker.Deck);

            Card tool = PutInHand("DualCrowbars");

            DecisionSelectTarget = mdp;

            QuickHPStorage(fixer.CharacterCard, mdp);
            UsePower(fixer);
            QuickHPCheck(-1, -2);
            AssertInPlayArea(fixer, tool); // Card put into play.
        }

        [Test()]
        public void TestMrFixerPowerC()
        {
            // Tool not in hand and empty deck.
            SetupGameController("BaronBlade", "MrFixer/RuduenWorkshop.MrFixerFlowingStrikeCharacter", "Legacy", "Megalopolis");
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
            QuickHPCheck(-1, -2);
            QuickHandCheck(2);
            AssertNotInPlay((Card c) => c.IsTool);
        }

        [Test()]
        public void TestLaComodoraPower()
        {
            SetupGameController("BaronBlade", "LaComodora/RuduenWorkshop.LaComodoraTemporalScavengeCharacter", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            GoToPlayCardPhase(comodora);

            Card equip = PlayCard("ConcordantHelm");

            GoToUsePowerPhase(comodora);
            UsePower(comodora);

            DecisionYesNo = true;
            DecisionSelectCard = equip;

            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);

            GoToUsePowerPhase(comodora);
            UsePower(comodora);
            AssertNotFlipped(equip);
            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            // Make sure effect has been re-applied and therefore still works here.
            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);
        }

        [Test()]
        public void TestLaComodoraPowerGuiseDangIt()
        {
            SetupGameController("BaronBlade", "LaComodora/RuduenWorkshop.LaComodoraTemporalScavengeCharacter", "Guise/SantaGuiseCharacter", "Megalopolis");
            Assert.IsTrue(comodora.CharacterCard.IsPromoCard);

            StartGame();

            GoToPlayCardPhase(comodora);

            Card equip = PlayCard("ConcordantHelm");

            GoToUsePowerPhase(comodora);
            UsePower(comodora);

            DecisionYesNo = true;
            DecisionSelectCard = equip;

            GoToStartOfTurn(baron);
            PutIntoPlay("DeviousDisruption");

            AssertFlipped(equip);
            AssertInPlayArea(comodora, equip);

            UsePower(guise, 1);
            AssertNotFlipped(equip);
        }

        [Test()]
        public void TestNightMistPowerDraw()
        {
            SetupGameController("BaronBlade", "NightMist/RuduenWorkshop.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "NightMist/RuduenWorkshop.NightMistLimitedNumerologyCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "OmnitronX/RuduenWorkshop.OmnitronXElectroShieldedSystemsCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestParsePowerTrashSkipAttack()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            DiscardTopCards(baron, 2);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = null;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(0);
            AssertNumberOfCardsInTrash(baron, 2);
        }

        [Test()]
        public void TestParsePowerTrashAttack()
        {
            // Tool in hand.
            SetupGameController("BaronBlade", "Parse/RuduenWorkshop.ParseLaplaceShotCharacter", "Megalopolis");
            Assert.IsTrue(parse.CharacterCard.IsPromoCard);

            StartGame();

            DiscardTopCards(env, 2);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            DecisionYesNo = true;

            QuickHPStorage(mdp);
            UsePower(parse);
            QuickHPCheck(-2);
            AssertNumberOfCardsInTrash(baron, 0);
        }

        [Test()]
        public void TestRaNormalPiercingHit()
        {
            SetupGameController("BaronBlade", "Ra/RuduenWorkshop.RaPiercingBlastCharacter", "TheBlock");
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
            SetupGameController("BaronBlade", "Ra/RuduenWorkshop.RaPiercingBlastCharacter", "TheBlock");
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
            SetupGameController("BaronBlade", "Ra/RuduenWorkshop.RaPiercingBlastCharacter", "MrFixer", "TheBlock");
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
            SetupGameController("BaronBlade", "TheScholar/RuduenWorkshop.TheScholarEquilibriumCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "TheScholar/RuduenWorkshop.TheScholarEquilibriumCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "Setback/RuduenWorkshop.SetbackRunOfLuckCharacter", "Legacy", "Megalopolis");
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
        public void TestSetbackDoubleOrNothingNoMatch()
        {
            SetupGameController("BaronBlade", "Setback/RuduenWorkshop.SetbackDoubleOrNothingCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "Setback/RuduenWorkshop.SetbackDoubleOrNothingCharacter", "Legacy", "Megalopolis");
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
        //    SetupGameController("BaronBlade", "Setback/RuduenWorkshop.SetbackRunOfLuckCharacter", "Legacy", "Megalopolis");
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
        //    SetupGameController("BaronBlade", "SkyScraper/RuduenWorkshop.SkyScraperConsistentNormalCharacter", "Megalopolis");
        //    Assert.IsTrue(sky.CharacterCard.IsPromoCard);

        //    StartGame();
        //}

        [Test()]
        public void TestStuntmanOnTurn()
        {
            SetupGameController("BaronBlade", "Stuntman/RuduenWorkshop.StuntmanForeshadowCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "Stuntman/RuduenWorkshop.StuntmanForeshadowCharacter", "Megalopolis");
            Assert.IsTrue(stunt.CharacterCard.IsPromoCard);

            StartGame();

            QuickHandStorage(stunt);
            UsePower(stunt);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestTachyonControlledPacePowerNoTrash()
        {
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Tachyon/RuduenWorkshop.TachyonControlledPaceCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Tempest/RuduenWorkshop.TempestRisingWindsCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("BaronBlade", "Tempest/RuduenWorkshop.TempestRisingWindsCharacter", "Legacy", "Megalopolis");
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
            SetupGameController("TheDreamer", "Tempest/RuduenWorkshop.TempestRisingWindsCharacter", "Legacy", "Unity", "MrFixer", "Megalopolis");
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
            SetupGameController("TheDreamer", "Tempest/RuduenWorkshop.TempestRisingWindsCharacter", "Legacy", "Unity", "MrFixer", "Megalopolis");
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
            SetupGameController("BaronBlade", "Unity/RuduenWorkshop.UnitySpareParts", "Megalopolis");
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
            SetupGameController("BaronBlade", "Unity/RuduenWorkshop.UnitySpareParts", "Megalopolis");
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
            SetupGameController("BaronBlade", "Unity/RuduenWorkshop.UnityToolkit", "Megalopolis");
            Assert.IsTrue(unity.CharacterCard.IsPromoCard);

            StartGame();

            // Set equipment so no unusual ones are used.
            Card card = PutOnDeck("ModularWorkbench");

            UsePower(unity);
            AssertIsInPlay(card);
        }

        public void TestVoidGuardIdealistNoConceptCards()
        {
            SetupGameController("BaronBlade", "VoidGuardTheIdealist/RuduenWorkshop.VoidGuardTheIdealistStreamOfConsciousnessCharacter", "TheBlock");

            StartGame();

            Assert.IsTrue(voidIdealist.CharacterCard.IsPromoCard);

            Card card = PutOnDeck("FlyingStabbyKnives");

            UsePower(voidIdealist);
            AssertIsInPlay(card);
        }

        [Test()]
        public void TestVoidGuardMedicoNormalChecks()
        {
            SetupGameController("BaronBlade", "VoidGuardDrMedico/RuduenWorkshop.VoidGuardDrMedicoOverdoseCharacter", "Megalopolis");
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
            SetupGameController("BaronBlade", "VoidGuardDrMedico/RuduenWorkshop.VoidGuardDrMedicoOverdoseCharacter", "Tachyon", "Megalopolis");
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
            SetupGameController("BaronBlade", "VoidGuardTheIdealist/RuduenWorkshop.VoidGuardTheIdealistStreamOfConsciousnessCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "VoidGuardMainstay/RuduenWorkshop.VoidGuardMainstayShrugItOffCharacter", "Megalopolis");

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
            SetupGameController("BaronBlade", "VoidGuardMainstay/RuduenWorkshop.VoidGuardMainstayShrugItOffCharacter", "Legacy", "TheBlock");

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
            SetupGameController("BaronBlade", "VoidGuardMainstay/RuduenWorkshop.VoidGuardMainstayShrugItOffCharacter", "Guise", "TheHarpy", "TheBlock");

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
            SetupGameController("BaronBlade", "VoidGuardWrithe/RuduenWorkshop.VoidGuardWritheAmorphousGearCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "Legacy", "TheVisionary/RuduenWorkshop.TheVisionaryProphesizeDoomCharacter", "Megalopolis");

            Assert.IsTrue(visionary.CharacterCard.IsPromoCard);

            StartGame();

            DestroyCard(FindCardInPlay("MobileDefensePlatform"));

            // Stack with mind spikes for simplicity.
            PutOnDeck(visionary, FindCardsWhere((Card c) => c.Identifier == "MindSpike"));

            UsePower(legacy);
            UsePower(legacy);

            DecisionSelectTarget = baron.CharacterCard;

            AssertHitPoints(baron, 40);
            UsePower(visionary);
            AssertHitPoints(baron, 35); // Spike 1.
            UsePower(visionary);
            AssertHitPoints(baron, 30); // Spike 2.
            UsePower(visionary);
            AssertHitPoints(baron, 26); // Environment hit. Not Legacy boosted.
        }

        [Test()]
        public void TestWraithPlaySafe()
        {
            SetupGameController("BaronBlade", "TheWraith/RuduenWorkshop.TheWraithImprovisedGearCharacter", "TheBlock");

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
            SetupGameController("BaronBlade", "TheWraith/RuduenWorkshop.TheWraithImprovisedGearCharacter", "TheBlock");

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
    }
}