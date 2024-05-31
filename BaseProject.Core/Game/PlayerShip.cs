using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BaseProject
{
    class PlayerShip : Entity
    {

        //Singleton 패턴은 객체 지향 프로그래밍에서 사용되는 디자인 패턴 중 하나로,
        //클래스의 인스턴스를 하나만 생성하여 전역적으로 접근할 수 있도록 하는 패턴입니다.
        //Singleton 패턴은 다음과 같은 상황에서 유용합니다.

        //인스턴스가 하나만 필요할 때: 예를 들어, 시스템 설정, 로그 관리, 데이터베이스 연결 등은
        //애플리케이션 내에서 하나의 인스턴스만 존재하는 것이 효율적입니다.

        //글로벌 액세스 포인트를 제공할 때: Singleton 인스턴스를 전역적으로 접근 가능하게 하여,
        //어디서든 동일한 인스턴스를 사용할 수 있도록 합니다.

        //Singleton 패턴의 주요 구성 요소
        //Private 생성자: 클래스 외부에서 인스턴스를 생성하지 못하도록 생성자를 private으로 정의합니다.
        //Static 인스턴스 변수: 클래스의 유일한 인스턴스를 저장하는 static 변수를 선언합니다.
        //Public 정적 메소드: 인스턴스를 반환하는 정적 메소드를 제공하여,
        //인스턴스가 존재하지 않으면 생성하고, 이미 존재하면 기존 인스턴스를 반환합니다.

        // Singleton 패턴을 사용하여 PlayerShip의 인스턴스를 하나만 유지합니다.
        private static PlayerShip instance;
        public static PlayerShip Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerShip(); // 새로운 PlayerShip 인스턴스를 생성합니다.

                return instance; // 생성된 인스턴스를 반환합니다.
            }
        }

        // 총알 발사 간의 딜레이를 설정합니다.
        const int cooldownFrames = 60; // 총알 발사 쿨다운 프레임 수
        int cooldowmRemaining = 0; // 남은 쿨다운 프레임 수

        // 플레이어가 죽었을 때 부활까지 남은 프레임 수를 저장합니다.
        int framesUntilRespawn = 0; // 부활까지 남은 프레임 수
        public bool IsDead { get { return framesUntilRespawn > 0; } } // 플레이어가 죽었는지 여부를 반환합니다.

        // 랜덤한 값을 생성하기 위한 Random 인스턴스입니다.
        static Random rand = new Random(); // Random 인스턴스 생성

        // PlayerShip 생성자입니다. Singleton 패턴을 위해 private으로 설정합니다.
        //PlayerShip이 생성 되면 아래의 priavate가 실행된다! 그렇담 플레이어 이미지와 초기위치를 새로이 주어야한다!
        private PlayerShip()
        {
            image = Art.Player; // 플레이어 이미지를 설정합니다.
            Position = BaseProjectGame.ScreenSize / 2; // 플레이어의 초기 위치를 화면 중앙으로 설정합니다.
            Radius = 10; // 충돌 판정을 위한 반지름을 설정합니다. 반지름이므로 상,하,좌,우 중앙을 기점으로 10 10 10 10 이 생긴다
            //그렇다는건 좌,우 의 거리 차이는 20이겟지?
        }

        // 매 프레임마다 호출되는 업데이트 메소드입니다.
        public override void Update()
        {
            if (IsDead) // 플레이어가 죽었을 때
            {
                if (--framesUntilRespawn == 0) // 부활까지 남은 프레임을 감소시키고, 0이 되면
                {
                    if (PlayerStatus.Lives == 0) // 플레이어의 생명이 모두 소진되었는지 확인합니다.
                    {
                        PlayerStatus.Reset(); // 플레이어 상태를 리셋합니다.
                        Position = BaseProjectGame.ScreenSize / 2; // 플레이어의 위치를 화면 중앙으로 설정합니다.
                    }
                }
                return; // 더 이상 업데이트를 진행하지 않습니다.
            }

            // 스페이스바 또는 마우스 좌 클릭 시 총알을 발사합니다.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var aim = Input.GetAimDirection(); // 조준 방향을 가져옵니다.
                if (aim.LengthSquared() > 0 && cooldowmRemaining <= 0) // 유효한 조준 방향이고, 쿨다운이 끝났다면
                {
                    cooldowmRemaining = cooldownFrames; // 쿨다운을 초기화합니다.
                    float aimAngle = aim.ToAngle(); // 조준 각도를 계산합니다.
                    Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle); // 조준 각도에 따른 회전 쿼터니언을 생성합니다.

                    // 총알의 발사 방향에 약간의 랜덤 스프레드를 추가합니다.
                    float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
                    Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f); // 총알의 속도를 계산합니다.

                    // 총알 발사 위치를 설정하고 발사합니다.
                    Vector2 offset = Vector2.Transform(new Vector2(35, -8), aimQuat); // 왼쪽 총알 위치
                    EntityManager.Add(new Bullet(Position + offset, vel)); // 총알을 EntityManager에 추가합니다.

                    offset = Vector2.Transform(new Vector2(35, 8), aimQuat); // 오른쪽 총알 위치
                    EntityManager.Add(new Bullet(Position + offset, vel)); // 총알을 EntityManager에 추가합니다.

                    Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0); // 총알 발사 소리를 재생합니다.
                }
            }

            if (cooldowmRemaining > 0) // 쿨다운이 남아있다면
                cooldowmRemaining--; // 쿨다운을 감소시킵니다.

            const float speed = 8; // 플레이어의 이동 속도를 설정합니다.
            Velocity += speed * Input.GetMovementDirection(); // 플레이어의 이동 벡터를 업데이트합니다.
            Position += Velocity; // 플레이어의 위치를 업데이트합니다.
            Position = Vector2.Clamp(Position, Size / 2, BaseProjectGame.ScreenSize - Size / 2); // 플레이어의 위치를 화면 내로 제한합니다.

            if (Velocity.LengthSquared() > 0) // 이동 속도가 0보다 크다면
                Orientation = Velocity.ToAngle(); // 플레이어의 방향을 업데이트합니다.

            Velocity = Vector2.Zero; // 이동 속도를 0으로 초기화합니다.
        }

        // 플레이어를 그립니다.
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead) // 플레이어가 죽지 않았을 때
                base.Draw(spriteBatch); // 기본 그리기 메소드를 호출하여 플레이어를 그립니다.
        }

        // 플레이어가 죽었을 때 처리합니다.
        public void Kill()
        {
            PlayerStatus.RemoveLife(); // 플레이어의 생명을 하나 제거합니다.
            framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120; // 게임 오버 여부에 따라 부활 시간을 설정합니다.
        }
    }
}
