using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace BaseProject
{
    static class EntityManager
    {
        // 엔티티, 적, 총알을 담는 리스트
        static List<Entity> entities = new List<Entity>();
        static List<Enemy> enemies = new List<Enemy>();
        static List<Bullet> bullets = new List<Bullet>();

        // 업데이트 중인지 여부를 나타내는 플래그
        static bool isUpdating;
        // 추가할 엔티티를 담는 리스트
        static List<Entity> addedEntities = new List<Entity>();

        // 현재 엔티티의 수를 반환하는 프로퍼티
        public static int Count { get { return entities.Count; } }

        // 엔티티를 추가하는 메소드
        public static void Add(Entity entity)
        {
            if (!isUpdating) // 업데이트 중이 아니면
                AddEntity(entity); // 바로 엔티티를 추가합니다.
            else
                addedEntities.Add(entity); // 그렇지 않으면 나중에 추가할 리스트에 담습니다.
        }

        // 엔티티를 리스트에 추가하고, 해당 타입의 리스트에도 추가하는 메소드
        private static void AddEntity(Entity entity)
        {
            entities.Add(entity); // 엔티티 리스트에 추가합니다.
            if (entity is Bullet)
                bullets.Add(entity as Bullet); // 총알이면 bullets 리스트에 추가합니다.
            else if (entity is Enemy)
                enemies.Add(entity as Enemy); // 적이면 enemies 리스트에 추가합니다.
        }

        // 엔티티를 업데이트하는 메소드
        public static void Update()
        {
            isUpdating = true; // 업데이트 중임을 표시합니다.
            HandleCollisions(); // 충돌 처리를 합니다.

            // 모든 엔티티를 업데이트합니다.
            foreach (var entity in entities)
                entity.Update();

            isUpdating = false; // 업데이트가 끝났음을 표시합니다.

            // 추가할 엔티티들을 실제로 추가합니다.
            foreach (var entity in addedEntities)
                AddEntity(entity);

            addedEntities.Clear(); // 추가할 엔티티 리스트를 비웁니다.

            // 만료된 엔티티들을 제거합니다.
            entities = entities.Where(x => !x.IsExpired).ToList();
            bullets = bullets.Where(x => !x.IsExpired).ToList();
            enemies = enemies.Where(x => !x.IsExpired).ToList();
        }

        // 충돌을 처리하는 메소드
        static void HandleCollisions()
        {
            // 적 간의 충돌을 처리합니다.
            for (int i = 0; i < enemies.Count; i++)
                for (int j = i + 1; j < enemies.Count; j++)
                {
                    if (IsColliding(enemies[i], enemies[j])) // 적들끼리 충돌했는지 확인합니다.
                    {
                        enemies[i].HandleCollision(enemies[j]); // 충돌을 처리합니다.
                        enemies[j].HandleCollision(enemies[i]);
                    }
                }

            // 총알과 적 사이의 충돌을 처리합니다.
            for (int i = 0; i < enemies.Count; i++)
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (IsColliding(enemies[i], bullets[j])) // 총알과 적이 충돌했는지 확인합니다.
                    {
                        enemies[i].WasShot(); // 적이 맞았음을 처리합니다.
                        bullets[j].IsExpired = true; // 총알을 만료 상태로 설정합니다.
                    }
                }

            // 플레이어와 적 사이의 충돌을 처리합니다.
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i])) // 적이 활성 상태이고 플레이어와 충돌했는지 확인합니다.
                {
                    KillPlayer(); // 플레이어를 죽입니다.
                    break; // 플레이어가 죽었으므로 반복을 중단합니다.
                }
            }

        }

        // 플레이어를 죽이고 적을 처리하는 메소드
        private static void KillPlayer()
        {
            PlayerShip.Instance.Kill(); // 플레이어를 죽입니다.
            enemies.ForEach(x => x.WasShot()); // 모든 적을 맞은 상태로 만듭니다.
            EnemySpawner.Reset(); // 적 생성기를 리셋합니다.
        }

        // 두 엔티티가 충돌했는지 확인하는 메소드
        private static bool IsColliding(Entity a, Entity b)
        {
            float radius = a.Radius + b.Radius; // 두 엔티티의 반지름을 더합니다.
            return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius; // 두 엔티티가 만료되지 않았고 거리 제곱이 반지름 제곱보다 작으면 충돌로 간주합니다.
        }

        // 주어진 위치와 반지름 내에 있는 엔티티들을 반환하는 메소드
        public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        {
            return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius); // 주어진 위치와 반지름 내에 있는 엔티티들을 찾습니다.
        }

        // 모든 엔티티를 그리는 메소드
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch); // 각 엔티티를 그립니다.
        }
    }
}
