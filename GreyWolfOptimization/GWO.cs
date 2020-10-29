using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GreyWolfOptimization
{
    class GWO
    {
        Random r = new Random();
        public Wolf alpha_wolf; // leader
        public Wolf beta_wolf;
        public Wolf gamma_wolf;
        public List<Wolf> wolves;

        public int max_iteration;
        public int count_position;
        public int number_of_population;
        public float upper_bound, lower_bound;

        public int step;

        public GWO(int max_iteration, int number_of_population,  float lower_bound, float upper_bound)
        {
            this.max_iteration = max_iteration;
            this.number_of_population = number_of_population;
            this.upper_bound = upper_bound;
            this.lower_bound = lower_bound;
            this.count_position = 2;

            this.step = 0;
            generatePopulation();
        }

        public float GetRandomNumber(float minimum, float maximum)
        {
            return float.Parse(r.NextDouble() + "") * (maximum - minimum) + minimum;
        }

        public void generatePopulation() {
            wolves = new List<Wolf>();
            for (int i = 0; i < number_of_population; i++){ // 0.0 - 1.0
                
                float[] position = new float[] { (float)Math.Round(GetRandomNumber(lower_bound, upper_bound), 3), (float)Math.Round(GetRandomNumber(lower_bound, upper_bound), 3) };
                wolves.Add(new Wolf(position, i + 1));
            }
        }

        public float distance(Wolf current_wolf, Wolf leader_wolf, int index)
        {
            return (float)Math.Round(2 * float.Parse(r.NextDouble() + "") * leader_wolf.position[index] - current_wolf.position[index],3);
        }

        public Wolf solve()
        {
            //stopping condition : max_iteration, konvergen, semua nilainya sama
            for (int i = 0; i < max_iteration; i++)
            {
                Console.Write($"Iterasi {i+1}");
                wolves = wolves.OrderByDescending(wolf =>  wolf.score).ToList();
                alpha_wolf = new Wolf(wolves[0].position, wolves[0].id);
                beta_wolf = new Wolf(wolves[1].position, wolves[1].id);
                gamma_wolf = new Wolf(wolves[2].position, wolves[2].id);


                foreach (Wolf wolf in wolves)
                {
                    for (int j = 0; j < count_position; j++)
                    {
                        float d_alpha = distance(wolf, alpha_wolf, j);
                        float d_beta = distance(wolf, beta_wolf, j);
                        float d_gamma = distance(wolf, gamma_wolf, j);
                        // a nya setiap iterasi bakal nambah kecil
                        // 
                        float a = 2 - (2 * (i / max_iteration));
                        float A = 2 * a * float.Parse(r.NextDouble()+"") - a;
                        float x_alpha = (float)Math.Round(alpha_wolf.position[j] - A * d_alpha,3);
                        float x_beta = (float)Math.Round(beta_wolf.position[j] - A * d_beta,3);
                        float x_gamma = (float)Math.Round(gamma_wolf.position[j] - A * d_gamma,3);

                        wolf.position[j] = (float)Math.Round((x_alpha + x_beta + x_gamma) / 3,3);
                        wolf.position[j] = wolf.position[j] > upper_bound ? upper_bound : wolf.position[j] < lower_bound ? lower_bound : wolf.position[j];
                    }
                    wolf.score = wolf.fitness();
                }
            }
            wolves = wolves.OrderByDescending(wolf => wolf.score).ToList();
            alpha_wolf = new Wolf(wolves[0].position, wolves[0].id);
            beta_wolf = new Wolf(wolves[1].position, wolves[1].id);
            gamma_wolf = new Wolf(wolves[2].position, wolves[2].id);
            return alpha_wolf;
        }

        public bool solveStep() {
            if (step >= max_iteration)
                return true;
            Console.Write($"Iterasi {step + 1}");
            wolves = wolves.OrderByDescending(wolf => wolf.score).ToList();
            alpha_wolf = new Wolf(wolves[0].position, wolves[0].id);
            beta_wolf = new Wolf(wolves[1].position, wolves[1].id);
            gamma_wolf = new Wolf(wolves[2].position, wolves[2].id);
            foreach (Wolf wolf in wolves)
            {
                wolf.prevPosition = new float[] { (float)Math.Round(wolf.position[0],3), (float)Math.Round(wolf.position[1],3) };
                for (int j = 0; j < count_position; j++)
                {
                    float d_alpha = distance(wolf, alpha_wolf, j);
                    float d_beta = distance(wolf, beta_wolf, j);
                    float d_gamma = distance(wolf, gamma_wolf, j);

                    float a = 2 - (2 * (step / max_iteration));
                    float A = 2 * a * float.Parse(r.NextDouble() + "") - a;
                    float x_alpha = alpha_wolf.position[j] - A * d_alpha;
                    float x_beta = beta_wolf.position[j] - A * d_beta;
                    float x_gamma = gamma_wolf.position[j] - A * d_gamma;

                    wolf.position[j] = (float)Math.Round((x_alpha + x_beta + x_gamma) / 3,3);
                    wolf.position[j] = wolf.position[j] > upper_bound ? upper_bound : wolf.position[j] < lower_bound ? lower_bound : wolf.position[j];
                }
                wolf.score = wolf.fitness();
            }
            step++;
            wolves = wolves.OrderByDescending(wolf => wolf.score).ToList();
            alpha_wolf = new Wolf(wolves[0].position, wolves[0].id);
            beta_wolf = new Wolf(wolves[1].position, wolves[1].id);
            gamma_wolf = new Wolf(wolves[2].position, wolves[2].id);
            return false;
        }
    }

    class Wolf
    {
        public float[] position; // [0]x sm [1]y
        public float[] prevPosition; // [0]x sm [1]y
        public float score;
        public int id;
        public int walk_ctr;
        public int walk_inc;
        public Wolf(float[] position, int id)
        {
            this.position = position;
            this.prevPosition = new float[] { position[0], position[1] };
            this.score = fitness();
            this.id = id;
            this.walk_ctr = 0;
            this.walk_inc = 1;
        }
        public float fitness() {
            return float.Parse(Math.Pow(1.5 - position[0] + position[0] * position[1],2) + Math.Pow(2.25 - position[0] + position[0] * Math.Pow(position[1],2),2) + Math.Pow((2.625 - position[0] + position[0] * Math.Pow(position[1],3)),2)+"");
        }
        public void walk() {
            walk_inc = walk_ctr == 2 ? -1 : walk_ctr == 0 ? 1 : walk_inc;
            walk_ctr += walk_inc;
        }

        public override string ToString()
        {
            return $"Wolf[{id}] => X : {position[0]}, Y : {position[1]}";
        }
    }
}
