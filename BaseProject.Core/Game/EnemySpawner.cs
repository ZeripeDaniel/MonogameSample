using Microsoft.Xna.Framework;
using System;

namespace BaseProject
{
    static class EnemySpawner
	{
		static Random rand = new Random();
		static float inverseSpawnChance = 90;
		static float inverseBlackHoleChance = 600;

		public static void Update()
		{
			if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
			{
				if (rand.Next((int)inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition()));

				if (rand.Next((int)inverseSpawnChance) == 0)
					EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition()));

			}

            // 시간이 지남에 따라 생성 속도를 천천히 증가시킵니다.
            if (inverseSpawnChance > 30)
				inverseSpawnChance -= 0.005f;
		}

		private static Vector2 GetSpawnPosition()
		{
			Vector2 pos;
			do
			{
				pos = new Vector2(rand.Next((int)BaseProjectGame.ScreenSize.X), rand.Next((int)BaseProjectGame.ScreenSize.Y));
			} 
			while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);

			return pos;
		}

		public static void Reset()
		{
			inverseSpawnChance = 90;
		}
	}
}