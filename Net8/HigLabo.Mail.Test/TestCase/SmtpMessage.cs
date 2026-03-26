using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using HigLabo.Converter;
using System.Net.Mail;
using HigLabo.Net.Smtp;
using HigLabo.Mime;

namespace HigLabo.Mail.Test;

[TestClass]
public class SmtpMessageTest
    {
    [TestMethod]
    public void SmtpMessage_TextPlain()
    {
        SmtpMessage textMessage = new SmtpMessage();
        textMessage.From = new HigLabo.Mime.MailAddress("from@mail.aa");
        textMessage.To.Add(new HigLabo.Mime.MailAddress("to@mail.aa"));
        textMessage.Subject = "Sample mail";
        textMessage.BodyText = "This is a sample mail!";
        textMessage.Date = new DateTimeOffset(2021, 11, 24, 12, 1, 2, new TimeSpan(3, 0, 0));

        String mailActual = textMessage.GetRawText();
        String mailExpected = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData", "SmtpMessage_TextPlain.txt"));

        Assert.AreEqual(mailExpected, mailActual);
    }
    
    [TestMethod]
    public void SmtpMessage_MultipartAlternative()
    {
        SmtpMessage textMessage = new SmtpMessage();
        textMessage.From = new HigLabo.Mime.MailAddress("from@mail.aa");
        textMessage.To.Add(new HigLabo.Mime.MailAddress("to@mail.aa"));
        textMessage.Subject = "Sample HTML mail";
        textMessage.SetBodyAsAlternativeContent("TEXT", "<b>TEXT</b>");

        //  same as sample message
        textMessage.Date = new DateTimeOffset(2021, 11, 24, 12, 1, 2, new TimeSpan(3, 0, 0));
        textMessage.Contents[0].ContentType.Boundary = "ba2b2ac2_df20_487e_88f8_75bf6ba6cd12";

        String mailActual = textMessage.GetRawText();
        String mailExpected = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData", "SmtpMessage_MultipartAlternative.txt"));

        Assert.AreEqual(mailExpected, mailActual);
    }
    
    [TestMethod]
    public void SmtpMessage_MultipartAlternativeAndAttachment()
    {
        SmtpMessage msg = new SmtpMessage();
        msg.From = new HigLabo.Mime.MailAddress("from@mail.aa");
        msg.To.Add(new HigLabo.Mime.MailAddress("to@mail.aa"));
        msg.Subject = "Sample HTML mail and attachment";
        msg.SetBodyAsAlternativeContent("TEXT", "<b>TEXT</b>");

        msg.ContentType = new HigLabo.Net.Smtp.ContentType("multipart/mixed");

        SmtpContent attachmentPart = new SmtpContent();
        attachmentPart.LoadText("Text file content");
        attachmentPart.ContentDisposition.FileName = "sample.txt";
        attachmentPart.ContentDisposition.Value = "attachment";
        msg.Contents.Add(attachmentPart);

        //  same as sample message
        msg.Date = new DateTimeOffset(2021, 11, 24, 12, 1, 2, new TimeSpan(3, 0, 0));
        msg.ContentType.Boundary = "9a1851fd_daec_47f6_b1c4_0c26fccdb9a4";
        msg.Contents[0].ContentType.Boundary = "f080b064_e115_4896_9121_88a7754ecc52";

        String mailActual = msg.GetRawText();
        String mailExpected = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "TestData", "SmtpMessage_MultipartAlternativeAndAttachment.txt"));

        Assert.AreEqual(mailExpected, mailActual);
    }
}