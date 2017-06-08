using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using URLShortener.Models;

namespace URLShortener.Controllers
{
    public class RedirectController : Controller
    {
        //
        // GET: /Redirect/

        public ActionResult Index()
        {
            // Get the query string
            var id = Request.QueryString["i"];

            // read in the hashtable
            // Set the path of the file
            var path = Server.MapPath(@"~/Vector.bin");

            // read the data from the file
            var binformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Hashtable vectorDeserialized = null;
            using (var fs = System.IO.File.Open(path, FileMode.Open))
            {
                vectorDeserialized = (Hashtable)binformatter.Deserialize(fs);
            }

            // look up the id and redirect to the long url
            if (vectorDeserialized.Contains(id))
            {
                // check for the http, if not then add it in
                if (vectorDeserialized[id].ToString().Substring(0, 4).ToLower().CompareTo("http") != 0)
                {
                    Response.Redirect("Http://" + vectorDeserialized[id].ToString());
                }
                else
                {
                    Response.Redirect(vectorDeserialized[id].ToString());
                }
            }

            return View();
        }

    }
}
