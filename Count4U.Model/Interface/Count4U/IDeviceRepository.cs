using System;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;

namespace Count4U.Model.Interface.Count4U
{
	public interface IDeviceRepository
	{
		void FillDevicesFromDocumrntHeaders(string pathDB);
		Devices GetDevices(string pathDB, bool fefill = false);
		Devices GetDevices(int topCount, string pathDB, bool fefill = false);
		Devices GetDevices(SelectParams selectParams, string pathDB, bool fefill = false);
		void Delete(Device device, string pathDB);
		void DeleteAll(string pathDB);
		void Insert(Device device, string pathDB);
		void Update(Device device, string pathDB);
		Device GetDeviceByName(string name, string pathDB, bool fefill = false);
		Device GetDeviceByCode(string code, string pathDB, bool fefill = false);
		Devices RefillDeviceStatisticByDeviceCode(DateTime inventorDate, DateTime endInventorDate, SelectParams selectParams, string pathDB);
		Devices RefillDeviceStatisticByDeviceAndWorker(DateTime startInventorDate, DateTime endInventorDate, /*SelectParams selectParams,*/ string pathDB);
		Devices RefillDeviceStatisticByWorker(DateTime startInventorDate, DateTime endInventorDate, /*SelectParams selectParams, */string pathDB);
		Devices RefillDeviceStatisticByDeviceAndWorkerAndItur(DateTime startInventorDate, DateTime endInventorDate, SelectParams selectParams, string pathDB);

		DateTime GetTheLastForDevice(/*SelectParams selectParams,*/ string pathDB);
		Dictionary<string, Device> GetDeviceDictionary(string pathDB, bool refill = false);
		void ClearDeviceDictionary();
		void AddDeviceInDictionary(string code, Device device);
		void RemoveDeviceFromDictionary(string code);
		bool IsExistDeviceInDictionary(string code);
		Device GetDeviceByCodeFromDictionary(string code);
		void FillDeviceDictionary(string pathDB);
		List<string> GetDeviceCodeList(string pathDB);

	}
}
