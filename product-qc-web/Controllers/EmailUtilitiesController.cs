using Microsoft.AspNetCore.Mvc;
using product_qc_web.Lib;
using product_qc_web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private string subject = "成品庫存與QC進度_" + DateTime.Now.ToString(DATE_FORMAT);
        private DateTime weekStamp = DateTime.Now.AddDays(-6);
        private readonly HexsaveContext _context;

        public EmailUtilitiesController(HexsaveContext context)
        {
            _context = context;
        }

        // GET: EmailUtilities
        public IActionResult SendEmail(string encryptMsg)
        {
            if (string.IsNullOrWhiteSpace(encryptMsg))
                return errorResponse("訊息不能為空的！！");

            Encryption secret = new Encryption();

            Dictionary<string, string> decryptMsg = secret.GetDecryptMsg(encryptMsg);

            if(decryptMsg.Count() < 1)
                return errorResponse("解密錯誤！！");
            if (!decryptMsg.ContainsKey(secret.PARA_FROM))
                return errorResponse("解密錯誤！！沒有寄件人！！");
            if (!decryptMsg.ContainsKey(secret.PARA_RECIPIENT))
                return errorResponse("解密錯誤！！沒有收件人！！");

            string from = decryptMsg[secret.PARA_FROM];
            string recipients = decryptMsg[secret.PARA_RECIPIENT];

            List<Type> t = new List<Type> { typeof(MetadataTDelivery), typeof(MetadataTProduct) };
            string fieldName = new FieldUtility().getFieldName(t);

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

            if(dataList.Count() == 0)
            {
                msg.Body = "<p>Hello dear,</p>" +
                     "<p>本週無異動的QC庫存資料，沒什麼大事，週末愉快~</p>" +
                     "<p>Best regards,<br />" +
                     "Jenkins</p><br />";
            }
            else
            {
                msg.Body = "<p>Hello dear,</p>" +
                      "<p>請查收附件，本週成品庫存與QC進度報表：" + subject + "。</p>" +
                      "<p>Best regards,<br />" +
                      "Jenkins</p><br />";

                Attachment att = new Attachment(ms, ct);
                msg.Attachments.Add(att);
            }
           
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;

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

        private IActionResult errorResponse(string errorMsg)
        {
            ViewData["ErrorMsg"] = "錯誤發生:" + errorMsg;
            return View();
        }


    }
}