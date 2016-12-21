using System.Collections.Generic;
using BlackJack.CardGameFramework;


namespace BlackJack
{
	public class BlackJackGame
	{
		#region Fields

        // private Deck and Player objects for the current deck, dealer, and player
		private Deck deck;
		private Player dealer;
        private Player player;
        private List<Player> players;

        private Player table;
        private List<BlackJackForm> playerForms;
        
        #endregion

        #region Properties

        // public properties to return the current player, dealer, and current deck
        public Player CurrentPlayer { get { return player; } }
 
        public Player Dealer { get { return dealer; } }
        public Deck CurrentDeck { get { return deck; } }
        public bool FirstTurn { get; set; }
        public bool ReBet { get; set; }
        public List<Player> Players { get { return players; } }
        public List<Card> TableCards { get; set; }
        public int FormTurn { get; set; }
        public enum GameState {PREFLOP,FLOP,TURN,RIVER};
        public decimal BetAmount { get; set; }
        public decimal MinBetAmount { get; set; }

        public GameState gameState { get; set; }
        #endregion

        #region Main Game Constructor

        /// <summary>
        /// Main Constructor for BlackJack Game
        /// </summary>
        /// <param name="initBalance"></param>
        public BlackJackGame(int initBalance)
		{
            BetAmount = 0;
            FormTurn = 0;
            // Create a dealer and one player with the initial balance.
            TableCards = new List<Card>();
            dealer = new Player();
            player = new Player(initBalance) ;
            players = new List<Player>();
            playerForms = new List<BlackJackForm>();
            table = new Player();

            //player2 = new Player(initBalance);

		}
        void GameOver()
        {
            foreach (Player p in Players)
                p.Hand.GetValueOfHand(TableCards);
            players.Sort();
            foreach(BlackJackForm form in playerForms)
            {
                form.SetTextBox("Player " + players[2].PlayerIndex + " has Won with " + players[2].Hand.GetValueOfHand(TableCards).ToString());
            }

            players[2].Balance += BetAmount;
            playerForms[players[2].PlayerIndex].ShowBankValue();


        }
        public void addPlayer(BlackJackForm form)
        {
            this.playerForms.Add(form);
            Player p = new Player(1000);
            p.PlayerIndex = form.FormIndex;
            this.players.Add(p);
        }
        #endregion

        #region Game Methods
        public void startGame()
        {
            DealNewGame();
            foreach (BlackJackForm form in playerForms)
            {
                form.UpdateUIPlayerCards();
            }
            foreach (BlackJackForm form in playerForms)
            {
                form.SetTextBox(gameState.ToString());
                if (FormTurn == form.FormIndex)
                    form.SetUpNewGame();
                else
                    form.LockControls();
            }

            FormTurn++;
        }


        public void SetUpGameInPlay()
        {
         
            if (FormTurn < 3)
            {

                foreach (BlackJackForm form in playerForms)
                {
                    if (FormTurn == form.FormIndex)
                    {
                        form.SetUpNewGame();
                        form.ShowMin();
                    }
                        
                    else
                        form.LockControls();
                }
            }
            else
            {
                gameState++;
                MinBetAmount = 0;
                foreach (Player p in players)
                    p.ClearBet();
                FormTurn = 0;
                foreach (BlackJackForm form in playerForms)
                {
                    form.SetTextBox(gameState.ToString());
                    form.ShowBankValue();
                    if (FormTurn == form.FormIndex)
                        form.SetUpNewGame();
                    else
                        form.LockControls();
                }
                switch (gameState)
                {
                   
                    case BlackJackGame.GameState.FLOP:
                        // gameOverTextBox.Text = "Flop";
                        for (int i = 0; i < 3; i++)
                        {
                            Card flopCard = CurrentDeck.Draw();
                            TableCards.Add(flopCard);

                        }


                        break;
                    case BlackJackGame.GameState.TURN:
                        // gameOverTextBox.Text = "Turn";
                        Card turnCard = CurrentDeck.Draw();
                        TableCards.Add(turnCard);

                        break;
                    case BlackJackGame.GameState.RIVER:
                        // gameOverTextBox.Text = "River";
                        Card riverCard = CurrentDeck.Draw();
                        TableCards.Add(riverCard);

                        GameOver();
                        break;

                        
                }
                foreach (BlackJackForm form in playerForms)
                {
                    switch (gameState)
                    {
                        case BlackJackGame.GameState.FLOP:
                            // gameOverTextBox.Text = "Flop";
                            form.updateTableCards();
                            break;
                        case BlackJackGame.GameState.TURN:
                            // gameOverTextBox.Text = "Turn";

                            form.updateTableCards();
                            break;
                        case BlackJackGame.GameState.RIVER:
                            // gameOverTextBox.Text = "River";

                            form.updateTableCards();

                            break;
                    }
                }

                
            }
            //foreach(Player p in players)
            //{
            //    playerForms[p.PlayerIndex].SetTextBox(p.Hand.GetValueOfHand(TableCards).ToString());
            //}
        }
       
        /// <summary>
        /// Deals a new game.  This is invoked through the Deal button in BlackJackForm.cs
        /// </summary>
        public void DealNewGame()
		{
            // Create a new deck and then shuffle the deck
            deck = new Deck();
            deck.Shuffle();

            // Reset the player and the dealer's hands in case this is not the first game
            foreach (Player p in Players)
            {
                p.NewHand();
            }
        

          
			// Deal two cards to each person's hand
            for(int p =0;p<players.Count;p++)
            {
                for (int i = 0; i < 2; i++)
                {
                    Card c = deck.Draw();
                    players[p].Hand.Cards.Add(c);

                   

                }
            }
            

            // Give the player and the dealer a handle to the current deck
           // player.CurrentDeck = deck;
            //dealer.CurrentDeck = deck;
		}

        /// <summary>
        /// This method finishes playing the dealer's hand
        /// </summary>
        public void DealerPlay()
        {
            // Dealer's card that was facing down is turned up when they start playing
            dealer.Hand.Cards[1].IsCardUp = true;

            // Check to see if dealer has a hand that is less than 17
            // If so, dealer should keep hitting until their hand is greater or equal to 17
            //if (dealer.Hand.GetSumOfHand() < 17)
            //{
            //    dealer.Hit();
            //    DealerPlay();
            //}
        }

        /// <summary>
        /// Update the player's record with a loss
        /// </summary>
        public void PlayerLose()
        {
            player.Losses += 1;
        }

        /// <summary>
        /// Update the player's record with a win
        /// </summary>
        public void PlayerWin()
        {
            player.Balance += player.Bet * 2;
            player.Wins += 1;
        }
		#endregion
	}
}
