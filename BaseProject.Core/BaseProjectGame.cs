using BloomPostprocess;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#if !__IOS__
using Microsoft.Xna.Framework.Media;
#endif

namespace BaseProject
{
    /// <summary>
    /// 이 클래스는 게임의 주요 타입입니다.
    /// </summary>
    public class BaseProjectGame : Microsoft.Xna.Framework.Game
    {
        // 유용한 정적 속성들
        public static BaseProjectGame Instance { get; private set; }
        public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
        public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
        public static GameTime GameTime { get; private set; }
        //public static ParticleManager<ParticleState> ParticleManager { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BloomComponent bloom;

        bool paused = false;


        public BaseProjectGame()
        {
            Instance = this;
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            bloom = new BloomComponent(this);
            Components.Add(bloom);
            bloom.Settings = new BloomSettings(null, 0.25f, 4, 2, 1, 1.5f, 1);
            bloom.Visible = false;
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";

            //ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

            base.Initialize();
        }

        /// <summary>
        /// 게임당 한 번 호출되어 모든 콘텐츠를 로드하는 곳입니다.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            Sound.Load(Content);

            EntityManager.Add(PlayerShip.Instance);

#if !__IOS__
            // PC에 연결된 상태에서 Media Player를 사용할 경우 예외가 발생하는 알려진 문제
            // 자세한 내용은 http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66 참조
            // 따라서 VS에서 이를 테스트하는 것은 불가능합니다.
            // 예외를 catch하고 무시해야 합니다.
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Sound.Music);
            }
            catch { }
#endif
        }

        /// <summary>
        /// 게임 로직을 실행하여 세계를 업데이트하고, 충돌을 체크하고, 입력을 수집하며, 오디오를 재생합니다.
        /// </summary>
        /// <param name="gameTime">시간 값의 스냅샷을 제공합니다.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

#if !__IOS__
            // 게임을 종료할 수 있도록 합니다.
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                this.Exit();
#endif

            if (Input.WasKeyPressed(Keys.P))
                paused = !paused;
            if (Input.WasKeyPressed(Keys.B))
                bloom.Visible = !bloom.Visible;

            if (!paused)
            {
                PlayerStatus.Update();
                EntityManager.Update();
                EnemySpawner.Update();
                //ParticleManager.Update();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// 배경에서 전경으로 게임을 그립니다.
        /// </summary>
        /// <param name="gameTime">시간 값의 스냅샷을 제공합니다.</param>
        protected override void Draw(GameTime gameTime)
        {
            /// <summary>
            ///spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            ///`Deferred`는 일반적으로 "연기된", "미룬", "나중에 처리하는"이라는 의미를 가지고 있습니다. 프로그래밍이나 그래픽 렌더링의 맥락에서, `Deferred`는 작업이나 계산을 즉시 수행하지 않고 나중에 수행하기 위해 연기하는 것을 의미합니다.
            ///여기서 `SpriteSortMode.Deferred`는 스프라이트를 그릴 때 즉시 정렬하지 않고, 그리기 작업이 완료될 때까지 정렬을 연기하는 것을 의미합니다. 이는 여러 스프라이트를 효율적으로 처리하고 최적의 순서로 렌더링하기 위해 사용됩니다.
            ///
            ///다른 `SpriteSortMode` 옵션들과 비교하면:
            ///- `Immediate`: 스프라이트를 추가하는 즉시 그립니다.
            ///- `Deferred`: 스프라이트를 모아두었다가 `End`가 호출될 때 한꺼번에 그립니다.
            ///- `Texture`: 텍스처별로 정렬하여 같은 텍스처를 사용하는 스프라이트를 함께 그립니다.
            ///- `BackToFront`와 `FrontToBack`: Z 순서에 따라 스프라이트를 정렬하여 그립니다.
            ///
            ///즉, `SpriteSortMode.Deferred`는 스프라이트를 그릴 때 성능을 최적화하기 위해 나중에 정렬하고 그리는 방식입니다.
            /// </summary>

            GraphicsDevice.Clear(Color.DarkOliveGreen);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();
           
            base.Draw(gameTime);
            // 블룸 없이 사용자 인터페이스 그리기
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            //아래는 다른 클래스를 사용하는 예제이다. 목숨이나 스코어 배수를 표현한다.
            //DrawTitleSafeAlignedString("Lives: " + PlayerStatus.Lives, 5);
            //DrawTitleSafeRightAlignedString("Score: " + PlayerStatus.Score, 5);
            //DrawTitleSafeRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

            // 커스텀 마우스 커서 그리기
            spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

            string text = "Hello World!\n";
            Vector2 textSize = Art.Font.MeasureString(text);
            spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
            
            spriteBatch.End();
        }

        private void DrawRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
        }

        private void DrawTitleSafeAlignedString(string text, int pos)
        {
            spriteBatch.DrawString(Art.Font, text, new Vector2(Viewport.TitleSafeArea.X + pos), Color.White);
        }

        private void DrawTitleSafeRightAlignedString(string text, float y)
        {
            var textWidth = Art.Font.MeasureString(text).X;
            spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5 - Viewport.TitleSafeArea.X, Viewport.TitleSafeArea.Y + y), Color.White);
        }
    }
}
