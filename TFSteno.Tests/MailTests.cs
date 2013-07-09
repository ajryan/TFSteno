using System;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TFSteno.Tests
{
    [TestClass]
    public class MailTests
    {
        [TestMethod]
        public void MailAddressParse()
        {
            var mailAddress = new MailAddress("A Guy <aguy@yahoo.com>");
            Assert.AreEqual("aguy@yahoo.com", mailAddress.Address);
        }
    }
}
