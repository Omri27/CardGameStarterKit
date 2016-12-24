using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Poker.CardGameFramework;

namespace Poker
{
    public partial class PokerForm : Form
    {
        #region Fields

        //Creates a new blackjack game with one player and an inital balance set through the settings designer
        private PokerGame game;
        private PictureBox[] playerCards;
        private PictureBox[] tableCards;
        private List<Card> listOfCard;
        private PokerForm pokerForm;

        public bool Folded { get; set; }

        public int  FormIndex { get; set; }
        public int FormId { get; set; }
        #endregion

        #region Main Constructor

        /// <summary>
        /// Main constructor for the PokerForm.  Initializes components, loads the card skin images, and sets up the new game
        /// </summary>
        public PokerForm(PokerGame game)
        {
            Folded = false;
            this.game = game;
            
            listOfCard = new List<Card>();
            InitializeComponent();
            LoadCardSkinImages();
           

            //game.gameState = PokerGame.GameState.PREFLOP;
            //SetUpGameInPlay();
        }

        public PokerForm(PokerForm pokerForm)
        {
            this.pokerForm = pokerForm;
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
                Player p = game.getPlayerByFormId(FormId);
                game.BetAmount +=p.Bet;
                // If the current bet is equal to 0, ask the player to place a bet
                if ((p.Bet == 0) && (p.Balance > 0))
                {
                    MessageBox.Show("You must place a bet before the dealer deals.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            
                else
                {
                    
                    // Place the bet
                    if (p.Bet > game.MinBetAmount)
                        game.MinBetAmount = p.Bet;
                    
                    p.PlaceBet();


                    ShowBankValue();
                    
                    switch (game.gameState)
                    {
                        case PokerGame.GameState.PREFLOP:

                            game.SetUpGameInPlay();
                           
                            break;

                        case PokerGame.GameState.FLOP:
                          
                            game.SetUpGameInPlay();
                        
                            break;
                        case PokerGame.GameState.TURN:
                          
                            game.SetUpGameInPlay();
                         
                            break;
                        case PokerGame.GameState.RIVER:
                          
                            game.SetUpGameInPlay();
                        
                            break;

                        //case PokerGame.GameState.NewStage: // To Add another level of the game you need to add another enum on gamestate & to the above switch case

                        //    game.SetUpGameInPlay();

                        //    break;
                           

                    }

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
        /// 
        private void FoldBtn_Click(object sender, EventArgs e)
        {
            
            game.TurnPlayerForm.RemoveAt(0);            
            game.Folded = true;
            LockControls();
            if (game.TurnPlayerForm.Count  == 0)
            {
                
                game.PlayerForms.Remove(this);              
                game.SetUpGameInPlay();
            }
            else
            {
                game.SetUpGameInPlay();
                game.PlayerForms.Remove(this);
                 

            }
            game.FoldedPlayers.Add(FormId);
            if (game.FoldedPlayers.Count == 2)
            {
                game.GameOver();
            }
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
        }
        /// <summary>
        /// Invoked when the hit button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      

        /// <summary>
        /// Invoked when the double down button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        

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
            Player p = game.getPlayerByFormId(FormId);
            //Clear the bet amount
            p.ClearBet();
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
            Player p = game.getPlayerByFormId(FormId);
            try
            {
                // Update the bet amount
                p.IncreaseBet(betValue);
               

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
            Player p = game.getPlayerByFormId(FormId);
                // Update the "My Account" value
                myAccountTextBox.Text = "$" + p.Balance.ToString();
                myBetTextBox.Text = "$" + p.Bet.ToString();
        }

        
        public void ShowMin()
        {
            Player p = game.getPlayerByFormId(FormId);
           p.IncreaseBet(game.MinBetAmount);
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
      

        /// <summary>
        /// Takes an EndResult value and shows the resulting game ending in the UI
        /// </summary>
        /// <param name="endState"></param>

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

            
            }

          
        }
        public void updateTableCards()
        {
            switch (game.gameState)
            {
                case PokerGame.GameState.FLOP:
                    for (int i = 0; i < 3; i++)
                    {
                        listOfCard.Add(game.TableCards[i]);
                        LoadCard(tableCards[i], game.TableCards[i]);
                        tableCards[i].Visible = true;
                        tableCards[i].BringToFront();
                    }

                    break;
                case PokerGame.GameState.TURN:
                    listOfCard.Add(game.TableCards[3]);
                    LoadCard(tableCards[3], game.TableCards[3]);
                    tableCards[3].Visible = true;
                    tableCards[3].BringToFront();
                    break;
                case PokerGame.GameState.RIVER:
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