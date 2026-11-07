// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.Runtime.CompilerServices;
using NPlug.Interop;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Note-on articulations of the MIDI 2.0 orchestral profile (<c>Vst::NoteOnOrchestralArticulation::IInfo</c>, VST 3.8.1):
/// the host queries the controller and reads the variations through the native interface.
/// </summary>
public unsafe class TestOrchestralArticulation
{
    [Test]
    public void VariationsHaveTheCppLayout()
    {
        Assert.AreEqual(16, sizeof(LibVst.Variations));
        Assert.AreEqual(17 * 16, sizeof(LibVst.ClassificationVariations));
        Assert.AreEqual(sizeof(LibVst.ClassificationVariations), Unsafe.SizeOf<AudioOrchestralArticulationVariations>());
    }

    [Test]
    public void TheHostReadsTheVariationsOfTheController()
    {
        var controller = new ArticulationController();
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(controller);
        try
        {
            // What a host does: query IInfo on the edit controller, then ask for a channel.
            var editController = comObject.QueryInterface<LibVst.IEditController>();
            void* obj;
            var iid = LibVst.IInfo.IId;
            Assert.AreEqual(LibVst.ComResult.Ok, editController->queryInterface(&iid, &obj).Value);
            var info = (LibVst.IInfo*)obj;

            LibVst.ClassificationVariations native;
            new Span<byte>(&native, sizeof(LibVst.ClassificationVariations)).Fill(0xCD); // what the host's memory held before
            Assert.AreEqual(LibVst.ComResult.Ok, info->getVariationsInfo(0, 3, &native).Value);
            var bytes = new ReadOnlySpan<byte>(&native, sizeof(LibVst.ClassificationVariations));
            Assert.AreEqual(2, bytes[(int)AudioOrchestralArticulationClassification.StaccatosAndShorts * 16 + 5], "staccato, subclass 5: 2 variations");
            Assert.AreEqual(1, bytes[(int)AudioOrchestralArticulationClassification.CustomNoteOn1 * 16 + 0]);
            Assert.AreEqual(2 + 1, bytes.ToArray().Sum(b => b), "everything else is 0");
            CollectionAssert.AreEqual(new[] { (0, (short)3) }, controller.Requests);

            Assert.AreEqual(LibVst.ComResult.False, info->getVariationsInfo(1, 0, &native).Value, "no articulations on bus 1");
            Assert.AreEqual(LibVst.ComResult.InvalidArg, info->getVariationsInfo(0, 0, null).Value);
            info->release();
        }
        finally
        {
            comObject.ReleaseRef();
        }
    }

    [Test]
    public void ControllersWithoutArticulationsDoNotExposeTheInterface()
    {
        var comObject = LibVst.ComObjectManager.Instance.GetOrCreateComObject(new PlainController());
        try
        {
            var editController = comObject.QueryInterface<LibVst.IEditController>();
            void* obj;
            var iid = LibVst.IInfo.IId;
            Assert.AreEqual(LibVst.ComResult.NoInterface, editController->queryInterface(&iid, &obj).Value);
        }
        finally
        {
            comObject.ReleaseRef();
        }
    }

    [Test]
    public void VariationsAreIndexedByClassificationAndSubclass()
    {
        var variations = new AudioOrchestralArticulationVariations();
        Assert.IsFalse(variations.IsAvailable(AudioOrchestralArticulationClassification.IntervallicTrills));
        variations[AudioOrchestralArticulationClassification.IntervallicTrills, 15] = 4;
        Assert.AreEqual(4, variations[AudioOrchestralArticulationClassification.IntervallicTrills, 15]);
        Assert.IsTrue(variations.IsAvailable(AudioOrchestralArticulationClassification.IntervallicTrills));
        Assert.IsFalse(variations.IsAvailable(AudioOrchestralArticulationClassification.AdditionalColors));
        variations.Clear();
        Assert.IsFalse(variations.IsAvailable(AudioOrchestralArticulationClassification.IntervallicTrills));
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = variations[AudioOrchestralArticulationClassification.Reserved3, 16]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = variations[(AudioOrchestralArticulationClassification)17, 0]);
    }

    [Test]
    public void NoteExpressionTypeIdsMatchTheSdk()
    {
        Assert.AreEqual((uint)LibVst.NoteExpressionTypeIDs.kMidi2RegisteredPerNoteControllerPlayingPosition, (uint)AudioNoteExpressionTypeId.Midi2RegisteredPerNoteControllerPlayingPosition);
        Assert.AreEqual((uint)LibVst.NoteExpressionTypeIDs.kMidi2NoteOnOrchestralArticulationStart, (uint)AudioNoteExpressionTypeId.Midi2NoteOnOrchestralArticulationStart);
        Assert.AreEqual((uint)LibVst.NoteExpressionTypeIDs.kMidi2NoteOnOrchestralArticulationEnd, (uint)AudioNoteExpressionTypeId.Midi2NoteOnOrchestralArticulationEnd);
        Assert.AreEqual((uint)LibVst.NoteExpressionTypeIDs.kMidi2NoteOffOrchestralArticulation, (uint)AudioNoteExpressionTypeId.Midi2NoteOffOrchestralArticulation);
        Assert.AreEqual((uint)LibVst.NoteExpressionTypeIDs.kMidi2NoteOffAttributeEnd, (uint)AudioNoteExpressionTypeId.Midi2NoteOffAttributeEnd);
    }

    private sealed class EmptyModel : AudioProcessorModel
    {
        public EmptyModel() : base("Empty")
        {
        }
    }

    private sealed class PlainController : AudioController<EmptyModel>
    {
    }

    private sealed class ArticulationController : AudioController<EmptyModel>, IAudioControllerNoteOnOrchestralArticulationInfo
    {
        public List<(int, short)> Requests { get; } = new();

        public bool TryGetOrchestralArticulationVariations(int busIndex, short channel, ref AudioOrchestralArticulationVariations variations)
        {
            Requests.Add((busIndex, channel));
            if (busIndex != 0) return false;
            variations[AudioOrchestralArticulationClassification.StaccatosAndShorts, 5] = 2;
            variations[AudioOrchestralArticulationClassification.CustomNoteOn1, 0] = 1;
            return true;
        }
    }
}
