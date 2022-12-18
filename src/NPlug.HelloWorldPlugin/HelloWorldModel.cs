// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public class HelloWorldModel : AudioProcessorModel
{
    public HelloWorldModel() : base("HelloWorld", DefaultProgramList)
    {
        AddByPassParameter();

        SubUnit1 = AddUnit(new AudioUnit("SubUnit1"));
        SubUnit11 = SubUnit1.AddUnit(new AudioUnit("SubUnit1.1"));
        SubUnit2 = AddUnit(new AudioUnit("SubUnit2"));
        SubUnit21 = SubUnit2.AddUnit(new AudioUnit("SubUnit2.1"));

        DelayParameter = AddParameter(new AudioParameter("Delay", units: "ms"));
        HelloParameter = AddParameter(new AudioRangeParameter("hello", minValue: 2000.0, maxValue: 20480.0, defaultPlainValue: 4000.0));
        ListParameter = AddParameter(new AudioStringListParameter("List", new[] { "A", "B", "C" }));

        ModWheelParameter = SubUnit1.AddParameter(new AudioParameter("Mod Wheel"));
    }

    public AudioParameter DelayParameter { get; }

    public AudioRangeParameter HelloParameter { get; }

    public AudioStringListParameter ListParameter { get; }

    public AudioParameter ModWheelParameter { get; }

    public AudioUnit SubUnit1 { get; }
    public AudioUnit SubUnit11 { get; }

    public AudioUnit SubUnit2 { get; }
    public AudioUnit SubUnit21 { get; }

    private static readonly AudioProgramList DefaultProgramList = new HelloWorldProgramListBuilder()
    {
        { "Test", 1.0, 2.0, 0, 1.0 },
        { "Test1", 2.0, 2.0, 0, 1.0 },
        { "Test2", 3.0, 2.0, 0, 1.0 },
        { "Test3", 4.0, 2.0, 0, 1.0 },
        { "Test4", 5.0, 2.0, 0, 1.0 },
        { "Test5", 6.0, 2.0, 0, 1.0 },
        { "Test6", 7.0, 2.0, 0, 1.0 },
        { "Test7", 8.0, 2.0, 0, 1.0 },
    }.Build();

    private class HelloWorldProgramListBuilder : AudioProgramListBuilder<HelloWorldModel>
    {
        public HelloWorldProgramListBuilder() : base("HelloWorld")
        {
        }

        public void Add(string programName, double delay, double hello, int index, double modWheel)
        {
            Add(new AudioProgram(programName), model =>
            {
                model.DelayParameter.NormalizedValue = delay;
                model.HelloParameter.Value = hello;
                model.ListParameter.SelectedItem = index;
                model.ModWheelParameter.NormalizedValue = modWheel;
            });
        }
    }
}