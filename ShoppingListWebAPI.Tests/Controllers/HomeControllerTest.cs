using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShoppingListWebAPI;
using ShoppingListWebAPI.Controllers;
using System.Web.Mvc;

namespace ShoppingListWebAPI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Przygotowanie
            HomeController controller = new HomeController();

            // Wykonanie
            ViewResult result = controller.Index() as ViewResult;

            // Sprawdzenie
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
