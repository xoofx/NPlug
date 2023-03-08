// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Indicates that the plug-in could or not support Prefetch (dynamically).
/// </summary>
/// <remarks>
///  vstIPlug vst365, Vst::IPrefetchableSupport
/// - [plug imp]
/// - [extends IComponent]
/// - [released: 3.6.5]
/// - [optional]
///
/// The plug-in should implement this interface if it needs to dynamically change between prefetchable or not.
/// By default (without implementing this interface) the host decides in which mode the plug-in is processed.
/// For more info about the prefetch processing mode check the ProcessModes::kPrefetch documentation. IPrefetchableSupport
/// 
/// Example Example
/// ```cpp
/// //------------------------------------------------------------------------
/// tresult PLUGIN_API myPlug::getPrefetchableSupport (PrefetchableSupport&amp; prefetchable)
/// {
/// 	prefetchable = kIsNeverPrefetchable;
/// 
/// 	switch (myPrefetchableMode)
/// 	{
/// 		case 0: prefetchable = kIsNeverPrefetchable; break;
/// 		case 1: prefetchable = kIsYetPrefetchable; break;
/// 		case 2: prefetchable = kIsNotYetPrefetchable; break;
/// 	}
/// 	return kResultOk;
/// }
/// ```
/// </remarks>
public interface IAudioProcessorPrefetchable : IAudioProcessor
{
    /// <summary>
    /// Retrieve the current prefetch support. Use <see cref="IAudioControllerHandler.RestartComponent"/> with <see cref="AudioRestartFlags.PrefetchableSupportChanged"/> to inform the host that this support has changed.
    /// </summary>
    AudioProcessorPrefetchableSupport PrefetchableSupport { get; }
}