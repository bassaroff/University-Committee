using System;
using System.Web.Mvc;
using FinalMVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalMVC.Tests.Controllers
{
    [TestClass]
    public class SubjectsControllerTest
    {
        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewResultNotNullAsync()
        {
            SubjectsController controller = new SubjectsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewEqualIndexCshtmlAsync()
        {
            SubjectsController controller = new SubjectsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexStringInViewbagAsync()
        {
            SubjectsController controller = new SubjectsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("This is subjects page!", result.ViewBag.Message);
        }
    }
}
