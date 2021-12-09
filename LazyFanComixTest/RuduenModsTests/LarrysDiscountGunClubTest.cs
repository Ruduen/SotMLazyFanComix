using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Reflection;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using LazyFanComix.LarrysDiscountGunClub;

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

            Card ammo = PlayCard("HollowPointCache");
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestAmmoNoGunOnlyAmmoRemains()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            MoveCards(env, (Card c) => c.IsEnvironment && !c.IsAmmo, env.TurnTaker.OutOfGame);

            Card ammo = PlayCard("HollowPointCache");
            AssertInTrash(ammo);

        }


        [Test()]
        public void TestGunMovementFriendly()
        {
            SetupGameController("Omnitron", "Expatriette/DarkWatchExpatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            Card pistol = PlayCard("GooLauncher");
            Card pistolHero = GetCard("GooLauncherHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);

            PlayCard("TechnologicalSingularity");
            AssertInTrash(expatriette, pistolHero);

            PlayCard(pistolHero);
            AssertInPlayArea(expatriette, pistolHero);

            QuickHPStorage(omnitron);
            UsePower(expatriette);
            UsePower(pistolHero);
            QuickHPCheck(-3); // Confirm damage is coming from correct source, and is increased appropriately. 

        }

        [Test()]
        public void TestGunMovementStolen()
        {
            SetupGameController("LaCapitan", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(capitan);

            Card pistol = PlayCard("GooLauncher");
            Card pistolHero = GetCard("GooLauncherHero");
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

            Card pistol = PlayCard("GooLauncher");
            Card pistolHero = GetCard("GooLauncherHero");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, pistolHero);
            AssertOffToTheSide(pistol);

            DestroyCard(expatriette);
            AssertOutOfGame(pistolHero);
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
            Card ammo = PlayCard("HollowPointCache");

            QuickHPStorage(expatriette);
            GoToStartOfTurn(env);
            QuickHPCheck(-4 - 3);
            AssertInTrash(ammo);
        }

        [Test()]
        public void TestVillainGunTShirtCannonAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "Legacy", "VoidGuardWrithe", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            GoToEndOfTurn(expatriette);

            PlayCard("TShirtCannon");
            Card ammo = PlayCard("HollowPointCache");

            QuickHPStorage(expatriette, legacy, voidWrithe);
            QuickHandStorage(expatriette, legacy, voidWrithe);
            GoToStartOfTurn(env);
            QuickHPCheck(-3, -3, 0);
            QuickHandCheck(-1, -1, 0);
            AssertInTrash(ammo);
        }


        [Test()]
        public void TestGooLauncherGainPowerAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("GooLauncher");
            Card gunHero = GetCard("GooLauncherHero");
            Card ammo = PlayCard("ConcussiveRounds");
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, gunHero);
            AssertOffToTheSide(gun);
            AssertNextToCard(ammo, gunHero);
            QuickHandCheck(-3);

            QuickHPStorage(omnitron);
            UsePower(gunHero);
            QuickHPCheck(-2 - 2 - 1); // Omnitron's damage is increased. 
            DealDamage(expatriette, omnitron, 2, DamageType.Sonic);
            QuickHPCheck(-2 - 1);
        }

        [Test()]
        public void TestGooLauncherFailGain()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            DestroyNonCharacterVillainCards();

            Card gun = PlayCard("GooLauncher");
            Card gunHero = GetCard("GooLauncherHero");
            DiscardAllCards(expatriette);
            DrawCard(expatriette, 2);

            QuickHandStorage(expatriette);
            GoToStartOfTurn(expatriette);
            AssertIsInPlay(gun);
            AssertOffToTheSide(gunHero);
            QuickHandCheck(-2);
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
        public void TestOverengineeredSlingshotGainPowerAmmo()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            QuickHandStorage(expatriette);
            GoToEndOfTurn(omnitron);

            Card gun = PlayCard("OverengineeredSlingshot");
            Card gunHero = GetCard("OverengineeredSlingshotHero");
            Card ammo = PlayCard("HollowPointCache");
            QuickHPStorage(expatriette);
            GoToStartOfTurn(expatriette);
            AssertInPlayArea(expatriette, gunHero);
            AssertOffToTheSide(gun);
            AssertNextToCard(ammo, gunHero);
            QuickHPCheck(-4);

            QuickHPStorage(omnitron);
            UsePower(gunHero);
            QuickHPCheck(-1 - 1 - 2 - 2);
        }

        [Test()]
        public void TestAmmoHollowPoint()
        {
            SetupGameController("Omnitron", "Expatriette", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();

            Card gun = PlayCard("AssaultRifle");
            Card ammo = PlayCard("HollowPointCache");
            AssertNextToCard(ammo, gun);

            QuickHPStorage(omnitron);
            UsePower(gun);
            QuickHPCheck(-2 - 2);

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

            Card notPlayed = PutOnDeck("GooLauncher");
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

            QuickHPStorage(dealer);
            GoToStartOfTurn(env);
            QuickHPCheck(1);
            GoToEndOfTurn(env);
            AssertIsInPlay(envir);

            DestroyCard(dealer);
            AssertIsInPlay(villain);

            QuickHandStorage(expatriette);
            PlayCard(dealer);
            DealDamage(dealer, dealer, 30, DamageType.Fire);
            AssertNotInPlay(villain2);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestStandoff()
        {
            SetupGameController("Omnitron", "Expatriette", "Haka", "LazyFanComix.LarrysDiscountGunClub");

            StartGame();


            Card play=PlayCard("Standoff");
            AssertNumberOfCardsInPlay((Card c) => c.IsGun, 1);

            QuickHPStorage(omnitron, expatriette, haka);
            DealDamage(omnitron, haka, 5, DamageType.Fire);
            DealDamage(haka, omnitron, 5, DamageType.Fire);
            // Highest are immune. 
            QuickHPCheck(0, 0, 0);

            DealDamage(omnitron, expatriette, 5, DamageType.Fire);
            DealDamage(expatriette, omnitron, 5, DamageType.Fire);

            // Damage is dealt. 
            QuickHPCheck(-5, -5, 0);

            SetHitPoints(expatriette, 15);
            SetHitPoints(haka, 15);

            DecisionYesNo = true;
            QuickHPStorage(omnitron, expatriette, haka);
            DealDamage(omnitron, haka, 5, DamageType.Fire);
            DealDamage(haka, omnitron, 5, DamageType.Fire);
            DealDamage(expatriette, omnitron, 5, DamageType.Fire);
            DealDamage(omnitron, expatriette, 5, DamageType.Fire);
            // Tied highest are immune if yes. 
            QuickHPCheck(0, 0, 0);

            DecisionYesNo = false;
            DealDamage(omnitron, haka, 5, DamageType.Fire);
            DealDamage(haka, omnitron, 5, DamageType.Fire);
            DealDamage(expatriette, omnitron, 5, DamageType.Fire);
            DealDamage(omnitron, expatriette, 5, DamageType.Fire);

            // First damage was not prevented, so damage is otherwise automatic for the rest.
            QuickHPCheck(-5, 0, -5);

            GoToStartOfTurn(env);
            AssertInTrash(play);

        }


        #endregion Card Tests

    }
}