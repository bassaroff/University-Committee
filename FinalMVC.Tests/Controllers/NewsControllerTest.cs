using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinalMVC.Controllers;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace FinalMVC.Tests.Controllers
{
    [TestClass]
    public class NewsControllerTest
    {
        [TestMethod]
        public async Task IndexViewResultNotNullAsync()
        {
            NewsController controller = new NewsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexViewEqualIndexCshtmlAsync()
        {
            NewsController controller = new NewsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IndexStringInViewbagAsync()
        {
            NewsController controller = new NewsController();

            ViewResult result = await controller.Index() as ViewResult;

            Assert.AreEqual("This is news page!", result.ViewBag.Message);
        }
    }
}
