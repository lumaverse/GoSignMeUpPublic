using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Service.BusinessLogic.GlobalTools
{
    public class FileManagement
    {
        public static void SaveFileToDirectory(string fileName, string fileDestination)
        {
            var source = Path.Combine(Environment.CurrentDirectory, fileName);
            var destination = Path.Combine(fileDestination, fileName);
            if (Directory.Exists(source)) {
                File.Copy(source, destination);
            }   
        }
    }
}
