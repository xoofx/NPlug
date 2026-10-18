// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NPlug.IO;
using NUnit.Framework;

namespace NPlug.Tests;

/// <summary>
/// Edit gestures (beginEdit / performEdit / endEdit) and state loading on <see cref="AudioController{TAudioControllerModel}"/>.
/// </summary>
public class TestControllerEdits
{
    [Test]
    public void SeveralParametersCanBeEditedConcurrently()
    {
        var controller = new TwoParameterController();
        var handler = new RecordingHandler();
        ((IAudioController)controller).SetControllerHandler(handler);
        var a = controller.Model.A;
        var b = controller.Model.B;

        controller.BeginEditParameter(a);
        a.NormalizedValue = 0.25;
        controller.BeginEditParameter(b);          // second gesture while the first is open (legal in VST3)
        b.NormalizedValue = 0.75;
        a.NormalizedValue = 0.30;                  // the first gesture is still valid
        Assert.AreEqual(2, controller.EditedParameterCount);
        Assert.AreSame(b, controller.EditedParameter);

        controller.EndEditParameter(a);
        Assert.IsFalse(controller.IsEditingParameter(a));
        Assert.IsTrue(controller.IsEditingParameter(b));
        controller.EndEditParameter();             // ends the remaining (most recent) one
        Assert.AreEqual(0, controller.EditedParameterCount);

        CollectionAssert.AreEqual(new[]
        {
            $"begin {a.Id.Value}", $"perform {a.Id.Value} 0.25",
            $"begin {b.Id.Value}", $"perform {b.Id.Value} 0.75",
            $"perform {a.Id.Value} 0.3",
            $"end {a.Id.Value}", $"end {b.Id.Value}",
        }, handler.Calls);
    }

    [Test]
    public void EditingWithoutBeginIsRejected()
    {
        var controller = new TwoParameterController();
        ((IAudioController)controller).SetControllerHandler(new RecordingHandler());
        Assert.Throws<InvalidOperationException>(() => controller.Model.A.NormalizedValue = 0.5);
        Assert.Throws<InvalidOperationException>(() => controller.EndEditParameter(controller.Model.A));

        controller.BeginEditParameter(controller.Model.A);
        Assert.Throws<InvalidOperationException>(() => controller.BeginEditParameter(controller.Model.A), "Nested edit of the same parameter.");
    }

    [Test]
    public void BeginEditWithoutHandlerLeavesNoState()
    {
        var controller = new TwoParameterController();
        Assert.Throws<InvalidOperationException>(() => controller.BeginEditParameter(controller.Model.A));
        Assert.IsNull(controller.EditedParameter);
        Assert.AreEqual(0, controller.EditedParameterCount);
    }

    [Test]
    public void LoadRaisesStateLoaded()
    {
        var source = new TwoParameterModel();
        source.Initialize();
        source.A.NormalizedValue = 0.125;
        source.B.NormalizedValue = 0.875;
        var stream = new MemoryStream();
        source.Save(new PortableBinaryWriter(stream), AudioProcessorModelStorageMode.Default);

        var target = new TwoParameterModel();
        target.Initialize();
        int loadedCount = 0;
        int changedCount = 0;
        target.StateLoaded += () => loadedCount++;
        target.ParameterValueChanged += _ => changedCount++;

        stream.Position = 0;
        target.Load(new PortableBinaryReader(stream), AudioProcessorModelStorageMode.Default);

        Assert.AreEqual(1, loadedCount);
        Assert.AreEqual(0, changedCount, "Load writes values directly; StateLoaded is the notification.");
        Assert.AreEqual(0.125, target.A.NormalizedValue);
        Assert.AreEqual(0.875, target.B.NormalizedValue);
    }

    private sealed class TwoParameterModel : AudioProcessorModel
    {
        public TwoParameterModel() : base("Two")
        {
            A = AddParameter(new AudioParameter("A"));
            B = AddParameter(new AudioParameter("B"));
        }

        public AudioParameter A { get; }
        public AudioParameter B { get; }
    }

    private sealed class TwoParameterController : AudioController<TwoParameterModel>
    {
    }

    private sealed class RecordingHandler : IAudioControllerHandler
    {
        public List<string> Calls { get; } = new();

        public void BeginEdit(AudioParameterId id) => Calls.Add($"begin {id.Value}");
        public void PerformEdit(AudioParameterId id, double valueNormalized) => Calls.Add(FormattableString.Invariant($"perform {id.Value} {valueNormalized}"));
        public void EndEdit(AudioParameterId id) => Calls.Add($"end {id.Value}");
        public void RestartComponent(AudioRestartFlags flags) { }
        public bool IsAdvancedEditSupported => false;
        public void SetDirty(bool state) { }
        public void RequestOpenEditor(string name) { }
        public void StartGroupEdit() { }
        public void FinishGroupEdit() { }
        public bool IsCreateContextMenuSupported => false;
        public IAudioContextMenu CreateContextMenu(IAudioPluginView plugView, AudioParameterId paramID) => throw new NotSupportedException();
        public bool IsRequestBusActivationSupported => false;
        public void RequestBusActivation(BusMediaType type, BusDirection dir, int index, bool state) { }
        public bool IsProgressSupported => false;
        public AudioProgressId StartProgress(AudioProgressType type, string? optionalDescription) => default;
        public void UpdateProgress(AudioProgressId id, double normValue) { }
        public void FinishProgress(AudioProgressId id) { }
        public bool IsUnitAndProgramListSupported => false;
        public void NotifyUnitSelection(AudioUnitId unitId) { }
        public void NotifyProgramListChange(AudioProgramListId listId, int programIndex) { }
    }
}
