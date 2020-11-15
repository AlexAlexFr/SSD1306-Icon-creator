using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconMaker
{
    public partial class Form1 : Form
    {
        //Pixels displayed as PictureBoxes
        private PictureBox[] Pictures;
        //Labels to show HEX of each column in each page
        private Label[] lbHEX;
        //Labels to show in the left page number
        private Label[] lbPages;
        //Labels to show in the top number of coulmn
        private Label[] lbColumns;

        //Size of icon
        Size szIconSize;

        //size of eachpixel displayed
        Size szRectSize;
        enum DisplayModes
        {
            Horizontal,
            Vertical
        }

        DisplayModes dModes;

        int iPagesNb;

        Bitmap bmIcon, bmMedium;

        public Form1()
        {
            InitializeComponent();
            szIconSize.Width = (int)numericWidth.Value;
            szIconSize.Height = (int)numericHeight.Value;

            szIconSize = new Size(szIconSize.Width, szIconSize.Height);
            szRectSize = new Size(21, 21);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            CreateIcon();

            rbArray.Clear();
        }

        private void FillColumnsHeaders()
        {

            panel2.Controls.Clear();
            lbColumns = new Label[szIconSize.Width];

            for (int i = 0; i < szIconSize.Width; i++)
            {
                lbColumns[i] = new Label();
                lbColumns[i].Text = i.ToString("X2");
                lbColumns[i].BorderStyle = BorderStyle.FixedSingle;
                lbColumns[i].Location = new Point(20 + i * szRectSize.Width, 0);
                lbColumns[i].AutoSize = true;
                panel2.Controls.Add(lbColumns[i]);
            }
        }

        private void FillPagesNumber()
        {
            lbPages = new Label[iPagesNb];
            for (int i =0; i<iPagesNb;i++)
            {
                lbPages[i] = new Label();
                lbPages[i].Text = "P" + i.ToString();
                lbPages[i].AutoSize = true;
                lbPages[i].Location = new Point(0, (i*(8*szRectSize.Height +15)) + (8 * szRectSize.Height + 15)/2);
                panel1.Controls.Add(lbPages[i]);
            }
        }

        private void CreateIcon()
        {
            panel1.Visible = false;

            if (Pictures != null)
            {
                Pictures = null;
            }
            szIconSize.Width = (int)numericWidth.Value;
            szIconSize.Height = (int)numericHeight.Value;

            FillColumnsHeaders();

            iPagesNb = (int)Math.Ceiling((double)szIconSize.Height / 8);

            panel1.Controls.Clear();
            

            if (rbHorMode.Checked == true)
            {
                dModes = DisplayModes.Horizontal;
            }
            else
            {
                dModes = DisplayModes.Vertical;
            }
            int xPos, yPos;

            int iColNum, iPageNum;

            iColNum = 0;
            iPageNum = 0;

            Pictures = new PictureBox[szIconSize.Width * (iPagesNb) * 8 + 1];
            lbHEX = new Label[iPagesNb * szIconSize.Width];
            
            int iPixelNumber = 0;
            int iLblNumber = 0;

         

            while (iPixelNumber < szIconSize.Width * iPagesNb * 8)
            {
                xPos = 20 + iColNum * szRectSize.Width;
                yPos = iPageNum * ((szRectSize.Height) * 8 + 15);
                for (int iByte = 0; iByte < 8; iByte++)
                {
                    Pictures[iPixelNumber] = new PictureBox();

                    Pictures[iPixelNumber].BackColor = Color.Azure;
                    Pictures[iPixelNumber].Size = new Size(szRectSize.Width, szRectSize.Height);
                    Pictures[iPixelNumber].Location = new Point(xPos, yPos);
                    Pictures[iPixelNumber].Visible = true;
                    Pictures[iPixelNumber].BorderStyle = BorderStyle.FixedSingle;
                    Pictures[iPixelNumber].Click += new EventHandler(ImageClick);
                    Pictures[iPixelNumber].Tag = iPixelNumber;
                   
                    panel1.Controls.Add(Pictures[iPixelNumber]);

                    iPixelNumber++;

                    yPos = yPos + szRectSize.Height;
                }
                lbHEX[iLblNumber] = new Label();
                lbHEX[iLblNumber].Text = "00";
                lbHEX[iLblNumber].Location = new Point(xPos, yPos);
                lbHEX[iLblNumber].Visible = true;
                lbHEX[iLblNumber].BorderStyle = BorderStyle.FixedSingle;
                lbHEX[iLblNumber].AutoSize = true;
                
                panel1.Controls.Add(lbHEX[iLblNumber]);
                iLblNumber++;

                if (rbHorMode.Checked == true)
                {
                    iColNum++;
                    if (iColNum > szIconSize.Width - 1)
                    {
                        iColNum = 0;
                        iPageNum++;
                    }
                }
                else
                {
                    iPageNum++;
                    if (iPageNum > iPagesNb - 1)
                    {
                        iPageNum = 0;
                        iColNum++;
                    }
                }

            }

            FillPagesNumber();

            bmIcon = new Bitmap(szIconSize.Width, iPagesNb*8);
            bmMedium = new Bitmap(szIconSize.Width * 2, iPagesNb * 16);

            Graphics grMedium = Graphics.FromImage(bmMedium);

            Graphics grSmall = Graphics.FromImage(bmIcon);
            grSmall.FillRectangle(Brushes.Black, 0, 0, szIconSize.Width, iPagesNb * 8);
            grMedium.FillRectangle(Brushes.Black, 0, 0, szIconSize.Width * 2, iPagesNb * 16);

            pbIconSmall.Image = bmIcon;
           
            panel1.Visible = true;
            button2.Enabled = true;
        }

        private void ImageClick(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            int iPicIndex = (int)((PictureBox)sender).Tag;

            int x = 0, y = 0;
            int iPage;
            int iRemainder;
            iPage = (int)Math.Floor((double)iPicIndex / (8 * szIconSize.Width));

            if (dModes == DisplayModes.Horizontal)
            {
                x = (iPicIndex - (iPage * 8 * szIconSize.Width)) / 8;
                Math.DivRem(iPicIndex - (iPage * 8 * szIconSize.Width), 8, out iRemainder);

                y = iPage * 8 + iRemainder;
            }
            else
            {
                x = iPicIndex / (iPagesNb*8);
                Math.DivRem(iPicIndex, iPagesNb*8, out iRemainder);
                y = iRemainder;
            }

            
            Graphics gr = Graphics.FromImage(bmMedium);
            if (pb.BackColor == Color.Azure)
            {
                pb.BackColor = Color.Blue;
                bmIcon.SetPixel(x, y, Color.White);
                gr.FillRectangle(Brushes.White, x * 2, y * 2, 2, 2);
            }
            else
            {
                pb.BackColor = Color.Azure;
                bmIcon.SetPixel(x, y, Color.Black);
                gr.FillRectangle(Brushes.Black, x * 2, y * 2, 2, 2);
            }

            

            int iLblIndex = (int)Math.Floor((double)iPicIndex / 8);
            int cValue = 0;
            for (int c=0;c<8;c++)
            {
                if (Pictures[iLblIndex * 8 + c].BackColor == Color.Blue)
                {
                    cValue = cValue|(1<<c);
                }
            }

            lbHEX[iLblIndex].Text = cValue.ToString("X2");
           
            if(pbIconSmall.Image == bmIcon)
            {
                pbIconSmall.Image = bmIcon;
            }
            else
            {
                pbIconSmall.Image = bmMedium;
            }
                        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rbArray.Clear();
            rbArray.AppendText("{" + Environment.NewLine);

            int iLblIndex = 0;
            if (dModes == DisplayModes.Horizontal)
            {
                for (int c=0; c<iPagesNb; c++)
                {
                    for (int i = 0; i < szIconSize.Width; i++)
                    {
                        rbArray.AppendText("0x" + lbHEX[iLblIndex].Text + ", ");
                        iLblIndex++;
                    }
                    rbArray.AppendText(Environment.NewLine);
                }
            }
            else
            {
                for (int i = 0; i < szIconSize.Width; i++)
                {
                    for (int c = 0; c < iPagesNb; c++)
                    {
                        rbArray.AppendText("0x" + lbHEX[iLblIndex].Text + ", ");
                        iLblIndex++;
                    }
                    rbArray.AppendText(Environment.NewLine);
                }
            }

            rbArray.Text = rbArray.Text.Substring(0, rbArray.Text.Length - 3);
            rbArray.AppendText(Environment.NewLine+"};");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(rbHorMode, "In horizontal adressing mode, the column address is automatically increased by 1." + Environment.NewLine +
                "If the column address pointer reaches column end address, the column address pointer is reset to column start address " + Environment.NewLine +
                "and page address pointer is increased by 1. In HEX array the values will be following column increase first.");
            toolTip1.SetToolTip(rbVertMode, "In vertical addressing mode,the page address pointer is increased automatically by 1. " + Environment.NewLine +
                "If the page address pointer reaches the page end address, the page address pointer is reset to page start address and" + Environment.NewLine +
                " column address pointer is increased by 1. In HEX array the values will be following page increase first.");
            toolTip2.SetToolTip(button1, "Creates a new empty icon template. Icon width and icon height must be defined respectively.");
            CreateIcon();
            toolTip3.SetToolTip(button2, "Array created in order do SSD1306 mode which was chosen during icon creation!"+Environment.NewLine+ "Creates array of hex in format { 0x00, 0x7F, ..., 0x12 }. ");
        }

        private void pbIconSmall_MouseClick(object sender, MouseEventArgs e)
        {
            if(pbIconSmall.Image == bmIcon)
            {
                pbIconSmall.Image = bmMedium;
            }
            else
            {
                pbIconSmall.Image = bmIcon;
            }
        }

 
    }
}
