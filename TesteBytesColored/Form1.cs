using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DS4Windows;

namespace TesteBytesColored
{

    public partial class Form1 : Form
    {

        int index = 0;
        bool isPlaying = false;

        Color[] cor = new Color[4];
        PictureBox[] pbx = new PictureBox[4];


        List<char> sequence = new List<char>(); //this list control the sequence of displaying
        private DS4Device ds4;
        private byte leftStrenght = 100;
        private byte rightStrenght = 100;

        public Form1()
        {
            InitializeComponent();

            // The array of picture boxes will make easier to manage them
            pbx[0] = b;
            pbx[1] = c;
            pbx[2] = d;
            pbx[3] = e;


            cor[0] = Color.Green;
            cor[1] = Color.Blue;
            cor[2] = Color.Purple;
            cor[3] = Color.Yellow;
            DS4Devices.findControllers();
            ds4 = DS4Devices.getDS4Controllers().First();
            ds4.StartUpdate();
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            // pauses if playing
            if (isPlaying)
            {
                isPlaying = false;
                update.Stop();
                SetActive(false);
            }
            else
            {
                //convert the texboxes values to designed types
                try
                {
                    isPlaying = true;

                    int tick = (int)Math.Floor(float.Parse(intervaloText.Text));
                    string tetxSequence = sequenceText.Text;

                    //verify if the string contains only letters
                    if (Regex.IsMatch(tetxSequence, @"^[a-zA-Z]+$"))
                    {

                        sequence.Clear();
                        foreach (char letter in tetxSequence)
                        {
                            sequence.Add(letter);
                        }

                        update.Interval = tick * 1000;
                        update.Start();
                    }
                    else
                        MessageBox.Show("A sequência deve conter apenas letras.");

                }
                catch (Exception exe)
                {
                    MessageBox.Show($"Não foi possivel definir o timer:\n{exe.Message}", "ERRO!");
                    isPlaying = false;
                }
            }
        }

        private void Update_Tick(object sender, EventArgs e)
        {
            if (index < sequence.Count)
            {
                var atual = sequence[index];

                //converts the char to a number
                int number = ((int)atual) - 64;
                //this is the logic who activates all the respective boxes according with table of Aa
                switch (number)
                {
                    case 1: // A - All off
                        SetActive(false);
                        break;
                    case 2: // Green - B
                        SetActive(0);
                        break;
                    case 3: // Blue - C
                        SetActive(1);
                        break;
                    case 4: // Purple - D
                        SetActive(2);
                        break;
                    case 5: // Yellow - E
                        SetActive(3);
                        break;
                    case 26: // All on - Z
                        SetActive(true);
                        break;
                    case 33: // B + C + D = a
                        SetActive(true, true, true, false);
                        break;
                    case 34: // B + C + E = b
                        SetActive(true, true, false, true);
                        break;
                    case 35: // B + D + E = c
                        SetActive(true, false, true, true);
                        break;
                    case 36: // C + D + E = d
                        SetActive(false, true, true, true);
                        break;
                    case 37: // B + C = e
                        SetActive(true, true, false, false);
                        break;
                    case 38: // B + D = f
                        SetActive(true, false, true, false);
                        break;
                    case 39: // B + E = g
                        SetActive(true, false, false, true);
                        break;
                    case 40: // C + D = h
                        SetActive(false, true, true, false);
                        break;
                    case 41: // C + E = i
                        SetActive(false, true, false, true);
                        break;
                    case 42: // D + E = j
                        SetActive(false, false, true, true);
                        break;
                    default:
                        SetActive(false);
                        break;
                }
                index++;

            }
            else
            {
                SetActive(false);
                update.Stop();
                isPlaying = false;
                index = 0;
                ds4.setRumble(0, 0);

            }



        }

        void SetActive(bool active)
        {

            if (active) // All 4 is active
            {
                for (int i = 0; i < pbx.Length; i++)
                {
                    pbx[i].BackColor = cor[i];
                }
                ds4.LightBarColor = new DS4Color(Color.White);
                ds4.setRumble(255, 255);

            }
            else // No one is active
            {
                ds4.LightBarColor = new DS4Color(Color.Black);
                ds4.setRumble(0, 0);

                foreach (var i in pbx)
                {
                    i.BackColor = Color.White;
                }

            }
        }

        void SetActive(int ind) //Activate only the box wich this id
        {
            ds4.setRumble(0, 0);
            SetActive(false);
            pbx[ind].BackColor = cor[ind];
            ds4.LightBarColor = new DS4Color(cor[ind]);

        }

        void SetActive(bool verde, bool azul, bool pink, bool amarelo) //Activate only those who is setted to true
        {
            SetActive(false);
            ds4.setRumble(rightStrenght, leftStrenght);

            List<Color> colors = new List<Color>();
            if (verde)
            {
                pbx[0].BackColor = cor[0];
                colors.Add(cor[0]);
            }
            if (azul)
            {
                pbx[1].BackColor = cor[1];
                colors.Add(cor[1]);

            }
            if (pink)
            {
                pbx[2].BackColor = cor[2];
                colors.Add(cor[2]);

            }
            if (amarelo)
            {
                colors.Add(cor[3]);
                pbx[3].BackColor = cor[3];
            }


            GetMixedColor(out var c, colors.ToArray());
            ds4.LightBarColor = new DS4Color(c);
        }

        private void GetMixedColor(out Color result, params Color[] cores)
        {
            int[] rgb = new int[3];
            foreach (var item in cores)
            {
                rgb[0] += item.R;
                rgb[1] += item.G;
                rgb[2] += item.B;
            }
            var sz = cores.Length;
            ds4.setRumble((byte)(sz * 40), (byte)(sz * 40));
            result = Color.FromArgb(rgb[0] / sz, rgb[1] / sz, rgb[2] / sz);

        }
    }
}
