using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace BaseProject
{
    static class Sound
    {
        public static Song Music { get; private set; }

        private static readonly Random rand = new Random();

        private static SoundEffect[] explosions;
        // 무작위 폭발 소리를 반환합니다.
        public static SoundEffect Explosion { get { return explosions[rand.Next(explosions.Length)]; } }

        private static SoundEffect[] shots;
        public static SoundEffect Shot { get { return shots[rand.Next(shots.Length)]; } }

        private static SoundEffect[] spawns;
        public static SoundEffect Spawn { get { return spawns[rand.Next(spawns.Length)]; } }

        public static void Load(ContentManager content)
        {
            Music = content.Load<Song>("Audio/Music");

            // 이 linq 표현식은 각 카테고리의 모든 소리를 배열로 로드하는 멋진 방법입니다.
            explosions = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/explosion-0" + x)).ToArray();
            shots = Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Audio/shoot-0" + x)).ToArray();
            spawns = Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/spawn-0" + x)).ToArray();
        }
    }
}
