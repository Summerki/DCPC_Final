using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl
{
    class CanDTO
    {
        private DateTime time;
        private uint id;
        private byte[] data;

        public CanDTO()
        {
            data = new byte[8];
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Time { get => time; set => time = value; }

        /// <summary>
        /// 帧id
        /// </summary>
        public uint Id { get => id; set => id = value; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get => data; set => data = value; }

        public byte[] ToBytes()
        {
            byte[] res = new byte[32];

            return res;
        }
    }
}
