using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Herschel.Lib;
using Spherical;

namespace Herschel.Lib.Test
{
    [TestClass]
    public class DetectorTest
    {
        [TestMethod]
        public void TestGetCorners()
        {
            var d = Detector.Blue;

            var c = d.GetCorners(new Cartesian(0, 0), 0);
        }
    }
}
