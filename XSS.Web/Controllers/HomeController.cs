using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Encodings.Web;
using XSS.Web.Models;

namespace XSS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HtmlEncoder _htmlEncoder;
        private JavaScriptEncoder _javaScriptEncoder;
        private UrlEncoder _urlEncoder;

        public HomeController(ILogger<HomeController> logger,
                              HtmlEncoder htmlEncoder,
                              JavaScriptEncoder javaScriptEncoder,
                              UrlEncoder urlEncoder)
        {
            _logger = logger;
            _htmlEncoder = htmlEncoder;
            _javaScriptEncoder = javaScriptEncoder;
            _urlEncoder = urlEncoder;
        }


        public IActionResult Login(string returnUrl = "/")
        {
            TempData["returnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            string returnUrl = TempData["returnUrl"].ToString();


            //user check

            //Open redirect attack check
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        public IActionResult CommentAdd()
        {
            HttpContext.Response.Cookies.Append("name", "aykut");
            HttpContext.Response.Cookies.Append("comment", "yorum 1");

            if (System.IO.File.Exists("comment.txt"))
            {
                ViewBag.comment = System.IO.File.ReadAllLines("comment.txt");
            }

            return View();
        }

        //Mevcut domainden gelen isteklere izin verir. Startup.cs de uygulama seviyesine çekildi
        //[ValidateAntiForgeryToken]
        //[IgnoreAntiforgeryToken] //Bu action için kontrolü pasif kılar
        [HttpPost]
        public IActionResult CommentAdd(string name, string comment)
        {
            //var sanitizer = new HtmlSanitizer();
            //ViewBag.name = sanitizer.Sanitize(name);
            //ViewBag.comment = sanitizer.Sanitize(comment);

            //ViewBag.name = name;
            //ViewBag.comment = comment;


            //Urlde gönderilecekse encode edilir
            //string encodeName = _urlEncoder.Encode(name);
            //string encodeComment = _urlEncoder.Encode(comment);



            System.IO.File.AppendAllText("comment.txt", $"{name}--{comment}\n");

            //return View();
            return RedirectToAction("CommentAdd");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
