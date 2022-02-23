using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static TimeSpan TempoRestante = new TimeSpan(0, 0, 0); // tempo selcionado pelo usuario
        public static TimeSpan TempoDecorrido = new TimeSpan(0, 0, 0);
        public static TimeSpan Intervalo = new TimeSpan(0, 0, 1);
        public static TimeSpan TempoEsgotado = new TimeSpan(0, 0, 0);
        public static Double ToquesPorMinuto = 0;
        public static int NumErros = 0;
        public static int NumToquesValidos = 0;
        public static String NomeArqTexto;
        public static Boolean EhBack = false;
        //public static WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (richTextBox2.TextLength ==0) return;
            TempoRestante = TempoRestante.Subtract(Intervalo);
            TempoDecorrido = TempoDecorrido.Add(Intervalo);            
            label5.Text = TempoDecorrido.ToString("hh'h, 'mm'min, 'ss's'");
            label6.Text = TempoRestante.ToString("hh'h, 'mm'min, 'ss's'");            
            label7.Text = richTextBox2.TextLength.ToString("00000") + " toques";
            NumToquesValidos =(richTextBox2.TextLength - NumErros);
            label9.Text = NumToquesValidos.ToString("0000") + " toques válidos";
            ToquesPorMinuto = (NumToquesValidos * 60) / TempoDecorrido.TotalSeconds;
            label1.Text = ToquesPorMinuto.ToString("00000") + " t/min";
            if (TempoRestante <= TempoEsgotado)
            {
                (new SoundPlayer("Bell.wav")).Play();
                timer1.Enabled = false;
                MessageBox.Show("           Resultado:   " + label9.Text + 
                                 ", com a velocidade de: " +
                                 label1.Text+" .", "Tempo esgotado !!!", MessageBoxButtons.OK);
                button4_Click(null, null); //Botão de stop
            }
        }
            
        private void button5_Click(object sender, EventArgs e)
        {          
            DialogResult result1;
            result1=openFileDialog1.ShowDialog();
            if (result1 == DialogResult.OK)
            {                
                NomeArqTexto = openFileDialog1.FileName;
                richTextBox1.LoadFile(NomeArqTexto, RichTextBoxStreamType.PlainText);
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Botão "play" (>)
            timer1.Enabled = true; // habilita o timer que foi carregado para o formulário e cujo intervalo foi definido em 1000 (um segundo)
            if (richTextBox2.TextLength == 0)
            {
                TempoRestante = new TimeSpan(Convert.ToInt32(numericUpDown2.Value),
                    Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown4.Value));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            TempoRestante = new TimeSpan(Convert.ToInt32(numericUpDown2.Value),
                    Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown4.Value));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Botão de STOP
            TempoRestante = new TimeSpan(Convert.ToInt32(numericUpDown2.Value),
                    Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown4.Value));
            TempoDecorrido = new TimeSpan(0, 0, 0);
            ToquesPorMinuto = 0;
            NumErros = 0;
            richTextBox2.Clear();
            //textBox2.Clear();
            label5.Text = "00h, 00min, 00s";
            label6.Text = TempoRestante.ToString("hh'h, 'mm'min, 'ss's'");
            label7.Text = "0000 toques";
            label1.Text = "0000 t/min";
            label8.Text = "0000 erros";
            label9.Text = "0000 toques";
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = SystemColors.MenuBar;
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox2.TextLength ==0) return ; //Se não tiver essa linha, dá erro (na linha 124)
            if (EhBack)
            {
                // Este flag (boolean) impede que, ao acionar o backspace, conte novamente
                // como erro as letras diferentes. 
                EhBack = false;
                return;
            }
            timer1.Enabled=true;
            richTextBox1.SelectionStart = richTextBox2.TextLength-1;
            richTextBox1.SelectionLength = 1;
            richTextBox2.SelectionStart = richTextBox2.TextLength-1;
            richTextBox2.SelectionLength = 1;
            if (richTextBox1.SelectedText != richTextBox2.SelectedText)
            {
                NumErros++;
                label8.Text = NumErros.ToString("0000") + " erros";
                //wplayer.URL = ".\\Drop.mp3";
                //wplayer.controls.play();
                (new SoundPlayer("Drop.wav")).Play();
                richTextBox2.SelectionColor = Color.Red;
            }
            else
            {
                richTextBox2.SelectionColor = Color.Black; 
            }
            richTextBox1.SelectionBackColor  = Color.LightGoldenrodYellow;
            richTextBox1.SelectionStart++;
            richTextBox2.SelectionStart++;
            
            richTextBox1.ScrollToCaret();
        }

        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (richTextBox1.SelectionStart > 0)
                {
                    e.Handled = true;  //Se não fizer isso, não dá para saber qual o caracter que foi apagado.
                    richTextBox1.SelectionStart--;
                    if (richTextBox2.SelectionStart > 0)
                    {
                        richTextBox2.SelectionStart--;
                    }
                    richTextBox2.SelectionLength = 1;
                    if (richTextBox1.SelectedText !=richTextBox2.SelectedText)
                    {        
                        //Esta função limpa o erro anterior e faz o contador decrescer
                        if (NumErros > 0)
                        {
                            NumErros--;
                        }
                        label8.Text = NumErros.ToString("0000") + " erros";
                    }
                    e.Handled = false; //Agora, já pode apagar o caracter, pois já foi feita a comparação  (no if anterior)
                    EhBack = true;
                    // Volta com a cor padrão (retira o amarelo de destaque do texto lido)
                    richTextBox1.SelectionBackColor = SystemColors.MenuBar;
                   
                }
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                NomeArqTexto = ".\\Exercicio 1.txt";
                richTextBox1.LoadFile(NomeArqTexto, RichTextBoxStreamType.PlainText);
                TempoRestante = new TimeSpan(Convert.ToInt32(numericUpDown2.Value),
                        Convert.ToInt32(numericUpDown3.Value), Convert.ToInt32(numericUpDown4.Value));
            }
            catch (Exception e1)
            {
                MessageBox.Show("Erro :" + e1.Message +"\n\r Escolha outro arquivo de texto. Clique no botão 'Texto' para isso");
            }
        }
    }
}
