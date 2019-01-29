using Microsoft.AspNetCore.Mvc;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace product_qc_web.Controllers
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

    public class EmailUtilitiesController : Controller
    {
        const string SMTP_SERVER = "smtp.gmail.com";
        const string CSV_SEPARATOR = ",";
        const string DATE_FORMAT = "yyyyMMdd";
        const string EMAIL_PASSWORD = "Hex54232885";
        private string subject = "成品庫存與QC進度_" + DateTime.Now.ToString(DATE_FORMAT);
        private DateTime weekStamp = DateTime.Now.AddDays(-6);
        private readonly HexsaveContext _context;

        public EmailUtilitiesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: EmailUtilities
        public IActionResult SendEmail(string from, string recipients)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(recipients))
                return errorResponse("寄件人或收信人不能為空的！！");

            List<Type> t = new List<Type> { typeof(MetadataTDelivery), typeof(MetadataTProduct) };
            string fieldName = concatField(getFieldName(t));

            if (string.IsNullOrWhiteSpace(fieldName))
                return errorResponse("Field不能為空！！");

            IQueryable <TDelivery> data = (from p in _context.TProduct
                                          join m in _context.TManufacture on p.ProductCode equals m.ProductCode
                                          join q in _context.TQualityCheck on new { m.WorkOrderNum, m.MachineNum } equals new { q.WorkOrderNum, q.MachineNum }
                                          join d in _context.TDelivery on new { m.WorkOrderNum, m.MachineNum } equals new { d.WorkOrderNum, d.MachineNum }
                                          where d.LastModifiedTime >= weekStamp
                                          orderby p.ProductName, q.QcFinishedTime descending, d.WorkOrderNum, d.MachineNum
                                          select new TDelivery()
                                          {
                                              ProductName = p.ProductName,
                                              WorkOrderNum = d.WorkOrderNum,
                                              MachineNum = d.MachineNum,
                                              QcFinishedTime = q.QcFinishedTime,
                                              DeliveryDestination = d.DeliveryDestination,
                                              ExchangeReturnMalfunctionNote = d.ExchangeReturnMalfunctionNote,
                                              LastModifiedTime = d.LastModifiedTime,
                                          }
                                         );

            List<TDelivery> dataList = data.ToList();

            string txtContent = fieldName + "\n" + string.Concat(from d in dataList
                                                   select string.Format("{0}" + CSV_SEPARATOR + "{1}" + CSV_SEPARATOR +
                                                   "{2}" + CSV_SEPARATOR + "{3}" + CSV_SEPARATOR +
                                                   "{4}" + CSV_SEPARATOR + "{5}\n",
                                                   d.ProductName, d.QcFinishedTime.ToString(DATE_FORMAT),
                                                   d.WorkOrderNum, d.MachineNum,
                                                   d.DeliveryDestination, d.ExchangeReturnMalfunctionNote));

            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms, Encoding.UTF8);
            writer.Write(txtContent);
            writer.Flush();
            ms.Position = 0;

            ContentType ct = new ContentType();
            ct.MediaType = MediaTypeNames.Text.Plain;
            ct.Name = subject + ".csv";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(from);
            foreach (string recipient in recipients.Split(";"))
            {
                msg.To.Add(recipient);
            }

            msg.Subject = subject;
            msg.Body = "<p>Hello dear,<p></br>" +
                "請查收附件，本週成品庫存與QC進度報表：" + subject + "。</br></br>" +
                "Best regards,</br>" +
                "Jenkins</br></br>";
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;

            Attachment att = new Attachment(ms, ct);
            msg.Attachments.Add(att);

            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = SMTP_SERVER;
                client.Port = 25;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(msg.From.Address, EMAIL_PASSWORD);
                client.Send(msg);
                client.Dispose();
                msg.Dispose();
            }
            catch (Exception ex)
            {
                return errorResponse("寄送郵件過程發生錯誤！！" + ex.ToString());
            }

            return View();
        }

        private Dictionary<string, string> getFieldName(List<Type> t)
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
            return allFieldName;
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

        private IActionResult errorResponse(string errorMsg)
        {
            ViewData["ErrorMsg"] = "錯誤發生:" + errorMsg;
            return View();
        }


    }
}