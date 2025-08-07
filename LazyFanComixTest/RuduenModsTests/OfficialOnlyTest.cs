using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using LazyFanComix.HeroPromos;
using NUnit.Framework;
using SpookyGhostwriter.Tsukiko;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LazyFanComixTest
{
  [TestFixture]
  public class OfficialOnlyTest : BaseTest
  {
    [OneTimeSetUp]
    public void DoSetup()
    {
    }

    #region Official Tests

    [Test()]
    public void TestLaComodoraRigging()
    {
      SetupGameController("BaronBlade", "LaComodora", "TheWraith", "TheCelestialTribunal");

      StartGame();

      PlayCard("TemporalRigging");
      GoToEndOfTurn(comodora);
    }

    #endregion Official Tests
  }
}