using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreyWolfOptimization
{
    public partial class Form1 : Form
    {
        GWO GreyWolf;
        bool start;
        bool finish;
        int iteration;

        
        const string IMAGE_URI_WOLF = "wolf-sprite.png";
        const string IMAGE_URI_SHADOW = "shadow.png";
        const string IMAGE_URI_MAP = "map.jpg";
        const float WALK_SPEED = 0.03f;
        public Form1()
        {
            InitializeComponent();
            start = false;
            finish = true;
            next_step = false;
            
            
            
            //pictureBox1.BackColor = Color.Transparent;
            //pictureBox2.BackColor = Color.Transparent;
            //pictureBox3.BackColor = Color.Transparent;
        }
        
        public float euclidean_distance(float x1, float y1, float x2, float y2) {
            return float.Parse(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2))+"");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int max_iteration = int.Parse(numericUpDown1.Value+"");
            int number_of_population = int.Parse(numericUpDown2.Value+"");
            //float lower_bound = float.Parse(numericUpDown3.Value+"");
            //float upper_bound = float.Parse(numericUpDown4.Value+"");
            float upper_bound = 4.5f;
            float lower_bound = -4.5f;
            iteration = -1;
            GreyWolf = new GWO(max_iteration, number_of_population,lower_bound, upper_bound);
            start = finish = true;
            next_step = false;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (start){
                this.Invalidate();
                if (iteration != GreyWolf.step)
                {
                    iteration = GreyWolf.step;
                    //listBox1.Items.Clear();
                    listBox1.Items.Add($"Iterasi {iteration}");
                    foreach (Wolf wolf in GreyWolf.wolves.OrderBy(w => w.id))
                    {
                        listBox1.Items.Add(wolf.ToString());
                    }
                }
            }

            if (next_step && !finish){
                this.Invalidate();
                if (iteration != GreyWolf.step)
                {
                    iteration = GreyWolf.step;
                    //listBox1.Items.Clear();
                    listBox1.Items.Add($"Iterasi {iteration}");
                    foreach (Wolf wolf in GreyWolf.wolves.OrderBy(w => w.id))
                    {
                        listBox1.Items.Add(wolf.ToString());
                    }
                }
            }

          
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawLine(new Pen(Color.White), 0 + 20, 120 + 20, 240 + 20, 120 + 20); // x
            g.DrawLine(new Pen(Color.White), 120 + 20, 0 + 20, 120 + 20, 240 + 20); // y
            g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(360, 170, 40, 40));
            g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(360, 220, 40, 40));
            g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(360, 270, 40, 40));
            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(360, 165, 40, 40), new Rectangle(48 * 6, 0, 48, 48), GraphicsUnit.Pixel);
            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(360, 215, 40, 40), new Rectangle(48 * 3, 48 * 4, 48, 48), GraphicsUnit.Pixel);
            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(360, 265, 40, 40), new Rectangle(48 * 3, 0, 48, 48), GraphicsUnit.Pixel);
            if (start)
                if (finish)
                {
                    finish = false;
                    if (GreyWolf.solveStep())
                    {
                        timer1.Stop();
                        start = false;
                        GreyWolf.step = 0;
                        Wolf best_wolf = GreyWolf.alpha_wolf;
                        MessageBox.Show(best_wolf.position[0] + " - " + best_wolf.position[1] + " => " + best_wolf.fitness());
                    }
                }
                else
                {
                    int k = 0;
                    finish = true;
                    foreach (Wolf wolf in GreyWolf.wolves.OrderBy(w => w.prevPosition[1]))
                    {
                        k++;
                        //Console.Write($"Wolf[0] = {GreyWolf.wolves[0].position[0]} - {GreyWolf.wolves[0].position[1]}\n");
                        //Console.Write($"Wolf[0] = {GreyWolf.wolves[0].prevPosition[0]} - {GreyWolf.wolves[0].prevPosition[1]}\n");

                        g.DrawString("Iterasi " + (GreyWolf.step), new Font(new FontFamily("Arial"), 14, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), 0, 0);
                        g.DrawString($"{wolf.prevPosition[0]}, {wolf.prevPosition[1]}", new Font(new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), wolf.prevPosition[0] * 20 + 110, wolf.prevPosition[1] * 20 + 115);
                        float wolfx = wolf.prevPosition[0] * 20 + 120;
                        float wolfy = wolf.prevPosition[1] * 20 + 120;
                        g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(wolfx, wolfy + 5, 40, 40));
                        // g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(20 * -4.5f + 120, 20 * -4.5f + 120, 40, 40), new Rectangle(48 * 6, 0, 48, 48), GraphicsUnit.Pixel);

                        if (Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) <= 0.2 && Math.Round(Math.Abs(wolf.prevPosition[1] - wolf.position[1]), 1) <= 0.2)
                        {   // sudah mencapai posisi
                            wolf.prevPosition[0] = wolf.position[0];
                            wolf.prevPosition[1] = wolf.position[1];

                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6, 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 48 * 4, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            continue;
                        }
                        if (wolf.prevPosition[0] < wolf.position[0] && wolf.prevPosition[1] < wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                        {   // kanan bawah
                            wolf.prevPosition[0] += WALK_SPEED;
                            wolf.prevPosition[1] += WALK_SPEED;

                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                        }
                        else if (wolf.prevPosition[0] > wolf.position[0] && wolf.prevPosition[1] > wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                        {   // kiri atas
                            wolf.prevPosition[0] -= WALK_SPEED;
                            wolf.prevPosition[1] -= WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                        }
                        else if (wolf.prevPosition[0] < wolf.position[0] && wolf.prevPosition[1] > wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                        {   // kanan atas
                            wolf.prevPosition[0] += WALK_SPEED;
                            wolf.prevPosition[1] -= WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                        }
                        else if (wolf.prevPosition[0] > wolf.position[0] && wolf.prevPosition[1] < wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                        {   // kiri bawah
                            wolf.prevPosition[0] -= WALK_SPEED;
                            wolf.prevPosition[1] += WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                        }
                        else if (wolf.prevPosition[1] < wolf.position[1])
                        {
                            // bawah
                            wolf.prevPosition[1] += WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                            }

                        }
                        else if (wolf.prevPosition[1] > wolf.position[1])
                        {
                            // atas
                            wolf.prevPosition[1] -= WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 144, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                            }

                        }
                        else if (wolf.prevPosition[0] > wolf.position[0])
                        {
                            // kiri
                            wolf.prevPosition[0] -= WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                            }

                        }
                        else if (wolf.prevPosition[0] < wolf.position[0])
                        {
                            // kanan
                            wolf.prevPosition[0] += WALK_SPEED;
                            if (wolf.id == GreyWolf.alpha_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.beta_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else if (wolf.id == GreyWolf.gamma_wolf.id)
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                            }
                        }


                        wolf.prevPosition[0] = (float)Math.Round(wolf.prevPosition[0], 3);
                        wolf.prevPosition[1] = (float)Math.Round(wolf.prevPosition[1], 3);
                        wolf.walk();
                        Console.WriteLine("X : " + wolf.prevPosition[0] + " - " + wolf.position[0]);
                        Console.WriteLine("Y : " + wolf.prevPosition[1] + " - " + wolf.position[1]);


                        if (Math.Abs(wolf.prevPosition[0] - wolf.position[0]) > 0.3 || Math.Abs(wolf.prevPosition[1] - wolf.position[1]) > 0.3)
                            finish = false;
                    }
                }
            else if (next_step)
            {
                finish = true;
                foreach (Wolf wolf in GreyWolf.wolves.OrderBy(w => w.prevPosition[1]))
                {
                    if (Math.Abs(wolf.prevPosition[0] - wolf.position[0]) > 0.3 || Math.Abs(wolf.prevPosition[1] - wolf.position[1]) > 0.3)
                        finish = false;

                    g.DrawString("Iterasi " + (GreyWolf.step), new Font(new FontFamily("Arial"), 14, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), 0, 0);
                    g.DrawString($"{wolf.prevPosition[0]}, {wolf.prevPosition[1]}", new Font(new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), wolf.prevPosition[0] * 20 + 110, wolf.prevPosition[1] * 20 + 115);
                    float wolfx = wolf.prevPosition[0] * 20 + 120;
                    float wolfy = wolf.prevPosition[1] * 20 + 120;
                    g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(wolfx, wolfy + 5, 40, 40));

                    // draw
                    if (Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) <= 0.2 && Math.Round(Math.Abs(wolf.prevPosition[1] - wolf.position[1]), 1) <= 0.2)
                    {   // sudah mencapai posisi
                        wolf.prevPosition[0] = wolf.position[0];
                        wolf.prevPosition[1] = wolf.position[1];

                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 48 * 4, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        continue;
                    }
                    if (wolf.prevPosition[0] < wolf.position[0] && wolf.prevPosition[1] < wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                    {   // kanan bawah
                        wolf.prevPosition[0] += WALK_SPEED;
                        wolf.prevPosition[1] += WALK_SPEED;

                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                    }
                    else if (wolf.prevPosition[0] > wolf.position[0] && wolf.prevPosition[1] > wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                    {   // kiri atas
                        wolf.prevPosition[0] -= WALK_SPEED;
                        wolf.prevPosition[1] -= WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                    }
                    else if (wolf.prevPosition[0] < wolf.position[0] && wolf.prevPosition[1] > wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                    {   // kanan atas
                        wolf.prevPosition[0] += WALK_SPEED;
                        wolf.prevPosition[1] -= WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                    }
                    else if (wolf.prevPosition[0] > wolf.position[0] && wolf.prevPosition[1] < wolf.position[1] && Math.Round(Math.Abs(wolf.prevPosition[0] - wolf.position[0]), 1) > 0.2)
                    {   // kiri bawah
                        wolf.prevPosition[0] -= WALK_SPEED;
                        wolf.prevPosition[1] += WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                    }
                    else if (wolf.prevPosition[1] < wolf.position[1])
                    {
                        // bawah
                        wolf.prevPosition[1] += WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 0, 48, 48), GraphicsUnit.Pixel);
                        }

                    }
                    else if (wolf.prevPosition[1] > wolf.position[1])
                    {
                        // atas
                        wolf.prevPosition[1] -= WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 144, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 144, 48, 48), GraphicsUnit.Pixel);
                        }

                    }
                    else if (wolf.prevPosition[0] > wolf.position[0])
                    {
                        // kiri
                        wolf.prevPosition[0] -= WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 49, 48, 48), GraphicsUnit.Pixel);
                        }

                    }
                    else if (wolf.prevPosition[0] < wolf.position[0])
                    {
                        // kanan
                        wolf.prevPosition[0] += WALK_SPEED;
                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 48 * 4 + 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3 + wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(wolf.walk_ctr * 48 + wolf.walk_ctr, 97, 48, 48), GraphicsUnit.Pixel);
                        }
                    }

                    wolf.prevPosition[0] = (float)Math.Round(wolf.prevPosition[0], 3);
                    wolf.prevPosition[1] = (float)Math.Round(wolf.prevPosition[1], 3);
                    wolf.walk();
                }
                if (finish)
                {
                    // draw last 
                    foreach (Wolf wolf in GreyWolf.wolves.OrderBy(w => w.prevPosition[1]))
                    {
                        g.DrawString("Iterasi " + (GreyWolf.step), new Font(new FontFamily("Arial"), 14, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), 0, 0);
                        float wolfx = wolf.prevPosition[0] * 20 + 120;
                        float wolfy = wolf.prevPosition[1] * 20 + 120;
                        g.DrawImage(Image.FromFile(IMAGE_URI_SHADOW), new RectangleF(wolfx, wolfy + 5, 40, 40));

                        wolf.prevPosition[0] = wolf.position[0];
                        wolf.prevPosition[1] = wolf.position[1];
                        g.DrawString($"{wolf.prevPosition[0]}, {wolf.prevPosition[1]}", new Font(new FontFamily("Arial"), 10, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.White), wolf.prevPosition[0] * 20 + 110, wolf.prevPosition[1] * 20 + 115);

                        if (wolf.id == GreyWolf.alpha_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 6, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.beta_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 48 * 4, 48, 48), GraphicsUnit.Pixel);
                        }
                        else if (wolf.id == GreyWolf.gamma_wolf.id)
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(48 * 3, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            g.DrawImage(Image.FromFile(IMAGE_URI_WOLF), new RectangleF(wolfx, wolfy, 40, 40), new Rectangle(0, 0, 48, 48), GraphicsUnit.Pixel);
                        }
                    }
                    //
                    finish = false;
                    GreyWolf.solveStep();
                    next_step = false;
                    if (GreyWolf.step >= GreyWolf.max_iteration)
                    {
                        timer1.Stop();
                        start = false;
                        GreyWolf.step = 0;
                        Wolf best_wolf = GreyWolf.alpha_wolf;
                        MessageBox.Show(best_wolf.position[0] + " - " + best_wolf.position[1] + " => " + best_wolf.fitness());
                    }
                }
            }
            
        }

        bool next_step = false;
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            next_step = true;
            start = false;
            timer1.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled) {
                button3.Text = "Play";
                timer1.Stop();
                start = false;
            } else {
                button3.Text = "Pause";
                start = true;
                timer1.Start();
            }
        }
    }
}
