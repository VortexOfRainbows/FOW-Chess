
using Chess.Managers;
using Chess.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;

namespace Chess
{
    public class CloudGroup
    {
        public const int CloudPerGroup = 9;
        public Cloud[] clouds;
        public Point tile;
        public void LoadColors(Color questionColor)
        {
            clouds = new Cloud[CloudPerGroup + 1];
            for(int i = 0; i < clouds.Length; i++)
            {
                if(i < clouds.Length - 1)
                    clouds[i] = new Cloud(-40 + i * 10);
                else
                {
                    clouds[i] = new Cloud(0);
                    clouds[i].CloudColor = questionColor;
                }
            }
        }
        public void Draw(Vector2 location, SpriteBatch spriteBatch, bool Question = false)
        {
            Texture2D texture = ContentService.Instance.Textures["Cloud"];
            Texture2D question = ContentService.Instance.Textures["Question"];
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            if(!Question)
            {
                for (int i = 0; i < clouds.Length - 1; i++)
                {
                    Cloud cloud = clouds[i];
                    spriteBatch.Draw(texture, location + cloud.offset, null, cloud.CloudColor * cloud.AlphaMultiplier * 0.55f, 0f, origin, 0.5f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                origin = new Vector2(question.Width / 2, question.Height / 2);
                spriteBatch.Draw(question, location + clouds[CloudPerGroup].offset * 0.1f, null, clouds[CloudPerGroup].CloudColor * clouds[CloudPerGroup].AlphaMultiplier, 0f, origin, 0.5f, SpriteEffects.None, 0f);
            }
        }
        public void Update()
        {
            foreach(Cloud cloud in clouds)
            {
                cloud.Update();
                if (Piece.VisiblePoints.Contains(tile))
                {
                    cloud.AlphaMultiplier = MathHelper.Lerp(cloud.AlphaMultiplier, 0f, 0.055f);
                }
                else
                {
                    cloud.AlphaMultiplier = MathHelper.Lerp(cloud.AlphaMultiplier, 1.0f, 0.055f);
                }
            }
        }
        public CloudGroup(Point tile)
        {
            this.tile = tile;
            Color questionColor = Color.Lerp(Color.White, Color.Black, 0.1f);
            if ((tile.X + tile.Y) % 2 == 0)
            {
                questionColor = Color.Lerp(Color.White, Color.Black, 0.9f);
            }
            LoadColors(questionColor);
        }
    }
    public class Cloud
    {
        public Color CloudColor;
        public Vector2 offset;
        public float AlphaMultiplier = 1.0f;
        public float iterator = 0f;
        public float iterateSpeed = 0f;
        public float yOffset = 0f;
        public Cloud(float yOffset)
        {
            float randFloat = System.Random.Shared.Next(100) / 100f;
            this.CloudColor = Color.Lerp(Color.Black, Color.White, randFloat * 0.4f + 0.3f);
            this.offset = new Vector2(0, yOffset);
            iterator = System.Random.Shared.Next(360) / 360f * MathHelper.TwoPi;
            iterateSpeed = System.Random.Shared.Next(360) / 360f;
            this.yOffset = yOffset;
        }
        public void Update()
        {
            iterator += MathHelper.ToRadians(iterateSpeed);
            offset.X = MathHelper.Lerp(offset.X, 0, 0.04f);
            offset += new Vector2(1.5f * (float)Math.Sin(iterator), 0);
        }
    }
    public static class Utils
    {
        public static float ToRotation(this Vector2 v)
        {
            return (float)Math.Atan2(v.Y, v.X);
        }
        public static Vector2 RotatedBy(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
        {
            float xMult = (float)Math.Cos(radians);
            float yMult = (float)Math.Sin(radians);
            Vector2 vector = spinningpoint - center;
            Vector2 result = center;
            result.X += vector.X * xMult - vector.Y * yMult;
            result.Y += vector.X * yMult + vector.Y * xMult;
            return result;
        }
        public static Vector2 SafeNormalize(this Vector2 v, Vector2 defaultValue = default(Vector2))
        {
            if (v == Vector2.Zero)
            {
                return defaultValue;
            }
            return Vector2.Normalize(v);
        }
        /// <summary>
        /// Returns the vector with X and Y parameters converted to integers through casting (truncated)
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 ToVectorPoint(this Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }
        public static void DrawStringWithBorder(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, Color borderColor, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layer, bool reverse, int borderSize = 8)
        {
            spriteBatch.DrawStringWithBorder(spriteFont, text, position, color, borderColor, rotation, origin, new Vector2(scale, scale), spriteEffects, layer, reverse, borderSize);
        }
        public static void DrawStringWithBorder(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, Color borderColor, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layer, bool reverse, int borderSize = 8)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 circular = new Vector2(borderSize, 0).RotatedBy(i * MathHelper.PiOver4);
                circular.X *= scale.X;
                circular.Y *= scale.Y;
                spriteBatch.DrawString(spriteFont, text, position + circular, borderColor, rotation, origin, scale, spriteEffects, layer, reverse);
            }
            spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, spriteEffects, layer, reverse);
        }
    }
}