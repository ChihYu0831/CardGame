using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardGame
{
    public partial class frmCardGame : Form
    {
        // 目前關卡的棋盤大小，例如 4、6、8
        private int gridSize = 4;

        // 存放每張牌的答案，例如 1,1,2,2,3,3...
        private List<int> cardValues = new List<int>();

        // 第一張被翻開的牌
        private Button firstCard = null;

        // 第二張被翻開的牌
        private Button secondCard = null;

        // 是否正在等待蓋牌，避免玩家亂點
        private bool isChecking = false;

        // 已成功配對的數量
        private int matchedPairs = 0;

        // 總配對數
        private int totalPairs = 0;

        // 翻牌次數
        private int moveCount = 0;

        // 遊戲秒數
        private int seconds = 0;

        // 用來延遲蓋牌的 Timer
        private Timer checkTimer = new Timer();

        public frmCardGame()
        {
            InitializeComponent();

            // 設定遊戲計時器
            gameTimer.Interval = 1000;
            gameTimer.Tick += gameTimer_Tick;

            // 設定檢查配對用的 Timer
            checkTimer.Interval = 800;
            checkTimer.Tick += checkTimer_Tick;

            lblStatus.Text = "請選擇關卡開始遊戲";

        }

        private void btnLevel1_Click(object sender, EventArgs e)
        {
            StartGame(4);
        }

        private void btnLevel2_Click(object sender, EventArgs e)
        {
            StartGame(6);
        }

        private void btnLevel3_Click(object sender, EventArgs e)
        {
            StartGame(8);
        }

        private void StartGame(int size)
        {
            gridSize = size;
            totalPairs = gridSize * gridSize / 2;
            matchedPairs = 0;
            moveCount = 0;
            seconds = 0;
            firstCard = null;
            secondCard = null;
            isChecking = false;

            gameTimer.Stop();
            checkTimer.Stop();

            CreateCards();
            UpdateStatus();

            gameTimer.Start();
        }

        private void CreateCards()
        {
            gamePanel.Controls.Clear();
            gamePanel.RowStyles.Clear();
            gamePanel.ColumnStyles.Clear();

            gamePanel.RowCount = gridSize;
            gamePanel.ColumnCount = gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                gamePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / gridSize));
                gamePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / gridSize));
            }

            GenerateCardValues();

            int index = 0;

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Button card = new Button();

                    card.Dock = DockStyle.Fill;
                    card.Margin = new Padding(5);
                    card.Font = new Font("Microsoft JhengHei", 16, FontStyle.Bold);
                    card.Text = "?";
                    card.Tag = cardValues[index];
                    card.BackColor = Color.LightSteelBlue;
                    card.Click += Card_Click;

                    gamePanel.Controls.Add(card, col, row);
                    index++;
                }
            }
        }

        private void GenerateCardValues()
        {
            cardValues.Clear();

            for (int i = 1; i <= totalPairs; i++)
            {
                cardValues.Add(i);
                cardValues.Add(i);
            }

            Random random = new Random();

            cardValues = cardValues.OrderBy(x => random.Next()).ToList();
        }

        private void Card_Click(object sender, EventArgs e)
        {
            if (isChecking)
                return;

            Button clickedCard = sender as Button;

            if (clickedCard == null)
                return;

            // 已經翻開或已經配對的牌不能再點
            if (clickedCard.Text != "?")
                return;

            FlipCard(clickedCard);

            if (firstCard == null)
            {
                firstCard = clickedCard;
            }
            else
            {
                secondCard = clickedCard;
                moveCount++;
                UpdateStatus();

                CheckMatch();
            }
        }

        private void FlipCard(Button card)
        {
            card.Text = card.Tag.ToString();
            card.BackColor = Color.White;
        }

        private void HideCard(Button card)
        {
            card.Text = "?";
            card.BackColor = Color.LightSteelBlue;
        }

        private void CheckMatch()
        {
            isChecking = true;

            int firstValue = Convert.ToInt32(firstCard.Tag);
            int secondValue = Convert.ToInt32(secondCard.Tag);

            if (firstValue == secondValue)
            {
                firstCard.BackColor = Color.LightGreen;
                secondCard.BackColor = Color.LightGreen;

                firstCard.Enabled = false;
                secondCard.Enabled = false;

                matchedPairs++;

                firstCard = null;
                secondCard = null;
                isChecking = false;

                UpdateStatus();

                if (matchedPairs == totalPairs)
                {
                    gameTimer.Stop();

                    MessageBox.Show(
                        "恭喜過關！\n" +
                        "花費時間：" + seconds + " 秒\n" +
                        "翻牌次數：" + moveCount + " 次",
                        "遊戲完成",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else
            {
                checkTimer.Start();
            }
        }

        private void checkTimer_Tick(object sender, EventArgs e)
        {
            checkTimer.Stop();

            HideCard(firstCard);
            HideCard(secondCard);

            firstCard = null;
            secondCard = null;
            isChecking = false;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            seconds++;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            lblStatus.Text =
                "關卡：" + gridSize + "x" + gridSize +
                "    時間：" + seconds + " 秒" +
                "    翻牌次數：" + moveCount +
                "    配對：" + matchedPairs + "/" + totalPairs;
        }
    }
}
