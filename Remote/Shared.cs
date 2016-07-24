using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote
{
    public interface ISharedTypeInterface
    {
        string GetRemoteStatus(string stringToPrint);
        string GetStatus();
        string WriteChunk(Remote.SharedType.Chunk chunk);
    }

    public class SharedType : MarshalByRefObject, ISharedTypeInterface
    {
        [Serializable]
        public class Chunk
        {
            public int num;
            public long startposition;
            public long len;
            public byte[] data;
            public string hash;
            public string filename;
        }

        public string WriteChunk(Chunk chunk)
        {
            try
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                using (FileStream fs = File.Open( path+"\\"+chunk.filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    fs.Seek(0, SeekOrigin.Begin);
                    for (int i = 0; i < chunk.num; i++)
                    {
                        fs.Seek(1000000, SeekOrigin.Current);
                    }
                    fs.Write(chunk.data, 0, chunk.data.Length);
                }
                return "OK";
            }
            catch (Exception ee)
            {
                return ee.Message + ee.Source + ee.StackTrace;
            }
        }

        public string GetStatus()
        {
            return "OK";
        }
        public string GetRemoteStatus(string stringToPrint)
        {
            return stringToPrint;
        }
    }
}
