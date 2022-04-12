using System.Collections.Generic;
using Count4U.Model.Lib.MultiPoint;

namespace Count4U.Model.Count4U
{
    public class FakeUploadPdaRepository : IWrapperMultiRepository
    {
        private const int FakeCount = 5;

		public List<IWrapperMulti> GetPortsAndWakeUP(int baudrate = 57600, string from = "", bool reWakeUp = true)
        {
            List<IWrapperMulti> result = new List<IWrapperMulti>();

            for (int i = 0; i < FakeCount; i++)
            {
                FakeUploadPdaUnit unit = new FakeUploadPdaUnit();
                unit.Id = i+1;
                unit.ComPortStatic = unit.Id.ToString();

                result.Add(unit);

                if (i == FakeCount - 1)
                {
                    unit.IsFault = true;
                }
            }

            return result;
        }


		public void WakeUpAllPorts(List<Multi> multis, int baudrate = 57600, string from = "")
		{
			
		}


		public void AbortThreadAllThreads(List<Multi> multis, string from = "")
		{

		}


		public void SetDateTimeAllPDA(List<Multi> multis, System.DateTime dateTime, string from = "")
		{
			throw new System.NotImplementedException();
		}


		public void DeleteFilesAllPDA(List<Multi> multis, List<string> exclude, string from = "")
		{
			throw new System.NotImplementedException();
		}

		public void WarmStartAllPDA(List<Multi> multis, string from = "")
		{
			throw new System.NotImplementedException();
		}

		public void RunFilesAllPDA(List<Multi> multis, List<string> files, string from = "")
		{
			throw new System.NotImplementedException();
		}


		public void GetTerminalIDAllPDA(List<Multi> multis, string from = "")
		{
			throw new System.NotImplementedException();
		}
	}
}