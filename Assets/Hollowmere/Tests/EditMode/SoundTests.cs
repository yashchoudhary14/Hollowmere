using Hollowmere.EditorTools;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Hollowmere.Tests
{
    /// <summary>
    /// The M1.5 audio pass: SoundBank plays the SoundLibrary's recorded clips and falls back to synthesised ones,
    /// footsteps pick a surface, knocks repeat into a knocking, and the library is filled.
    /// </summary>
    public class SoundTests
    {
        SoundLibrary _saved;

        [SetUp] public void Save() => _saved = SoundBank.Library;
        [TearDown] public void Restore() => SoundBank.Library = _saved;

        static SoundLibrary Built()
        {
            var lib = AssetDatabase.LoadAssetAtPath<SoundLibrary>(SoundLibraryBuilder.LibraryPath);
            if (lib == null) Assert.Ignore("Sound library not built yet.");
            return lib;
        }

        [Test]
        public void SurfaceFor_ReadsTheBuilderMaterials()
        {
            var village = new Vector3(10f, 0f, 10f);
            var forest = new Vector3(170f, 0f, 10f);
            Assert.That(SoundBank.SurfaceFor("Floor", village), Is.EqualTo(Surface.Wood));
            Assert.That(SoundBank.SurfaceFor("BridgeWood", forest), Is.EqualTo(Surface.Wood));
            Assert.That(SoundBank.SurfaceFor("Paving", village), Is.EqualTo(Surface.Stone));
            Assert.That(SoundBank.SurfaceFor("HearthStone", village), Is.EqualTo(Surface.Stone));
            Assert.That(SoundBank.SurfaceFor("Ground", village), Is.EqualTo(Surface.Dirt));
            Assert.That(SoundBank.SurfaceFor("Ground", forest), Is.EqualTo(Surface.Leaves), "beyond the wall is the forest floor");
            Assert.That(SoundBank.SurfaceFor("Road", new Vector3(0f, 0f, 250f)), Is.EqualTo(Surface.Dirt), "the fog road is mud, not leaves");
            Assert.That(SoundBank.SurfaceFor("", village), Is.EqualTo(Surface.Dirt));
        }

        [Test]
        public void WithoutALibrary_EverySoundIsSynthesised()
        {
            SoundBank.Library = null;
            Assert.That(SoundBank.Creak, Is.Not.Null);
            Assert.That(SoundBank.Creak.name, Is.EqualTo("Creak"));
            Assert.That(SoundBank.Footstep(Surface.Wood).name, Is.EqualTo("Step"));
            Assert.That(SoundBank.Knock.name, Is.EqualTo("Knock"));
            Assert.That(SoundBank.Chant.name, Is.EqualTo("Chant"));
            Assert.That(SoundBank.BellStrike, Is.Null, "no recorded strike: the synthesised bell rings alone");
            Assert.That(SoundBank.GatePitch, Is.EqualTo(0.55f));
        }

        [Test]
        public void WithTheLibrary_RecordedClipsPlay_InTurn()
        {
            var lib = Built();
            SoundBank.Library = lib;
            Assert.That(lib.DoorCreak, Has.Member(SoundBank.Creak));
            Assert.That(lib.StepWood, Has.Member(SoundBank.Footstep(Surface.Wood)));
            Assert.That(lib.StepStone, Has.Member(SoundBank.Footstep(Surface.Stone)));
            Assert.That(lib.StepLeaves, Has.Member(SoundBank.Footstep(Surface.Leaves)));
            Assert.That(lib.LandDirt, Has.Member(SoundBank.Landing(Surface.Dirt)));
            Assert.That(SoundBank.Chant, Is.EqualTo(lib.Chant));
            Assert.That(SoundBank.GatePitch, Is.EqualTo(1f));
            var a = SoundBank.Footstep(Surface.Dirt);
            var b = SoundBank.Footstep(Surface.Dirt);
            Assert.That(a, Is.Not.EqualTo(b), "variations play in turn, not the same clip twice");
        }

        [Test]
        public void ARecordedKnock_RepeatsIntoAKnocking()
        {
            var lib = Built();
            var single = lib.Knock[0];
            var knocking = SoundBank.Repeat(single, 3, 0.32f);
            Assert.That(knocking, Is.Not.Null, "one-shots load into memory, so their samples can be read");
            Assert.That(knocking.length, Is.EqualTo(single.length + 0.64f).Within(0.01f));
        }

        [Test]
        public void Library_IsFilled()
        {
            var lib = Built();
            foreach (var steps in new[] { lib.StepWood, lib.StepStone, lib.StepDirt, lib.StepLeaves })
                Assert.That(steps, Has.Length.EqualTo(8));
            foreach (var lands in new[] { lib.LandWood, lib.LandStone, lib.LandDirt, lib.LandLeaves })
                Assert.That(lands, Has.Length.EqualTo(2));
            foreach (var shots in new[] { lib.BellStrike, lib.DoorCreak, lib.GateGroan, lib.Knock, lib.Thud, lib.HeavyThud, lib.Gasp, lib.Chime, lib.Clunk, lib.HollowBreath, lib.HollowExhale, lib.DistantDogs, lib.HouseCreaks })
                Assert.That(shots, Is.Not.Empty);
            foreach (var loop in new[] { lib.Chant, lib.DayOutdoors, lib.NightWind, lib.RainOutdoors, lib.RoomTone, lib.ClockTicks, lib.ChimneyWind, lib.RainWindow, lib.HearthFire })
                Assert.That(loop, Is.Not.Null);
        }
    }
}
