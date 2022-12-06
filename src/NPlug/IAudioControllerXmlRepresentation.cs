// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System.IO;

namespace NPlug;

/// <summary>
/// Extended plug-in interface IEditController for a component.
/// </summary>
/// <remarks>
///  vstIPlug vst350 : Vst::IXmlRepresentationController
/// - [plug imp]
/// - [extends IEditController]
/// - [released: 3.5.0]
/// - [optional] A representation based on XML is a way to export, structure, and group plug-ins parameters for a specific remote (hardware or software rack (such as quick controls)).@n It allows to describe each parameter more precisely (what is the best matching to a knob, different title lengths matching limited remote display,...).@n See an @ref Example. @n @n - A representation is composed of pages (this means that to see all exported parameters, the user has to navigate through the pages).
/// - A page is composed of cells (for example 8 cells per page).
/// - A cell is composed of layers (for example a cell could have a knob, a display, and a button, which means 3 layers).
/// - A layer is associated to a plug-in parameter using the ParameterID as identifier:
/// - it could be a knob with a display for title and/or value, this display uses the same parameterId, but it could an another one.
/// - switch
/// - link which allows to jump directly to a subpage (another page) 
/// - more... See Vst::LayerType
/// .@n This representation is implemented as XML text following the Document Type Definition (DTD): http://dtd.steinberg.net/VST-Remote-1.1.dtd
/// Example
/// Here an example of what should be passed in the stream of getXmlRepresentationStream:
/// ```xml
/// &lt;?xml version="1.0" encoding="utf-8"?&gt;
/// &lt;!DOCTYPE vstXML PUBLIC "-//Steinberg//DTD VST Remote 1.1//EN" "http://dtd.steinberg.net/VST-Remote-1.1.dtd"&gt;
/// &lt;vstXML version="1.0"&gt;
/// 	&lt;plugin classID="341FC5898AAA46A7A506BC0799E882AE" name="Chorus" vendor="Steinberg Media Technologies" /&gt;
/// 	&lt;originator&gt;My name&lt;/originator&gt;
/// 	&lt;date&gt;2010-12-31&lt;/date&gt;
/// 	&lt;comment&gt;This is an example for 4 Cells per Page for the Remote named ProductRemote 
/// 	         from company HardwareCompany.&lt;/comment&gt;
/// 
/// 	&lt;!-- ===================================== --&gt;
/// 	&lt;representation name="ProductRemote" vendor="HardwareCompany" version="1.0"&gt;
/// 		&lt;page name="Root"&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="knob" parameterID="0"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Mix dry/wet&lt;/name&gt;
/// 						&lt;name&gt;Mix&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="display"&gt;&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="knob" parameterID="3"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Delay&lt;/name&gt;
/// 						&lt;name&gt;Dly&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="knob" parameterID="15"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Spatial&lt;/name&gt;
/// 						&lt;name&gt;Spat&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 		&lt;/page&gt;
/// 		&lt;page name="Page 2"&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="LED" ledStyle="spread" parameterID="2"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Width +&lt;/name&gt;
/// 						&lt;name&gt;Widt&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 				&lt;!--this is the switch for shape A/B--&gt;
/// 				&lt;layer type="switch" switchStyle="pushIncLooped" parameterID="4"&gt;&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="display"&gt;&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="LED" ledStyle="singleDot" parameterID="17"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Sync Note +&lt;/name&gt;
/// 						&lt;name&gt;Note&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 				&lt;!--this is the switch for sync to tempo on /off--&gt;
/// 				&lt;layer type="switch" switchStyle="pushIncLooped" parameterID="16"&gt;&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 			&lt;cell&gt;
/// 				&lt;layer type="knob" parameterID="1"&gt;
/// 					&lt;titleDisplay&gt;
/// 						&lt;name&gt;Rate&lt;/name&gt;
/// 					&lt;/titleDisplay&gt;
/// 				&lt;/layer&gt;
/// 			&lt;/cell&gt;
/// 		&lt;/page&gt;
/// 	&lt;/representation&gt;
/// &lt;/vstXML&gt;
/// ```
/// </remarks>
public interface IAudioControllerXmlRepresentation : IAudioController
{
    /// <summary>
    /// Retrieves a stream containing a XmlRepresentation for a wanted representation info
    /// </summary>
    void GetXmlRepresentationStream(in AudioControllerRepresentationInfo info, Stream output);
}