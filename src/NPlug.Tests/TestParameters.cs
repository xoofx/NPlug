// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using NUnit.Framework;

namespace NPlug.Tests;

public class TestParameters
{
    [Test]
    public void TestParameter()
    {
        var parameter = new AudioParameter("Hello");
        var model = new SimpleModel();
        model.AddParameter(parameter);

        void CheckParameter()
        {
            var savePrecision = parameter.Precision;
            parameter.NormalizedValue = 0.1;
            Assert.AreEqual("0.1000", parameter.ToString(parameter.NormalizedValue));

            parameter.NormalizedValue = 0.234;
            parameter.Precision = 2;
            Assert.AreEqual("0.23", parameter.ToString(parameter.NormalizedValue));

            parameter.NormalizedValue = 0.1;
            Assert.AreEqual(0.1, parameter.NormalizedValue);
            parameter.NormalizedValue = 0.9;
            Assert.AreEqual(0.9, parameter.NormalizedValue);
            parameter.NormalizedValue = -0.1;
            Assert.AreEqual(0.0, parameter.NormalizedValue);
            parameter.NormalizedValue = 1.1;
            Assert.AreEqual(1.0, parameter.NormalizedValue);

            var valueChanged = 0.0;
            model.ParameterValueChanged += audioParameter => { valueChanged = audioParameter.NormalizedValue; };
            parameter.NormalizedValue = 0.5;
            Assert.AreEqual(0.5, valueChanged);
            parameter.Precision = savePrecision;
        }

        // Check the parameter 
        CheckParameter();

        Assert.False(model.Initialized);
        model.Initialize();
        Assert.True(model.Initialized);

        CheckParameter();

        // Cannot modify the model after initializing
        Assert.Throws<InvalidOperationException>(() => model.AddParameter(new AudioParameter("Test")));
    }

    [Test]
    public void TestBoolParameter()
    {
        var boolParameter = new AudioBoolParameter("Hello");
        Assert.False(boolParameter.Value);
        Assert.AreEqual("Off", boolParameter.ToString(boolParameter.NormalizedValue));

        boolParameter.Value = true;
        Assert.True(boolParameter.Value);
        Assert.AreEqual("On", boolParameter.ToString(boolParameter.NormalizedValue));
    }

    [Test]
    public void TestRangeParameter()
    {
        var rangeParameter = new AudioRangeParameter("MinMax", minValue: -40.0, maxValue: 100.0);

        Assert.AreEqual(0.0, rangeParameter.Value);

        rangeParameter.Value = -100.0;
        Assert.AreEqual(-40.0, rangeParameter.Value);
        Assert.AreEqual(0.0, rangeParameter.NormalizedValue);

        rangeParameter.Value = 150.0;
        Assert.AreEqual(100.0, rangeParameter.Value);
        Assert.AreEqual(1.0, rangeParameter.NormalizedValue);
        
        // Must be 0.5 normalized -40 + (100 + 40) / 2
        rangeParameter.Value = 30.0;
        Assert.AreEqual(30.0, rangeParameter.Value);
        Assert.AreEqual(0.5, rangeParameter.NormalizedValue);
    }

    [Test]
    public void TestStringListParameter()
    {
        var input = Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => ((char)x).ToString()).ToArray();
        for (int length = 2; length < input.Length + 1; length++)
        {
            var slice = input.AsSpan().Slice(0, length);

            var listParameter = new AudioStringListParameter("List", slice.ToArray());

            for (int i = 0; i < listParameter.Items.Length; i++)
            {
                listParameter.SelectedItem = i;
                Assert.AreEqual(listParameter.Items[i], listParameter.ToString(listParameter.NormalizedValue));
            }
        }
    }

    private class SimpleModel : AudioProcessorModel
    {
        public SimpleModel() : base("Simple")
        {
        }
    }
}