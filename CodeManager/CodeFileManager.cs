using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;

namespace CodeManager
{
    class CodeFileManager
    {
        private const string DataPath = "E:\\MemoryDump.bin";
        private CodeEntry[] codes;

        public void LoadCodes(string filePath)
        {
            using (BinaryReader cryptReader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            using (BinaryReader dataReader = new BinaryReader(File.Open(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {

            }

        }

        public void SaveCodesClearMemory()
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(DataPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
            {
                
            }
        }
    }

    [Serializable]
    struct CodeEntry : ISerializable, IComparable<CodeEntry>
    {
        public string Institution;
        public string UserName;
        public string Password;
        public string Comments;

        public CodeEntry(string insitution, string userName, string password, string comments)
        {
            Institution = insitution;
            UserName = userName;
            Password = password;
            Comments = comments;
        }

        protected CodeEntry(SerializationInfo info, StreamingContext context)
        {
            Institution = info.GetString("Institution");
            UserName = info.GetString("UserName");
            Password = info.GetString("Password");
            Comments = info.GetString("Comments");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("Institution", Institution);
            info.AddValue("UserName", UserName);
            info.AddValue("Password", Password);
            info.AddValue("Comments", Comments);
        }
    
        public int CompareTo(CodeEntry other)
        {
 	        int r = Institution.CompareTo(other.Institution);
            if (r == 0)
            {
                r = UserName.CompareTo(other.UserName);
            }
            if (r == 0)
            {
                r = Comments.CompareTo(other.Comments);
            }
            if (r == 0)
            {
                r = Password.CompareTo(other.Password);
            }
            return r;
        }
    }
}
