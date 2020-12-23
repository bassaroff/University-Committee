using System;
using System.Web.Mvc;
using FinalMVC.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinalMVC.Tests.Controllers
{
    [TestClass]
    public class CoursesControllerTest
    {
        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewResultNotNullAsync()
        {
            CoursesController controller = new CoursesController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewEqualIndexCshtmlAsync()
        {
            CoursesController controller = new CoursesController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexStringInViewbagAsync()
        {
            CoursesController controller = new CoursesController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("This is subjects page!", result.ViewBag.Message);
        }
    }
}
