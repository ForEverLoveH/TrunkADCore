﻿using NPOI.DDF;
using System;
using System.Drawing;

namespace ADCoreDataCommon.GameModel 
{
    public class SavaStudentData
    {
        public SavaStudentData()
        {
            id = String.Empty;
            idNumber = String.Empty;
            name = String.Empty;
            groupName = String.Empty;
            score = 0;
            RoundId = 1;
            state = 0;
        }
        public string id;//编号
        public string idNumber;//考号
        public string name;//姓名
        public double score;//成绩
        public int RoundId;//轮次
        //状态 0:未测试 1:已测试 2:中退 3:缺考 4:犯规 5:弃权
        public int state;//状态
        public string groupName;
    }
    public  struct ImageAndIndex
    {
        public int index;
        public Bitmap image;
    }
}