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
            Load(false);
        }

        public void Load(bool loading)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CoverageDSPriv));
                using var fs = new FileStream(FilePath, FileMode.Open);
                using var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                CoverageDSPriv coverage = (CoverageDSPriv)xmlSerializer.Deserialize(reader);
                Modules = coverage.Items.Where(i => i is CoverageDSPrivModule).Select(i => i as CoverageDSPrivModule);
                Files = coverage.Items.Where(i => i is CoverageDSPrivSourceFileNames).Select(i => i as CoverageDSPrivSourceFileNames);
            }
            catch(Exception e)
            {
                if(loading || !e.Message.Contains("An XML declaration with an encoding is required for all non-UTF8 documents."))
                {
                    throw new Exception("File format incorrect");
                }
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
                else
                {
                    Directory.CreateDirectory(tempDir);
                    var tempFile = Path.Combine(tempDir, Path.GetFileName(FilePath));
                    using var tempFs = new FileStream(tempFile, FileMode.Create);
                    using var copyFs = new FileStream(FilePath, FileMode.Open);
                    using var sr = new StreamReader(copyFs, true);
                    using var sw = new StreamWriter(tempFs, sr.CurrentEncoding);
                    sw.WriteLine($"<?xml version=\"1.0\" encoding=\"{sr.CurrentEncoding.BodyName}\"?>");
                    while (!sr.EndOfStream)
                    {
                        var ln = sr.ReadLine();
                        sw.WriteLine(ln);
                    }
                    FilePath = tempFile;
                }
                Load(true);
            }
        }

        public string FilePath { get; private set; }
    }
}
