using System;
using System.Web.Mvc;
using FinalMVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalMVC.Tests.Controllers
{
    public class ApplicationUsersControllerTest
    {
        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewResultNotNullAsync()
        {
            ApplicationUsersController controller = new ApplicationUsersController();

            ViewResult result =  controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewEqualIndexCshtmlAsync()
        {
            ApplicationUsersController controller = new ApplicationUsersController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexStringInViewbagAsync()
        {
            ApplicationUsersController controller = new ApplicationUsersController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.AreEqual("This is users page!", result.ViewBag.Message);
        }
    }
}

