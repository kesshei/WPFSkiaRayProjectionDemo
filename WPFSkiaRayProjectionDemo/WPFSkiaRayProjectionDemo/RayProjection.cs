using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFSkiaRayProjectionDemo
{
    /// <summary>
    /// 光线投影法碰撞检测
    /// 投篮小游戏
    /// </summary>
    public class RayProjection
    {
        public SKPoint centerPoint;
        public double G = 0.3;
        public double F = 0.98;
        public double Easing = 0.03;
        public bool IsMoving = false;
        public SKPoint CurrentMousePoint = SKPoint.Empty;
        public SKPoint lastPoint = SKPoint.Empty;
        public Rect Box;
        public Ball Ball;
        public SKCanvas canvas;
        public int ALLCount = 10;
        public List<bool> bools = new List<bool>();
        public bool IsOver = false;
        /// <summary>
        /// 渲染
        /// </summary>
        public void Render(SKCanvas canvas, SKTypeface Font, int Width, int Height)
        {
            canvas.Clear(SKColors.White);
            this.canvas = canvas;
            centerPoint = new SKPoint(Width / 2, Height / 2);
            //球
            if (Ball == null)
            {
                Ball = new Ball()
                {
                    X = 50,
                    Y = Height - 50,
                    Radius = 30
                };
            }
            //箱子
            var boxX = Width - 170;
            var boxY = Height - 80;
            if (Box.X == 0)
            {
                Box = new Rect(boxX, boxY, 120, 70);
            }
            else
            {
                if (Box.X != boxX && Box.Y != boxY)
                {
                    Box.X = boxX;
                    Box.Y = boxY;
                }
            }

            if (bools.Count >= ALLCount)
            {
                IsOver = true;
            }

            if (!IsOver)
            {
                if (IsMoving)
                {
                    BallMove(Width, Height);
                }
                else
                {
                    DrawLine();
                }

                //弹球
                DrawCircle(canvas, Ball);
                //矩形
                DrawRect(canvas, Box);

                //计分
                using var paint1 = new SKPaint
                {
                    Color = SKColors.Blue,
                    IsAntialias = true,
                    Typeface = Font,
                    TextSize = 24
                };
                string count = $"总次数:{ALLCount} 剩余次数:{ALLCount - bools.Count} 投中次数:{bools.Count(t => t)}";
                canvas.DrawText(count, 100, 20, paint1);
            }
            else
            {
                SKColor sKColor = SKColors.Blue;
                //计分
                var SuccessCount = bools.Count(t => t);
                string count = "";
                switch (SuccessCount)
                {
                    case 0:
                        {
                            count = $"太糗了吧，一个都没投中！";
                            sKColor = SKColors.Black;
                        }
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        {
                            count = $"你才投中:{SuccessCount}次，继续努力！";
                            sKColor = SKColors.Blue;
                        }
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        {
                            count = $"恭喜 投中:{SuccessCount}次!!!";
                            sKColor = SKColors.YellowGreen;
                        }
                        break;
                    case 10: { count = $"全部投中，你太厉害了!";
                            sKColor = SKColors.Red;
                        } break;
                }
                using var paint1 = new SKPaint
                {
                    Color = sKColor,
                    IsAntialias = true,
                    Typeface = Font,
                    TextSize = 48
                };
                var fontCenter = paint1.MeasureText(count);
                canvas.DrawText(count, centerPoint.X - fontCenter / 2, centerPoint.Y, paint1);
            }
            using var paint = new SKPaint
            {
                Color = SKColors.Blue,
                IsAntialias = true,
                Typeface = Font,
                TextSize = 24
            };
            string by = $"by 蓝创精英团队";
            canvas.DrawText(by, 600, 20, paint);
        }
        /// <summary>
        /// 画一个圆
        /// </summary>
        public void DrawCircle(SKCanvas canvas, Ball ball)
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Blue,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                StrokeWidth = 2
            };
            canvas.DrawCircle((float)ball.X, (float)ball.Y, ball.Radius, paint);
        }
        /// <summary>
        /// 画一个矩形
        /// </summary>
        public void DrawRect(SKCanvas canvas, Rect box)
        {
            using var paint = new SKPaint
            {
                Color = SKColors.Green,
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                StrokeWidth = 2
            };
            canvas.DrawRect((float)box.X, (float)box.Y, (float)box.Width, (float)box.Height, paint);
        }
        /// <summary>
        /// 划线
        /// </summary>
        public void DrawLine()
        {
            //划线
            using var LinePaint = new SKPaint
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 2,
                IsStroke = true,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            };
            var path = new SKPath();
            path.MoveTo((float)CurrentMousePoint.X, (float)CurrentMousePoint.Y);
            path.LineTo((float)Ball.X, (float)Ball.Y);
            path.Close();
            canvas.DrawPath(path, LinePaint);
        }
        public void BallMove(int Width, int Height)
        {
            Ball.VX *= F;
            Ball.VY *= F;
            Ball.VY += G;

            Ball.X += Ball.VX;
            Ball.Y += Ball.VY;

            var hit = CheckHit();
            // 边界处理和碰撞检测
            if (hit || Ball.X - Ball.Radius > Width || Ball.X + Ball.Radius < 0 || Ball.Y - Ball.Radius > Height || Ball.Y + Ball.Radius < 0)
            {
                bools.Add(hit);
                IsMoving = false;
                Ball.X = 50;
                Ball.Y = Height - 50;
            }

            lastPoint.X = (float)Ball.X;
            lastPoint.Y = (float)Ball.Y;
        }
        public bool CheckHit()
        {
            var k1 = (Ball.Y - lastPoint.Y) / (Ball.X - lastPoint.X);
            var b1 = lastPoint.Y - k1 * lastPoint.X;
            var k2 = 0;
            var b2 = Ball.Y;
            var cx = (b2 - b1) / (k1 - k2);
            var cy = k1 * cx + b1;
            if (cx - Ball.Radius / 2 > Box.X && cx + Ball.Radius / 2 < Box.X + Box.Width && Ball.Y - Ball.Radius > Box.Y)
            {
                return true;
            }
            return false;
        }
        public void MouseMove(SKPoint sKPoint)
        {
            CurrentMousePoint = sKPoint;
        }
        public void MouseDown(SKPoint sKPoint)
        {
            CurrentMousePoint = sKPoint;
        }
        public void MouseUp(SKPoint sKPoint)
        {
            if (bools.Count < ALLCount)
            {
                IsMoving = true;
                Ball.VX = (sKPoint.X - Ball.X) * Easing;
                Ball.VY = (sKPoint.Y - Ball.Y) * Easing;
                lastPoint.X = (float)Ball.X;
                lastPoint.Y = (float)Ball.Y;
            }
        }
    }
}
