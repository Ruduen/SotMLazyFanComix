using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using LazyFanComix.HeroPromos;
using System.Reflection;
using SpookyGhostwriter.Tsukiko;
using Handelabra.Sentinels.Engine.Controller;

namespace LazyFanComixTest
{
    [TestFixture]
    public class SpookyGhostwriterPromosTest : BaseTest
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            // Tell the engine about our mod assembly so it can load up our code.
            // It doesn't matter which type as long as it comes from the mod's assembly.
            ModHelper.AddAssembly("SpookyGhostwriter", Assembly.GetAssembly(typeof(TsukikoCharacterCardController)));
            ModHelper.AddAssembly("LazyFanComix", Assembly.GetAssembly(typeof(PromoDefaultCharacterCardController)), true, true);
        }

        #region Homebrew Tests

        [Test()]
        public void TestEscarlata()
        {
            SetupGameController("BaronBlade", "Legacy", "SpookyGhostwriter.Escarlata/LazyFanComix.EscarlataBurnBrighter", "Megalopolis");

            HeroTurnTakerController Escarlata = FindHero("Escarlata");

            Assert.IsTrue(Escarlata.CharacterCard.IsPromoCard);

            StartGame();
            DestroyNonCharacterVillainCards();
            QuickHPStorage(Escarlata, baron);

            // Power deals 1 damage.
            UsePower(Escarlata);
            QuickHPCheck(0, -1);

            // Power cancels 2 instances of fire, and increases by 1 each. One incoming instance of damage as well, to verify tracking. 
            DealDamage(Escarlata, Escarlata, 3, DamageType.Fire);
            DealDamage(Escarlata, Escarlata, 3, DamageType.Fire);
            DealDamage(baron, Escarlata, 1, DamageType.Fire);
            DealDamage(Escarlata, baron, 1, DamageType.Fire);
            QuickHPCheck(-1, -1 -1 -1);

            // Individual uses consumed.
            DealDamage(Escarlata, baron, 1, DamageType.Fire);
            QuickHPCheck(0, -1);

            // Stacking power - only one cancel each. Two damage.
            UsePower(Escarlata);
            UsePower(Escarlata);
            QuickHPCheck(0, -2);

            // Power cancels 2 instances of fire, and increases by 1 each.
            DealDamage(Escarlata, Escarlata, 1, DamageType.Fire);
            DealDamage(Escarlata, Escarlata, 1, DamageType.Fire);
            DealDamage(Escarlata, baron, 1, DamageType.Fire);
            QuickHPCheck(0, -1 -1 -1);
        }

        //[Test()]
        //public void TestTsukiko()
        //{
        //    SetupGameController("BaronBlade", "Legacy", "SpookyGhostwriter.Tsukiko/LazyFanComix.TsukikoCultivateCharacter", "Megalopolis");

        //    HeroTurnTakerController Tsukiko = FindHero("Tsukiko");

        //    Assert.IsTrue(Tsukiko.CharacterCard.IsPromoCard);

        //    StartGame();

        //    Card topCard = PutOnDeck("BowTie");

        //    UsePower(Tsukiko);

        //    AssertIsTarget(topCard);
        //    AssertIsInPlay(topCard);
        //}
        #endregion
    }
}