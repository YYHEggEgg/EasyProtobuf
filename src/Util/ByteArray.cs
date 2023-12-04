using System.Text;

namespace YSFreedom.Common.Util
{
    public static class ByteArray
    {
        public static void SetUInt16(this byte[] arr, int offset, ushort value)
        {
            arr[offset + 1] = (byte)(value & 0xFF);
            arr[offset + 0] = (byte)((value & 0xFF00) >> 8);
        }

        public static void SetUInt32(this byte[] arr, int offset, uint value)
        {
            arr[offset + 3] = (byte)(value & 0xFF);
            arr[offset + 2] = (byte)((value & 0xFF00) >> 8);
            arr[offset + 1] = (byte)((value & 0xFF0000) >> 16);
            arr[offset + 0] = (byte)((value & 0xFF000000) >> 24);
        }

        public static void SetUInt64(this byte[] arr, int offset, ulong value)
        {
            arr[offset + 7] = (byte)(value & 0xFF);
            arr[offset + 6] = (byte)((value & 0xFF00) >> 8);
            arr[offset + 5] = (byte)((value & 0xFF0000) >> 16);
            arr[offset + 4] = (byte)((value & 0xFF000000) >> 24);
            arr[offset + 3] = (byte)((value & 0xFF00000000) >> 32);
            arr[offset + 2] = (byte)((value & 0xFF0000000000) >> 40);
            arr[offset + 1] = (byte)((value & 0xFF000000000000) >> 48);
            arr[offset + 0] = (byte)((value & 0xFF00000000000000) >> 56);
        }

        public static ushort GetUInt16(this byte[] arr, int offset)
        {
            ushort ret = (ushort)((ushort)arr[offset + 1] | ((ushort)arr[offset + 0] << 8));
            return ret;
        }

        public static uint GetUInt32(this byte[] arr, int offset)
        {
            uint ret = ((uint)arr[offset + 3] | ((uint)arr[offset + 2] << 8) |
                         ((uint)arr[offset + 1] << 16) | ((uint)arr[offset + 0] << 24));
            return ret;
        }

        public static ulong GetUInt64(this byte[] arr, int offset)
        {
            ulong ret = ((ulong)arr[offset + 7]) | ((ulong)arr[offset + 6] << 8) |
                         ((ulong)arr[offset + 5] << 16) | ((ulong)arr[offset + 4] << 24) |
                         ((ulong)arr[offset + 3] << 32) | ((ulong)arr[offset + 2] << 40) |
                         ((ulong)arr[offset + 1] << 48) | ((ulong)arr[offset + 0] << 56);
            return ret;
        }

        public static byte[] Fill0(this byte[] arr, int toLength)
        {
            if (arr.Length < toLength)
            {
                var rtn = new byte[toLength];
                Array.Copy(arr, 0, rtn, 0, arr.Length);
                for(int i = arr.Length; i < toLength; i++)
                {
                    rtn[i] = 0;
                }
                return rtn;
            }
            return arr;
        }
    }
}
