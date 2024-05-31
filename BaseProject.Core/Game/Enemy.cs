using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BaseProject
{
    // 적 클래스는 게임 내에서 적을 나타냅니다.
    class Enemy : Entity
    {
        // 랜덤 값을 생성하기 위한 Random 인스턴스입니다.
        public static Random rand = new Random();

        // 적의 행동을 저장하는 리스트입니다.
        private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();

        // 적이 활성화될 때까지 남은 시간을 저장하는 변수입니다.
        private int timeUntilStart = 60;

        // 적이 활성화되었는지 여부를 나타냅니다.
        public bool IsActive { get { return timeUntilStart <= 0; } }

        // 적의 점수 값을 저장하는 변수입니다.
        public int PointValue { get; private set; }

        // 적 생성자입니다. 이미지와 위치를 설정합니다.
        public Enemy(Texture2D image, Vector2 position)
        {
            this.image = image; // 적의 이미지를 설정합니다.
            Position = position; // 적의 위치를 설정합니다.
            Radius = image.Width / 2f; // 이미지의 너비를 반지름으로 설정하여 충돌 감지에 사용합니다.
            color = Color.Transparent; // 초기 색상은 투명으로 설정합니다.
            PointValue = 1; // 기본 점수 값을 설정합니다.
        }

        // 시커(Seeker) 적을 생성하는 메소드입니다.
        public static Enemy CreateSeeker(Vector2 position)
        {
            var enemy = new Enemy(Art.Seeker, position); // 새로운 시커 적을 생성합니다.
            enemy.AddBehaviour(enemy.FollowPlayer(0.9f)); // 플레이어를 따라가는 행동을 추가합니다.
            enemy.PointValue = 2; // 시커 적의 점수 값을 설정합니다.

            return enemy;
        }

        // 방황자(Wanderer) 적을 생성하는 메소드입니다.
        public static Enemy CreateWanderer(Vector2 position)
        {
            var enemy = new Enemy(Art.Wanderer, position); // 새로운 방황자 적을 생성합니다.
            enemy.AddBehaviour(enemy.MoveRandomly()); // 무작위로 움직이는 행동을 추가합니다.

            return enemy;
        }

        // 적의 상태를 업데이트하는 메소드입니다.
        public override void Update()
        {
            if (timeUntilStart <= 0) // 적이 활성화되었으면
                ApplyBehaviours(); // 행동을 적용합니다.
            else
            {
                timeUntilStart--; // 활성화되기까지 남은 시간을 감소시킵니다.
                color = Color.White * (1 - timeUntilStart / 60f); // 색상을 점차적으로 변경하여 스폰인 효과를 만듭니다.
            }

            Position += Velocity; // 위치를 속도만큼 이동시킵니다.
            Position = Vector2.Clamp(Position, Size / 2, BaseProjectGame.ScreenSize - Size / 2); // 화면 경계를 벗어나지 않도록 위치를 제한합니다.

            Velocity *= 0.8f; // 속도를 점차적으로 감소시킵니다.
        }

        // 적을 그리는 메소드입니다.
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (timeUntilStart > 0) // 적이 활성화되기 전이라면
            {
                // 스폰인 효과의 일부로 스프라이트의 확장 및 페이드 아웃 버전을 그립니다.
                float factor = timeUntilStart / 60f; // 적이 스폰되면 1에서 0으로 감소합니다.
                spriteBatch.Draw(image, Position, null, Color.White * factor, Orientation, Size / 2f, 2 - factor, SpriteEffects.None, 0);
            }

            base.Draw(spriteBatch); // 기본 그리기 메소드를 호출하여 적을 그립니다.
        }

        // 행동을 추가하는 메소드입니다.
        private void AddBehaviour(IEnumerable<int> behaviour)
        {
            behaviours.Add(behaviour.GetEnumerator()); // 행동을 리스트에 추가합니다.
        }

        // 추가된 행동을 적용하는 메소드입니다.
        private void ApplyBehaviours()
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                if (!behaviours[i].MoveNext()) // 행동을 한 단계 실행합니다.
                    behaviours.RemoveAt(i--); // 행동이 끝나면 리스트에서 제거합니다.
            }
        }

        // 다른 적과 충돌했을 때 처리하는 메소드입니다.
        public void HandleCollision(Enemy other)
        {
            var d = Position - other.Position; // 충돌한 적과의 거리 벡터를 계산합니다.
            Velocity += 10 * d / (d.LengthSquared() + 1); // 반발력을 적용하여 속도를 변경합니다.
        }

        // 적이 총에 맞았을 때 처리하는 메소드입니다.
        public void WasShot()
        {
            IsExpired = true; // 적을 만료 상태로 설정합니다.
            PlayerStatus.AddPoints(PointValue); // 플레이어에게 점수를 추가합니다.
            PlayerStatus.IncreaseMultiplier(); // 플레이어의 점수 배율을 증가시킵니다.

            float hue1 = rand.NextFloat(0, 6); // 첫 번째 색상의 색조를 랜덤하게 선택합니다.
            float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f; // 두 번째 색상의 색조를 랜덤하게 선택합니다.
            Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1); // 첫 번째 색상을 HSV 색상 모델로 변환합니다.
            Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1); // 두 번째 색상을 HSV 색상 모델로 변환합니다.

            //for (int i = 0; i < 120; i++) // 120개의 파티클을 생성합니다.
            //{
            //    float speed = 18f * (1f - 1 / rand.NextFloat(1, 10)); // 파티클의 속도를 랜덤하게 설정합니다.
            //    var state = new ParticleState()
            //    {
            //        Velocity = rand.NextVector2(speed, speed), // 파티클의 속도를 설정합니다.
            //        Type = ParticleType.Enemy, // 파티클의 타입을 적으로 설정합니다.
            //        LengthMultiplier = 1 // 파티클의 길이 배율을 설정합니다.
            //    };

            //    Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1)); // 두 색상을 랜덤하게 혼합하여 파티클 색상을 설정합니다.
            //    BaseProjectGame.ParticleManager.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state); // 파티클을 생성합니다.
            //}

            Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0); // 폭발 소리를 재생합니다.
        }

        #region Behaviours
        // 플레이어를 따라가는 행동을 정의합니다.
        IEnumerable<int> FollowPlayer(float acceleration)
        {
            while (true)
            {
                if (!PlayerShip.Instance.IsDead) // 플레이어가 살아있으면
                    Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration); // 플레이어를 향해 가속합니다.

                if (Velocity != Vector2.Zero) // 속도가 0이 아니면
                    Orientation = Velocity.ToAngle(); // 적의 방향을 속도 방향으로 설정합니다.

                yield return 0; // 다음 프레임으로 넘어갑니다.
            }
        }

        // 무작위로 움직이는 행동을 정의합니다.
        IEnumerable<int> MoveRandomly()
        {
            float direction = rand.NextFloat(0, MathHelper.TwoPi); // 초기 이동 방향을 랜덤하게 설정합니다.

            while (true)
            {
                direction += rand.NextFloat(-0.1f, 0.1f); // 방향을 약간 랜덤하게 변경합니다.
                direction = MathHelper.WrapAngle(direction); // 방향을 -π에서 π 범위로 조정합니다.

                for (int i = 0; i < 6; i++) // 6번 반복하여 이동합니다.
                {
                    Velocity += MathUtil.FromPolar(direction, 0.4f); // 현재 방향으로 이동 벡터를 계산하여 속도에 더합니다.
                    Orientation -= 0.05f; // 적의 방향을 천천히 회전시킵니다.

                    var bounds = BaseProjectGame.Viewport.Bounds; // 화면 경계를 가져옵니다.
                    bounds.Inflate(-image.Width / 2 - 1, -image.Height / 2 - 1); // 경계를 적 이미지 크기만큼 줄입니다.

                    // 적이 경계 밖에 있으면 가장자리에서 멀어지게 만듭니다.
                    if (!bounds.Contains(Position.ToPoint()))
                        direction = (BaseProjectGame.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2); // 화면 중앙을 향해 새로운 방향을 설정합니다.

                    yield return 0; // 다음 프레임으로 넘어갑니다.
                }
            }
        }
        #endregion
    }
}