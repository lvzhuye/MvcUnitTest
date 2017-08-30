using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcUnitTest
{
    class Program
    {
        private static object user = null;
        static void Main(string[] args)
        {
            MultiThreadedInvoke();

            Thread.Sleep(100 * 1000);
        }

        public static void MultiThreadedInvoke()
        {
            //用户信息
            var userMock = new Mock<IPrincipal>();
            userMock.Setup(um => um.Identity.IsAuthenticated).Returns(true);
            //用户名
            userMock.Setup(um => um.Identity.Name).Returns("");
            //AuthidentityType???
            userMock.Setup(um => um.Identity.AuthenticationType).Returns("Forms");

            user = userMock.Object;

            //构造模拟Request
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(cm => cm.User).Returns(userMock.Object);

            //构造ControllerContext
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(ccm => ccm.HttpContext)
                .Returns(contextMock.Object);

            for (int i = 0; i < 100; i++)
            {
                //调用Control对应的Action

                Thread thread = new Thread(ActionInvoke);

                thread.Start(controllerContextMock.Object);
  
            }



        }



        private static void ActionInvoke(object controllerContext)
        {

            //构造模拟HttpContext.Current
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""), new HttpResponse(new StringWriter(new StringBuilder())));
            //每个线程独立的，Asp.net多线程特性
            HttpContext.Current.User = (IPrincipal)user;

            //新建Controller
            var businessController = new BusinessController();
            //赋值ControllerContext
            businessController.ControllerContext = (ControllerContext)controllerContext;

            //构造请求业务参数

            //调用Action

            //打印结果

            Debug.WriteLine(((ViewResult)result).ViewData.Values.Count);

        }
    }
}
