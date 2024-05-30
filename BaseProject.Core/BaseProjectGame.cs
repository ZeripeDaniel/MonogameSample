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
    /// �� Ŭ������ ������ �ֿ� Ÿ���Դϴ�.
    /// </summary>
    public class BaseProjectGame : Microsoft.Xna.Framework.Game
    {
        // ������ ���� �Ӽ���
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
        /// ���Ӵ� �� �� ȣ��Ǿ� ��� �������� �ε��ϴ� ���Դϴ�.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            Sound.Load(Content);

            EntityManager.Add(PlayerShip.Instance);

#if !__IOS__
            // PC�� ����� ���¿��� Media Player�� ����� ��� ���ܰ� �߻��ϴ� �˷��� ����
            // �ڼ��� ������ http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66 ����
            // ���� VS���� �̸� �׽�Ʈ�ϴ� ���� �Ұ����մϴ�.
            // ���ܸ� catch�ϰ� �����ؾ� �մϴ�.
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Sound.Music);
            }
            catch { }
#endif
        }

        /// <summary>
        /// ���� ������ �����Ͽ� ���踦 ������Ʈ�ϰ�, �浹�� üũ�ϰ�, �Է��� �����ϸ�, ������� ����մϴ�.
        /// </summary>
        /// <param name="gameTime">�ð� ���� �������� �����մϴ�.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            Input.Update();

#if !__IOS__
            // ������ ������ �� �ֵ��� �մϴ�.
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
        /// ��濡�� �������� ������ �׸��ϴ�.
        /// </summary>
        /// <param name="gameTime">�ð� ���� �������� �����մϴ�.</param>
        protected override void Draw(GameTime gameTime)
        {
            /// <summary>
            ///spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            ///`Deferred`�� �Ϲ������� "�����", "�̷�", "���߿� ó���ϴ�"�̶�� �ǹ̸� ������ �ֽ��ϴ�. ���α׷����̳� �׷��� �������� �ƶ�����, `Deferred`�� �۾��̳� ����� ��� �������� �ʰ� ���߿� �����ϱ� ���� �����ϴ� ���� �ǹ��մϴ�.
            ///���⼭ `SpriteSortMode.Deferred`�� ��������Ʈ�� �׸� �� ��� �������� �ʰ�, �׸��� �۾��� �Ϸ�� ������ ������ �����ϴ� ���� �ǹ��մϴ�. �̴� ���� ��������Ʈ�� ȿ�������� ó���ϰ� ������ ������ �������ϱ� ���� ���˴ϴ�.
            ///
            ///�ٸ� `SpriteSortMode` �ɼǵ�� ���ϸ�:
            ///- `Immediate`: ��������Ʈ�� �߰��ϴ� ��� �׸��ϴ�.
            ///- `Deferred`: ��������Ʈ�� ��Ƶξ��ٰ� `End`�� ȣ��� �� �Ѳ����� �׸��ϴ�.
            ///- `Texture`: �ؽ�ó���� �����Ͽ� ���� �ؽ�ó�� ����ϴ� ��������Ʈ�� �Բ� �׸��ϴ�.
            ///- `BackToFront`�� `FrontToBack`: Z ������ ���� ��������Ʈ�� �����Ͽ� �׸��ϴ�.
            ///
            ///��, `SpriteSortMode.Deferred`�� ��������Ʈ�� �׸� �� ������ ����ȭ�ϱ� ���� ���߿� �����ϰ� �׸��� ����Դϴ�.
            /// </summary>

            GraphicsDevice.Clear(Color.DarkOliveGreen);

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            EntityManager.Draw(spriteBatch);
            spriteBatch.End();
           
            base.Draw(gameTime);
            // ��� ���� ����� �������̽� �׸���
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            //�Ʒ��� �ٸ� Ŭ������ ����ϴ� �����̴�. ����̳� ���ھ� ����� ǥ���Ѵ�.
            //DrawTitleSafeAlignedString("Lives: " + PlayerStatus.Lives, 5);
            //DrawTitleSafeRightAlignedString("Score: " + PlayerStatus.Score, 5);
            //DrawTitleSafeRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);

            // Ŀ���� ���콺 Ŀ�� �׸���
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
