// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug.SimpleProgramChange;

public class SimpleProgramChangeModel : AudioProcessorModel
{
    public SimpleProgramChangeModel() : base("Root", DefaultProgramListBuilder)
    {
        AddByPassParameter();
        Gain = AddParameter(new AudioParameter("Gain", defaultNormalizedValue: 1.0));
    }

    public AudioParameter Gain { get; }
    
    private static readonly AudioProgramListBuilder DefaultProgramListBuilder = GenerateProgramListBuilder();

    /// <summary>
    /// Creates a program list that will change the gain depending on the program index
    /// </summary>
    private static AudioProgramListBuilder GenerateProgramListBuilder()
    {
        var builder = new AudioProgramListBuilder<SimpleProgramChangeModel>("Bank");
        const int programCount = 10; // Display only 10 programs
        for (int i = 0; i < programCount; i++)
        {
            int programIndex = i;
            builder.Add(model =>
            {
                // We map the gain to the program index (from 0.0 to 1.0 when changing the program)
                model.Gain.NormalizedValue = ((double)programIndex) / (programCount - 1);
                return new AudioProgram($"Prog {programIndex}");
            });
        }

        return builder;
    }
}
