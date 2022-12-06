// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace NPlug;

/// <summary>
/// Channel context interface: Vst::IInfoListener
/// </summary>
/// <remarks>
///  vstIHost vst365- [plug imp]
/// - [extends IEditController]
/// - [released: 3.6.5]
/// - [optional]Allows the host to inform the plug-in about the context in which the plug-in is instantiated,
/// mainly channel based info (color, name, index,...). Index can be defined inside a namespace 
/// (for example, index start from 1 to N for Type Input/Output Channel (Index namespace) and index 
/// start from 1 to M for Type Audio Channel).@n As soon as the plug-in provides this IInfoListener interface, the host will call setChannelContextInfos 
/// for each change occurring to this channel (new name, new color, new indexation,...) IChannelContextExample Example@code {.cpp}
/// //------------------------------------------------------------------------
/// tresult PLUGIN_API MyPlugin::setChannelContextInfos (IAttributeList* list)
/// {
/// 	if (list)
/// 	{
/// 		// optional we can ask for the Channel Name Length
/// 		int64 length;
/// 		if (list-&gt;getInt (ChannelContext::kChannelNameLengthKey, length) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 		
/// 		// get the Channel Name where we, as plug-in, are instantiated
/// 		String128 name;
/// 		if (list-&gt;getString (ChannelContext::kChannelNameKey, name, sizeof (name)) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 
/// 		// get the Channel UID
/// 		if (list-&gt;getString (ChannelContext::kChannelUIDKey, name, sizeof (name)) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 		
/// 		// get Channel Index
/// 		int64 index;
/// 		if (list-&gt;getInt (ChannelContext::kChannelIndexKey, index) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 		
/// 		// get the Channel Color
/// 		int64 color;
/// 		if (list-&gt;getInt (ChannelContext::kChannelColorKey, color) == kResultTrue)
/// 		{
/// 			uint32 channelColor = (uint32)color;
/// 			String str;
/// 			str.printf ("%x%x%x%x", ChannelContext::GetAlpha (channelColor),
/// 			ChannelContext::GetRed (channelColor),
/// 			ChannelContext::GetGreen (channelColor),
/// 			ChannelContext::GetBlue (channelColor));
/// 			String128 string128;
/// 			Steinberg::UString (string128, 128).fromAscii (str);
/// 			...
/// 		}
/// 
/// 		// get Channel Index Namespace Order of the current used index namespace
/// 		if (list-&gt;getInt (ChannelContext::kChannelIndexNamespaceOrderKey, index) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 	
/// 		// get the channel Index Namespace Length
/// 		if (list-&gt;getInt (ChannelContext::kChannelIndexNamespaceLengthKey, length) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 		
/// 		// get the channel Index Namespace
/// 		String128 namespaceName;
/// 		if (list-&gt;getString (ChannelContext::kChannelIndexNamespaceKey, namespaceName, sizeof (namespaceName)) == kResultTrue)
/// 		{
/// 			...
/// 		}
/// 
/// 		// get plug-in Channel Location
/// 		int64 location;
/// 		if (list-&gt;getInt (ChannelContext::kChannelPluginLocationKey, location) == kResultTrue)
/// 		{
/// 			String128 string128;
/// 			switch (location)
/// 			{
/// 				case ChannelContext::kPreVolumeFader:
/// 					Steinberg::UString (string128, 128).fromAscii ("PreVolFader");
/// 				break;
/// 				case ChannelContext::kPostVolumeFader:
/// 					Steinberg::UString (string128, 128).fromAscii ("PostVolFader");
/// 				break;
/// 				case ChannelContext::kUsedAsPanner:
/// 					Steinberg::UString (string128, 128).fromAscii ("UsedAsPanner");
/// 				break;
/// 				default: Steinberg::UString (string128, 128).fromAscii ("unknown!");
/// 				break;
/// 			}
/// 		}
/// 		
/// 		// do not forget to call addRef () if you want to keep this list
/// 	}
/// }
/// @endcode
/// </remarks>
public interface IAudioControllerInfoListener : IAudioController
{
    /// <summary>
    /// Receive the channel context infos from host.
    /// </summary>
    void SetChannelContextInfos(in AudioAttributeList list);
}