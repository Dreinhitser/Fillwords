using System.Drawing.Design;

namespace Fillwords
{
    public partial class Form1 : Form
    {
        readonly string path = "database.txt";
        char[,] lettersMatrix;
        int[,] numsMatrix;
        int tableSizeX = 5;
        int tableSizeY = 4;
        List<Label> letters;
        List<Label> markedLetters;

        bool isReset = true;
        int sequenceSum = 0;

        Color currentColor = Color.Green;

        public Form1()
        {
            InitializeComponent();

            tableLayoutPanel1.RowCount = tableSizeY;
            tableLayoutPanel1.ColumnCount = tableSizeX;

            letters = new List<Label>();
            markedLetters = new List<Label>();
        }

        public void ChangeColor()
        {
            Random rand = new Random();
            currentColor = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));        
        }
        public void ParseName(string name, out int wordNumber, out int letterNumber)
        {
            string[] strings = name.Split('_');
            wordNumber = Convert.ToInt32(strings[1]);
            letterNumber = Convert.ToInt32(strings[2]);
        }

        public void ResetLetters(Label label)
        {
            foreach (var item in markedLetters)
            {
                item.BackColor = Color.White;

            }
            isReset = true;
            sequenceSum = 0;
            markedLetters.Clear();
        }


        public bool IsLetterLast(Label label)
        {
            ParseName(label.Name, out int wordNumber, out int letterNumber);

            foreach (var item in letters)
            {
                ParseName(item.Name, out int wNumber, out int lNumber);

                if (wNumber == wordNumber && lNumber > letterNumber)
                {  
                    return false;
                }
            }

            return true;
        }

        public int GetWordLength(Label label)
        {
            ParseName(label.Name, out int wordNumber, out int _);
            int size = 0;

            foreach (var item in letters)
            {
                ParseName(item.Name, out int wNumber, out int _);

                if (wNumber == wordNumber)
                {
                    size++;
                }
            }

            return size;
        }

        public bool isInSingleWord()
        {
            for (int i = 0; i < markedLetters.Count; i++)
            {
                ParseName(markedLetters[i].Name, out int wNumber1, out int lNumber1);
                for (int j = i; j < markedLetters.Count; j++)
                {
                    ParseName(markedLetters[j].Name, out int wNumber2, out int lNumber2);

                    if (numsMatrix[wNumber1, lNumber1] != numsMatrix[wNumber2, lNumber2])
                        return false;
                }
            }
            return true;
        }

        public void Letter_Click(object? sender, EventArgs e)
        {
            Label? label = sender as Label;

            if (label == null)
                return;

            ParseName(label.Name, out int wordNumber, out int letterNumber);

            richTextBox1.Text += label.Text + ": " + wordNumber + " " + letterNumber + "\n";
            textBoxWord.Text = isReset.ToString();

            sequenceSum++;

            if (isReset)
            {
                label.BackColor = currentColor;
                markedLetters.Add(label);
            }
            else if (isInSingleWord())
            {
                label.BackColor = currentColor;
                markedLetters.Add(label);

                if (IsLetterLast(label) && sequenceSum == GetWordLength(label))
                {
                    isReset = true;
                    sequenceSum = 0;
                    markedLetters.Clear();
                    ChangeColor();
                }
                else if (IsLetterLast(label) && sequenceSum != GetWordLength(label))
                {
                    richTextBox1.Text += IsLetterLast(label).ToString();
                    ResetLetters(label);
                }
            }
            else
            {
                ResetLetters(label);
            }
        }

        public void InitTable()
        {
            for (int i = 0; i < tableSizeY; i++)
            {
                for (int j = 0; j < tableSizeX; j++)
                {
                    Label label = new Label();
                    label.AutoSize = true;
                    label.Name = $"{lettersMatrix[i, j]}_{i}_{j}";
                    label.Text = lettersMatrix[i, j].ToString();
                    label.Click += new EventHandler(Letter_Click);
                    letters.Add(label);
                    tableLayoutPanel1.Controls.Add(label);
                }
            }
        }

        public void ReadTable()
        {
            StreamReader reader = new StreamReader(path);
            tableSizeX = Convert.ToInt32(reader.ReadLine());
            tableSizeY = Convert.ToInt32(reader.ReadLine());

            lettersMatrix = new char[tableSizeY, tableSizeX];
            numsMatrix = new int[tableSizeY, tableSizeX];
            for (int i = 0; i < tableSizeY; i++)
            {
                string line = reader.ReadLine();
                for (int j = 0; j < tableSizeX; j++)
                {
                    lettersMatrix[i, j] = line[j];
                }
            }

            for (int i = 0; i < tableSizeY; i++)
            {
                string line = reader.ReadLine();
                for (int j = 0; j < tableSizeX; j++)
                {
                    numsMatrix[i, j] = Convert.ToInt32(line[j]);
                }
            }
            reader.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadTable();
            InitTable();
            tableLayoutPanel1.Visible = true;
        }
    }
}