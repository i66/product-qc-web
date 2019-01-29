using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace product_qc_web.Lib
{
    public struct CsvField
    {
        public const string PRODUCT_NAME = "ProductName";
        public const string QC_FINISHED_TIME = "QcFinishedTime";
        public const string WORK_ORDER_NUM = "WorkOrderNum";
        public const string MACHINE_NUM = "MachineNum";
        public const string DELIVERY_DEST = "DeliveryDestination";
        public const string EXCHANGE_MAL_NOTE = "ExchangeReturnMalfunctionNote";
    }

    public class FieldUtility
    {
        public string getFieldName(List<Type> t)
        {
            if (t == null)
                return null;

            Dictionary<string, string> allFieldName = new Dictionary<string, string>();

            foreach (Type item in t)
            {
                PropertyInfo[] props = item.GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object[] attrs = prop.GetCustomAttributes(true);
                    foreach (object attr in attrs)
                    {
                        DisplayAttribute field = attr as DisplayAttribute;
                        if (field != null)
                            allFieldName.Add(prop.Name, field.Name);
                    }
                }
            }
            return concatField(allFieldName);
        }

        private string concatField(Dictionary<string, string> allField)
        {
            if (allField == null || !areAllFieldExist(allField))
                return null;

            string result = allField[CsvField.PRODUCT_NAME] + "," + allField[CsvField.QC_FINISHED_TIME] + "," +
                            allField[CsvField.WORK_ORDER_NUM] + "," + allField[CsvField.MACHINE_NUM] + "," +
                            allField[CsvField.DELIVERY_DEST] + "," + allField[CsvField.EXCHANGE_MAL_NOTE];

            return result;
        }

        private bool areAllFieldExist(Dictionary<string, string> allField)
        {
            if (!allField.ContainsKey(CsvField.PRODUCT_NAME))
                return false;
            if (!allField.ContainsKey(CsvField.QC_FINISHED_TIME))
                return false;
            if (!allField.ContainsKey(CsvField.WORK_ORDER_NUM))
                return false;
            if (!allField.ContainsKey(CsvField.MACHINE_NUM))
                return false;
            if (!allField.ContainsKey(CsvField.DELIVERY_DEST))
                return false;
            if (!allField.ContainsKey(CsvField.EXCHANGE_MAL_NOTE))
                return false;

            return true;
        }
    }
}
