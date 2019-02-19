using Microsoft.AspNetCore.Mvc;
using product_qc_web.Lib;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace product_qc_web.Controllers
{
    public class EmailUtilitiesController : Controller
    {
        const string SMTP_SERVER = "smtp.gmail.com";
        const string CSV_SEPARATOR = ",";
        const string DATE_FORMAT = "yyyyMMdd";
        const string EMAIL_PASSWORD = "hex54232885";
        private string m_subject = "成品庫存與QC進度_" + DateTime.Now.ToString(DATE_FORMAT);
        private readonly HexsaveContext _context;

        public EmailUtilitiesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: EmailUtilities
        public IActionResult SendEmail(string encryptMsg)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(encryptMsg))
                    return errorResponse("訊息不能為空的！！");

                string fileFieldName = getFileFieldName();
                if (string.IsNullOrWhiteSpace(fileFieldName))
                    return errorResponse("Field不能為空！！");

                DataModel decryptedData = new Encryption().StartToDecryptMsg(encryptMsg);

                if (!decryptedData.isValid)
                    return errorResponse("解密錯誤！！");

                string from = decryptedData.from;
                string recipients = decryptedData.recipient;
                string attachedFile = getFileContent();

                using (MailMessage msg = createEmailMsg(from, recipients, attachedFile))
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Host = SMTP_SERVER;
                        client.Port = 25;
                        client.EnableSsl = true;
                        client.Credentials = new NetworkCredential(msg.From.Address, EMAIL_PASSWORD);
                        client.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                return errorResponse("寄送郵件過程發生錯誤！！" + ex.ToString());
            }

            return View();
        }

        private IActionResult errorResponse(string errorMsg)
        {
            ViewData["ErrorMsg"] = "錯誤發生:" + errorMsg;
            return View();
        }

        private List<TDelivery> getModifiedQcDataInOneWeek()
        {
            DateTime weekStamp = DateTime.Now.AddDays(-6);
            IQueryable<TDelivery> data = (from p in _context.TProduct
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

            return dataList;
        }

        private string getFileFieldName()
        {
            List<Type> t = new List<Type> { typeof(MetadataTDelivery), typeof(MetadataTProduct) };
            string fieldName = new FieldUtility().getFieldName(t);

            return fieldName;
        }

        private string getFileContent()
        {
            List<TDelivery> modifiedQcDataInOneWeek = getModifiedQcDataInOneWeek();

            if (modifiedQcDataInOneWeek.Count() == 0)
                return null;

            string fieldName = getFileFieldName();
            string txtContent = fieldName + "\n" + string.Concat(from d in modifiedQcDataInOneWeek
                                                                 select string.Format("{0}" + CSV_SEPARATOR + "{1}" + CSV_SEPARATOR +
                                                                 "{2}" + CSV_SEPARATOR + "{3}" + CSV_SEPARATOR +
                                                                 "{4}" + CSV_SEPARATOR + "{5}\n",
                                                                 d.ProductName, d.QcFinishedTime.ToString(DATE_FORMAT),
                                                                 d.WorkOrderNum, d.MachineNum,
                                                                 d.DeliveryDestination, d.ExchangeReturnMalfunctionNote));

            return txtContent;
        }

        private MailMessage createEmailMsg(string from, string recipients, string attachedFileContent)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(from);
            foreach (string recipient in recipients.Split(";"))
            {
                msg.To.Add(recipient);
            }

            msg.Subject = m_subject;
            msg.Body = getMsgBody(attachedFileContent);

            if (!string.IsNullOrWhiteSpace(attachedFileContent))
            {
                msg.Attachments.Add(new Attachment(createFileMemoryStream(attachedFileContent), createFileContentType()));
            }
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;

            return msg;
        }

        private string getMsgBody(string attachedFileContent)
        {
            string body = string.Empty;
            if (string.IsNullOrWhiteSpace(attachedFileContent))
            {
                body = "<p>Hello dear,</p>" +
                     "<p>本週無異動的QC庫存資料，沒什麼大事，週末愉快~</p>" +
                     "<p>Best regards,<br />" +
                     "Jenkins</p><br />";
            }
            else
            {
                body = "<p>Hello dear,</p>" +
                      "<p>請查收附件，本週成品庫存與QC進度報表：" + m_subject + "。</p>" +
                      "<p>Best regards,<br />" +
                      "Jenkins</p><br />";
            }

            return body;
        }

        private MemoryStream createFileMemoryStream(string attachedFile)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms, Encoding.UTF8);
            writer.Write(attachedFile);
            writer.Flush();
            ms.Position = 0;

            return ms;
        }

        private ContentType createFileContentType()
        {
            ContentType ct = new ContentType();
            ct.MediaType = MediaTypeNames.Text.Plain;
            ct.Name = m_subject + ".csv";

            return ct;
        }

    }
}