using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Herschel.Lib;
using Jhu.Spherical;

namespace Herschel.Lib.Test
{
    [TestClass]
    public class DetectorTest
    {
        [TestMethod]
        public void TestGetCorners()
        {
            var d = new DetectorPacsPhoto();
            var c = d.GetCorners(new Cartesian(0, 0), 0);
        }
    }
}
