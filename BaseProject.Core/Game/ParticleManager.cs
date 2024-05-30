using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaseProject
{
    public class ParticleManager<T>
	{
        // 이 파티클매니저는 각 입자에 대해 호출됩니다.
        private Action<Particle> updateParticle;
		private CircularParticleArray particleList;

        /// <summary>
        /// 파티클 생성을 허용합니다.
        /// </summary>
        /// <param name="capacity">최대 파티클 수입니다. 이 크기의 배열은 사전 할당됩니다.</param>
        /// <param name="updateParticle">입자에 대한 사용자 정의 동작을 지정할 수 있는 대리자입니다. 파티클당, 프레임당 한 번씩 호출됩니다.</param>
        public ParticleManager(int capacity, Action<Particle> updateParticle)
		{
			this.updateParticle = updateParticle;
			particleList = new CircularParticleArray(capacity);

            // 재사용을 위해 목록을 빈 입자 개체로 채웁니다.
            for (int i = 0; i < capacity; i++)
				particleList[i] = new Particle();
		}

        /// <summary>
        /// 매 프레임마다 호출되도록 입자 상태를 업데이트합니다.
        /// </summary>
        public void Update()
		{
			int removalCount = 0;
			for (int i = 0; i < particleList.Count; i++)
			{
				var particle = particleList[i];

				updateParticle(particle);

				particle.PercentLife -= 1f / particle.Duration;

                // 삭제된 입자를 목록 끝까지 선별합니다.
                Swap(particleList, i - removalCount, i);

                // 알파 < 0이면 이 입자를 삭제합니다.
                if (particle.PercentLife < 0)
					removalCount++;
			}
			particleList.Count -= removalCount;
		}

		private static void Swap(CircularParticleArray list, int index1, int index2)
		{
			var temp = list[index1];
			list[index1] = list[index2];
			list[index2] = temp;
		}

        /// <summary>
        /// 입자를 그립니다.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < particleList.Count; i++)
			{
				var particle = particleList[i];

				Vector2 origin = new Vector2(particle.Texture.Width / 2, particle.Texture.Height / 2);
				spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation, origin, particle.Scale, 0, 0);
			}
		}

		public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
		{
			CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
		}

		public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
		{
			Particle particle;
			if (particleList.Count == particleList.Capacity)
			{
                // 목록이 가득 차면 가장 오래된 입자를 덮어쓰고 원형 목록을 회전합니다.
                particle = particleList[0];
				particleList.Start++;
			}
			else
			{
				particle = particleList[particleList.Count];
				particleList.Count++;
			}

            // 입자(파티클) 생성
            particle.Texture = texture;
			particle.Position = position;
			particle.Tint = tint;

			particle.Duration = duration;
			particle.PercentLife = 1f;
			particle.Scale = scale;
			particle.Orientation = theta;
			particle.State = state;
		}

		/// <summary>
		/// 모든 파티클 파괴
		/// </summary>
		public void Clear()
		{
			particleList.Count = 0;
		}

		public int ParticleCount
		{
			get { return particleList.Count; }
		}

		public class Particle
		{
			public Texture2D Texture;
			public Vector2 Position;
			public float Orientation;

			public Vector2 Scale = Vector2.One;

			public Color Tint;
			public float Duration;
			public float PercentLife = 1f;
			public T State;
		}

        // 임의의 시작점이 있는 원형 배열을 나타냅니다.
		// 배열이 가득 차면 가장 오래된 입자를 효율적으로 덮어쓰는 데 유용합니다.
		// 간단히 partitionList[0]을 덮어쓰고 시작을 진행하세요.
        private class CircularParticleArray
		{
			private int start;
			public int Start
			{
				get { return start; }
				set { start = value % list.Length; }
			}

			public int Count { get; set; }
			public int Capacity { get { return list.Length; } }
			private Particle[] list;

			public CircularParticleArray() { }  // for serialization

			public CircularParticleArray(int capacity)
			{
				list = new Particle[capacity];
			}

			public Particle this[int i]
			{
				get { return list[(start + i) % list.Length]; }
				set { list[(start + i) % list.Length] = value; }
			}
		}
	}
}