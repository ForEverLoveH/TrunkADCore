using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace TrunkADCore.ADCoreSystem
{
    public class TxtFile
    {
         #region Instance
        private static object logLock;

        private static TxtFile _instance;

        //  private static string logFileName;
        private TxtFile() { }

        /// <summary>
        /// Logger instance
        /// </summary>
        public static TxtFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TxtFile();
                    logLock = new object();
                    // logFileName = Guid.NewGuid() + ".log";
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        public void write(string filename, string str)
        {
            try
            {
                // str = title.Text;
                StreamWriter sw = new StreamWriter(filename, false);
                sw.WriteLine(str);
                sw.Close();//写入
                sw.Dispose();
            }
            catch (Exception) { }
        }
        public void SaveList(ListBox lst)
        {
            string strl = "";
            foreach (string str in lst.Items)
            {
                strl += str + "\r\n";
            }
            TxtFile.Instance.write(lst.Name, strl);
        }
        public ListBox loadList(string filename, ListBox lst)
        {
            //ListBox lst=new ListBox();
            string str = "";
            try
            {
                if (File.Exists(filename))
                {
                    StreamReader sr = new StreamReader(filename, false);
                    while (true)
                    {
                        str = sr.ReadLine();
                        if (null != str)
                        {
                            if (str != "")
                                lst.Items.Add(str);
                        }
                        else
                        {
                            break;
                        }
                    }

                    sr.Close();
                }
            }
            catch (Exception e) { };

            return lst;
        }
        public void saveInt(string filename, int i)
        {
            string str = i + "";
            write(filename, str);
        }


        int str2int(string str)
        {
            int i = 0;
            if (null == str)
                return 0;
            int.TryParse(str, out i);
            return i;
        }
        public int loadInt(string filename)
        {
            string[] strg = read(filename);
            if (null != strg)
            {
                if (strg.Length > 0)
                {
                    return str2int(strg[0]);
                }
            }
            return 0;
        }

        public string load1LineString(string filename)
        {
            string[] strg = read(filename);
            if (null != strg)
            {
                if (strg.Length > 0)
                {
                    return strg[0];
                }
            }
            return "";
        }
        public string loadString(string filename)
        {
            ArrayList list = new ArrayList();
            string str = "";
            try
            {
                if (File.Exists(filename))
                {
                    StreamReader sr = new StreamReader(filename, false);
                    while (true)
                    {
                        str = sr.ReadLine();
                        if (null != str)
                        {
                            list.Add(str);
                        }
                        else
                        {
                            break;
                        }
                    }

                    sr.Close();
                }
            }
            catch (Exception) { }
            if (list.Count == 0)
                list.Add("");
            string str1 = "";
            for (int i = 0; i < list.Count; i++)
            {
                str1 += list[i];
                if (i < (list.Count - 1))
                    str1 += "\r\n";
            }
            return str1;// list.ToString();
            // return (string[])list.ToArray(typeof(string));
        }


        public string[] read(string filename)
        {
            ArrayList list = new ArrayList();
            string str = "";
            try
            {
                if (File.Exists(filename))
                {
                    StreamReader sr = new StreamReader(filename, false);
                    while (true)
                    {
                        str = sr.ReadLine();
                        if (null != str)
                        {
                            list.Add(str);
                        }
                        else
                        {
                            break;
                        }
                    }

                    sr.Close();
                }
            }
            catch (Exception) { }
            if (list.Count == 0)
                list.Add("");
            return (string[])list.ToArray(typeof(string));
        }


        
    }
}