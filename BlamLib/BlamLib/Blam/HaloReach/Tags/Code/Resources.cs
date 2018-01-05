/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using System.IO;
using System.Xml;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.HaloReach.Tags
{
	#region multilingual_unicode_string_list
	partial class multilingual_unicode_string_list_group
	{
		#region multilingual_unicode_string_reference_block
		partial class multilingual_unicode_string_reference_block
		{
			public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				const string k_ident = "\t\t\t";
				var unic = owner as multilingual_unicode_string_list_group;

				s.WriteLine(StringId);
				s.Write(k_ident);
				s.WriteLine(unic.GetStringUtf8(LanguageOffsets[(int)LanguageType.English]));
			}

			public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				var unic = owner as multilingual_unicode_string_list_group;

				s.WriteStartElement("entry");
				s.WriteAttributeString("key", StringId.ToString());
				s.WriteAttributeString("value", unic.GetStringUtf8(LanguageOffsets[(int)LanguageType.English]));
				s.WriteEndElement();

#if false
				s.WriteStartElement("localization");
				for (LanguageType lang = LanguageType.English+1; lang < LanguageType.kMax; lang++)
				{
					s.WriteAttributeString("key", lang.ToString());
					s.WriteAttributeString("value", unic.GetStringUtf8(LanguageOffsets[(int)lang]));
				}
				s.WriteEndElement();
#endif
			}
		}
		#endregion

		#region multilingual_unicode_string_list_C_block
		partial class multilingual_unicode_string_list_C_block
		{
			public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				const string k_ident = "\t\t\t";

				s.WriteLine(k_ident+
					"Component={0}\t" + "Property={1}",
					Component, Property);
			}

			public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
			{
				s.WriteStartElement("entry");
				s.WriteAttributeString("component", Component.ToString());
				s.WriteAttributeString("prop", Property.ToString());
				s.WriteEndElement();
			}
		}
		#endregion

		string GetStringUtf8(int offset)
		{
			byte[] data = StringData.Value;
			int x;
			for (/*int*/ x = offset; x < data.Length; x++)
			{
				byte b0 = data[x];
				if (b0 == 0) break;
			}
			return System.Text.Encoding.UTF8.GetString(data, offset, x - offset);
		}
		void StringToStreamUnicode(int offset, StreamWriter s)
		{
			byte[] data = StringData.Value;
			int x;
			for (/*int*/ x = offset; x < data.Length-1; x+=2)
			{
				byte b0 = data[x];
				byte b1 = data[x+1];
				if(b0 == 0 && b1 == 0) break;

				char c = (char)(b0 << 8 | b1);
				s.Write(c);
			}
			s.WriteLine();
		}

		internal void ReconstructHack(CacheFile c, Cache.CacheFileLanguagePackResource english)
		{
			var interop = new Cache.CacheFileLanguageHandleInterop[((int)LanguageType.English)+1];
			for (int x = 0; x < interop.Length; x++)
				interop[x] = new Cache.CacheFileLanguageHandleInterop();
			int total_string_buffer_size = 0;

			// For each language, get the string data referenced by this tag
			for (LanguageType lang = LanguageType.English; lang < (LanguageType.English+1); lang++)
				total_string_buffer_size = interop[(int)lang].Initialize(//clpi.LanguagePackGet(lang),
					english,
					total_string_buffer_size, LanguageHandles[(int)lang]);

			// Resize the string data container to have enough memory for all of our language strings
			this.StringData.Value = new byte[total_string_buffer_size];

			// For each language, copy the string data and initialize the reference blocks
			for (LanguageType lang = LanguageType.English; lang < (LanguageType.English+1); lang++)
				interop[(int)lang].ReconstructTagData(c, this, (int)lang);

			// Null the handles for the extracted tag
			foreach (var handle in LanguageHandles)
				handle.Value = 0;
		}

		public void ToStream(StreamWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			const string k_ident = "\t";

			s.WriteLine(k_ident+"StringRefs");
			for (int x = 0; x < StringRefs.Count; x++)
			{
				s.Write(k_ident+"\t");
				StringRefs[x].ToStream(s, tag, this);
			}
			s.WriteLine();
			s.WriteLine(k_ident+"BlockC");
			for (int x = 0; x < BlockC.Count; x++)
			{
				s.Write(k_ident+"\t");
				s.Write(x.ToString("X4"));
				s.Write("\t");
				BlockC[x].ToStream(s, tag, this);
			}
			s.WriteLine();
		}

		public void ToStream(XmlWriter s,
				Managers.TagManager tag, TI.Definition owner)
		{
			s.WriteStartElement("unic");
			s.WriteAttributeString("name", tag.Name);
			{
				s.WriteStartElement("references");
				foreach (var sr in StringRefs)
					sr.ToStream(s, tag, this);
				s.WriteEndElement();

				if (false && BlockC.Count > 0)
				{
					s.WriteStartElement("unkC");
					foreach (var sr in BlockC)
						sr.ToStream(s, tag, this);
					s.WriteEndElement();
				}
			}
			s.WriteEndElement();
		}
	};
	#endregion
}