using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Herschel.Ws.Api
{
    public class TextResponseMessageBodyWriter : StreamBodyWriter
    {
        private object result;

        public TextResponseMessageBodyWriter(object result)
            : base(true)
        {
            this.result = result;
        }

        protected override void OnWriteBodyContents(Stream stream)
        {
            if (result != null)
            {
                var writer = new StreamWriter(stream, System.Text.Encoding.ASCII);

                WriteBodyContents(writer);

                writer.Flush();
            }
        }

        private void WriteBodyContents(TextWriter writer)
        {
            var type = result.GetType();

            if (result is IDataReader)
            {
                WriteDataReader(writer, (IDataReader)result);
            }
            else if (!(result is String) && type.IsArray)
            {
                WriteArray(writer, (Array)result);
                return;
            }
            else if (!(result is String) && result is IEnumerable)
            {
                WriteEnumerable(writer, (IEnumerable)result);
            }
            else
            {
                WriteObject(writer, result);
            }
        }

        private void WriteDataReader(TextWriter writer, IDataReader reader)
        {
            // Write header
            int q = 0;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).IsPrimitive)
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(reader.GetName(i));
                    q++;
                }
            }
            Console.WriteLine();

            // Write rows
            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(values);

                q = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (reader.GetFieldType(i).IsPrimitive)
                    {
                        if (q > 0)
                        {
                            writer.Write("\t");
                        }

                        writer.Write(values[i].ToString());
                        q++;
                    }
                }
                Console.WriteLine();
            }
        }

        private void WriteArray(TextWriter writer, Array results)
        {
            if (results.Rank != 1)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (i == 0)
                {
                    WriteObjectHeader(writer, results.GetValue(i));
                }

                WriteObject(writer, results.GetValue(i));
            }
        }

        private void WriteEnumerable(TextWriter writer, IEnumerable results)
        {
            int q = 0;
            foreach (var value in results)
            {
                if (q == 0)
                {
                    WriteObjectHeader(writer, value);
                }

                WriteObject(writer, value);
                q++;
            }
        }

        private void WriteObjectHeader(TextWriter writer, object value)
        {
            var type = value.GetType();

            if (type.IsPrimitive)
            {
                //WritePrimitiveValue(writer, value);
            }
            else
            {
                WriteComplexObjectHeader(writer, value);
            }
        }

        private void WriteObject(TextWriter writer, object value)
        {
            var type = value.GetType();

            if (type.IsPrimitive || value is String)
            {
                WritePrimitiveValue(writer, value);
            }
            else
            {
                WriteComplexObject(writer, value);
            }
        }

        private void WritePrimitiveValue(TextWriter writer, object value)
        {
            writer.WriteLine(value.ToString());
        }

        private void WriteComplexObjectHeader(TextWriter writer, object value)
        {
            var type = value.GetType();
            var props = type.GetProperties();

            writer.Write("#");

            int q = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (!IsIgnoredProperty(props[i]))
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(props[i].Name);
                    q++;
                }
            }

            writer.WriteLine();
        }

        private void WriteComplexObject(TextWriter writer, object value)
        {
            var type = value.GetType();
            var props = type.GetProperties();

            int q = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (!IsIgnoredProperty(props[i]))
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(props[i].GetValue(value).ToString());
                    q++;
                }
            }

            writer.WriteLine();
        }

        private bool IsIgnoredProperty(PropertyInfo prop)
        {
            return prop.GetCustomAttribute(typeof(IgnoreDataMemberAttribute), true) != null;
        }
    }
}