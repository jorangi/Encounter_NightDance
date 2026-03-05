namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 선형 합동 생성기 - 고정 의사 난수 생성 알고리즘 (싱글톤)
    /// </summary>
    public class LinearCongruentialGenerator
    {
        private static LinearCongruentialGenerator _instance;
        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static LinearCongruentialGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    uint seed = 20250503;
                    _instance = new LinearCongruentialGenerator(seed);
                }
                return _instance;
            }
        }
        private uint _state; // 난수 값
        public uint State {get=>_state; set=>_state = value;} // Undo를 위해 외부에서 값 설정 가능하도록 프로퍼티 노출
        private const uint a = 1664525; // 곱셈수
        private const uint c = 1013904223; // 덧셈수
        
        /// <summary>
        /// 생성자 - 시드값으로 초기화
        /// </summary>
        /// <param name="seed"></param>
        public LinearCongruentialGenerator(uint seed)
        {
            _state = seed;
        }
        /// <summary>
        /// 다음 난수 반환
        /// </summary>
        /// <returns></returns>
        public uint Next()
        {
            // X_{n+1} = (a * X_n + c) % m => 32비트 오버플로우로 인해 2^32를 초과시 알아서 나머지 효과 발생
            _state = a * _state + c;
            return _state;
        }
        /// <summary>
        /// 0.0 ~ 1.0 사이의 부동소수점 난수 반환
        /// </summary>
        /// <returns></returns>
        public float NextFloat()
        {
            return Next() / (float)uint.MaxValue;
        }
    }
}