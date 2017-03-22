using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Loader
{
    public abstract class InputFile
    {
        private Dictionary<string, int> columns;

        protected Dictionary<string, int> Columns
        {
            get { return columns; }
        }

        protected void ParseColumns(string line)
        {
            columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            var parts = line.Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                columns.Add(parts[i], i);
            }
        }

        protected string[] SplitLine(string line)
        {
            var parts = new List<string>();
            var buffer = new char[0x400];
            int bufferpos;
            bool inquotes;

            line = line.Trim();

            // Split line at spaces but not within quotes
            parts.Clear();
            bufferpos = 0;
            inquotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inquotes = !inquotes;
                }
                else if (line[i] != ' ' || inquotes)
                {
                    buffer[bufferpos] = line[i];
                    bufferpos++;
                }

                if (!inquotes && line[i] == ' ' || i == line.Length - 1)
                {
                    // end of column
                    parts.Add(new String(buffer, 0, bufferpos));
                    bufferpos = 0;
                }
            }

            return parts.ToArray();
        }
    }
}
