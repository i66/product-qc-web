using System;
using System.Collections.Generic;

namespace product_qc_web.Lib
{
    public class ParserUtility
    {
        public const string SEPERATOR = ",";
        public List<int> ParsingMachineNum(string machineNumList)
        {
            if (string.IsNullOrWhiteSpace(machineNumList))
                return new List<int>();
            string[] parserResult = machineNumList.Split(SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            List<int> result = new List<int>();
            foreach (string numData in parserResult)
            {
                int temp;
                if (!int.TryParse(numData.Trim(), out temp))
                    continue;
                if (result.Contains(temp))
                    continue;
                result.Add(temp);
            }
            return result;
        }

    }
}
