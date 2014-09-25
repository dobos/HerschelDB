using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;
using Herschel.Lib;

namespace Herschel.Loader
{
    class Program
    {
        static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["Herschel"].ConnectionString; }
        }

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var verb = args[0].ToLowerInvariant();

            switch (verb)
            {
                case "prepare":
                    PreparePointings(args);
                    break;
                case "load":
                    LoadPointings(args);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void PreparePointings(string[] args)
        {
            var inst = args[1].ToLowerInvariant();
            var path = args[2];
            var output = args[3];
            int fnum = int.Parse(args[4]);

            var file = GetPointingsFile(inst);
            file.PreparePointings(path, output, fnum);
        }

        private static void LoadPointings(string[] args)
        {
            var inst = args[1].ToLowerInvariant();
            var path = args[2];
            int fnum = int.Parse(args[3]);

            var file = GetPointingsFile(inst);
            file.LoadPointings(path, fnum);
        }

        private static PointingsFile GetPointingsFile(string inst)
        {
            switch (inst.ToLowerInvariant())
            {
                case "pacs":
                    return new PointingsFilePacs();
                case "spire":
                    return new PointingsFileSpire();
                default:
                    throw new NotImplementedException();
            }
        }

        static long GetID(string filename)
        {
            var fn = Path.GetFileNameWithoutExtension(filename).Substring(7);
            return long.Parse(fn, System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
