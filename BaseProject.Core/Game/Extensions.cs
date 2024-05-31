using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BaseProject
{
    static class Extensions
    {
        // start와 end 지점 사이에 주어진 색상과 두께로 선을 그립니다.
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 2f)
        {
            Vector2 delta = end - start; // 시작점과 끝점 사이의 벡터를 계산합니다.
            // 선을 그립니다. Art.Pixel은 1x1 크기의 텍스처입니다.
            spriteBatch.Draw(Art.Pixel, start, null, color, delta.ToAngle(), new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
        }

        // 벡터의 각도를 라디안 단위로 반환합니다.
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X); // Y축과 X축의 비율로 각도를 계산합니다.
        }

        // 벡터의 길이를 주어진 길이로 조정합니다.
        public static Vector2 ScaleTo(this Vector2 vector, float length)
        {
            return vector * (length / vector.Length()); // 주어진 길이에 맞게 벡터를 스케일링합니다.
        }

        // 벡터를 Point 구조체로 변환합니다.
        public static Point ToPoint(this Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y); // Vector2를 정수형 Point로 변환합니다.
        }

        // 랜덤한 float 값을 반환합니다. 범위는 minValue와 maxValue 사이입니다.
        public static float NextFloat(this Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue; // 랜덤한 float 값을 생성합니다.
        }

        // 주어진 길이 범위 내에서 랜덤한 벡터를 반환합니다.
        public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
        {
            double theta = rand.NextDouble() * 2 * Math.PI; // 랜덤한 각도를 생성합니다.
            float length = rand.NextFloat(minLength, maxLength); // 랜덤한 길이를 생성합니다.
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta)); // 주어진 길이와 각도로 벡터를 생성합니다.
        }
    }
}
