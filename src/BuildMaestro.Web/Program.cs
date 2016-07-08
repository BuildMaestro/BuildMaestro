using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;

namespace BuildMaestro.Web
{
    public class Program
    {
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
