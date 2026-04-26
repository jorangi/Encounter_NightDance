using System;
using System.Text;

namespace Encounter.NightDance.Core
{
    public static class Murmur3Hash
    {
        public static uint GetHash(string str, uint seed = 0)
        {
            if(string.IsNullOrEmpty(str)) return 0;
            byte[] encodedStr = Encoding.UTF8.GetBytes(str);
            int len = encodedStr.Length;
            uint h1 = seed;

            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            int nblock = len >> 2; // 4바이트씩 나누기
            for(int i = 0; i < nblock; i++)
            {
                uint k1 = BitConverter.ToUInt32(encodedStr, i << 2);
                k1 *= c1; // 상수 곱
                k1 = (k1 << 15) | (k1 >> 17); //왼쪽으로 15 비트 회전, 왼쪽으로 15비트 민 값과 오른쪽으로 17비트 민 값을 비트합 연산을 하여 같은 결과
                k1 *= c2; // 다시 상수 곱

                h1 ^= k1; // 시드값과 XOR 연산 - 하는 이유는 찾아보니 비트의 종속성을 해소하기 위해라고 한다. |, &, xand 전부 비트의 종속성을 갖기 때문?
                h1 = (h1 << 13) | (h1 >> 19); // 13비트 회전
                h1 = h1 * 5 + 0xe6546b64; //매직 상수 곱하고 더하기
            }//반복
            int tailIndex = nblock << 2; // 4바이트씩 묶고 남은 부분 처리될 시작 인덱스
            uint k2 = 0;

            switch(len & 3) // 전체 길이 % 4의 결과에 따라 남은 바이트 처리
            {
                //3바이트 남음 -> 16~23비트
                case 3: k2 ^= (uint)encodedStr[tailIndex + 2] << 16; goto case 2;
                //2바이트 남음 -> 8~15비트
                case 2: k2 ^= (uint)encodedStr[tailIndex + 1] << 8; goto case 1;
                //1바이트 남음 -> 0~7비트
                case 1:
                //곱 회전, XOR로 처리
                    k2 ^= encodedStr[tailIndex];
                    k2 *= c1;
                    k2 = (k2 << 15) | (k2 >> 17);
                    k2 *= c2;
                    h1 ^= k2;
                    break;
            }
            //마지막 보정
            h1 ^= (uint)len;
            h1 ^= h1 >> 16;
            h1 *= 0x85ebca6b;
            h1 ^= h1 >> 13;
            h1 *= 0xc2b2ae35;
            h1 ^= h1 >> 16;
            return h1;
        }
    }
}