using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace BaseProject
{
    static class Input
	{
		private static KeyboardState keyboardState, lastKeyboardState;
		private static MouseState mouseState, lastMouseState;
		private static GamePadState gamepadState, lastGamepadState;

		private static bool isAimingWithMouse = false;

		public static Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y); } }

		public static void Update()
		{
			lastKeyboardState = keyboardState;
			lastMouseState = mouseState;
			lastGamepadState = gamepadState;

			keyboardState = Keyboard.GetState();
			mouseState = Mouse.GetState();
			gamepadState = GamePad.GetState(PlayerIndex.One);

            // 플레이어가 화살표 키 중 하나를 누르거나 게임 패드를 사용하여 조준하는 경우 마우스 조준을 비활성화하고 싶습니다. 그렇지 않으면,
            // 플레이어가 마우스를 움직이면 마우스 조준을 활성화합니다.
            if (new[] { Keys.Left, Keys.Right, Keys.Up, Keys.Down }.Any(x => keyboardState.IsKeyDown(x)) || gamepadState.ThumbSticks.Right != Vector2.Zero)
				isAimingWithMouse = false;
			else if (MousePosition != new Vector2(lastMouseState.X, lastMouseState.Y))
				isAimingWithMouse = true;
		}

        // 방금 키를 눌렀는지 확인합니다.
        public static bool WasKeyPressed(Keys key)
		{
			return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
		}

		public static bool WasButtonPressed(Buttons button)
		{
			return lastGamepadState.IsButtonUp(button) && gamepadState.IsButtonDown(button);
		}

		public static Vector2 GetMovementDirection()
		{
			
			Vector2 direction = gamepadState.ThumbSticks.Left;
			direction.Y *= -1;  // y축 반전

            if (keyboardState.IsKeyDown(Keys.A))
				direction.X -= 1;
			if (keyboardState.IsKeyDown(Keys.D))
				direction.X += 1;
			if (keyboardState.IsKeyDown(Keys.W))
				direction.Y -= 1;
			if (keyboardState.IsKeyDown(Keys.S))
				direction.Y += 1;

            // 벡터의 길이를 최대 1로 고정합니다.
            if (direction.LengthSquared() > 1)
				direction.Normalize();

			return direction;
		}

		public static Vector2 GetAimDirection()
		{
			if (isAimingWithMouse)
				return GetMouseAimDirection();

			Vector2 direction = gamepadState.ThumbSticks.Right;
			direction.Y *= -1;

			if (keyboardState.IsKeyDown(Keys.Left))
				direction.X -= 1;
			if (keyboardState.IsKeyDown(Keys.Right))
				direction.X += 1;
			if (keyboardState.IsKeyDown(Keys.Up))
				direction.Y -= 1;
			if (keyboardState.IsKeyDown(Keys.Down))
				direction.Y += 1;

            // 목표 입력이 없으면 0을 반환합니다. 그렇지 않으면 길이가 1이 되도록 방향을 정규화합니다.
            if (direction == Vector2.Zero)
				return Vector2.Zero;
			else
				return Vector2.Normalize(direction);
		}

		private static Vector2 GetMouseAimDirection()
		{
			Vector2 direction = MousePosition - PlayerShip.Instance.Position;

			if (direction == Vector2.Zero)
				return Vector2.Zero;
			else
				return Vector2.Normalize(direction);
		}

		public static bool WasBombButtonPressed()
		{
			return WasButtonPressed(Buttons.LeftTrigger) || WasButtonPressed(Buttons.RightTrigger) || WasKeyPressed(Keys.Space);
		}
	}
}