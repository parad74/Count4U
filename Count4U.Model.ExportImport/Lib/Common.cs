using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Lib.MultiPoint
{

	public enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 };

	public enum MPStage { Bell = 0x07, Upload = 0x4C, Download = 0x55 };

	public enum MPMemory { RAM = 0x00, FLASH = 0x02, ROM = 0x03, NONE = 0x01 };

	public struct ItemDir
	{
		public string name;
		public int size;
		//public int memory;
	}

	public delegate void MPState(IMulti from, MPStage stage, int state, string text, int current);

}
