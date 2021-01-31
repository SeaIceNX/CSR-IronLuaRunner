using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSLR
{
    public class ToolFunc
    {
        private static Dictionary<int, Thread> thr = new Dictionary<int, Thread>();
        #region TOOLAPI
        public void WriteAllText(string path, string contenst)
        {
            File.WriteAllText(path, contenst);
        }

        public void AppendAllText(string path, string contenst)
        {
            File.AppendAllText(path, contenst);
        }

        public string[] ReadAllLine(string path)
        {
            return File.ReadAllLines(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string WorkingPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public string ToMD5(string word)
        {
            string md5output = "";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] date = Encoding.Default.GetBytes(word);
            byte[] date1 = md5.ComputeHash(date);
            md5.Clear();
            for (int i = 0; i < date1.Length - 1; i++)
            {
                md5output += date1[i].ToString("X");
            }
            return md5output;
        }

        public string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        public void CreateDir(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool IfFile(string path)
        {
            return File.Exists(path);
        }

        public bool IfDir(string path)
        {
            return Directory.Exists(path);
        } 

        public void ThrowException(string msg)
        {
            throw new ArgumentOutOfRangeException(msg);
        }
        public Task TaskRun(LuaFunction func) 
        {
           return Task.Run(() => func.Call());
        }
        public int Schedule(LuaFunction func, int delay, int cycle) 
        {
            var t = new Thread(() =>
            {
                for (int i = 0; i < cycle; i++)
                {
                    func.Call();
                    Thread.Sleep(delay);
                }
            });
            t.Start();
            int id = t.ManagedThreadId;
            thr.Add(id, t);
            new Thread(() =>
            {
                t.Join();
                thr.Remove(id);
            });
            return id;
        }
        public int Schedule(LuaFunction func, int delay) 
        {
            return Schedule(func, delay, 1);
        }
        public bool Cancel(int id)
        {
            if (!thr.ContainsKey(id))
                return false;
            thr[id].Abort();
            thr.Remove(id);
            return true;
        }
        public void RunCode(string code)
        {
            CSharpLuaRunner.CSharpLuaRunner.lua.DoString(code);
        }
        #endregion
    }
}
