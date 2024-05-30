using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BaseProject
{
	abstract class Entity
	{
		protected Texture2D image;
        // 이미지의 색조입니다. 이를 통해 투명도를 변경할 수도 있습니다.
        protected Color color = Color.White;	

		public Vector2 Position, Velocity;
		public float Orientation;
		public float Radius = 20;   // 원형 충돌 감지에 사용됩니다.
        public bool IsExpired;      // 엔터티가 파괴되어 삭제되어야 하는 경우 true입니다.

        public Vector2 Size
		{
			get
			{
				return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
			}
		}

		public abstract void Update();

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
		}
	}
}