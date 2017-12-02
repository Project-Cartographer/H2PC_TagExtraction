/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
using System;
using System.Collections.Generic;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Cache
{
	/// <summary>
	/// Structure used to reference a single language's string data in a cache's language pack
	/// </summary>
	struct CacheFileLanguagePackStringReference : IO.IStreamable
	{
		public Blam.StringId NameId;
		public int Offset;

		public CacheFileLanguagePackStringReference(Blam.StringIdDesc desc)
		{
			NameId = new Blam.StringId(desc, 0);
			Offset = 0;
		}

		#region IStreamable Members
		public void Read(BlamLib.IO.EndianReader s)
		{
			NameId.Read(s, (s.Owner as CacheFile).StringIds.Definition.Description);
			Offset = s.ReadInt32();
		}

		public void Write(BlamLib.IO.EndianWriter s)
		{
			NameId.Write(s);
			s.Write(Offset);
		}
		#endregion
	};

	abstract class CacheFileLanguagePackResource : IO.IStreamable
	{
		public TI.LongInteger Count;
		public TI.LongInteger Size;
		protected TI.LongInteger OffsetReferences;
		protected TI.LongInteger OffsetStrings;

		protected CacheFileLanguagePackResource()
		{
			Count = new TI.LongInteger();
			Size = new TI.LongInteger();
			OffsetReferences = new TI.LongInteger();
			OffsetStrings = new TI.LongInteger();
		}

		#region IStreamable Members
		public abstract void Read(BlamLib.IO.EndianReader s);
		public abstract void Write(BlamLib.IO.EndianWriter s);
		#endregion

		#region Cache interop
		internal virtual int LanguageHandleGetReferenceIndex(int id)
		{
			return id & 0xFFFF;
		}
		internal virtual int LanguageHandleGetCount(int id)
		{
			return (id >> 16) & 0xFFFF;
		}

		protected CacheFileLanguagePackStringReference[] mStringReferences;
		protected byte[] mStringData;
		/// <summary>
		/// Has this language pack already been loaded from a cache file?
		/// </summary>
		internal bool IsLoaded { get { return mStringReferences != null; } }

		int GetStringBufferSize(int reference_index)
		{
			if (reference_index == mStringReferences.Length - 1)
				return Size.Value - mStringReferences[reference_index].Offset;

			return mStringReferences[reference_index + 1].Offset -
				mStringReferences[reference_index].Offset;
		}

		int GetTotalStringBufferSize(int reference_index, int count)
		{
			if (reference_index + count == mStringReferences.Length)
				return Size.Value - mStringReferences[reference_index].Offset;

			return mStringReferences[reference_index + count].Offset -
				mStringReferences[reference_index].Offset;
		}

		/// <summary>
		/// Get the base offset of a set of string references based on the their 
		/// initial reference index
		/// </summary>
		/// <param name="reference_index">First string's reference index</param>
		/// <returns></returns>
		int GetStringBaseOffset(int reference_index)
		{
			return mStringReferences[reference_index].Offset;
		}

		protected void InitializeStringReferences(int count, Managers.StringIdCollectionDefintion def)
		{
			mStringReferences = new CacheFileLanguagePackStringReference[count];
			for (int x = 0; x < mStringReferences.Length; x++)
				mStringReferences[x] = new CacheFileLanguagePackStringReference(def.Description);
		}
		public abstract void ReadFromCache(Blam.CacheFile cf);
		#endregion

		#region Reconstruction interface
		/// <summary>
		/// Copy the string data of <paramref name="reference_index"/> into the <paramref name="dst"/> array 
		/// at starting index <paramref name="dst_offset"/> and return the total bytes copied. Also provides the 
		/// string id name of <paramref name="reference_index"/>.
		/// </summary>
		/// <param name="reference_index"></param>
		/// <param name="dst"></param>
		/// <param name="dst_offset"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		internal int CopyStringReferenceData(int reference_index, byte[] dst, int dst_offset, out Blam.StringId name)
		{
			int offset = GetStringBaseOffset(reference_index);
			int size = GetStringBufferSize(reference_index);

			Array.Copy(mStringData, offset, dst, dst_offset, size);

			name = mStringReferences[reference_index].NameId;

			return size;
		}
		/// <summary>
		/// Predict how much space this language's string data will need to consume in a 
		/// <see cref="multilingual_unicode_string_list_group"/> tag
		/// </summary>
		/// <param name="reference_index">Start of the tag's string references</param>
		/// <param name="count">Number of string references the tag has</param>
		/// <returns></returns>
		internal int PredictRequiredStringDataSize(int reference_index, int count)
		{
			return GetTotalStringBufferSize(reference_index, count);
		}
		#endregion
	};
	interface ICacheFileLanguagePackContainer
	{
		CacheFileLanguagePackResource LanguagePackGet(int language_type);
	};

	interface IMultilingualUnicodeStringReference
	{
		TI.StringId Name { get; }
		TI.LongInteger[] LanguageOffsets { get; }
	};
	interface IMultilingualUnicodeStringList
	{
		IMultilingualUnicodeStringReference NewReference();
		IMultilingualUnicodeStringReference GetReference(int element_index);

		IEnumerable<IMultilingualUnicodeStringReference> StringRefs { get; }
		TI.Data StringData { get; }
		TI.LongInteger[] LanguageHandles { get; }
	};
	class CacheFileLanguageHandleInterop
	{
		CacheFileLanguagePackResource mPack;
		int mReferenceIndex;
		int mReferenceCount;
		int mTagDataOffset;
		int mTagDataSize;

		#region Initialize
		void DecodeHandle(int handle)
		{
			mReferenceIndex = mPack.LanguageHandleGetReferenceIndex(handle);
			mReferenceCount = mPack.LanguageHandleGetCount(handle);
		}
		void PredictRequiredStringDataSize()
		{
			if (mReferenceCount > 0)
				mTagDataSize = mPack.PredictRequiredStringDataSize(mReferenceIndex, mReferenceCount);
		}

		public int Initialize(CacheFileLanguagePackResource lang_pack, int tag_data_offset, int handle)
		{
			mPack = lang_pack;
			mTagDataOffset = tag_data_offset;

			DecodeHandle(handle);
			PredictRequiredStringDataSize();

			return tag_data_offset + mTagDataSize;
		}
		#endregion

		#region Reconstruct
		static IMultilingualUnicodeStringReference AddOrGetReferenceByName(Blam.CacheFile c, 
			IMultilingualUnicodeStringList list, Blam.StringId name)
		{
			// See if a reference already exists for [name]...
			foreach (var sref in list.StringRefs)
				if (sref.Name.Handle == name)
					return sref;

			// one doesn't, so add it and return the new block data
			IMultilingualUnicodeStringReference sr = list.NewReference();

			// Initialize name id
			sr.Name.Handle = name;
			sr.Name.OwnerId = c.TagIndexManager.IndexId;

			// Initialize all the offsets to be invalid
			foreach (var lang_offset in sr.LanguageOffsets)
				lang_offset.Value = -1;

			return sr;
		}
		public void ReconstructTagData(Blam.CacheFile c, 
			IMultilingualUnicodeStringList list, int this_lang)
		{
			for (int x = 0, dst_offset = mTagDataOffset, size; x < mReferenceCount; x++, dst_offset += size)
			{
				var dst = list.StringData.Value;

				Blam.StringId name;
				size = mPack.CopyStringReferenceData(mReferenceIndex + x, dst, dst_offset, out name);

				var sref = AddOrGetReferenceByName(c, list, name);
				sref.LanguageOffsets[this_lang].Value = dst_offset;
			}
		}
		#endregion
	};
};