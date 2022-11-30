using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  TrunkADCore.ADCoreSystem 
{
    public  class FromDataItemModel
    {
        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// 上传的文件内容
        /// </summary>
        public Stream FileContent { set; get; }
        /// <summary>
        ///  表单键 request["KEY"]
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 表单值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0)
                {
                    return false;
                }
                if (FileContent != null && FileContent.Length > 0 && string.IsNullOrEmpty(FileName))
                {
                    throw new Exception("上传文件时filename属性值不能为空");

                }
                return true;
            }


        }

    }
}
