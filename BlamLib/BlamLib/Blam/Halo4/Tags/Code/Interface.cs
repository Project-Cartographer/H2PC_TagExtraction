/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo4.Tags
{
	#region multiplayer_variant_settings_interface_definition
	partial class multiplayer_variant_settings_interface_definition_group : ITempToStreamInterface, ITempToXmlStreamInterface
	{
		public struct CategoryInfo
		{
			public string Title;
			public string TagName;

			public CategoryInfo(string title, string tag)
			{
				Title = title;
				TagName = tag;
			}
		}
		public static SortedList<int, CategoryInfo> SettingCategories = new SortedList<int, CategoryInfo>();
		#region multiplayer_variant_settings_interface_definition_0_block
		partial class multiplayer_variant_settings_interface_definition_0_block
		{
			#region unknown_4_block
			partial class unknown_4_block
			{
				static void ReferenceToStream(StreamWriter s, TI.TagReference tr)
				{
					if (tr.GroupTag == TI.TagGroup.Null)
						s.Write("NONE");
					else
					{
						s.Write(tr.GroupTag.TagToString());
						s.Write(" ");
						if (!tr.Datum.IsNull)
							s.Write(tr.GetTagPath());
						else
							s.Write("NONE");
					}
				}

				public void ToStream(StreamWriter s,
					Managers.TagManager tag, TI.Definition owner)
				{
					const string k_ident = "\t\t\t\t";

					if(Unknown20 != 0)
						s.Write("{0}\t", Unknown20.Value.ToString("X8"));
					if (!Title.Handle.IsNull)
						s.Write("{0}", Title);
					s.WriteLine();
					if (!Settings.Datum.IsNull)
					{
						s.Write(k_ident+"\tSettings\t"); ReferenceToStream(s, Settings);
						s.WriteLine();
					}
					if (!Template.Datum.IsNull)
					{
						s.Write(k_ident+"\tTemplate\t"); ReferenceToStream(s, Template);
						s.WriteLine();
					}
					if (!ValuePairs.Datum.IsNull)
					{
						s.Write(k_ident+"\tValuePairs\t"); ReferenceToStream(s, ValuePairs);
						s.WriteLine();
					}
				}

				public void ToStream(XmlWriter s,
					Managers.TagManager tag, TI.Definition owner)
				{
					s.WriteStartElement("entry");

					if (!Title.Handle.IsNull)
						s.WriteAttributeString("titleId", Title.ToString());
					if (!Description.Handle.IsNull)
						s.WriteAttributeString("descId", Description.ToString());
					if (Unknown20 != 0)
					{
						s.WriteAttributeString("settingCategory", Unknown20.ToString());
						if (!SettingCategories.ContainsKey(Unknown20.Value))
						{
							string title = Title.ToString();
							string tagname = null;

							if (!Settings.Datum.IsNull)
								tagname = Settings.GetTagPath();
							else if (!Template.Datum.IsNull)
								tagname = Template.GetTagPath();

							SettingCategories[Unknown20.Value] = new CategoryInfo(title, tagname);
						}
					}

					if (!Settings.Datum.IsNull)
						s.WriteElementString("settings", Settings.GetTagPath());
					if (!Template.Datum.IsNull)
						s.WriteElementString("template", Template.GetTagPath());
					if (!ValuePairs.Datum.IsNull)
						s.WriteElementString("values", ValuePairs.GetTagPath());

					s.WriteEndElement();
				}
			};
			#endregion

			public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				const string k_ident = "\t\t\t";

				s.WriteLine("{0}\t{1}", SettingCategory.Value.ToString("X8"), Name);
				s.WriteLine(k_ident+"Options");
				for (int x = 0; x < Options.Count; x++)
				{
					s.Write(k_ident+"\t");
					s.Write(x.ToString("X4"));
					s.Write("\t");
					Options[x].ToStream(s, tag, this);
				}
				s.WriteLine();
			}

			public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				s.WriteStartElement("entry");
				s.WriteAttributeString("settingCategory", SettingCategory.Value.ToString());
				string name = Name.ToString();
				s.WriteAttributeString("name", name);

				SettingCategories[SettingCategory.Value] = new CategoryInfo(name, tag.Name);

				s.WriteStartElement("options");
				foreach (var opt in Options)
					opt.ToStream(s, tag, this);
				s.WriteEndElement();

				s.WriteEndElement();
			}
		};
		#endregion

		public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			const string k_ident = "\t";

			if (Unknown0.Value != 0)
				System.Diagnostics.Debugger.Break();
			s.WriteLine(k_ident+"Unknown0={0}", Unknown0.Value.ToString("X8"));
			s.WriteLine(k_ident+"Categories");
			for (int x = 0; x < Categories.Count; x++)
			{
				s.Write(k_ident+"\t");
				s.Write(x.ToString("X4"));
				s.Write("\t");
				Categories[x].ToStream(s, tag, this);
			}
			s.WriteLine();
		}

		public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			s.WriteStartElement("goof");
			s.WriteAttributeString("name", tag.Name);
			{
				s.WriteStartElement("categories");
				foreach (var cat in Categories)
					cat.ToStream(s, tag, this);
				s.WriteEndElement();
			}
			s.WriteEndElement();
		}
	};
	#endregion

	#region text_value_pair_definition_group
	partial class text_value_pair_definition_group : ITempToStreamInterface, ITempToXmlStreamInterface
	{
		#region text_value_pair_reference_block
		partial class text_value_pair_reference_block
		{
			public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				const string k_ident = "\t\t\t\t";

				s.WriteLine(Name);
				s.WriteLine(k_ident+
					"Integer={0}\t" + "Real={1}\t" + 
					"StringId={2}\t" + "Flags={3}\t",
					Integer.Value.ToString("X8"), Real.Value.ToString("r"), 
					StringId, Flags);
			}

			public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner, int type)
			{
				s.WriteStartElement("entry");

				s.WriteAttributeString("labelId", Name.ToString());
				if (!Description.Handle.IsNull) s.WriteAttributeString("descId", Description.ToString());
				switch (type)
				{
					case 0: s.WriteAttributeString("int", Integer.Value.ToString()); break;
					case 1: s.WriteAttributeString("stringId", StringId.ToString()); break;
					case 2: System.Diagnostics.Debugger.Break(); break;
					case 3: s.WriteAttributeString("real", Real.Value.ToString("r")); break;
				}
				if (Flags.Value != 0)
					s.WriteAttributeString("flags", Flags.ToString());

				s.WriteEndElement();
			}
		};
		#endregion

		public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			const string k_ident = "\t";

			s.WriteLine(k_ident+"Title={0}", Title);
			s.WriteLine(k_ident+"Type={0}\tParam={1}", Type, Parameter);
			s.WriteLine(k_ident+"Values");
			for (int x = 0; x < Values.Count; x++)
			{
				s.Write(k_ident+"\t");
				s.Write(x.ToString("X4"));
				s.Write("\t");
				Values[x].ToStream(s, tag, this);
			}
			s.WriteLine();
		}

		public static SortedList<int, string> SettingParameters = new SortedList<int, string>();
		public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			s.WriteStartElement("sily");
			s.WriteAttributeString("name", tag.Name);
			{
				s.WriteAttributeString("titleId", Title.ToString());
				if (!Description.Handle.IsNull) s.WriteAttributeString("descId", Description.ToString());
				s.WriteAttributeString("param", Parameter.ToString());

				SettingParameters[Parameter.Value] = tag.Name;

				s.WriteStartElement("values");
				s.WriteAttributeString("type", Type.ToString());
				foreach (var vp in Values)
					vp.ToStream(s, tag, this, Type.Value);
				s.WriteEndElement();
			}
			s.WriteEndElement();
		}
	};
	#endregion
}