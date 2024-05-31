using Microsoft.Xna.Framework;
using System;

namespace BaseProject
{
    // 적 생성기 클래스
    static class EnemySpawner
    {
        // 랜덤 값을 생성하기 위한 Random 인스턴스입니다.
        static Random rand = new Random();

        // 적 생성 확률의 역수를 나타내는 변수입니다. 값이 작을수록 생성 확률이 높아집니다.
        static float inverseSpawnChance = 90;

        // 매 프레임마다 호출되어 적을 생성할지 결정합니다.
        public static void Update()
        {
            // 플레이어가 죽지 않았고, 엔티티 수가 200보다 적을 때 적을 생성합니다.
            if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
            {
                // 일정 확률로 시커(Seeker) 적을 생성합니다.
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

                // 일정 확률로 방황자(Wanderer) 적을 생성합니다.
                if (rand.Next((int)inverseSpawnChance) == 0)
                    EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));
            }

            // 시간이 지남에 따라 적 생성 속도를 천천히 증가시킵니다.
            if (inverseSpawnChance > 30)
                inverseSpawnChance -= 0.005f;
        }

        // 적의 생성 위치를 결정하는 메소드입니다.
        private static Vector2 GetSpawnPosition()
        {
            Vector2 pos;
            do
            {
                // 화면 내의 랜덤한 위치를 생성합니다.
                pos = new Vector2(rand.Next((int)BaseProjectGame.ScreenSize.X), rand.Next((int)BaseProjectGame.ScreenSize.Y));
            }
            // 생성된 위치가 플레이어 위치로부터 일정 거리 이상 떨어져 있는지 확인합니다.
            while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

            return pos; // 유효한 생성 위치를 반환합니다.
        }

        // 적 생성 속도를 초기화하는 메소드입니다.
        public static void Reset()
        {
            inverseSpawnChance = 90; // 초기값으로 되돌립니다.
        }
    }
}
