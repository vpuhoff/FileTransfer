using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileReceiver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remote;

namespace FileReceiver.Tests
{
    [TestClass()]
    public class MainFormTests
    {
       
        [TestMethod()]
        public void CreateServerAndClient()
        {
            try
            {
                Server srv = new Server();
                Client cl = new Client();
                cl.RemoteObject.GetRemoteStatus("test");
            }
            catch (Exception ee)
            {
                Assert.Fail(ee.Message+ee.Source );
            }
        }
    }
}