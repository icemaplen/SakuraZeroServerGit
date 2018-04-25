﻿using SakuraZeroCommon.Core;
using System;
using System.Linq;
using System.Text;

namespace SakuraZeroCommon.Prorocal
{
    public class ProtocalBytes : ProtocalBase
    {
        public byte[] bytes;        //传输的字节流
        public override ERequestCode RequestCode
        {
            get;
            protected set;
        }

        public override EActionCode ActionCode
        {
            get;
            protected set;
        }

        public ProtocalBytes()
        {
            AddInt((int)ERequestCode.None);
            AddInt((int)EActionCode.None);
        }

        /// <summary>
        /// 构造一个新的协议并初始化协议头部.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        public ProtocalBytes(ERequestCode request, EActionCode action)
        {
            RequestCode = request;
            ActionCode = action;

            AddInt((int)RequestCode);
            AddInt((int)ActionCode);
        }

        /// <summary>
        /// 解析出协议头部的RequestCode和ActionCode，并设置在协议中.
        /// </summary>
        public void Init()
        {
            RequestCode = (ERequestCode)Enum.Parse(typeof(ERequestCode), GetInt(0).ToString());
            ActionCode = (EActionCode)Enum.Parse(typeof(EActionCode), GetInt(sizeof(int)).ToString());
        }

        /// <summary>
        /// 解码，将字节流转换成ProtocalBytes对象.
        /// </summary>
        /// <param name="readBuff"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public override ProtocalBase Decode(byte[] readBuff, int start, int length)
        {
            ProtocalBytes protocal = new ProtocalBytes();
            protocal.bytes = new byte[length];
            Array.Copy(readBuff, start, protocal.bytes, 0, length);
            protocal.Init();
            return protocal;
        }

        /// <summary>
        /// 解码器
        /// </summary>
        /// <returns></returns>
        public override byte[] Encode()
        {
            return bytes;
        }

        /// <summary>
        /// 描述.提取每一个字节，并转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string GetDesc()
        {
            if (bytes == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append((int)b);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 从字节数组的start处开始读取字符串
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetString(int start, ref int end)
        {
            if (bytes == null || bytes.Length < start + sizeof(Int32))
            {
                return "";
            }

            Int32 strLen = BitConverter.ToInt32(bytes, start);
            if (bytes.Length < start + sizeof(Int32) + strLen)
            {
                return "";
            }

            string str = Encoding.UTF8.GetString(bytes, start + sizeof(Int32), strLen);
            end = start + sizeof(Int32) + strLen;
            return str;
        }

        public string GetString(int start)
        {
            int end = 0;
            return GetString(start, ref end);
        }

        public void AddString(string str)
        {
            Int32 len = str.Length;
            byte[] lenBytes = BitConverter.GetBytes(len);
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            if (bytes == null)
            {
                bytes = lenBytes.Concat(strBytes).ToArray();
            }
            else
            {
                bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
            }
        }

        public void AddInt(int num)
        {
            byte[] numBytes = BitConverter.GetBytes(num);
            bytes = bytes == null ? numBytes : bytes.Concat(numBytes).ToArray();
        }

        public int GetInt(int start, ref int end)
        {
            if (bytes == null || bytes.Length < start + sizeof(Int32))
            {
                return 0;
            }
            end = start + sizeof(Int32);
            return BitConverter.ToInt32(bytes, start);
        }

        public int GetInt(int start)
        {
            int end = 0;
            return GetInt(start, ref end);
        }

        public void AddFloat(float num)
        {
            byte[] numBytes = BitConverter.GetBytes(num);
            bytes = bytes == null ? numBytes : bytes.Concat(numBytes).ToArray();
        }

        public float GetFloat(int start, ref int end)
        {
            if (bytes == null || bytes.Length < start + sizeof(float))
            {
                return 0;
            }
            end = start + sizeof(float);
            return BitConverter.ToSingle(bytes, start);
        }

        public float GetFloat(int start)
        {
            int end = 0;
            return GetFloat(start, ref end);
        }
    }
}
