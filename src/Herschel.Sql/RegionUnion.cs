//------------------------------------------------------------------------------
// <copyright file="CSSqlAggregate.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Spherical;
using Herschel.Lib;

namespace Herschel.Sql
{
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, Name = "fRegionUnion", MaxByteSize = -1)]
    public struct RegionUnion : IBinarySerialize
    {
        private Region region;

        public void Init()
        {
            region = null;
        }

        public void Accumulate(SqlBytes value)
        {
            var r = Util.GetRegion(value);

            if (region == null)
            {
                region = r;
            }
            else
            {
                region.Union(r);
            }
        }

        public void Merge(RegionUnion group)
        {
            if (this.region == null)
            {
                this.region = group.region;
            }
            else if (group.region != null)
            {
                this.region.Union(group.region);
            }
        }

        public SqlBytes Terminate()
        {
            region.Simplify();
            return Util.SetRegion(region);
        }

        void IBinarySerialize.Read(System.IO.BinaryReader r)
        {
            region = Util.ReadRegion(r.BaseStream);
        }

        void IBinarySerialize.Write(System.IO.BinaryWriter w)
        {
            Util.WriteRegion(region, w.BaseStream);
        }
    }
}