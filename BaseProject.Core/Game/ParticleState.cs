using Microsoft.Xna.Framework;
using System;

namespace BaseProject
{
    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

	public struct ParticleState
	{
		public Vector2 Velocity;
		public ParticleType Type;
		public float LengthMultiplier;

		private static Random rand = new Random();

		public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
		{
			Velocity = velocity;
			Type = type;
			LengthMultiplier = lengthMultiplier;
		}

		public static ParticleState GetRandom(float minVel, float maxVel)
		{
			var state = new ParticleState();
			state.Velocity = rand.NextVector2(minVel, maxVel);
			state.Type = ParticleType.None;
			state.LengthMultiplier = 1;

			return state;
		}

		public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
		{
			var vel = particle.State.Velocity;
			float speed = vel.Length();

            // Vector2.Add()를 사용하는 것이 "x.Position += vel;"을 사용하는 것보다 약간 더 빠릅니다.
			// Vector2는 참조로 전달되므로 복사할 필요가 없기 때문입니다.
			// 매우 많은 수의 입자를 업데이트해야 할 수 있으므로 이 방법은 최적화를 위한 좋은 후보입니다.
			//뱀서류들도 이러한 형태로 최적화를 하지요.. 그냥 Vector2로 받아오게되면 메모리 감당안됨
            Vector2.Add(ref particle.Position, ref vel, out particle.Position);

            // PercentLife 또는 속도가 낮은 경우 입자를 페이드합니다.
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
			alpha *= alpha;

			particle.Tint.A = (byte)(255 * alpha);

            // 총알 입자의 길이는 다른 입자보다 속도에 덜 의존합니다.
            if (particle.State.Type == ParticleType.Bullet)
				particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
			else
				particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

			particle.Orientation = vel.ToAngle();

			var pos = particle.Position;
			int width = (int)BaseProjectGame.ScreenSize.X;
			int height = (int)BaseProjectGame.ScreenSize.Y;

            // 화면 가장자리와 충돌
            if (pos.X < 0)
				vel.X = Math.Abs(vel.X);
			else if (pos.X > width)
				vel.X = -Math.Abs(vel.X);
			if (pos.Y < 0)
				vel.Y = Math.Abs(vel.Y);
			else if (pos.Y > height)
				vel.Y = -Math.Abs(vel.Y);

			if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f) // 비정규화된 부동 소수점은 심각한 성능 문제를 야기합니다.
                vel = Vector2.Zero;
			else if (particle.State.Type == ParticleType.Enemy)
				vel *= 0.94f;
			else
				vel *= 0.96f + Math.Abs(pos.X) % 0.04f; // rand.Next()는 스레드에 안전하지 않으므로 난수를 만들 때 위치를 사용하세요

            particle.State.Velocity = vel;
		}
	}
}