using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BlackJack.CardGameFramework;

namespace BlackJack
{
    public partial class BlackJackForm : Form
    {
        #region Fields

        //Creates a new blackjack game with one player and an inital balance set through the settings designer
        private BlackJackGame game;
        private PictureBox[] playerCards;
        private PictureBox[] tableCards;
        private List<Card> listOfCard;
        private BlackJackForm blackJackForm;

        public bool Folded { get; set; }

        public int  FormIndex { get; set; }

        #endregion

        #region Main Constructor

        /// <summary>
        /// Main constructor for the BlackJackForm.  Initializes components, loads the card skin images, and sets up the new game
        /// </summary>
        public BlackJackForm(BlackJackGame game)
        {
            Folded = false;
            this.game = game;
            
            listOfCard = new List<Card>();
            InitializeComponent();
            LoadCardSkinImages();
           

            //game.gameState = BlackJackGame.GameState.PREFLOP;
            //SetUpGameInPlay();
        }

        public BlackJackForm(BlackJackForm blackJackForm)
        {
            this.blackJackForm = blackJackForm;
        }

        #endregion

        #region Game Event Handlers

        /// <summary>
        /// Invoked when the deal button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DealBtn_Click(object sender, EventArgs e)
        {
            try
            {
                game.BetAmount += game.Players[FormIndex].Bet;
                // If the current bet is equal to 0, ask the player to place a bet
                if ((game.Players[FormIndex].Bet == 0) && (game.Players[FormIndex].Balance > 0))
                {
                    MessageBox.Show("You must place a bet before the dealer deals.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            
                else
                {
                   
                    // Place the bet
                    if (game.Players[FormIndex].Bet > game.MinBetAmount)
                        game.MinBetAmount = game.Players[FormIndex].Bet;
                    
                    game.Players[FormIndex].PlaceBet();


                    ShowBankValue();
                    
                    switch (game.gameState)
                    {
                        case BlackJackGame.GameState.PREFLOP:
                            //game.gameState = BlackJackGame.GameState.FLOP;

                            game.SetUpGameInPlay();
                            //MessageBox.Show(game.CurrentPlayer.Hand.GetValueOfHand(listOfCard).ToString());
                            break;

                        case BlackJackGame.GameState.FLOP:
                            //game.gameState = BlackJackGame.GameState.TURN;
                            game.SetUpGameInPlay();
                           // MessageBox.Show(game.CurrentPlayer.Hand.GetValueOfHand(listOfCard).ToString());
                            break;
                        case BlackJackGame.GameState.TURN:
                            //game.gameState = BlackJackGame.GameState.RIVER;
                            game.SetUpGameInPlay();
                           // game.CurrentPlayer.Hand.GetValueOfHand(listOfCard);
                            break;
                        case BlackJackGame.GameState.RIVER:
                            //game.gameState = BlackJackGame.GameState.RIVER;
                            game.SetUpGameInPlay();
                            // game.CurrentPlayer.Hand.GetValueOfHand(listOfCard);
                            break;
                            // MessageBox.Show(game.CurrentPlayer.Hand.GetValueOfHand(listOfCard).ToString());
                            //MessageBox.Show(game.CurrentPlayer.Hand.CompareTo(game.CpuPlayer.Hand).ToString());

                    }

                    //game.FormTurn++;




                    // Clear the table, set up the UI for playing a game, and deal a new game
                    //ClearTable();
                    //SetUpGameInPlay();
                    //game.DealNewGame();
                    //UpdateUIPlayerCards();

                        // Check see if the current player has blackjack

                }
            }
            catch (Exception NotEnoughMoneyException)
            {
                MessageBox.Show(NotEnoughMoneyException.StackTrace);
            }
        }

        /// <summary>
        /// Invoked when the player has finished their turn and clicked the stand button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldBtn_Click(object sender, EventArgs e)
        {
            
            game.TurnPlayerForm.RemoveAt(0);
            
            //game.FoldList.Add(FormIndex);
            game.Folded = true;
            LockControls();
            if (game.TurnPlayerForm.Count  == 0)
            {
                game.PlayerForms.RemoveAt(FormIndex);
                game.SetUpGameInPlay();
            }
            else
            {
                game.SetUpGameInPlay();
                game.PlayerForms.RemoveAt(FormIndex);
                 

            }
            game.FoldedPlayers.Add(FormIndex);
           // game.Players.RemoveAt(FormIndex);
            //game.SetUpGameInPlay();
        }
        public void LockControls()
        {
           
            dealButton.Enabled = false;     
            FoldButton.Enabled = false;
            clearBetButton.Enabled = false;
            tenButton.Enabled = false;
            twentyFiveButton.Enabled = false;
            fiftyButton.Enabled = false;
            hundredButton.Enabled = false;
            //ShowBankValue();
        }
        /// <summary>
        /// Invoked when the hit button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void HitBtn_Click(object sender, EventArgs e)
        //{
        //    // It is no longer the first turn, set this to false so that the cards will all be facing upwards
        //    firstTurn = false;
        //    // Hit once and update UI cards
        //    game.CurrentPlayer.Hit();
        //    UpdateUIPlayerCards();

        //    // Check to see if player has bust
        //    if (game.CurrentPlayer.HasBust())
        //    {
        //        EndGame(EndResult.PlayerBust);
        //    }
        //}

        /// <summary>
        /// Invoked when the double down button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void DblDwnBtn_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Double the player's bet amount
        //        game.CurrentPlayer.DoubleDown();
        //        UpdateUIPlayerCards();
        //        ShowBankValue();

        //        //Make sure that the player didn't bust
        //        if (game.CurrentPlayer.HasBust())
        //        {
        //            EndGame(EndResult.PlayerBust);
        //        }
        //        else
        //        {
        //            // Otherwise, let the dealer finish playing
        //            game.DealerPlay();
        //            UpdateUIPlayerCards();
        //            EndGame(GetGameResult());
        //        }
        //    }
        //    catch (Exception NotEnoughMoneyException)
        //    {
        //        MessageBox.Show(NotEnoughMoneyException.Message);
        //    }
        //}

        /// <summary>
        /// Exits the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Place a bet for 10 dollars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TenBtn_Click(object sender, EventArgs e)
        {
            Bet(10);
        }

        /// <summary>
        /// Place a bet for 25 dollars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwentyFiveBtn_Click(object sender, EventArgs e)
        {
            Bet(25);
        }

        /// <summary>
        /// Place a bet for 50 dollars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FiftyBtn_Click(object sender, EventArgs e)
        {
            Bet(50);
        }

        /// <summary>
        /// Place a bet for 100 dollars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HundredBtn_Click(object sender, EventArgs e)
        {
            Bet(100);
        }

        /// <summary>
        /// Clear the bet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearBetBtn_Click(object sender, EventArgs e)
        {
            //Clear the bet amount
            game.Players[FormIndex].ClearBet();
            ShowBankValue();
        }

        #endregion

        #region Game Methods

        /// <summary>
        /// This method updates the current bet by a specified bet amount
        /// </summary>
        /// <param name="betValue"></param>
        private void Bet(decimal betValue)
        {
            try
            {
                // Update the bet amount
                game.Players[FormIndex].IncreaseBet(betValue);
               

                // Update the "My Bet" and "My Account" values
                ShowBankValue();
            }
            catch (Exception NotEnoughMoneyException)
            {
                MessageBox.Show(NotEnoughMoneyException.Message);
            }
        }

        /// <summary>
        /// Set the "My Account" value in the UI
        /// </summary>
        public void ShowBankValue()
        {
                // Update the "My Account" value
                myAccountTextBox.Text = "$" + game.Players[FormIndex].Balance.ToString();
                myBetTextBox.Text = "$" + game.Players[FormIndex].Bet.ToString();
        }
        public void ShowMin()
        {
            game.Players[FormIndex].IncreaseBet(game.MinBetAmount);
            myBetTextBox.Text = "$" + game.MinBetAmount;
        }

        /// <summary>
        /// Clear the dealer and player cards
        /// </summary>
        private void ClearTable()
        {
            for (int i = 0; i < 2; i++)
            {
                
                playerCards[i].Image = null;
                playerCards[i].Visible = false;
            }
        }

        /// <summary>
        /// Get the game result.  This returns an EndResult value
        /// </summary>
        /// <returns></returns>
        private EndResult GetGameResult()
        {
            EndResult endState;
            // Check for blackjack
            if (game.Dealer.Hand.NumCards == 2 && game.Dealer.HasBlackJack())
            {
                endState = EndResult.DealerBlackJack;
            }
            // Check if the dealer has bust
            else if (game.Dealer.HasBust())
            {
                endState = EndResult.DealerBust;
            }
            else if (game.Dealer.Hand.CompareFaceValue(game.CurrentPlayer.Hand) > 0)
            {
                //dealer wins
                endState = EndResult.DealerWin;
            }
            else if (game.Dealer.Hand.CompareFaceValue(game.CurrentPlayer.Hand) == 0)
            {
                // push
                endState = EndResult.Push;
            }
            else
            {
                // player wins
                endState = EndResult.PlayerWin;
            }
            return endState;
        }

        /// <summary>
        /// Takes an EndResult value and shows the resulting game ending in the UI
        /// </summary>
        /// <param name="endState"></param>
        private void EndGame(EndResult endState)
        {
            switch (endState)
            {
                case EndResult.DealerBust:
                    gameOverTextBox.Text = "Dealer Bust!";
                    game.PlayerWin();
                    break;
                case EndResult.DealerBlackJack:
                    gameOverTextBox.Text = "Dealer BlackJack!";
                    game.PlayerLose();
                    break;
                case EndResult.DealerWin:
                    gameOverTextBox.Text = "Dealer Won!";
                    game.PlayerLose();
                    break;
                case EndResult.PlayerBlackJack:
                    gameOverTextBox.Text = "BlackJack!";
                    game.CurrentPlayer.Balance += (game.CurrentPlayer.Bet * (decimal)2.5);
                    game.CurrentPlayer.Wins += 1;
                    break;
                case EndResult.PlayerBust:
                    gameOverTextBox.Text = "You Bust!";
                    game.PlayerLose();
                    break;
                case EndResult.PlayerWin:
                    gameOverTextBox.Text = "You Won!";
                    game.PlayerWin();
                    break;
                case EndResult.Push:
                    gameOverTextBox.Text = "Push";
                    game.CurrentPlayer.Push += 1;
                    game.CurrentPlayer.Balance += game.CurrentPlayer.Bet;
                    break;
            }
            // Update the "My Record" values
            winTextBox.Text = game.CurrentPlayer.Wins.ToString();
            lossTextBox.Text = game.CurrentPlayer.Losses.ToString();
            tiesTextBox.Text = game.CurrentPlayer.Push.ToString();
            SetUpNewGame();
            ShowBankValue();
            gameOverTextBox.Show();
            // Check if the current player is out of money
            if (game.CurrentPlayer.Balance == 0)
            {
                MessageBox.Show("Out of Money.  Please create a new game to play again.");
                this.Close();
            }
        }

        #endregion

        #region Game UI Methods

        /// <summary>
        /// Load the Deck Card Images
        /// </summary>
        private void LoadCardSkinImages()
        {
            try
            {
                // Load the card skin image from file
                Image cardSkin = Image.FromFile(Properties.Settings.Default.CardGameImageSkinPath);
                // Set the three cards on the UI to the card skin image
                deckCard1PictureBox.Image = cardSkin;
                deckCard2PictureBox.Image = cardSkin;
                deckCard3PictureBox.Image = cardSkin;
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Card skin images are not loading correctly.  Make sure the card skin images are in the correct location.");
            }


            playerCards = new PictureBox[] { playerCard1, playerCard2 };
            tableCards = new PictureBox[] { tableCard1PictureBox, tableCard2PictureBox, tableCard3PictureBox, tableCard4PictureBox, tableCard5PictureBox };
        }

        /// <summary>
        /// Set up the UI for when the game is in play after the player has hit deal game
        /// </summary>
      

        /// <summary>
        /// Set up the UI for a new game
        /// </summary>
        public void SetUpNewGame()
        {
            //photoPictureBox.ImageLocation = Properties.Settings.Default.PlayerImage;
            //photoPictureBox.Visible = true;
            //playerNameLabel.Text = Properties.Settings.Default.PlayerName;
            //dealButton.Enabled = true;
            //doubleDownButton.Enabled = false;
            //standButton.Enabled = false;
            //hitButton.Enabled = false;
            //clearBetButton.Enabled = true;
            //tenButton.Enabled = true;
            //twentyFiveButton.Enabled = true;
            //fiftyButton.Enabled = true;
            //hundredButton.Enabled = true;
            //gameOverTextBox.Hide();
            //playerTotalLabel.Hide();
            //firstTurn = true;
            //ShowBankValue();

            photoPictureBox.ImageLocation = Properties.Settings.Default.PlayerImage;
            photoPictureBox.Visible = true;
            playerNameLabel.Text = Properties.Settings.Default.PlayerName;
            dealButton.Enabled = true;
            FoldButton.Enabled = true;
            clearBetButton.Enabled = true;
            tenButton.Enabled = true;
            twentyFiveButton.Enabled = true;
            fiftyButton.Enabled = true;
            hundredButton.Enabled = true;
            


        }

        /// <summary>
        /// Refresh the UI to update the player cards
        /// </summary>

        public void UpdateUIPlayerCards()
        {
            // Update the value of the hand
            playerTotalLabel.Text = game.CurrentPlayer.Hand.GetSumOfHand().ToString();

            List<Card> pcards = game.Players[FormIndex].Hand.Cards;
           // List<Card> cCards = game.CpuPlayer.Hand.Cards;
            for (int i = 0; i < pcards.Count; i++)
            {
                // Load each card from file
                LoadCard(playerCards[i], pcards[i]);
                playerCards[i].Visible = true;
                playerCards[i].BringToFront();

                //LoadCard(cpuCards[i], cCards[i]);
                //cpuCards[i].Visible = true;
                //cpuCards[i].BringToFront();
            }

           // List<Card> dcards = game.Dealer.Hand.Cards;
            //for (int i = 0; i < dcards.Count; i++)
            //{
            //    LoadCard(dealerCards[i], dcards[i]);
            //    dealerCards[i].Visible = true;
            //    dealerCards[i].BringToFront();
            //}
        }
        public void updateTableCards()
        {
            switch (game.gameState)
            {
                case BlackJackGame.GameState.FLOP:
                    for (int i = 0; i < 3; i++)
                    {
                        listOfCard.Add(game.TableCards[i]);
                        LoadCard(tableCards[i], game.TableCards[i]);
                        tableCards[i].Visible = true;
                        tableCards[i].BringToFront();
                    }

                    break;
                case BlackJackGame.GameState.TURN:
                    listOfCard.Add(game.TableCards[3]);
                    LoadCard(tableCards[3], game.TableCards[3]);
                    tableCards[3].Visible = true;
                    tableCards[3].BringToFront();
                    break;
                case BlackJackGame.GameState.RIVER:
                    listOfCard.Add(game.TableCards[4]);
                    LoadCard(tableCards[4], game.TableCards[4]);
                    tableCards[4].Visible = true;
                    tableCards[4].BringToFront();

                    break;
            }

        }
        /// <summary>
        /// Takes the card value and loads the corresponding card image from file
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="c"></param>
        private void LoadCard(PictureBox pb, Card c)
        {
            //MessageBox.Show(pb.Name);
            try
            {
                StringBuilder image = new StringBuilder();

                switch (c.Suit)
                {
                    case Suit.Diamonds:
                        image.Append("di");
                        break;
                    case Suit.Hearts:
                        image.Append("he");
                        break;
                    case Suit.Spades:
                        image.Append("sp");
                        break;
                    case Suit.Clubs:
                        image.Append("cl");
                        break;
                }

                switch (c.FaceVal)
                {
                    case FaceValue.Ace:
                        image.Append("1");
                        break;
                    case FaceValue.King:
                        image.Append("k");
                        break;
                    case FaceValue.Queen:
                        image.Append("q");
                        break;
                    case FaceValue.Jack:
                        image.Append("j");
                        break;
                    case FaceValue.Ten:
                        image.Append("10");
                        break;
                    case FaceValue.Nine:
                        image.Append("9");
                        break;
                    case FaceValue.Eight:
                        image.Append("8");
                        break;
                    case FaceValue.Seven:
                        image.Append("7");
                        break;
                    case FaceValue.Six:
                        image.Append("6");
                        break;
                    case FaceValue.Five:
                        image.Append("5");
                        break;
                    case FaceValue.Four:
                        image.Append("4");
                        break;
                    case FaceValue.Three:
                        image.Append("3");
                        break;
                    case FaceValue.Two:
                        image.Append("2");
                        break;
                }

                image.Append(Properties.Settings.Default.CardGameImageExtension);
                string cardGameImagePath = Properties.Settings.Default.CardGameImagePath;
                string cardGameImageSkinPath = Properties.Settings.Default.CardGameImageSkinPath;
                image.Insert(0, cardGameImagePath);
                //check to see if the card should be faced down or up;
                //if (!c.IsCardUp)
                //    image.Replace(image.ToString(), cardGameImageSkinPath);
               
                pb.Image = new Bitmap(image.ToString());
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Card images are not loading correctly.  Make sure all card images are in the right location.");
            }
        }
        #endregion
        public void SetTextBox(string text)
        {
            gameOverTextBox.Text = text;
        }
        private void BlackJackForm_Load(object sender, EventArgs e)
        {

        }

        private void tableCard1PictureBox_Click(object sender, EventArgs e)
        {

        }

        private void deckCard3PictureBox_Click(object sender, EventArgs e)
        {

        }

        private void gameOverTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}