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
    public class URLController : Controller
    {
        //
        // GET: /URL/

        [HttpGet]
        public ActionResult Index()
        {
            URL url = new URL();
            return View(url);
        }

        public ActionResult Index(URL url)
        {
            // Set the path of the file
            var path = Server.MapPath(@"~/Vector.bin");

            // read the data from the file
            var binformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Hashtable vectorDeserialized = null;
            using (var fs = System.IO.File.Open(path, FileMode.Open))
            {
                vectorDeserialized = (Hashtable)binformatter.Deserialize(fs);
            }

            // Assign the long url a unique id by converting to base 64
            String id = HelperMethods.ToBase(vectorDeserialized.Count + 1, HelperMethods.ALPHANUMERIC);

            // Add the id and long url to the hashtable
            vectorDeserialized.Add(id, url.LongURL);

            // Save the data to the file
            using (var fs = System.IO.File.Create(path))
            {
                binformatter.Serialize(fs, vectorDeserialized);
            }

            // Show short url
            if (ModelState.IsValid)
            {
                url.ShortURL = "http://localhost:3206/Redirect?i=" + id;
            }
            return View(url);
        }
    }

    // This class provides helper methods for the URLController
    public static class HelperMethods
    {
        // Base 64
        public static string ALPHANUMERIC =
            "0123456789" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz";

        //Remove 0oO1iIl - Base52
        public static string ALPHANUMERIC_ALT =
            "23456789" +
            "ABCDEFGHJKLMNPRSTUVWXYZ" +
            "abcdefghjkmnpqrstuvwxyz";

        // Convert to a base larger than the one provided
        public static string ToBase(this long input, string baseChars)
        {
            string r = string.Empty;
            int targetBase = baseChars.Length;
            do
            {
                r = string.Format("{0}{1}",
                    baseChars[(int)(input % targetBase)],
                    r);
                input /= targetBase;
            } while (input > 0);

            return r;
        }

        // Convert to a base smaller than the one provided
        // This is never used
        public static long FromBase(this string input, string baseChars)
        {
            int srcBase = baseChars.Length;
            long id = 0;
            string r = input.Reverse().ToString();

            for (int i = 0; i < r.Length; i++)
            {
                int charIndex = baseChars.IndexOf(r[i]);
                id += charIndex * (long)Math.Pow(srcBase, i);
            }

            return id;
        }
    }
}
