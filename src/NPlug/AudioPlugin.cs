// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

public class AudioHost
{

}

public class AudioPlugin
{

    public virtual void Initialize(AudioHost host)
    {
    }

    public virtual void Terminate()
    {
    }
}