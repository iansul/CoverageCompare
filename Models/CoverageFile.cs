using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Models
{
    public class CoverageFile
    {

        public IEnumerable<CoverageDSPrivModule> Modules { get; private set; }
        public IEnumerable<CoverageDSPrivSourceFileNames> Files { get; private set; }

        public CoverageFile(string filePath)
        {
            FilePath = filePath;
            try
            {
                Load();
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("An XML declaration with an encoding is required for all non-UTF8 documents."))
                {
                    throw;
                }
                FilePath = CopyToTempFileAndIncludeXmlDeclaration(FilePath);
                Load();
            }
        }

        private string CopyToTempFileAndIncludeXmlDeclaration(string fromPath)
        {
            string tempDir;
            int maxTries = 10;
            do
            {
                tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            while (Directory.Exists(tempDir) && maxTries-- > 0);
            if (Directory.Exists(tempDir))
            {
                throw new Exception("Unable to generate unique temp dir");
            }
            Directory.CreateDirectory(tempDir);
            var tempFile = Path.Combine(tempDir, Path.GetFileName(FilePath));
            using var tempFs = new FileStream(tempFile, FileMode.Create);
            using var copyFs = new FileStream(fromPath, FileMode.Open);
            using var sr = new StreamReader(copyFs, true);
            using var sw = new StreamWriter(tempFs, sr.CurrentEncoding);
            sw.WriteLine($"<?xml version=\"1.0\" encoding=\"{sr.CurrentEncoding.BodyName}\"?>");
            while (!sr.EndOfStream)
            {
                var ln = sr.ReadLine();
                sw.WriteLine(ln);
            }
            return tempFile;
        }

        internal void Load()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CoverageDSPriv));
            using var fs = new FileStream(FilePath, FileMode.Open);
            using var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            CoverageDSPriv coverage = (CoverageDSPriv)xmlSerializer.Deserialize(reader);
            Modules = coverage.Items.Where(i => i is CoverageDSPrivModule).Select(i => i as CoverageDSPrivModule);
            Files = coverage.Items.Where(i => i is CoverageDSPrivSourceFileNames).Select(i => i as CoverageDSPrivSourceFileNames);
        }

        public string FilePath { get; private set; }
    }
}
