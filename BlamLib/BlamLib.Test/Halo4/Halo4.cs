/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlamLib.Test
{
	using GameTagGroups = Blam.Halo4.TagGroups;
	using GameTags = Blam.Halo4.Tags;

	[TestClass]
	public partial class Halo4 : BaseTestClass
	{
		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			(Program.GetManager(BlamVersion.Halo4_Xbox) as Managers.IStringIdController)
				.StringIdCacheOpen(BlamVersion.Halo4_Xbox);

			Directory.CreateDirectory(kTestResultsPath);
			Directory.CreateDirectory(kTagDumpPath);
		}
		[ClassCleanup]
		public static void Dispose()
		{
			(Program.GetManager(BlamVersion.Halo4_Xbox) as Managers.IStringIdController)
				.StringIdCacheClose(BlamVersion.Halo4_Xbox);
		}

		static bool MapNeedsUniqueName(string header_name)
		{
			if (header_name.Contains("_patch"))
				return true;

			return false;
		}
		static readonly string[] kMapNames_Retail = {
			@"Retail\maps\mainmenu.map",

			@"Retail\maps\m05_prologue.map",
			@"Retail\maps\m10_crash.map",
			@"Retail\maps\m020.map",
			@"Retail\maps\m30_cryptum.map",
			@"Retail\maps\m40_invasion.map",
			@"Retail\maps\m60_rescue.map",
			@"Retail\maps\m70_liftoff.map",
			@"Retail\maps\m80_delta.map",
			@"Retail\maps\m90_sacrifice.map",
			@"Retail\maps\m95_epilogue.map",

			@"Retail\maps\ff81_courtyard.map",
			@"Retail\maps\ff82_scurve.map",
			@"Retail\maps\ff84_temple.map",
			@"Retail\maps\ff86_sniperalley.map",
			@"Retail\maps\ff87_chopperbowl.map",
			@"Retail\maps\ff90_fortsw.map",
			@"Retail\maps\ff91_complex.map",
			@"Retail\maps\ff92_valhalla.map",

			@"Retail\maps\ca_blood_cavern.map",
			@"Retail\maps\ca_blood_crash.map",
			@"Retail\maps\ca_canyon.map",
			@"Retail\maps\ca_forge_bonanza.map",
			@"Retail\maps\ca_forge_erosion.map",
			@"Retail\maps\ca_forge_ravine.map",
			@"Retail\maps\ca_gore_valley.map",
			@"Retail\maps\ca_redoubt.map",
			@"Retail\maps\ca_tower.map",
			@"Retail\maps\ca_warhouse.map",
			@"Retail\maps\wraparound.map",
			@"Retail\maps\z05_cliffside.map",
			@"Retail\maps\z11_valhalla.map",

			// dlc_crimson
			@"Retail\dlc_crimson\dlc_dejewel.map",
			@"Retail\dlc_crimson\dlc_dejunkyard.map",
			@"Retail\dlc_crimson\zd_02_grind.map",

			@"Retail\dlc_spops_1.5\dlc01_engine.map",
			@"Retail\dlc_spops_1.5\dlc01_factory.map",
			@"Retail\dlc_spops_1.5\ff151_mezzanine.map",
			@"Retail\dlc_spops_1.5\ff152_vortex.map",
			@"Retail\dlc_spops_1.5\ff153_caverns.map",
			@"Retail\dlc_spops_1.5\ff154_hillside.map",
			@"Retail\dlc_spops_1.5\ff155_breach.map",
		};

		// NOTE: Getting out of memory exceptions sometimes when running H4 cache output tests...
		// Need to verify if this is BlamLib's StringId system's fault or just due to the fact that
		// Halo4 maps have loads of strings and we process multiple maps at a time (ThreadPool queue)

		#region CacheOutputInformation
		static void CacheOutputInformationMethod(object param)
		{
			var args = param as CacheFileOutputInfoArgs;

			using (var handler = new CacheHandler<Blam.Halo4.CacheFile>(args.Game, args.MapPath))
			{
				handler.Read();
				var cache = handler.CacheInterface;

				string header_name = cache.Header.Name;
				if (MapNeedsUniqueName(header_name))
					header_name = cache.GetUniqueName();

				Blam.CacheFile.OutputStringIds(cache,
					BuildResultPath(kTestResultsPath, args.Game, header_name, "string_ids", "txt"), true);
				Blam.CacheFile.OutputTags(cache,
					BuildResultPath(kTestResultsPath, args.Game, header_name, null, "txt"));
			}

			args.SignalFinished();
		}
		[TestMethod]
		public void Halo4TestCacheOutputXbox()
		{
			CacheFileOutputInfoArgs.TestMethodSerial(TestContext,
				CacheOutputInformationMethod,
				BlamVersion.Halo4_Xbox, kDirectoryXbox, kMapNames_Retail);
		}
		#endregion

		static readonly string[] kMapNames_MegaloData = {
			@"Retail\maps\ca_blood_cavern.map",

			@"Retail\maps\ca_forge_bonanza.map",
			//@"Retail\maps\ca_forge_erosion.map",
			//@"Retail\maps\ca_forge_ravine.map",

			//@"Retail\maps\ff92_valhalla.map",

			@"Retail\dlc_crimson\zd_02_grind.map",

			@"Retail\maps\mainmenu.map", // only use for sily
		};
		static readonly List<TagInterface.TagGroup> kMegaloGroupTags = new List<TagInterface.TagGroup> {
			GameTagGroups.capg,
			GameTagGroups.ggol,
			GameTagGroups.gmeg,
			GameTagGroups.ingd,
			GameTagGroups.lgtd,
			GameTagGroups.mgee,
			GameTagGroups.mgls,
			GameTagGroups.msit,
			GameTagGroups.motl,
		};
		static readonly List<TagInterface.TagGroup> kInterfaceGroupTags = new List<TagInterface.TagGroup> {
			GameTagGroups.goof,
			GameTagGroups.sily,
		};
		static void CacheOutputMegaloInformationMethod(object param)
		{
			var args = param as CacheFileOutputInfoArgs;

			using (var handler = new CacheHandler<Blam.Halo4.CacheFile>(args.Game, args.MapPath))
			{
				handler.Read();
				var cache = handler.CacheInterface;

				string header_name = cache.Header.Name;
				if (MapNeedsUniqueName(header_name))
					header_name = cache.GetUniqueName();

				var xml_settings = new System.Xml.XmlWriterSettings();
				xml_settings.Indent = true;
				xml_settings.IndentChars = "\t";

				string results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "megalo", "txt");
				#region StreamWriter
				if (false)using (var s = new System.IO.StreamWriter(results_path))
				{
					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (!kMegaloGroupTags.Contains(tag.GroupTag)) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var to_stream = def as Blam.Halo4.Tags.ITempToStreamInterface;

						if (to_stream != null)
						{
							s.WriteLine("{0}\t{1}", man.GroupTag.TagToString(), man.Name);
							to_stream.ToStream(s, man, null);
						}

						cache.TagIndexManager.Unload(index);
					}
				}
				#endregion
				#region XmlWriter
				results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "megalo", "xml");
				if(false)using (var s = System.Xml.XmlTextWriter.Create(results_path, xml_settings))
				{
					s.WriteStartDocument(true);
					s.WriteStartElement("megaloDefinitions");
					s.WriteAttributeString("game", "Halo4_Xbox");

					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (!kMegaloGroupTags.Contains(tag.GroupTag)) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var to_stream = def as GameTags.ITempToXmlStreamInterface;

						to_stream.ToStream(s, man, null);

						cache.TagIndexManager.Unload(index);
					}

					s.WriteEndElement();
					s.WriteEndDocument();
				}
				#endregion

				#region StreamWriter Interface
				results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "interface", "txt");
				if(false)using (var s = new System.IO.StreamWriter(results_path))
				{
					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (!kInterfaceGroupTags.Contains(tag.GroupTag)) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var sily = def as Blam.Halo4.Tags.text_value_pair_definition_group;

						if (sily != null)
						{
							s.WriteLine("{0}\t{1}", man.GroupTag.TagToString(), man.Name);
							int type = sily.Type.Value;
							if (type != 0 && type != 1 && type != 3)
								s.WriteLine("LOOKATME");
							sily.ToStream(s, man, null);
						}
						else
						{
							var to_stream = def as Blam.Halo4.Tags.ITempToStreamInterface;
							s.WriteLine("{0}\t{1}", man.GroupTag.TagToString(), man.Name);
							to_stream.ToStream(s, man, null);
						}

						cache.TagIndexManager.Unload(index);
					}
				}
				#endregion
				#region XmlWriter Interface
				results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "interface", "xml");
				if(true)using (var s = System.Xml.XmlTextWriter.Create(results_path, xml_settings))
				{
					s.WriteStartDocument(true);
					s.WriteStartElement("interfaceDefinitions");
					s.WriteAttributeString("game", "Halo4_Xbox");

					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (!kInterfaceGroupTags.Contains(tag.GroupTag)) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var to_stream = def as GameTags.ITempToXmlStreamInterface;

						to_stream.ToStream(s, man, null);

						cache.TagIndexManager.Unload(index);
					}

					s.WriteEndElement();
					s.WriteEndDocument();
				}
				#endregion
			}

			args.SignalFinished();
		}
		[TestMethod]
		public void Halo4DumpMegloData()
		{
			BlamVersion game = BlamVersion.Halo4_Xbox;

			CacheFileOutputInfoArgs.TestMethodSerial(TestContext,
				CacheOutputMegaloInformationMethod,
				game, kDirectoryXbox, kMapNames_MegaloData);

			string results_path = BuildResultPath(kTagDumpPath, game, "", "settings", "xml");
			var xml_settings = new System.Xml.XmlWriterSettings();
			xml_settings.Indent = true;
			xml_settings.IndentChars = "  ";
			xml_settings.Encoding = System.Text.Encoding.ASCII;
			using (var s = System.Xml.XmlTextWriter.Create(results_path, xml_settings))
			{
				s.WriteStartDocument(true);
				s.WriteStartElement("settingsDefinitions");
				s.WriteAttributeString("game", game.ToString());

				s.WriteStartElement("options");
				foreach (var kv in Blam.Halo4.Tags.text_value_pair_definition_group.SettingParameters)
				{
					s.WriteStartElement("entry");
					s.WriteAttributeString("key", kv.Key.ToString());
					s.WriteAttributeString("value", Path.GetFileName(kv.Value));
					s.WriteAttributeString("tagName", kv.Value);
					s.WriteEndElement();
				}
				s.WriteEndElement();

				s.WriteStartElement("categories");
				foreach (var kv in Blam.Halo4.Tags.multiplayer_variant_settings_interface_definition_group.SettingCategories)
				{
					s.WriteStartElement("entry");
					s.WriteAttributeString("key", kv.Key.ToString());
					s.WriteAttributeString("value", Path.GetFileName(kv.Value.TagName));
					s.WriteAttributeString("title", kv.Value.Title);
					s.WriteAttributeString("tagName", kv.Value.TagName);
					s.WriteEndElement();
				}
				s.WriteEndElement();


				s.WriteEndElement();
				s.WriteEndDocument();
			}
		}

		static void CacheRebuildUnicTagsSeekToLanguagePackData(Blam.Halo4.CacheFile cache)
		{
			bool is_patch = cache.IsPatchCache;

			Blam.Cache.CacheItemGen3 tag_containing_header = null;
			var tag_group_containing_header = is_patch ? GameTagGroups.patg : GameTagGroups.matg;
			foreach (var tag in cache.IndexHalo4.Tags)
			{
				if (tag.GroupTag == tag_group_containing_header)
				{
					tag_containing_header = tag as Blam.Cache.CacheItemGen3;
					break;
				}
			}
			Assert.IsNotNull(tag_containing_header);

			int offset = tag_containing_header.Offset;
			if (!is_patch)
				offset += Blam.Cache.CacheFileLanguagePackResourceGen3.kGlobalsOffsetHalo4;

			cache.InputStream.Seek(offset);
		}
		static void CacheRebuildUnicTagsMethod(object param)
		{
			var args = param as CacheFileOutputInfoArgs;

			using (var handler = new CacheHandler<Blam.Halo4.CacheFile>(args.Game, args.MapPath))
			{
				handler.Read();
				var cache = handler.CacheInterface;

				string header_name = cache.Header.Name;
				if (MapNeedsUniqueName(header_name))
					header_name = cache.GetUniqueName();

				CacheRebuildUnicTagsSeekToLanguagePackData(cache);
				var lang_pack_english = new Blam.Cache.CacheFileLanguagePackResourceGen3(null);
				lang_pack_english.Read(cache.InputStream);
				lang_pack_english.ReadFromCache(cache);
				lang_pack_english.ToString();

				var xml_settings = new System.Xml.XmlWriterSettings();
				xml_settings.Indent = true;
				xml_settings.IndentChars = "\t";

				string results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "unic", "txt");
				#region StreamWriter
				if(false)using (var s = new System.IO.StreamWriter(results_path, false, System.Text.Encoding.UTF8))
				{
					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (tag.GroupTag != GameTagGroups.unic) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var unic = def as Blam.Halo4.Tags.multilingual_unicode_string_list_group;

						if (unic != null)
						{
							s.WriteLine("{0}\t{1}", man.GroupTag.TagToString(), man.Name);
							unic.ReconstructHack(cache, lang_pack_english);
							unic.ToStream(s, man, null);
						}

						cache.TagIndexManager.Unload(index);
					}
				}
				#endregion
				#region XmlWriter
				results_path = BuildResultPath(kTagDumpPath, args.Game, header_name, "unic", "xml");
				xml_settings.Encoding = System.Text.Encoding.UTF8;
				xml_settings.CheckCharacters = false;
				using (var s = System.Xml.XmlTextWriter.Create(results_path, xml_settings))
				{
					s.WriteStartDocument(true);
					s.WriteStartElement("localizationDefinitions");
					s.WriteAttributeString("game", "Halo4_Xbox");

					foreach (var tag in cache.IndexHalo4.Tags)
					{
						if (tag.GroupTag != GameTagGroups.unic) continue;

						var index = cache.TagIndexManager.Open(tag.Datum);
						var man = cache.TagIndexManager[index];
						var def = man.TagDefinition;
						var unic = def as Blam.Halo4.Tags.multilingual_unicode_string_list_group;

						unic.ReconstructHack(cache, lang_pack_english);
						unic.ToStream(s, man, null);

						cache.TagIndexManager.Unload(index);
					}

					s.WriteEndElement();
					s.WriteEndDocument();
				}
				xml_settings.CheckCharacters = true;
				#endregion
			}
		}
		[TestMethod]
		public void Halo4RebuildUnicTags()
		{
			CacheFileOutputInfoArgs.TestMethodSerial(TestContext,
				CacheRebuildUnicTagsMethod,
				BlamVersion.HaloReach_Xbox, kDirectoryXbox, @"Retail\maps\mainmenu.map");
		}

		static readonly string[] kMapNames_Retail_Patches = {
			@"Retail\maps\patches\mainmenu_patch_TU2.map",
//			@"Retail\maps\mainmenu.map",
//			@"Retail\maps\patches\mainmenu_patch_TU3.map",
//			@"Retail\maps\patches\mainmenu_patch_TU6.map",
		};
		[TestMethod]
		public void Halo4TestCachePatchesXbox()
		{
			CacheFileOutputInfoArgs.TestMethodSerial(TestContext,
				CacheOutputInformationMethod,
				BlamVersion.Halo4_Xbox, kDirectoryXbox, kMapNames_Retail_Patches);
		}
	};
}