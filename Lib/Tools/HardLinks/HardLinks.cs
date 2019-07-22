using Lib.Logger;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Tools.HardLinks
{
    public class HardLinks : IHardLinks
    {
        protected readonly ILogWriter _logWriter;

        public HardLinks(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void CreateHardLink(string from, string to)
        {

            if (File.Exists(to))
            {
                _logWriter.WriteLine($"Create hard link target alreading exists, deleting '${to}'");
                File.Delete(to);
            }

            Alphaleonis.Win32.Filesystem.File.CreateHardlink(to, from);

            _logWriter.WriteLine($"Success on creating a hardlink from '{from}' to '{to}'");

        }

        public int GetOtherHardLinksCount(string filepath)
        {
            // Get link count excluding provided node

            var links = from link in Alphaleonis.Win32.Filesystem.File.EnumerateHardlinks(filepath)
                        where Path.GetFullPath(filepath) != Path.GetFullPath(link)
                        select link;

            return links.Count();
        }

    }
}
