using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Count4U.Model.Count4U
{
	public static class BitStatus
	{
		public static string ToString(int sumBit)
		{
			string ret = "";
			List<int> bits = SplitBit(sumBit);
			if (bits.Count() == 0)
			{
				ret = "0";
			}
			else
			{
				foreach (var bit in bits)
				{
					ret = ret + bit.ToString() + ";";
				}
				ret = ret.Trim(';');
			}
			return ret;
		}

		public static List<BitArray> GetBitList(int[] bits)
		{
			//BitArray bit = new BitArray(new int[] {4});

			//BitArray[] bitarray = new BitArray[] {
			//    new BitArray(new int[] {4}), 
			//    new BitArray(new int[] {4})};

			//List<BitArray> ba = new List<BitArray>();
			//ba.Add(new BitArray(new int[] { 4 }));
			List<BitArray> bitList = new List<BitArray>();
			for (int i = 0; i < bits.Length; i++)
			{
				bitList.Add(new BitArray(new int[] { bits[i] }));
			}
			return bitList;
		}

		public static List<int> SplitBit(int bit)
		{
			List<int> bitList = new List<int>();
			if (bit == null) return bitList;

			if (bit / 1024 > 0)
			{
				bitList.Add(1024);
				bit = bit % 1024;
			}
			if (bit / 512 > 0)
			{
				bitList.Add(512);
				bit = bit % 512;
			}
			if (bit / 256 > 0)
			{
				bitList.Add(256);
				bit = bit % 256;
			}
			if (bit / 128 > 0)
			{
				bitList.Add(128);
				bit = bit % 128;
			}
			if (bit / 64 > 0)
			{
				bitList.Add(64);
				bit = bit % 64;
			}
			if (bit / 32 > 0)
			{
				bitList.Add(32);
				bit = bit % 32;
			}
			if (bit / 16 > 0)
			{
				bitList.Add(16);
				bit = bit % 16;
			}
			if (bit / 8 > 0)
			{
				bitList.Add(8);
				bit = bit % 8;
			}
			if (bit / 4 > 0)
			{
				bitList.Add(4);
				bit = bit % 4;
			}
			if (bit / 2 > 0)
			{
				bitList.Add(2);
				bit = bit % 2;
			}
			if (bit / 1 > 0)
			{
				bitList.Add(1);
			}
			return bitList;
		}

		public static BitArray GetResultBitArrayOr(int[] bits)
		{
			BitArray bitArrayOr = new BitArray(new int[]{0});
			if (bits == null) return bitArrayOr;
			for (int i = 0; i < bits.Length; i++)
			{
				BitArray bitArrayOr1 = new BitArray(new int[] { bits[i] });
				bitArrayOr = bitArrayOr.Or(bitArrayOr1);
			}
			return bitArrayOr;
		}

		public static BitArray GetResultBitArrayAnd(int[] bits)
		{
			BitArray bitArrayAdd = new BitArray(new int[] { 0 });
			if (bits == null) return bitArrayAdd;
			for (int i = 0; i < bits.Length; i++)
			{
				BitArray bitArrayAdd1 = new BitArray(new int[] { bits[i] });
				bitArrayAdd = bitArrayAdd.And(bitArrayAdd1);
			}
			return bitArrayAdd;
		}

		public static int GetResultBitIntArrayAnd(int[] bits)
		{
			BitArray bitArrayAdd = new BitArray(new int[] { 0 });
			if (bits == null) return 0;
			if (bits.Length > 0)
			{
				bitArrayAdd = new BitArray(new int[] { bits[0] });
				for (int i = 1; i < bits.Length; i++)
				{
					BitArray bitArrayAdd1 = new BitArray(new int[] { bits[i] });
					bitArrayAdd = bitArrayAdd.And(bitArrayAdd1);
				}
			}
			int[] array = new int[1];
			bitArrayAdd.CopyTo(array, 0);
			return array[0];
		}

		public static BitArray GetBitArray(int bit)
		{
			return new BitArray(new int[] { bit });
		}

		public static int GetInt32(BitArray bitArray)
		{
			if (bitArray == null) return 0;
			int[] array = new int[1];
			bitArray.CopyTo(array, 0);
			return array[0];
		}

			public static int Or(this int entity, int inStatusBit)
			{
				BitArray bitArrayOr = new BitArray(new int[] { entity });
				BitArray bitArrayOr1 = new BitArray(new int[] { inStatusBit });
				bitArrayOr = bitArrayOr.Or(bitArrayOr1);
				return GetInt32(bitArrayOr);
			}
	}
}
