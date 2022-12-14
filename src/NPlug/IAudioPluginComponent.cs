// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// The host uses this interface to initialize and to terminate the plug-in component.
/// The context that is passed to the initialize method contains any interface to the
/// host that the plug-in will need to work.These interfaces can vary from category to category.
/// A list of supported host context interfaces should be included in the documentation
/// of a specific category. 
/// </summary>
public interface IAudioPluginComponent : IAudioPluginObject
{
    /// <summary>
    /// The host passes a number of interfaces as context to initialize the plug-in class.
    /// \param context, passed by the host, is mandatory and should implement IHostApplication
    /// </summary>
    /// <param name="hostApplication">The host.</param>
    /// <remarks>
    /// Extensive memory allocations etc.should be performed in this method rather than in
    /// the class' constructor! If the method does NOT return kResultOk, the object is released
    /// immediately.In this case terminate is not called!
    /// </remarks>
    bool Initialize(AudioHostApplication hostApplication);

    /// <summary>
    /// Gets the associated host, Must be not null after a successful <see cref="Initialize"/>
    /// </summary>
    AudioHostApplication? Host { get; }
    
    /// <summary>
    /// This function is called before the plug-in is unloaded and can be used for
    /// cleanups.You have to release all references to any host application interfaces.
    /// </summary>
    void Terminate();
}