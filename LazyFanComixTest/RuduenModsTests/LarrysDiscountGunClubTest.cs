using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using LazyFanComix.LarrysDiscountGunClub;
using System.Collections.Generic;
using SpookyGhostwriter.Tsukiko;

namespace LazyFanComixTest
{
    [TestFixture]
    public class LarrysDiscountGunClubTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            //var a = Assembly.GetAssembly(typeof(InquirerCharacterCardController)); // replace with your own type
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(SonicCannonCardController))); // replace with your own namespace
            ModHelper.AddAssembly("SpookyGhostwriter", Assembly.GetAssembly(typeof(SGC_TsukikoCharacterCardController)));
        }

        #region Load Tests


        [Test()]
        public void TestModWorks()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsInstanceOf(typeof(TurnTakerController), env);
        }

        #endregion Load Tests

        #region Generic Tests

        [Test()]
        public void TestAmmoNoGun()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            PutInTrash("LockedAndLoaded", 0);
            PutInTrash("LockedAndLoaded", 1);
            Card ammo = PlayCard("QuantumRounds");
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestAmmoNoGunOnlyAmmoRemains()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            MoveCards(env, (Card c) => c.IsEnvironment && !c.IsAmmo, env.TurnTaker.OutOfGame);

            Card ammo = PlayCard("QuantumRounds");
            AssertInTrash(ammo);

        }


        [Test()]
        public void TestGunMovementFriendly()
        {
            SetupGameController("Omnitron", "Expatriette/DarkWatchExpatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            Card pistol = PlayCard("TShirtCannon");
            Card pistolHero = GetCard("TShirtCannonHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);

            PlayCard("TechnologicalSingularity");
            AssertInTrash(expatriette, pistolHero);

            PlayCard(pistolHero);
            AssertInPlayArea(expatriette, pistolHero);

            QuickHPStorage(omnitron);
            UsePower(expatriette);
            DiscardAllCards(expatriette);
            UsePower(pistolHero);
            QuickHPCheck(-4); // Confirm damage is coming from correct source, and is increased appropriately. 

        }

        [Test()]
        public void TestGunMovementStolen()
        {
            SetupGameController("LaCapitan", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(capitan);

            Card pistol = PlayCard("TShirtCannon");
            Card pistolHero = GetCard("TShirtCannonHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);

            MoveCard(capitan, pistolHero, capitan.CharacterCard.UnderLocation);
            Assert.IsTrue(pistolHero.Owner == expatriette.TurnTaker);

            DestroyCard(pistolHero);
            AssertInTrash(expatriette, pistolHero);
        }


        [Test()]
        public void TestGunMovementIncap()
        {
            SetupGameController("LaCapitan", "Expatriette", "Legacy", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(capitan);

            Card pistol = PlayCard("TShirtCannon");
            Card pistolHero = GetCard("TShirtCannonHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);

            DestroyCard(expatriette);
            AssertOutOfGame(pistolHero);
        }


        [Test()]
        public void TestGunFullAmmo()
        {
            SetupGameController("LaCapitan", "Expatriette", "Legacy", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(capitan);

            Card gun = PlayCard("TShirtCannon");
            PlayCard("HollowPoints");
            Card failToPlay = PlayCard("IncendiaryRounds");
            AssertInTrash(failToPlay);

            Card gunHero = GetCard("TShirtCannonHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, gunHero);
            AssertOffToTheSide(gun);

            PlayCard(failToPlay);
            AssertInTrash(failToPlay);
        }


        #endregion Generic Tests

        #region Card Tests

        [Test()]
        public void TestVillainGunSonicCannonAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(expatriette);

            PlayCard("SonicCannon");
            Card ammo = PlayCard("QuantumRounds");

            QuickHPStorage(expatriette);
            GoToStartOfTurn(env);
            QuickHPCheck(-4 - 3);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestVillainGunPaintballMortarAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "Legacy", "VoidGuardWrithe", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(expatriette);

            PlayCard("PaintballMortar");
            Card ammo = PlayCard("QuantumRounds");

            QuickHPStorage(expatriette, legacy, voidWrithe);
            QuickHandStorage(expatriette, legacy, voidWrithe);
            GoToStartOfTurn(env);
            QuickHPCheck(-3, -3, 0);
            QuickHandCheck(-1, -1, 0);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestVillainGunBloodLeechAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "Legacy", "VoidGuardWrithe", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(expatriette);

            PlayCard("TheBloodLeech");
            Card ammo = PlayCard("QuantumRounds");

            SetHitPoints(omnitron, 50);

            QuickHPStorage(omnitron, expatriette, legacy, voidWrithe);
            GoToStartOfTurn(env);
            QuickHPCheck(7, -1, -1, -1);
            AssertInTrash(ammo);
        }


        [Test()]
        public void TestTShirtCannonGainPowerAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("TShirtCannon");
            Card gunHero = GetCard("TShirtCannonHero");
            Card ammo = PlayCard("ConcussiveRounds");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, gunHero);
            AssertOffToTheSide(gun);
            AssertNextToCard(ammo, gunHero);
            QuickHandCheck(-1);

            QuickHPStorage(omnitron);
            DecisionSelectCard = PutInHand("Unload");
            UsePower(gunHero);
            QuickHPCheck(-3 - 0 - 2);
            DecisionSelectCard = PutInHand("AssaultRifle");
            UsePower(gunHero);
            QuickHPCheck(-3 - 1);
        }


        [Test()]
        public void TestTShirtCannonGainPowerTsukiko()
        {
            SetupGameController("Omnitron", "SpookyGhostwriter.Tsukiko", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            HeroTurnTakerController Tsukiko = FindHero("Tsukiko");

            QuickHandStorage(Tsukiko);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("TShirtCannon");
            Card gunHero = GetCard("TShirtCannonHero");
            GoToStartOfTurn(Tsukiko);
            AssertInPlayArea(Tsukiko, gunHero);
            AssertOffToTheSide(gun);

            QuickHPStorage(omnitron);
            DecisionSelectCard = PutInHand("SGC_HighHeals");
            UsePower(gunHero);
            QuickHPCheck(-3 - 1 - 1);
        }


        [Test()]
        public void TestTShirtCannonFailGain()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("TShirtCannon");
            Card gunHero = GetCard("TShirtCannonHero");
            DiscardAllCards(expatriette);

            QuickHandStorage(expatriette);
            GoToStartOfTurn(expatriette);
            AssertIsInPlay(gun);
            AssertOffToTheSide(gunHero);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestGlitterGunGainPowerAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            Card pistol = PlayCard("GlitterGun");
            Card pistolHero = GetCard("GlitterGunHero");
            Card ammo = PlayCard("ConcussiveRounds");
            Card destroyed = PlayCard("AssaultRifle");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);
            AssertNextToCard(ammo, pistolHero);
            AssertInTrash(destroyed);

            QuickHPStorage(omnitron);
            UsePower(pistolHero);
            QuickHPCheck(-1 - 2);
            QuickHPStorage(expatriette);
            DealDamage(omnitron, expatriette, 2, DamageType.Sonic);
            QuickHPCheck(-2 + 1);
        }


        [Test()]
        public void TestGlitterGunNumerology()
        {
            SetupGameController("Omnitron", "TheHarpy", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(omnitron);

            Card pistol = PlayCard("GlitterGun");
            Card pistolHero = GetCard("GlitterGunHero");
            Card numeral = PlayCard("AppliedNumerology");
            GoToStartOfTurn(harpy);
            AssertInPlayArea(harpy, pistolHero);
            AssertOffToTheSide(pistol);
            AssertInTrash(numeral);

            string description = pistolHero.GetPowerDescription(0);
            List<string> numeralString = pistolHero.Definition.ExtractPowerNumeralsFromPowerDescription(description).ToList();

            Assert.IsTrue(numeralString.Count() == 3);

            PlayCard(numeral);
            UsePower(pistolHero);
        }

        [Test()]
        public void TestOverengineeredSlingshotGainPowerAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            Card gun = PlayCard("OverengineeredSlingshot");
            Card gunHero = GetCard("OverengineeredSlingshotHero");
            Card ammo = PutOnDeck("HollowPoints");
            QuickHPStorage(expatriette);
            DecisionYesNo = true;
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, gunHero);
            AssertOffToTheSide(gun);
            QuickHPCheck(-3); // Damage to take.

            Card stack = expatriette.HeroTurnTaker.Hand.Cards.FirstOrDefault();
            DecisionSelectCard = stack;

            QuickHPStorage(omnitron);
            UsePower(gunHero);
            QuickHPCheck(-2 - 2);
            AssertOnTopOfLocation(stack, expatriette.HeroTurnTaker.Deck, 1);
        }

        [Test()]
        public void TestOverengineeredSlingshotRedirected()
        {
            SetupGameController("Omnitron", "Tachyon", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            // Even if damage is redirected, the card is still earned.

            QuickHandStorage(tachyon);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("OverengineeredSlingshot");
            Card gunHero = GetCard("OverengineeredSlingshotHero");
            PlayCard("SynapticInterruption");
            QuickHPStorage(tachyon, omnitron);
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { omnitron.CharacterCard };
            GoToStartOfTurn(tachyon);
            AssertInPlayArea(tachyon, gunHero);
            AssertOffToTheSide(gun);
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestOverengineeredSlingshotPass()
        {
            SetupGameController("Omnitron", "Tachyon", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(tachyon);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("OverengineeredSlingshot");
            Card gunHero = GetCard("OverengineeredSlingshotHero");
            QuickHPStorage(tachyon);
            DecisionYesNo = false;
            GoToStartOfTurn(tachyon);
            AssertInPlayArea(env, gun);
            AssertOffToTheSide(gunHero);
            QuickHPCheck(0);
        }


        [Test()]
        public void TestAmmoQuantumRounds()
        {
            SetupGameController("AmbuscadeTeam", "Expatriette", "Haka", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();
            RemoveVillainCards();

            Card glamour = PlayCard("Glamour");
            PlayCard("TaMoko");
            SetHitPoints(haka, 6);
            SetHitPoints(expatriette, 5);

            Card gun = PlayCard("AssaultRifle");
            Card ammo = PlayCard("QuantumRounds");
            AssertNextToCard(ammo, gun);

            DecisionSelectCards = new Card[] { ambuscadeTeam.CharacterCard, glamour, null };
            QuickHPStorage(ambuscadeTeam.CharacterCard, glamour);
            UsePower(gun);
            QuickHPCheck(-2, 0);

            AssertIncapacitated(haka);
            AssertInTrash(ammo);
        }


        [Test()]
        public void TestAmmoConcussive()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            Card gun = PlayCard("AssaultRifle");
            Card ammo = PlayCard("ConcussiveRounds");
            AssertNextToCard(ammo, gun);

            QuickHPStorage(omnitron);
            UsePower(gun);
            QuickHPCheck(-2 - 2);

            AssertInTrash(ammo);
        }

        [Test()]
        public void TestLockedAndLoaded()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            Card notPlayed = PutOnDeck("TShirtCannon");
            Card play = PlayCard("LockedAndLoaded");



            AssertNumberOfCardsInPlay((Card c) => c.IsGun && c != notPlayed, 1);
            AssertNumberOfCardsInPlay((Card c) => c.IsAmmo, 1);
            AssertInTrash(play);
        }

        [Test()]
        public void TestLarry()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(expatriette);

            Card villain2 = PutOnDeck("InterpolationBeam");
            Card villain = PutOnDeck("ElectroPulseExplosive");
            Card envir = PutOnDeck("SonicCannon");
            PlayCard("AssaultRifle");

            Card dealer = PlayCard("Larry");
            SetHitPoints(dealer, 10);

            QuickHPStorage(dealer, expatriette.CharacterCard);
            GoToStartOfTurn(env);
            QuickHPCheck(1, -1 - 1); // 1 base, 1 nemesis
            GoToEndOfTurn(env);
            AssertIsInPlay(envir);

            DestroyCard(dealer);
            AssertIsInPlay(villain);

            PlayCard(dealer);
            DealDamage(dealer, dealer, 30, DamageType.Fire);
            AssertNotInPlay(villain2);
        }

        [Test()]
        public void TestStandoff()
        {
            SetupGameController("BaronBlade", "Expatriette", "Haka", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();
            DestroyNonCharacterVillainCards();

            Card play = PlayCard("Standoff");
            AssertNumberOfCardsInPlay((Card c) => c.IsGun, 1);

            QuickHPStorage(baron, expatriette, haka);
            DealDamage(baron, haka, 5, DamageType.Fire);
            DealDamage(haka, baron, 5, DamageType.Fire);
            // Highest are immune. 
            QuickHPCheck(0, 0, 0);

            DealDamage(baron, expatriette, 5, DamageType.Fire);
            DealDamage(expatriette, baron, 5, DamageType.Fire);

            // Damage is dealt. 
            QuickHPCheck(-5, -5, 0);

            SetHitPoints(expatriette, 15);
            SetHitPoints(haka, 15);

            DecisionYesNo = true;
            QuickHPStorage(baron, expatriette, haka);
            DealDamage(baron, haka, 5, DamageType.Fire);
            DealDamage(haka, baron, 5, DamageType.Fire);
            DealDamage(expatriette, baron, 5, DamageType.Fire);
            DealDamage(baron, expatriette, 5, DamageType.Fire);
            // Tied highest are immune if yes. 
            QuickHPCheck(0, 0, 0);

            DecisionYesNo = false;
            DealDamage(baron, haka, 5, DamageType.Fire);
            DealDamage(haka, baron, 5, DamageType.Fire);
            DealDamage(expatriette, baron, 5, DamageType.Fire);
            DealDamage(baron, expatriette, 5, DamageType.Fire);

            // First damage was not prevented, so damage is otherwise automatic for the rest.
            QuickHPCheck(-5, 0, -5);

            GoToStartOfTurn(env);
            AssertInTrash(play);
            AssertIsInPlay("Larry");
        }


        #endregion Card Tests

    }
}