using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BaseProject
{
    // Entity 클래스는 게임 내 모든 엔터티의 기본 클래스입니다.
    abstract class Entity
    {
        // 엔터티의 이미지를 저장하는 변수입니다.
        protected Texture2D image;

        // 이미지의 색조를 나타내는 변수입니다. 이를 통해 투명도를 변경할 수도 있습니다.
        protected Color color = Color.White;

        // 엔터티의 현재 위치를 나타내는 변수입니다.
        public Vector2 Position;

        // 엔터티의 속도를 나타내는 변수입니다.
        public Vector2 Velocity;

        // 엔터티의 방향(각도)을 나타내는 변수입니다.
        public float Orientation;

        // 엔터티의 반지름을 나타내는 변수로, 원형 충돌 감지에 사용됩니다.
        public float Radius = 20;

        // 엔터티가 파괴되어 삭제되어야 하는 경우 true가 되는 변수입니다.
        public bool IsExpired;

        // 엔터티의 크기를 반환하는 프로퍼티입니다.
        public Vector2 Size
        {
            get
            {
                // 이미지가 null이면 (즉, 이미지가 없으면) 크기는 (0, 0)입니다.
                // 그렇지 않으면 이미지의 너비와 높이를 반환합니다.
                return image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);
            }
        }

        // 모든 엔터티는 반드시 Update 메소드를 구현해야 합니다.
        // 이 메소드는 엔터티의 상태를 매 프레임마다 업데이트합니다.
        public abstract void Update();

        // 엔터티를 그리는 메소드입니다. 기본적인 그리기 방법을 정의합니다.
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // 이미지, 위치, 색조, 회전, 중심점, 스케일, 스프라이트 효과, 레이어 깊이 순으로 설정하여 이미지를 그립니다.
            spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, SpriteEffects.None, 0);
        }
    }
}
