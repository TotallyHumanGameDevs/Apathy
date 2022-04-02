using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace Apathy
{
    public partial class MainWindow : Window
    {
        private readonly string directoryString = "G:\\Nickolas>";
        private Input input = new Input();
        private bool talking = false;

        public MainWindow()
        {
            InitializeComponent();
            input.currentRoom = input.shed;
            input.mW = this;
            MWTextBox.Text = directoryString;
            MWTextBox.CaretIndex = directoryString.Length;
            MWTextBox.Focus();

            MWTextBlock.Inlines.Add("\n\n");
            GuideTalk("99*Welcome, player." +
                "\nYou’ll be playing as the gravedigger and will be doing everything by inputting writen commands." +
                "\nYour goal is to complete the ritual and become an unstoppable necromancer, a wizard with the power to raise the dead, and conquer the world.2" +
                "\nSome tips:9" +
                "\n   One:4 You can get a more in-depth description of objects by examining them." +
                "\n   Two:4 The gravedigger is quite old, so be careful with him." +
                "\nThe first step is to dig a grave in the garden outside.2" +
                "\nYou have three minutes.", 38);
        }

        bool interurrupted = false;

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((MWTextBox.CaretIndex == directoryString.Length && (e.Key == Key.Back || e.Key == Key.Left)) ||
                interurrupted)
                e.Handled = true;
            else
                base.OnPreviewKeyDown(e);

            MWScrollViewer.ScrollToBottom();

            if (talking && !interurrupted)
            {
                MWTextBlock.Inlines.Add("-");

                interurrupted = true;
                talking = false;
                textTimer.Stop();
                //  120
                //GuideTalk("9\n*Interupting1 is quite 2rude.", 5);
                GuideTalk("9\n*Hi.", 5);
            }

            if (e.Key == Key.Enter)
            {
                if (!interurrupted && !pausing)
                {
                    input.PlayerInput(MWTextBox.Text.Remove(0, directoryString.Length));
                    MWTextBlock.Inlines.Add("\n" + MWTextBox.Text);
                    MWTextBox.Text = directoryString;
                    MWTextBox.CaretIndex = directoryString.Length;
                }
            }
        }

        List<char> phrase;
        int ticks;
        string previousChar;
        DispatcherTimer textTimer;
        bool italic;
        bool bold;
        bool linebreak;
        bool pausing;
        public void GuideTalk(string _string, int tickSpeed)
        {
            ticks = 0;
            previousChar = " ";
            italic = false;
            bold = false;
            linebreak = false;
            pausing = true;

            phrase = _string.ToCharArray().ToList();
            textTimer = new DispatcherTimer();
            textTimer.Tick += new EventHandler(TextTimer_Tick);
            textTimer.Interval = new TimeSpan(0, 0, 0, 0, tickSpeed);
            textTimer.Start();
        }
        public void TextTimer_Tick(object sender, EventArgs e)
        {
            if (phrase[0].ToString() == "\n")
            {
                linebreak = true;
                phrase.RemoveAt(0);
            }
            //  CHECKING FOR PUNCTUATION
            if ((ticks != 8 && previousChar == ".") ||
                (ticks != 4 && previousChar == ",") ||
                (ticks != 10 && ((previousChar == "?" || previousChar == "!") && (phrase[0].ToString() != "?" && phrase[0].ToString() != "!"))))
            {
                ticks++;
                return;
            }
            if (previousChar == "." || previousChar == "," || previousChar == "?" || previousChar == "!")
            {
                previousChar = "";
                ticks = 0;
            }
            //  CHECKING FOR NUMBERS
            bool isNum = int.TryParse(phrase[0].ToString(), out int num);
            if (ticks != num && isNum)
            {
                ticks++;
                return;
            }
            ticks = 0;
            if (isNum)
                phrase.RemoveAt(0);
            //  CHECKING FOR FONT STUFF
            if (phrase[0].ToString() == "*")
            {
                if (phrase[1].ToString() == "*")
                {
                    if (phrase[2].ToString() == "*")
                    {
                        italic = !italic;
                        bold = !bold;
                        phrase.RemoveRange(0, 3);
                    }
                    else
                    {
                        bold = !bold;
                        phrase.RemoveRange(0, 2);
                    }
                }
                else
                {
                    italic = !italic;
                    phrase.RemoveAt(0);
                }
            }
            //  CHECKING FOR NUMBERS AFTER THE STUFF ALREADY CHECKED
            isNum = int.TryParse(phrase[0].ToString(), out num);
            if (isNum)
            {
                ticks++;
                return;
            }
            ticks = 0;

            if (linebreak)
            {
                MWTextBlock.Inlines.Add("\n");
                linebreak = false;
            }

            //  ACTUAL TALKING
            talking = true;
            pausing = false;
            MWRectangle.Height = 15.2;

            MWTextBlock.Inlines.Add(new Run(phrase[0].ToString())
            {
                FontStyle = (italic ? FontStyles.Italic : FontStyles.Normal),
                FontWeight = (bold ? FontWeights.Bold : FontWeights.Normal)
            });
            previousChar = phrase[0].ToString();
            phrase.RemoveAt(0);

            if (phrase.Count == 0)
            {
                interurrupted = false;
                talking = false;
                textTimer.Stop();
                MWRectangle.Height = 0;
                MWTextBlock.Inlines.Add("\n");
            }
        }
    }
}